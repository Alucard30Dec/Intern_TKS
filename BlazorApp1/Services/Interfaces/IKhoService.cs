using BlazorApp1.Models.Common;
using BlazorApp1.Models.Kho;

namespace BlazorApp1.Services.Interfaces;

/// <summary>
/// Dich vu xu ly CRUD cho danh muc kho.
/// </summary>
public interface IKhoService
{
    /// <summary>
    /// Lay danh sach kho de hien thi tren man hinh danh muc.
    /// </summary>
    Task<IReadOnlyList<KhoListItemVm>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Lay du lieu chi tiet theo ID de do vao form sua.
    /// </summary>
    Task<ServiceResult<KhoUpsertVm>> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tao moi kho theo quy tac nghiep vu hien hanh.
    /// </summary>
    Task<ServiceResult> CreateAsync(KhoUpsertVm model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cap nhat kho theo ID.
    /// </summary>
    Task<ServiceResult> UpdateAsync(KhoUpsertVm model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Xoa ban ghi khoi danh sach hien thi (soft delete).
    /// </summary>
    Task<ServiceResult> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
