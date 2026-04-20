using BlazorApp1.Models.Common;
using BlazorApp1.Models.SanPham;

namespace BlazorApp1.Services.Interfaces;

/// <summary>
/// Dich vu xu ly CRUD cho danh muc san pham.
/// </summary>
public interface ISanPhamService
{
    /// <summary>
    /// Lay danh sach san pham de hien thi tren man hinh danh muc.
    /// </summary>
    Task<IReadOnlyList<SanPhamListItemVm>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Lay du lieu chi tiet theo ID de do vao form sua.
    /// </summary>
    Task<ServiceResult<SanPhamUpsertVm>> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tao moi san pham theo quy tac nghiep vu hien hanh.
    /// </summary>
    Task<ServiceResult> CreateAsync(SanPhamUpsertVm model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cap nhat san pham theo ID.
    /// </summary>
    Task<ServiceResult> UpdateAsync(SanPhamUpsertVm model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Xoa ban ghi khoi danh sach hien thi (soft delete).
    /// </summary>
    Task<ServiceResult> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
