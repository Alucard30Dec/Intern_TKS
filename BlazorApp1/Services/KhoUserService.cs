using BlazorApp1.Domain.Entities;
using BlazorApp1.Infrastructure.Data;
using BlazorApp1.Models.Common;
using BlazorApp1.Models.KhoUser;
using BlazorApp1.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp1.Services;

/// <summary>
/// Xu ly nghiep vu bai 6: them, sua, xoa mem phan quyen kho-user.
/// </summary>
public sealed class KhoUserService : IKhoUserService
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
    private readonly ILogger<KhoUserService> _logger;

    public KhoUserService(
        IDbContextFactory<AppDbContext> dbContextFactory,
        ILogger<KhoUserService> logger)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }

    /// <summary>
    /// Lay danh sach phan quyen kho-user cho man hinh.
    /// </summary>
    public async Task<IReadOnlyList<KhoUserListItemVm>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await dbContext.KhoUsers
            .AsNoTracking()
            .Where(x => x.Is_Active)
            .OrderBy(x => x.Ma_Dang_Nhap)
            .ThenBy(x => x.Kho != null ? x.Kho.Ten_Kho : string.Empty)
            .Select(x => new KhoUserListItemVm
            {
                Kho_User_ID = x.Kho_User_ID,
                Ma_Dang_Nhap = x.Ma_Dang_Nhap,
                Kho_ID = x.Kho_ID,
                Ten_Kho = x.Kho != null ? x.Kho.Ten_Kho : string.Empty,
                Is_Active = x.Is_Active
            })
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Lay chi tiet phan quyen theo ID de nap vao form sua.
    /// </summary>
    public async Task<ServiceResult<KhoUserUpsertVm>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return ServiceResult<KhoUserUpsertVm>.Fail("ID không hợp lệ.");
        }

        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var entity = await dbContext.KhoUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Kho_User_ID == id, cancellationToken);

            if (entity is null)
            {
                return ServiceResult<KhoUserUpsertVm>.Fail("Không tìm thấy phân quyền kho-user.");
            }

            return ServiceResult<KhoUserUpsertVm>.Ok(new KhoUserUpsertVm
            {
                Kho_User_ID = entity.Kho_User_ID,
                Ma_Dang_Nhap = entity.Ma_Dang_Nhap,
                Kho_ID = entity.Kho_ID
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Get KhoUser by id failed. Kho_User_ID={Kho_User_ID}", id);
            return ServiceResult<KhoUserUpsertVm>.Fail("Không thể tải thông tin phân quyền kho-user.");
        }
    }

    /// <summary>
    /// Tao moi phan quyen kho-user va tra ve ket qua nghiep vu.
    /// </summary>
    public async Task<ServiceResult> CreateAsync(KhoUserUpsertVm model, CancellationToken cancellationToken = default)
    {
        var validation = ValidateBusinessRules(model);
        if (!validation.Success)
        {
            return validation;
        }

        var normalizedLogin = NormalizeLogin(model.Ma_Dang_Nhap);

        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            if (!await KhoExistsAsync(dbContext, model.Kho_ID, cancellationToken))
            {
                return ServiceResult.Fail("Kho không tồn tại hoặc đã ngưng sử dụng.");
            }

            var activeDuplicated = await dbContext.KhoUsers.AnyAsync(
                x => x.Ma_Dang_Nhap.ToUpper() == normalizedLogin.ToUpper()
                     && x.Kho_ID == model.Kho_ID
                     && x.Is_Active,
                cancellationToken);
            if (activeDuplicated)
            {
                return ServiceResult.Fail("Bộ mã đăng nhập và kho đã tồn tại.");
            }

            var inactiveRecord = await dbContext.KhoUsers.FirstOrDefaultAsync(
                x => x.Ma_Dang_Nhap.ToUpper() == normalizedLogin.ToUpper()
                     && x.Kho_ID == model.Kho_ID
                     && !x.Is_Active,
                cancellationToken);

            if (inactiveRecord is not null)
            {
                inactiveRecord.Is_Active = true;
                await dbContext.SaveChangesAsync(cancellationToken);
                return ServiceResult.Ok("Khôi phục phân quyền kho-user thành công.");
            }

            var entity = new KhoUser
            {
                Ma_Dang_Nhap = normalizedLogin,
                Kho_ID = model.Kho_ID,
                Is_Active = true
            };

            dbContext.KhoUsers.Add(entity);
            await dbContext.SaveChangesAsync(cancellationToken);
            return ServiceResult.Ok("Thêm phân quyền kho-user thành công.");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Create KhoUser failed. Ma_Dang_Nhap={Ma_Dang_Nhap}, Kho_ID={Kho_ID}", normalizedLogin, model.Kho_ID);
            return ServiceResult.Fail("Không thể thêm phân quyền kho-user. Có thể dữ liệu đã bị trùng.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while creating KhoUser. Ma_Dang_Nhap={Ma_Dang_Nhap}, Kho_ID={Kho_ID}", normalizedLogin, model.Kho_ID);
            return ServiceResult.Fail("Không thể thêm phân quyền kho-user do lỗi hệ thống.");
        }
    }

    /// <summary>
    /// Cap nhat phan quyen kho-user theo ID.
    /// </summary>
    public async Task<ServiceResult> UpdateAsync(KhoUserUpsertVm model, CancellationToken cancellationToken = default)
    {
        if (model.Kho_User_ID <= 0)
        {
            return ServiceResult.Fail("ID không hợp lệ.");
        }

        var validation = ValidateBusinessRules(model);
        if (!validation.Success)
        {
            return validation;
        }

        var normalizedLogin = NormalizeLogin(model.Ma_Dang_Nhap);

        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var entity = await dbContext.KhoUsers
                .FirstOrDefaultAsync(x => x.Kho_User_ID == model.Kho_User_ID, cancellationToken);

            if (entity is null)
            {
                return ServiceResult.Fail("Không tìm thấy phân quyền kho-user để cập nhật.");
            }

            if (!await KhoExistsAsync(dbContext, model.Kho_ID, cancellationToken))
            {
                return ServiceResult.Fail("Kho không tồn tại hoặc đã ngưng sử dụng.");
            }

            var duplicated = await dbContext.KhoUsers.AnyAsync(
                x => x.Kho_User_ID != model.Kho_User_ID
                     && x.Ma_Dang_Nhap.ToUpper() == normalizedLogin.ToUpper()
                     && x.Kho_ID == model.Kho_ID,
                cancellationToken);
            if (duplicated)
            {
                return ServiceResult.Fail("Bộ mã đăng nhập và kho đã tồn tại.");
            }

            entity.Ma_Dang_Nhap = normalizedLogin;
            entity.Kho_ID = model.Kho_ID;

            await dbContext.SaveChangesAsync(cancellationToken);
            return ServiceResult.Ok("Cập nhật phân quyền kho-user thành công.");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Update KhoUser failed. Kho_User_ID={Kho_User_ID}", model.Kho_User_ID);
            return ServiceResult.Fail("Không thể cập nhật phân quyền kho-user. Có thể dữ liệu đã bị trùng.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while updating KhoUser. Kho_User_ID={Kho_User_ID}", model.Kho_User_ID);
            return ServiceResult.Fail("Không thể cập nhật phân quyền kho-user do lỗi hệ thống.");
        }
    }

    /// <summary>
    /// Xoa mem phan quyen kho-user theo ID (an khoi UI, van giu du lieu trong DB).
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

            var entity = await dbContext.KhoUsers
                .FirstOrDefaultAsync(x => x.Kho_User_ID == id, cancellationToken);

            if (entity is null)
            {
                return ServiceResult.Fail("Không tìm thấy phân quyền kho-user để xóa.");
            }

            if (!entity.Is_Active)
            {
                return ServiceResult.Fail("Phân quyền kho-user này đã được xóa khỏi danh sách hiển thị trước đó.");
            }

            entity.Is_Active = false;
            await dbContext.SaveChangesAsync(cancellationToken);
            return ServiceResult.Ok("Đã xóa khỏi danh sách hiển thị. Dữ liệu vẫn được lưu trong hệ thống.");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Soft delete KhoUser failed. Kho_User_ID={Kho_User_ID}", id);
            return ServiceResult.Fail("Không thể xóa khỏi danh sách hiển thị vì dữ liệu đang được ràng buộc ở nghiệp vụ khác.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while soft deleting KhoUser. Kho_User_ID={Kho_User_ID}", id);
            return ServiceResult.Fail("Không thể xóa khỏi danh sách hiển thị do lỗi hệ thống.");
        }
    }

    private static ServiceResult ValidateBusinessRules(KhoUserUpsertVm model)
    {
        if (model is null)
        {
            return ServiceResult.Fail("Dữ liệu không hợp lệ.");
        }

        var normalizedLogin = NormalizeLogin(model.Ma_Dang_Nhap);
        if (string.IsNullOrWhiteSpace(normalizedLogin))
        {
            return ServiceResult.Fail("Mã đăng nhập không được để trống.");
        }

        if (normalizedLogin.Length > 100)
        {
            return ServiceResult.Fail("Mã đăng nhập tối đa 100 ký tự.");
        }

        if (model.Kho_ID <= 0)
        {
            return ServiceResult.Fail("Kho không được để trống.");
        }

        return ServiceResult.Ok();
    }

    private static Task<bool> KhoExistsAsync(
        AppDbContext dbContext,
        int khoId,
        CancellationToken cancellationToken)
    {
        return dbContext.Khos.AnyAsync(
            x => x.Kho_ID == khoId && x.Is_Active,
            cancellationToken);
    }

    private static string NormalizeLogin(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        return value.Trim().ToUpperInvariant();
    }
}
