using BlazorApp1.Models.Common;
using BlazorApp1.Models.LoaiSanPham;

namespace BlazorApp1.Services.Interfaces;

/// <summary>
/// Dich vu xu ly CRUD cho danh muc loai san pham.
/// </summary>
public interface ILoaiSanPhamService
{
    /// <summary>
    /// Lay danh sach loai san pham de hien thi tren man hinh danh muc.
    /// </summary>
    Task<IReadOnlyList<LoaiSanPhamListItemVm>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Lay du lieu chi tiet theo ID de do vao form sua.
    /// </summary>
    Task<ServiceResult<LoaiSanPhamUpsertVm>> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tao moi loai san pham theo quy tac nghiep vu hien hanh.
    /// </summary>
    Task<ServiceResult> CreateAsync(LoaiSanPhamUpsertVm model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cap nhat loai san pham theo ID.
    /// </summary>
    Task<ServiceResult> UpdateAsync(LoaiSanPhamUpsertVm model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Xoa ban ghi khoi danh sach hien thi (soft delete).
    /// </summary>
    Task<ServiceResult> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
