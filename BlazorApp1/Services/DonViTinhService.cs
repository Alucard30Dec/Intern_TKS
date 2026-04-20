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
            .OrderBy(x => x.TenDonViTinh)
            .Select(x => new DonViTinhListItemVm
            {
                Id = x.Id,
                TenDonViTinh = x.TenDonViTinh,
                GhiChu = x.GhiChu
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
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (entity is null)
            {
                return ServiceResult<DonViTinhUpsertVm>.Fail("Không tìm thấy đơn vị tính.");
            }

            return ServiceResult<DonViTinhUpsertVm>.Ok(new DonViTinhUpsertVm
            {
                Id = entity.Id,
                TenDonViTinh = entity.TenDonViTinh,
                GhiChu = entity.GhiChu
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Get DonViTinh by id failed. Id={Id}", id);
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

        var normalizedName = NormalizeName(model.TenDonViTinh);

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
                TenDonViTinh = normalizedName,
                // Luu null thay vi chuoi rong de tranh sai lech khi loc/bao cao du lieu "co ghi chu".
                GhiChu = NormalizeNullableText(model.GhiChu)
            };

            dbContext.DonViTinhs.Add(entity);
            await dbContext.SaveChangesAsync(cancellationToken);
            return ServiceResult.Ok("Thêm đơn vị tính thành công.");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Create DonViTinh failed. Name={TenDonViTinh}", normalizedName);
            return ServiceResult.Fail("Không thể thêm đơn vị tính. Có thể dữ liệu đã bị trùng.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while creating DonViTinh. Name={TenDonViTinh}", normalizedName);
            return ServiceResult.Fail("Không thể thêm đơn vị tính do lỗi hệ thống.");
        }
    }

    /// <summary>
    /// Cap nhat don vi tinh theo ID.
    /// </summary>
    public async Task<ServiceResult> UpdateAsync(DonViTinhUpsertVm model, CancellationToken cancellationToken = default)
    {
        if (model.Id <= 0)
        {
            return ServiceResult.Fail("ID không hợp lệ.");
        }

        var validation = ValidateBusinessRules(model);
        if (!validation.Success)
        {
            return validation;
        }

        var normalizedName = NormalizeName(model.TenDonViTinh);

        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var entity = await dbContext.DonViTinhs
                .FirstOrDefaultAsync(x => x.Id == model.Id, cancellationToken);

            if (entity is null)
            {
                return ServiceResult.Fail("Không tìm thấy đơn vị tính để cập nhật.");
            }

            var duplicated = await ExistsByNameAsync(dbContext, normalizedName, model.Id, cancellationToken);
            if (duplicated)
            {
                return ServiceResult.Fail("Tên đơn vị tính đã tồn tại.");
            }

            entity.TenDonViTinh = normalizedName;
            // Giu quy uoc luu null dong nhat voi tao moi va voi data cu.
            entity.GhiChu = NormalizeNullableText(model.GhiChu);

            await dbContext.SaveChangesAsync(cancellationToken);
            return ServiceResult.Ok("Cập nhật đơn vị tính thành công.");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Update DonViTinh failed. Id={Id}", model.Id);
            return ServiceResult.Fail("Không thể cập nhật đơn vị tính. Có thể dữ liệu đã bị trùng.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while updating DonViTinh. Id={Id}", model.Id);
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
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

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
            _logger.LogError(ex, "Delete DonViTinh failed. Id={Id}", id);
            return ServiceResult.Fail("Không thể xóa đơn vị tính. Dữ liệu có thể đang được sử dụng ở màn hình khác.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while deleting DonViTinh. Id={Id}", id);
            return ServiceResult.Fail("Không thể xóa đơn vị tính do lỗi hệ thống.");
        }
    }

    private static ServiceResult ValidateBusinessRules(DonViTinhUpsertVm model)
    {
        if (model is null)
        {
            return ServiceResult.Fail("Dữ liệu không hợp lệ.");
        }

        var normalizedName = NormalizeName(model.TenDonViTinh);

        if (string.IsNullOrWhiteSpace(normalizedName))
        {
            return ServiceResult.Fail("Tên đơn vị tính không được để trống.");
        }

        if (normalizedName.Length > 100)
        {
            return ServiceResult.Fail("Tên đơn vị tính tối đa 100 ký tự.");
        }

        var ghiChu = NormalizeNullableText(model.GhiChu);
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
            x => (!ignoreId.HasValue || x.Id != ignoreId.Value)
                 && x.TenDonViTinh.ToUpper() == compareValue,
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
