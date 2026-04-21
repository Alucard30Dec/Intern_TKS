using BlazorApp1.Domain.Entities;
using BlazorApp1.Infrastructure.Data;
using BlazorApp1.Models.Common;
using BlazorApp1.Models.NhaCungCap;
using BlazorApp1.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp1.Services;

/// <summary>
/// Xu ly nghiep vu bai 4: them, sua, xoa mem danh muc nha cung cap.
/// </summary>
public sealed class NhaCungCapService : INhaCungCapService
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
    private readonly ILogger<NhaCungCapService> _logger;

    public NhaCungCapService(
        IDbContextFactory<AppDbContext> dbContextFactory,
        ILogger<NhaCungCapService> logger)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }

    /// <summary>
    /// Lay danh sach nha cung cap cho man hinh danh muc.
    /// </summary>
    public async Task<IReadOnlyList<NhaCungCapListItemVm>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await dbContext.NhaCungCaps
            .AsNoTracking()
            .Where(x => x.Is_Active)
            .OrderBy(x => x.Ten_NCC)
            .ThenBy(x => x.Ma_NCC)
            .Select(x => new NhaCungCapListItemVm
            {
                NCC_ID = x.NCC_ID,
                Ma_NCC = x.Ma_NCC ?? string.Empty,
                Ten_NCC = x.Ten_NCC,
                Ghi_Chu = x.Ghi_Chu,
                Is_Active = x.Is_Active
            })
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Lay chi tiet nha cung cap theo ID de nap vao form sua.
    /// </summary>
    public async Task<ServiceResult<NhaCungCapUpsertVm>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return ServiceResult<NhaCungCapUpsertVm>.Fail("ID không hợp lệ.");
        }

        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var entity = await dbContext.NhaCungCaps
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.NCC_ID == id, cancellationToken);

            if (entity is null)
            {
                return ServiceResult<NhaCungCapUpsertVm>.Fail("Không tìm thấy nhà cung cấp.");
            }

            if (!entity.Is_Active)
            {
                return ServiceResult<NhaCungCapUpsertVm>.Fail("Nhà cung cấp đã ngưng sử dụng.");
            }

            return ServiceResult<NhaCungCapUpsertVm>.Ok(new NhaCungCapUpsertVm
            {
                NCC_ID = entity.NCC_ID,
                Ma_NCC = entity.Ma_NCC ?? string.Empty,
                Ten_NCC = entity.Ten_NCC,
                Ghi_Chu = entity.Ghi_Chu
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Get NhaCungCap by id failed. NCC_ID={NCC_ID}", id);
            return ServiceResult<NhaCungCapUpsertVm>.Fail("Không thể tải thông tin nhà cung cấp.");
        }
    }

    /// <summary>
    /// Tao moi nha cung cap va tra ve ket qua nghiep vu.
    /// </summary>
    public async Task<ServiceResult> CreateAsync(NhaCungCapUpsertVm model, CancellationToken cancellationToken = default)
    {
        var validation = ValidateBusinessRules(model);
        if (!validation.Success)
        {
            return validation;
        }

        var normalizedCode = NormalizeCode(model.Ma_NCC);
        var normalizedName = NormalizeName(model.Ten_NCC);

        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var duplicatedName = await ExistsByNameAsync(dbContext, normalizedName, null, cancellationToken);
            if (duplicatedName)
            {
                return ServiceResult.Fail("Tên nhà cung cấp đã tồn tại.");
            }

            var duplicatedCode = await ExistsByCodeAsync(dbContext, normalizedCode, null, cancellationToken);
            if (duplicatedCode)
            {
                return ServiceResult.Fail("Mã nhà cung cấp đã tồn tại.");
            }

            var entity = new NhaCungCap
            {
                Ma_NCC = normalizedCode,
                Ten_NCC = normalizedName,
                Ghi_Chu = NormalizeNullableText(model.Ghi_Chu),
                Is_Active = true
            };

            dbContext.NhaCungCaps.Add(entity);
            await dbContext.SaveChangesAsync(cancellationToken);
            return ServiceResult.Ok("Thêm nhà cung cấp thành công.");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Create NhaCungCap failed. Ma_NCC={Ma_NCC}, Ten_NCC={Ten_NCC}", normalizedCode, normalizedName);
            return ServiceResult.Fail("Không thể thêm nhà cung cấp. Mã hoặc tên nhà cung cấp có thể đã bị trùng.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while creating NhaCungCap. Ten_NCC={Ten_NCC}", normalizedName);
            return ServiceResult.Fail("Không thể thêm nhà cung cấp do lỗi hệ thống.");
        }
    }

    /// <summary>
    /// Cap nhat nha cung cap theo ID.
    /// </summary>
    public async Task<ServiceResult> UpdateAsync(NhaCungCapUpsertVm model, CancellationToken cancellationToken = default)
    {
        if (model.NCC_ID <= 0)
        {
            return ServiceResult.Fail("ID không hợp lệ.");
        }

        var validation = ValidateBusinessRules(model);
        if (!validation.Success)
        {
            return validation;
        }

        var normalizedCode = NormalizeCode(model.Ma_NCC);
        var normalizedName = NormalizeName(model.Ten_NCC);

        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var entity = await dbContext.NhaCungCaps
                .FirstOrDefaultAsync(x => x.NCC_ID == model.NCC_ID, cancellationToken);

            if (entity is null)
            {
                return ServiceResult.Fail("Không tìm thấy nhà cung cấp để cập nhật.");
            }

            if (!entity.Is_Active)
            {
                return ServiceResult.Fail("Nhà cung cấp đã ngưng sử dụng, không thể cập nhật.");
            }

            var duplicatedName = await ExistsByNameAsync(dbContext, normalizedName, model.NCC_ID, cancellationToken);
            if (duplicatedName)
            {
                return ServiceResult.Fail("Tên nhà cung cấp đã tồn tại.");
            }

            var duplicatedCode = await ExistsByCodeAsync(dbContext, normalizedCode, model.NCC_ID, cancellationToken);
            if (duplicatedCode)
            {
                return ServiceResult.Fail("Mã nhà cung cấp đã tồn tại.");
            }

            entity.Ma_NCC = normalizedCode;
            entity.Ten_NCC = normalizedName;
            entity.Ghi_Chu = NormalizeNullableText(model.Ghi_Chu);

            await dbContext.SaveChangesAsync(cancellationToken);
            return ServiceResult.Ok("Cập nhật nhà cung cấp thành công.");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Update NhaCungCap failed. NCC_ID={NCC_ID}", model.NCC_ID);
            return ServiceResult.Fail("Không thể cập nhật nhà cung cấp. Mã hoặc tên nhà cung cấp có thể đã bị trùng.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while updating NhaCungCap. NCC_ID={NCC_ID}", model.NCC_ID);
            return ServiceResult.Fail("Không thể cập nhật nhà cung cấp do lỗi hệ thống.");
        }
    }

    /// <summary>
    /// Xoa mem nha cung cap theo ID (an khoi UI, van giu du lieu trong DB).
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

            var entity = await dbContext.NhaCungCaps
                .FirstOrDefaultAsync(x => x.NCC_ID == id, cancellationToken);

            if (entity is null)
            {
                return ServiceResult.Fail("Không tìm thấy nhà cung cấp để xóa.");
            }

            if (!entity.Is_Active)
            {
                return ServiceResult.Fail("Nhà cung cấp này đã được xóa khỏi danh sách hiển thị trước đó.");
            }

            var hasNhapReference = await dbContext.NhapKhos.AnyAsync(
                x => x.Is_Active && x.NCC_ID == id,
                cancellationToken);
            if (hasNhapReference)
            {
                return ServiceResult.Fail("Không thể xóa nhà cung cấp vì đã phát sinh trong phiếu nhập kho.");
            }

            entity.Is_Active = false;
            await dbContext.SaveChangesAsync(cancellationToken);
            return ServiceResult.Ok("Đã xóa khỏi danh sách.");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Soft delete NhaCungCap failed. NCC_ID={NCC_ID}", id);
            return ServiceResult.Fail("Không thể xóa khỏi danh sách hiển thị vì dữ liệu đang được ràng buộc ở nghiệp vụ khác.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while soft deleting NhaCungCap. NCC_ID={NCC_ID}", id);
            return ServiceResult.Fail("Không thể xóa khỏi danh sách hiển thị do lỗi hệ thống.");
        }
    }

    private static ServiceResult ValidateBusinessRules(NhaCungCapUpsertVm model)
    {
        if (model is null)
        {
            return ServiceResult.Fail("Dữ liệu không hợp lệ.");
        }

        var normalizedName = NormalizeName(model.Ten_NCC);
        if (string.IsNullOrWhiteSpace(normalizedName))
        {
            return ServiceResult.Fail("Tên nhà cung cấp không được để trống.");
        }

        if (normalizedName.Length > 150)
        {
            return ServiceResult.Fail("Tên nhà cung cấp tối đa 150 ký tự.");
        }

        var normalizedCode = NormalizeCode(model.Ma_NCC);
        if (string.IsNullOrWhiteSpace(normalizedCode))
        {
            return ServiceResult.Fail("Mã nhà cung cấp không được để trống.");
        }

        if (normalizedCode.Length > 50)
        {
            return ServiceResult.Fail("Mã nhà cung cấp tối đa 50 ký tự.");
        }

        if (!BusinessValidationRules.IsValidCode(normalizedCode))
        {
            return ServiceResult.Fail("Mã nhà cung cấp chỉ gồm chữ in hoa, số và các ký tự . _ / -.");
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

        return await dbContext.NhaCungCaps.AnyAsync(
            x => (!ignoreId.HasValue || x.NCC_ID != ignoreId.Value)
                 && x.Ten_NCC.ToUpper() == compareValue,
            cancellationToken);
    }

    private static async Task<bool> ExistsByCodeAsync(
        AppDbContext dbContext,
        string normalizedCode,
        int? ignoreId,
        CancellationToken cancellationToken)
    {
        var compareValue = normalizedCode.ToUpper();

        return await dbContext.NhaCungCaps.AnyAsync(
            x => (!ignoreId.HasValue || x.NCC_ID != ignoreId.Value)
                 && x.Ma_NCC.ToUpper() == compareValue,
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

    private static string NormalizeCode(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        return value.Trim().ToUpperInvariant();
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
