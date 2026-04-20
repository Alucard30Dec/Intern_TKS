using BlazorApp1.Domain.Entities;
using BlazorApp1.Infrastructure.Data;
using BlazorApp1.Models.Common;
using BlazorApp1.Models.SanPham;
using BlazorApp1.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp1.Services;

/// <summary>
/// Xu ly nghiep vu bai 3: them, sua, xoa mem danh muc san pham.
/// </summary>
public sealed class SanPhamService : ISanPhamService
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
    private readonly ILogger<SanPhamService> _logger;

    public SanPhamService(
        IDbContextFactory<AppDbContext> dbContextFactory,
        ILogger<SanPhamService> logger)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }

    /// <summary>
    /// Lay danh sach san pham cho man hinh danh muc.
    /// </summary>
    public async Task<IReadOnlyList<SanPhamListItemVm>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await dbContext.SanPhams
            .AsNoTracking()
            .Where(x => x.Is_Active)
            .OrderBy(x => x.Ma_San_Pham)
            .ThenBy(x => x.Ten_San_Pham)
            .Select(x => new SanPhamListItemVm
            {
                San_Pham_ID = x.San_Pham_ID,
                Ma_San_Pham = x.Ma_San_Pham,
                Ten_San_Pham = x.Ten_San_Pham,
                Loai_San_Pham_ID = x.Loai_San_Pham_ID,
                Ten_LSP = x.Loai_San_Pham != null ? x.Loai_San_Pham.Ten_LSP : string.Empty,
                Don_Vi_Tinh_ID = x.Don_Vi_Tinh_ID,
                Ten_Don_Vi_Tinh = x.Don_Vi_Tinh != null ? x.Don_Vi_Tinh.Ten_Don_Vi_Tinh : string.Empty,
                Ghi_Chu = x.Ghi_Chu,
                Is_Active = x.Is_Active
            })
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Lay chi tiet san pham theo ID de nap vao form sua.
    /// </summary>
    public async Task<ServiceResult<SanPhamUpsertVm>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return ServiceResult<SanPhamUpsertVm>.Fail("ID không hợp lệ.");
        }

        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var entity = await dbContext.SanPhams
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.San_Pham_ID == id, cancellationToken);

            if (entity is null)
            {
                return ServiceResult<SanPhamUpsertVm>.Fail("Không tìm thấy sản phẩm.");
            }

            return ServiceResult<SanPhamUpsertVm>.Ok(new SanPhamUpsertVm
            {
                San_Pham_ID = entity.San_Pham_ID,
                Ma_San_Pham = entity.Ma_San_Pham,
                Ten_San_Pham = entity.Ten_San_Pham,
                Loai_San_Pham_ID = entity.Loai_San_Pham_ID,
                Don_Vi_Tinh_ID = entity.Don_Vi_Tinh_ID,
                Ghi_Chu = entity.Ghi_Chu
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Get SanPham by id failed. San_Pham_ID={San_Pham_ID}", id);
            return ServiceResult<SanPhamUpsertVm>.Fail("Không thể tải thông tin sản phẩm.");
        }
    }

    /// <summary>
    /// Tao moi san pham va tra ve ket qua nghiep vu.
    /// </summary>
    public async Task<ServiceResult> CreateAsync(SanPhamUpsertVm model, CancellationToken cancellationToken = default)
    {
        var validation = ValidateBusinessRules(model);
        if (!validation.Success)
        {
            return validation;
        }

        var normalizedCode = NormalizeCode(model.Ma_San_Pham);
        var normalizedName = NormalizeName(model.Ten_San_Pham);

        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            if (!await LoaiSanPhamExistsAsync(dbContext, model.Loai_San_Pham_ID, cancellationToken))
            {
                return ServiceResult.Fail("Loại sản phẩm không tồn tại.");
            }

            if (!await DonViTinhExistsAsync(dbContext, model.Don_Vi_Tinh_ID, cancellationToken))
            {
                return ServiceResult.Fail("Đơn vị tính không tồn tại.");
            }

            var duplicatedCode = await ExistsByCodeAsync(dbContext, normalizedCode, null, cancellationToken);
            if (duplicatedCode)
            {
                return ServiceResult.Fail("Mã sản phẩm đã tồn tại.");
            }

            var entity = new SanPham
            {
                Ma_San_Pham = normalizedCode,
                Ten_San_Pham = normalizedName,
                Loai_San_Pham_ID = model.Loai_San_Pham_ID,
                Don_Vi_Tinh_ID = model.Don_Vi_Tinh_ID,
                // Luu null thay vi chuoi rong de dong nhat voi thong ke va bao cao.
                Ghi_Chu = NormalizeNullableText(model.Ghi_Chu),
                Is_Active = true
            };

            dbContext.SanPhams.Add(entity);
            await dbContext.SaveChangesAsync(cancellationToken);
            return ServiceResult.Ok("Thêm sản phẩm thành công.");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Create SanPham failed. Ma_San_Pham={Ma_San_Pham}", normalizedCode);
            return ServiceResult.Fail("Không thể thêm sản phẩm. Có thể dữ liệu đã bị trùng hoặc không hợp lệ.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while creating SanPham. Ma_San_Pham={Ma_San_Pham}", normalizedCode);
            return ServiceResult.Fail("Không thể thêm sản phẩm do lỗi hệ thống.");
        }
    }

    /// <summary>
    /// Cap nhat san pham theo ID.
    /// </summary>
    public async Task<ServiceResult> UpdateAsync(SanPhamUpsertVm model, CancellationToken cancellationToken = default)
    {
        if (model.San_Pham_ID <= 0)
        {
            return ServiceResult.Fail("ID không hợp lệ.");
        }

        var validation = ValidateBusinessRules(model);
        if (!validation.Success)
        {
            return validation;
        }

        var normalizedCode = NormalizeCode(model.Ma_San_Pham);
        var normalizedName = NormalizeName(model.Ten_San_Pham);

        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var entity = await dbContext.SanPhams
                .FirstOrDefaultAsync(x => x.San_Pham_ID == model.San_Pham_ID, cancellationToken);

            if (entity is null)
            {
                return ServiceResult.Fail("Không tìm thấy sản phẩm để cập nhật.");
            }

            if (!await LoaiSanPhamExistsAsync(dbContext, model.Loai_San_Pham_ID, cancellationToken))
            {
                return ServiceResult.Fail("Loại sản phẩm không tồn tại.");
            }

            if (!await DonViTinhExistsAsync(dbContext, model.Don_Vi_Tinh_ID, cancellationToken))
            {
                return ServiceResult.Fail("Đơn vị tính không tồn tại.");
            }

            var duplicatedCode = await ExistsByCodeAsync(dbContext, normalizedCode, model.San_Pham_ID, cancellationToken);
            if (duplicatedCode)
            {
                return ServiceResult.Fail("Mã sản phẩm đã tồn tại.");
            }

            entity.Ma_San_Pham = normalizedCode;
            entity.Ten_San_Pham = normalizedName;
            entity.Loai_San_Pham_ID = model.Loai_San_Pham_ID;
            entity.Don_Vi_Tinh_ID = model.Don_Vi_Tinh_ID;
            entity.Ghi_Chu = NormalizeNullableText(model.Ghi_Chu);

            await dbContext.SaveChangesAsync(cancellationToken);
            return ServiceResult.Ok("Cập nhật sản phẩm thành công.");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Update SanPham failed. San_Pham_ID={San_Pham_ID}", model.San_Pham_ID);
            return ServiceResult.Fail("Không thể cập nhật sản phẩm. Có thể dữ liệu đã bị trùng hoặc không hợp lệ.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while updating SanPham. San_Pham_ID={San_Pham_ID}", model.San_Pham_ID);
            return ServiceResult.Fail("Không thể cập nhật sản phẩm do lỗi hệ thống.");
        }
    }

    /// <summary>
    /// Xoa mem san pham theo ID (an khoi UI, van giu du lieu trong DB).
    /// </summary>
    public async Task<ServiceResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return ServiceResult.Fail("ID không hợp lệ.");
        }

        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var entity = await dbContext.SanPhams
                .FirstOrDefaultAsync(x => x.San_Pham_ID == id, cancellationToken);

            if (entity is null)
            {
                return ServiceResult.Fail("Không tìm thấy sản phẩm để xóa.");
            }

            if (!entity.Is_Active)
            {
                return ServiceResult.Fail("Sản phẩm này đã được xóa khỏi danh sách hiển thị trước đó.");
            }

            entity.Is_Active = false;
            await dbContext.SaveChangesAsync(cancellationToken);
            return ServiceResult.Ok("Đã xóa khỏi danh sách hiển thị. Dữ liệu vẫn được lưu trong hệ thống.");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Soft delete SanPham failed. San_Pham_ID={San_Pham_ID}", id);
            return ServiceResult.Fail("Không thể xóa khỏi danh sách hiển thị vì dữ liệu đang được ràng buộc ở nghiệp vụ khác.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while soft deleting SanPham. San_Pham_ID={San_Pham_ID}", id);
            return ServiceResult.Fail("Không thể xóa khỏi danh sách hiển thị do lỗi hệ thống.");
        }
    }

    private static ServiceResult ValidateBusinessRules(SanPhamUpsertVm model)
    {
        if (model is null)
        {
            return ServiceResult.Fail("Dữ liệu không hợp lệ.");
        }

        var normalizedCode = NormalizeCode(model.Ma_San_Pham);
        if (string.IsNullOrWhiteSpace(normalizedCode))
        {
            return ServiceResult.Fail("Mã sản phẩm không được để trống.");
        }

        if (normalizedCode.Length > 50)
        {
            return ServiceResult.Fail("Mã sản phẩm tối đa 50 ký tự.");
        }

        var normalizedName = NormalizeName(model.Ten_San_Pham);
        if (string.IsNullOrWhiteSpace(normalizedName))
        {
            return ServiceResult.Fail("Tên sản phẩm không được để trống.");
        }

        if (normalizedName.Length > 200)
        {
            return ServiceResult.Fail("Tên sản phẩm tối đa 200 ký tự.");
        }

        if (model.Loai_San_Pham_ID <= 0)
        {
            return ServiceResult.Fail("Loại sản phẩm không được để trống.");
        }

        if (model.Don_Vi_Tinh_ID <= 0)
        {
            return ServiceResult.Fail("Đơn vị tính không được để trống.");
        }

        var ghiChu = NormalizeNullableText(model.Ghi_Chu);
        if (ghiChu is not null && ghiChu.Length > 255)
        {
            return ServiceResult.Fail("Ghi chú tối đa 255 ký tự.");
        }

        return ServiceResult.Ok();
    }

    private static async Task<bool> ExistsByCodeAsync(
        AppDbContext dbContext,
        string normalizedCode,
        int? ignoreId,
        CancellationToken cancellationToken)
    {
        // Quy tac nghiep vu coi ma khong phan biet hoa-thuong de tranh tao trung ma chi khac casing.
        var compareValue = normalizedCode.ToUpper();

        return await dbContext.SanPhams.AnyAsync(
            x => (!ignoreId.HasValue || x.San_Pham_ID != ignoreId.Value)
                 && x.Ma_San_Pham.ToUpper() == compareValue,
            cancellationToken);
    }

    private static Task<bool> LoaiSanPhamExistsAsync(
        AppDbContext dbContext,
        int loaiSanPhamId,
        CancellationToken cancellationToken)
    {
        return dbContext.LoaiSanPhams.AnyAsync(
            x => x.Loai_San_Pham_ID == loaiSanPhamId && x.Is_Active,
            cancellationToken);
    }

    private static Task<bool> DonViTinhExistsAsync(
        AppDbContext dbContext,
        int donViTinhId,
        CancellationToken cancellationToken)
    {
        return dbContext.DonViTinhs.AnyAsync(
            x => x.Don_Vi_Tinh_ID == donViTinhId && x.Is_Active,
            cancellationToken);
    }

    private static string NormalizeCode(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        return value.Trim().ToUpperInvariant();
    }

    private static string NormalizeName(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var parts = value
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        return string.Join(" ", parts);
    }

    private static string? NormalizeNullableText(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value.Trim();
    }
}
