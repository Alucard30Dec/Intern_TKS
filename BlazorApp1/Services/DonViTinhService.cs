using BlazorApp1.Domain.Entities;
using BlazorApp1.Infrastructure.Data;
using BlazorApp1.Models.Common;
using BlazorApp1.Models.DonViTinh;
using BlazorApp1.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp1.Services;

/// <summary>
/// Xu ly nghiep vu bai 1: them, sua, xoa danh muc don vi tinh.
/// </summary>
public sealed class DonViTinhService : IDonViTinhService
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
    private readonly ILogger<DonViTinhService> _logger;

    public DonViTinhService(
        IDbContextFactory<AppDbContext> dbContextFactory,
        ILogger<DonViTinhService> logger)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }

    /// <summary>
    /// Lay danh sach don vi tinh cho man hinh danh muc.
    /// </summary>
    public async Task<IReadOnlyList<DonViTinhListItemVm>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await dbContext.DonViTinhs
            .AsNoTracking()
            .OrderBy(x => x.Ten_Don_Vi_Tinh)
            .Select(x => new DonViTinhListItemVm
            {
                Don_Vi_Tinh_ID = x.Don_Vi_Tinh_ID,
                Ten_Don_Vi_Tinh = x.Ten_Don_Vi_Tinh,
                Ghi_Chu = x.Ghi_Chu
            })
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Lay chi tiet don vi tinh theo ID de nap vao form sua.
    /// </summary>
    public async Task<ServiceResult<DonViTinhUpsertVm>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return ServiceResult<DonViTinhUpsertVm>.Fail("ID không hợp lệ.");
        }

        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var entity = await dbContext.DonViTinhs
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Don_Vi_Tinh_ID == id, cancellationToken);

            if (entity is null)
            {
                return ServiceResult<DonViTinhUpsertVm>.Fail("Không tìm thấy đơn vị tính.");
            }

            return ServiceResult<DonViTinhUpsertVm>.Ok(new DonViTinhUpsertVm
            {
                Don_Vi_Tinh_ID = entity.Don_Vi_Tinh_ID,
                Ten_Don_Vi_Tinh = entity.Ten_Don_Vi_Tinh,
                Ghi_Chu = entity.Ghi_Chu
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Get DonViTinh by id failed. Don_Vi_Tinh_ID={Don_Vi_Tinh_ID}", id);
            return ServiceResult<DonViTinhUpsertVm>.Fail("Không thể tải thông tin đơn vị tính.");
        }
    }

    /// <summary>
    /// Tao moi don vi tinh va tra ve ket qua nghiep vu.
    /// </summary>
    public async Task<ServiceResult> CreateAsync(DonViTinhUpsertVm model, CancellationToken cancellationToken = default)
    {
        var validation = ValidateBusinessRules(model);
        if (!validation.Success)
        {
            return validation;
        }

        var normalizedName = NormalizeName(model.Ten_Don_Vi_Tinh);

        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var duplicated = await ExistsByNameAsync(dbContext, normalizedName, null, cancellationToken);
            if (duplicated)
            {
                return ServiceResult.Fail("Tên đơn vị tính đã tồn tại.");
            }

            var entity = new DonViTinh
            {
                Ten_Don_Vi_Tinh = normalizedName,
                // Luu null thay vi chuoi rong de tranh sai lech khi loc/bao cao du lieu "co ghi chu".
                Ghi_Chu = NormalizeNullableText(model.Ghi_Chu)
            };

            dbContext.DonViTinhs.Add(entity);
            await dbContext.SaveChangesAsync(cancellationToken);
            return ServiceResult.Ok("Thêm đơn vị tính thành công.");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Create DonViTinh failed. Name={Ten_Don_Vi_Tinh}", normalizedName);
            return ServiceResult.Fail("Không thể thêm đơn vị tính. Có thể dữ liệu đã bị trùng.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while creating DonViTinh. Name={Ten_Don_Vi_Tinh}", normalizedName);
            return ServiceResult.Fail("Không thể thêm đơn vị tính do lỗi hệ thống.");
        }
    }

    /// <summary>
    /// Cap nhat don vi tinh theo ID.
    /// </summary>
    public async Task<ServiceResult> UpdateAsync(DonViTinhUpsertVm model, CancellationToken cancellationToken = default)
    {
        if (model.Don_Vi_Tinh_ID <= 0)
        {
            return ServiceResult.Fail("ID không hợp lệ.");
        }

        var validation = ValidateBusinessRules(model);
        if (!validation.Success)
        {
            return validation;
        }

        var normalizedName = NormalizeName(model.Ten_Don_Vi_Tinh);

        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var entity = await dbContext.DonViTinhs
                .FirstOrDefaultAsync(x => x.Don_Vi_Tinh_ID == model.Don_Vi_Tinh_ID, cancellationToken);

            if (entity is null)
            {
                return ServiceResult.Fail("Không tìm thấy đơn vị tính để cập nhật.");
            }

            var duplicated = await ExistsByNameAsync(dbContext, normalizedName, model.Don_Vi_Tinh_ID, cancellationToken);
            if (duplicated)
            {
                return ServiceResult.Fail("Tên đơn vị tính đã tồn tại.");
            }

            entity.Ten_Don_Vi_Tinh = normalizedName;
            // Giu quy uoc luu null dong nhat voi tao moi va voi data cu.
            entity.Ghi_Chu = NormalizeNullableText(model.Ghi_Chu);

            await dbContext.SaveChangesAsync(cancellationToken);
            return ServiceResult.Ok("Cập nhật đơn vị tính thành công.");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Update DonViTinh failed. Don_Vi_Tinh_ID={Don_Vi_Tinh_ID}", model.Don_Vi_Tinh_ID);
            return ServiceResult.Fail("Không thể cập nhật đơn vị tính. Có thể dữ liệu đã bị trùng.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while updating DonViTinh. Don_Vi_Tinh_ID={Don_Vi_Tinh_ID}", model.Don_Vi_Tinh_ID);
            return ServiceResult.Fail("Không thể cập nhật đơn vị tính do lỗi hệ thống.");
        }
    }

    /// <summary>
    /// Xoa don vi tinh theo ID.
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

            var entity = await dbContext.DonViTinhs
                .FirstOrDefaultAsync(x => x.Don_Vi_Tinh_ID == id, cancellationToken);

            if (entity is null)
            {
                return ServiceResult.Fail("Không tìm thấy đơn vị tính để xóa.");
            }

            dbContext.DonViTinhs.Remove(entity);
            await dbContext.SaveChangesAsync(cancellationToken);
            return ServiceResult.Ok("Xóa đơn vị tính thành công.");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Delete DonViTinh failed. Don_Vi_Tinh_ID={Don_Vi_Tinh_ID}", id);
            return ServiceResult.Fail("Không thể xóa đơn vị tính. Dữ liệu có thể đang được sử dụng ở màn hình khác.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while deleting DonViTinh. Don_Vi_Tinh_ID={Don_Vi_Tinh_ID}", id);
            return ServiceResult.Fail("Không thể xóa đơn vị tính do lỗi hệ thống.");
        }
    }

    private static ServiceResult ValidateBusinessRules(DonViTinhUpsertVm model)
    {
        if (model is null)
        {
            return ServiceResult.Fail("Dữ liệu không hợp lệ.");
        }

        var normalizedName = NormalizeName(model.Ten_Don_Vi_Tinh);

        if (string.IsNullOrWhiteSpace(normalizedName))
        {
            return ServiceResult.Fail("Tên đơn vị tính không được để trống.");
        }

        if (normalizedName.Length > 100)
        {
            return ServiceResult.Fail("Tên đơn vị tính tối đa 100 ký tự.");
        }

        var ghiChu = NormalizeNullableText(model.Ghi_Chu);
        if (ghiChu is not null && ghiChu.Length > 255)
        {
            return ServiceResult.Fail("Ghi chú tối đa 255 ký tự.");
        }

        return ServiceResult.Ok();
    }

    private static async Task<bool> ExistsByNameAsync(
        AppDbContext dbContext,
        string normalizedName,
        int? ignoreId,
        CancellationToken cancellationToken)
    {
        // Quy tac nghiep vu coi "Cai" va "CAI" la trung; so sanh upper de dam bao nhat quan o moi cau hinh collation.
        var compareValue = normalizedName.ToUpper();

        return await dbContext.DonViTinhs.AnyAsync(
            x => (!ignoreId.HasValue || x.Don_Vi_Tinh_ID != ignoreId.Value)
                 && x.Ten_Don_Vi_Tinh.ToUpper() == compareValue,
            cancellationToken);
    }

    private static string NormalizeName(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        // Gop nhieu khoang trang lien tiep ve mot khoang trang de tranh trung du lieu "ao".
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



