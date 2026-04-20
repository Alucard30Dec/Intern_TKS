using BlazorApp1.Models.Common;
using BlazorApp1.Models.NhaCungCap;

namespace BlazorApp1.Services.Interfaces;

/// <summary>
/// Dich vu xu ly CRUD cho danh muc nha cung cap.
/// </summary>
public interface INhaCungCapService
{
    /// <summary>
    /// Lay danh sach nha cung cap de hien thi tren man hinh danh muc.
    /// </summary>
    Task<IReadOnlyList<NhaCungCapListItemVm>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Lay du lieu chi tiet theo ID de do vao form sua.
    /// </summary>
    Task<ServiceResult<NhaCungCapUpsertVm>> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tao moi nha cung cap theo quy tac nghiep vu hien hanh.
    /// </summary>
    Task<ServiceResult> CreateAsync(NhaCungCapUpsertVm model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cap nhat nha cung cap theo ID.
    /// </summary>
    Task<ServiceResult> UpdateAsync(NhaCungCapUpsertVm model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Xoa ban ghi khoi danh sach hien thi (soft delete).
    /// </summary>
    Task<ServiceResult> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
