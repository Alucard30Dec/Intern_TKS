using BlazorApp1.Models.Common;
using BlazorApp1.Models.KhoUser;

namespace BlazorApp1.Services.Interfaces;

/// <summary>
/// Dich vu xu ly CRUD cho phan quyen kho-user (bai 6).
/// </summary>
public interface IKhoUserService
{
    /// <summary>
    /// Lay danh sach phan quyen kho-user de hien thi tren man hinh.
    /// </summary>
    Task<IReadOnlyList<KhoUserListItemVm>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Lay du lieu chi tiet theo ID de do vao form sua.
    /// </summary>
    Task<ServiceResult<KhoUserUpsertVm>> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tao moi phan quyen kho-user theo quy tac nghiep vu hien hanh.
    /// </summary>
    Task<ServiceResult> CreateAsync(KhoUserUpsertVm model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cap nhat phan quyen kho-user theo ID.
    /// </summary>
    Task<ServiceResult> UpdateAsync(KhoUserUpsertVm model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Xoa ban ghi khoi danh sach hien thi (soft delete).
    /// </summary>
    Task<ServiceResult> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
