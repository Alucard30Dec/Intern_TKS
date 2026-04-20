using BlazorApp1.Domain.Entities;
using BlazorApp1.Infrastructure.Data;
using BlazorApp1.Models.Common;
using BlazorApp1.Models.Kho;
using BlazorApp1.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp1.Services;

/// <summary>
/// Xu ly nghiep vu bai 5: them, sua, xoa mem danh muc kho.
/// </summary>
public sealed class KhoService : IKhoService
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
    private readonly ILogger<KhoService> _logger;

    public KhoService(
        IDbContextFactory<AppDbContext> dbContextFactory,
        ILogger<KhoService> logger)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }

    /// <summary>
    /// Lay danh sach kho cho man hinh danh muc.
    /// </summary>
    public async Task<IReadOnlyList<KhoListItemVm>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await dbContext.Khos
            .AsNoTracking()
            .Where(x => x.Is_Active)
            .OrderBy(x => x.Ten_Kho)
            .Select(x => new KhoListItemVm
            {
                Kho_ID = x.Kho_ID,
                Ten_Kho = x.Ten_Kho,
                Ghi_Chu = x.Ghi_Chu,
                Is_Active = x.Is_Active
            })
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Lay chi tiet kho theo ID de nap vao form sua.
    /// </summary>
    public async Task<ServiceResult<KhoUpsertVm>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return ServiceResult<KhoUpsertVm>.Fail("ID không hợp lệ.");
        }

        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var entity = await dbContext.Khos
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Kho_ID == id, cancellationToken);

            if (entity is null)
            {
                return ServiceResult<KhoUpsertVm>.Fail("Không tìm thấy kho.");
            }

            return ServiceResult<KhoUpsertVm>.Ok(new KhoUpsertVm
            {
                Kho_ID = entity.Kho_ID,
                Ten_Kho = entity.Ten_Kho,
                Ghi_Chu = entity.Ghi_Chu
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Get Kho by id failed. Kho_ID={Kho_ID}", id);
            return ServiceResult<KhoUpsertVm>.Fail("Không thể tải thông tin kho.");
        }
    }

    /// <summary>
    /// Tao moi kho va tra ve ket qua nghiep vu.
    /// </summary>
    public async Task<ServiceResult> CreateAsync(KhoUpsertVm model, CancellationToken cancellationToken = default)
    {
        var validation = ValidateBusinessRules(model);
        if (!validation.Success)
        {
            return validation;
        }

        var normalizedName = NormalizeName(model.Ten_Kho);

        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var duplicated = await ExistsByNameAsync(dbContext, normalizedName, null, cancellationToken);
            if (duplicated)
            {
                return ServiceResult.Fail("Tên kho đã tồn tại.");
            }

            var entity = new Kho
            {
                Ten_Kho = normalizedName,
                Ghi_Chu = NormalizeNullableText(model.Ghi_Chu),
                Is_Active = true
            };

            dbContext.Khos.Add(entity);
            await dbContext.SaveChangesAsync(cancellationToken);
            return ServiceResult.Ok("Thêm kho thành công.");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Create Kho failed. Ten_Kho={Ten_Kho}", normalizedName);
            return ServiceResult.Fail("Không thể thêm kho. Có thể dữ liệu đã bị trùng.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while creating Kho. Ten_Kho={Ten_Kho}", normalizedName);
            return ServiceResult.Fail("Không thể thêm kho do lỗi hệ thống.");
        }
    }

    /// <summary>
    /// Cap nhat kho theo ID.
    /// </summary>
    public async Task<ServiceResult> UpdateAsync(KhoUpsertVm model, CancellationToken cancellationToken = default)
    {
        if (model.Kho_ID <= 0)
        {
            return ServiceResult.Fail("ID không hợp lệ.");
        }

        var validation = ValidateBusinessRules(model);
        if (!validation.Success)
        {
            return validation;
        }

        var normalizedName = NormalizeName(model.Ten_Kho);

        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var entity = await dbContext.Khos
                .FirstOrDefaultAsync(x => x.Kho_ID == model.Kho_ID, cancellationToken);

            if (entity is null)
            {
                return ServiceResult.Fail("Không tìm thấy kho để cập nhật.");
            }

            var duplicated = await ExistsByNameAsync(dbContext, normalizedName, model.Kho_ID, cancellationToken);
            if (duplicated)
            {
                return ServiceResult.Fail("Tên kho đã tồn tại.");
            }

            entity.Ten_Kho = normalizedName;
            entity.Ghi_Chu = NormalizeNullableText(model.Ghi_Chu);

            await dbContext.SaveChangesAsync(cancellationToken);
            return ServiceResult.Ok("Cập nhật kho thành công.");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Update Kho failed. Kho_ID={Kho_ID}", model.Kho_ID);
            return ServiceResult.Fail("Không thể cập nhật kho. Có thể dữ liệu đã bị trùng.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while updating Kho. Kho_ID={Kho_ID}", model.Kho_ID);
            return ServiceResult.Fail("Không thể cập nhật kho do lỗi hệ thống.");
        }
    }

    /// <summary>
    /// Xoa mem kho theo ID (an khoi UI, van giu du lieu trong DB).
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

            var entity = await dbContext.Khos
                .FirstOrDefaultAsync(x => x.Kho_ID == id, cancellationToken);

            if (entity is null)
            {
                return ServiceResult.Fail("Không tìm thấy kho để xóa.");
            }

            if (!entity.Is_Active)
            {
                return ServiceResult.Fail("Kho này đã được xóa khỏi danh sách hiển thị trước đó.");
            }

            entity.Is_Active = false;
            await dbContext.SaveChangesAsync(cancellationToken);
            return ServiceResult.Ok("Đã xóa khỏi danh sách hiển thị. Dữ liệu vẫn được lưu trong hệ thống.");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Soft delete Kho failed. Kho_ID={Kho_ID}", id);
            return ServiceResult.Fail("Không thể xóa khỏi danh sách hiển thị vì dữ liệu đang được ràng buộc ở nghiệp vụ khác.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while soft deleting Kho. Kho_ID={Kho_ID}", id);
            return ServiceResult.Fail("Không thể xóa khỏi danh sách hiển thị do lỗi hệ thống.");
        }
    }

    private static ServiceResult ValidateBusinessRules(KhoUpsertVm model)
    {
        if (model is null)
        {
            return ServiceResult.Fail("Dữ liệu không hợp lệ.");
        }

        var normalizedName = NormalizeName(model.Ten_Kho);
        if (string.IsNullOrWhiteSpace(normalizedName))
        {
            return ServiceResult.Fail("Tên kho không được để trống.");
        }

        if (normalizedName.Length > 150)
        {
            return ServiceResult.Fail("Tên kho tối đa 150 ký tự.");
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
        var compareValue = normalizedName.ToUpper();

        return await dbContext.Khos.AnyAsync(
            x => (!ignoreId.HasValue || x.Kho_ID != ignoreId.Value)
                 && x.Ten_Kho.ToUpper() == compareValue,
            cancellationToken);
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
