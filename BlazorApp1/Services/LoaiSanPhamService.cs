using BlazorApp1.Domain.Entities;
using BlazorApp1.Infrastructure.Data;
using BlazorApp1.Models.Common;
using BlazorApp1.Models.LoaiSanPham;
using BlazorApp1.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp1.Services;

/// <summary>
/// Xu ly nghiep vu bai 2: them, sua, xoa mem danh muc loai san pham.
/// </summary>
public sealed class LoaiSanPhamService : ILoaiSanPhamService
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
    private readonly ILogger<LoaiSanPhamService> _logger;

    public LoaiSanPhamService(
        IDbContextFactory<AppDbContext> dbContextFactory,
        ILogger<LoaiSanPhamService> logger)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }

    /// <summary>
    /// Lay danh sach loai san pham cho man hinh danh muc.
    /// </summary>
    public async Task<IReadOnlyList<LoaiSanPhamListItemVm>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await dbContext.LoaiSanPhams
            .AsNoTracking()
            .Where(x => x.Is_Active)
            .OrderBy(x => x.Ma_LSP)
            .ThenBy(x => x.Ten_LSP)
            .Select(x => new LoaiSanPhamListItemVm
            {
                Loai_San_Pham_ID = x.Loai_San_Pham_ID,
                Ma_LSP = x.Ma_LSP,
                Ten_LSP = x.Ten_LSP,
                Ghi_Chu = x.Ghi_Chu,
                Is_Active = x.Is_Active
            })
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Lay chi tiet loai san pham theo ID de nap vao form sua.
    /// </summary>
    public async Task<ServiceResult<LoaiSanPhamUpsertVm>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return ServiceResult<LoaiSanPhamUpsertVm>.Fail("ID không hợp lệ.");
        }

        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var entity = await dbContext.LoaiSanPhams
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Loai_San_Pham_ID == id, cancellationToken);

            if (entity is null)
            {
                return ServiceResult<LoaiSanPhamUpsertVm>.Fail("Không tìm thấy loại sản phẩm.");
            }

            return ServiceResult<LoaiSanPhamUpsertVm>.Ok(new LoaiSanPhamUpsertVm
            {
                Loai_San_Pham_ID = entity.Loai_San_Pham_ID,
                Ma_LSP = entity.Ma_LSP,
                Ten_LSP = entity.Ten_LSP,
                Ghi_Chu = entity.Ghi_Chu
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Get LoaiSanPham by id failed. Loai_San_Pham_ID={Loai_San_Pham_ID}", id);
            return ServiceResult<LoaiSanPhamUpsertVm>.Fail("Không thể tải thông tin loại sản phẩm.");
        }
    }

    /// <summary>
    /// Tao moi loai san pham va tra ve ket qua nghiep vu.
    /// </summary>
    public async Task<ServiceResult> CreateAsync(LoaiSanPhamUpsertVm model, CancellationToken cancellationToken = default)
    {
        var validation = ValidateBusinessRules(model);
        if (!validation.Success)
        {
            return validation;
        }

        var normalizedCode = NormalizeCode(model.Ma_LSP);
        var normalizedName = NormalizeName(model.Ten_LSP);

        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var duplicatedCode = await ExistsByCodeAsync(dbContext, normalizedCode, null, cancellationToken);
            if (duplicatedCode)
            {
                return ServiceResult.Fail("Mã đã tồn tại.");
            }

            var duplicatedName = await ExistsByNameAsync(dbContext, normalizedName, null, cancellationToken);
            if (duplicatedName)
            {
                return ServiceResult.Fail("Tên đã tồn tại.");
            }

            var entity = new LoaiSanPham
            {
                Ma_LSP = normalizedCode,
                Ten_LSP = normalizedName,
                // Luu null thay vi chuoi rong de tranh sai lech khi thong ke ban ghi "co ghi chu".
                Ghi_Chu = NormalizeNullableText(model.Ghi_Chu),
                Is_Active = true
            };

            dbContext.LoaiSanPhams.Add(entity);
            await dbContext.SaveChangesAsync(cancellationToken);
            return ServiceResult.Ok("Thêm loại sản phẩm thành công.");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Create LoaiSanPham failed. Ma_LSP={Ma_LSP}", normalizedCode);
            return ServiceResult.Fail("Không thể thêm loại sản phẩm. Có thể dữ liệu đã bị trùng.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while creating LoaiSanPham. Ma_LSP={Ma_LSP}", normalizedCode);
            return ServiceResult.Fail("Không thể thêm loại sản phẩm do lỗi hệ thống.");
        }
    }

    /// <summary>
    /// Cap nhat loai san pham theo ID.
    /// </summary>
    public async Task<ServiceResult> UpdateAsync(LoaiSanPhamUpsertVm model, CancellationToken cancellationToken = default)
    {
        if (model.Loai_San_Pham_ID <= 0)
        {
            return ServiceResult.Fail("ID không hợp lệ.");
        }

        var validation = ValidateBusinessRules(model);
        if (!validation.Success)
        {
            return validation;
        }

        var normalizedCode = NormalizeCode(model.Ma_LSP);
        var normalizedName = NormalizeName(model.Ten_LSP);

        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var entity = await dbContext.LoaiSanPhams
                .FirstOrDefaultAsync(x => x.Loai_San_Pham_ID == model.Loai_San_Pham_ID, cancellationToken);

            if (entity is null)
            {
                return ServiceResult.Fail("Không tìm thấy loại sản phẩm để cập nhật.");
            }

            var duplicatedCode = await ExistsByCodeAsync(dbContext, normalizedCode, model.Loai_San_Pham_ID, cancellationToken);
            if (duplicatedCode)
            {
                return ServiceResult.Fail("Mã đã tồn tại.");
            }

            var duplicatedName = await ExistsByNameAsync(dbContext, normalizedName, model.Loai_San_Pham_ID, cancellationToken);
            if (duplicatedName)
            {
                return ServiceResult.Fail("Tên đã tồn tại.");
            }

            entity.Ma_LSP = normalizedCode;
            entity.Ten_LSP = normalizedName;
            // Giu quy uoc luu null dong nhat voi tao moi va voi data cu.
            entity.Ghi_Chu = NormalizeNullableText(model.Ghi_Chu);

            await dbContext.SaveChangesAsync(cancellationToken);
            return ServiceResult.Ok("Cập nhật loại sản phẩm thành công.");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Update LoaiSanPham failed. Loai_San_Pham_ID={Loai_San_Pham_ID}", model.Loai_San_Pham_ID);
            return ServiceResult.Fail("Không thể cập nhật loại sản phẩm. Có thể dữ liệu đã bị trùng.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while updating LoaiSanPham. Loai_San_Pham_ID={Loai_San_Pham_ID}", model.Loai_San_Pham_ID);
            return ServiceResult.Fail("Không thể cập nhật loại sản phẩm do lỗi hệ thống.");
        }
    }

    /// <summary>
    /// Xoa mem loai san pham theo ID (an khoi UI, van giu du lieu trong DB).
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

            var entity = await dbContext.LoaiSanPhams
                .FirstOrDefaultAsync(x => x.Loai_San_Pham_ID == id, cancellationToken);

            if (entity is null)
            {
                return ServiceResult.Fail("Không tìm thấy loại sản phẩm để xóa.");
            }

            if (!entity.Is_Active)
            {
                return ServiceResult.Fail("Loại sản phẩm này đã được xóa khỏi danh sách hiển thị trước đó.");
            }

            entity.Is_Active = false;
            await dbContext.SaveChangesAsync(cancellationToken);
            return ServiceResult.Ok("Đã xóa khỏi danh sách hiển thị. Dữ liệu vẫn được lưu trong hệ thống.");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Soft delete LoaiSanPham failed. Loai_San_Pham_ID={Loai_San_Pham_ID}", id);
            return ServiceResult.Fail("Không thể xóa khỏi danh sách hiển thị vì dữ liệu đang được ràng buộc ở nghiệp vụ khác.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while soft deleting LoaiSanPham. Loai_San_Pham_ID={Loai_San_Pham_ID}", id);
            return ServiceResult.Fail("Không thể xóa khỏi danh sách hiển thị do lỗi hệ thống.");
        }
    }

    private static ServiceResult ValidateBusinessRules(LoaiSanPhamUpsertVm model)
    {
        if (model is null)
        {
            return ServiceResult.Fail("Dữ liệu không hợp lệ.");
        }

        var normalizedCode = NormalizeCode(model.Ma_LSP);
        if (string.IsNullOrWhiteSpace(normalizedCode))
        {
            return ServiceResult.Fail("Mã không được để trống.");
        }

        if (normalizedCode.Length > 50)
        {
            return ServiceResult.Fail("Mã tối đa 50 ký tự.");
        }

        var normalizedName = NormalizeName(model.Ten_LSP);
        if (string.IsNullOrWhiteSpace(normalizedName))
        {
            return ServiceResult.Fail("Tên không được để trống.");
        }

        if (normalizedName.Length > 120)
        {
            return ServiceResult.Fail("Tên tối đa 120 ký tự.");
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

        return await dbContext.LoaiSanPhams.AnyAsync(
            x => (!ignoreId.HasValue || x.Loai_San_Pham_ID != ignoreId.Value)
                 && x.Ma_LSP.ToUpper() == compareValue,
            cancellationToken);
    }

    private static async Task<bool> ExistsByNameAsync(
        AppDbContext dbContext,
        string normalizedName,
        int? ignoreId,
        CancellationToken cancellationToken)
    {
        // Quy tac nghiep vu coi ten khong phan biet hoa-thuong.
        var compareValue = normalizedName.ToUpper();

        return await dbContext.LoaiSanPhams.AnyAsync(
            x => (!ignoreId.HasValue || x.Loai_San_Pham_ID != ignoreId.Value)
                 && x.Ten_LSP.ToUpper() == compareValue,
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
