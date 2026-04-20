using BlazorApp1.Models.Common;
using BlazorApp1.Models.DonViTinh;

namespace BlazorApp1.Services.Interfaces;

/// <summary>
/// Dich vu xu ly CRUD cho danh muc don vi tinh.
/// </summary>
public interface IDonViTinhService
{
    /// <summary>
    /// Lay danh sach don vi tinh de hien thi tren man hinh danh muc.
    /// </summary>
    Task<IReadOnlyList<DonViTinhListItemVm>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Lay du lieu chi tiet theo ID de do vao form sua.
    /// </summary>
    Task<ServiceResult<DonViTinhUpsertVm>> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tao moi don vi tinh theo quy tac nghiep vu hien hanh.
    /// </summary>
    Task<ServiceResult> CreateAsync(DonViTinhUpsertVm model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cap nhat don vi tinh theo ID.
    /// </summary>
    Task<ServiceResult> UpdateAsync(DonViTinhUpsertVm model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Xoa don vi tinh theo ID.
    /// </summary>
    Task<ServiceResult> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
