using BlazorApp1.Models.Common;
using BlazorApp1.Models.NhapKho;

namespace BlazorApp1.Services.Interfaces;

/// <summary>
/// Dich vu xu ly nghiep vu phieu nhap kho (bai 7).
/// </summary>
public interface INhapKhoService
{
    /// <summary>
    /// Lay danh sach phieu nhap kho de hien thi tren man hinh quan ly.
    /// </summary>
    Task<IReadOnlyList<NhapKhoListItemVm>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Lay danh sach chi tiet cua mot phieu nhap kho.
    /// </summary>
    Task<ServiceResult<IReadOnlyList<NhapKhoDetailListItemVm>>> GetDetailsAsync(int nhapKhoId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tao moi phieu nhap kho.
    /// </summary>
    Task<ServiceResult> CreateAsync(NhapKhoCreateVm model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cap nhat phieu nhap kho.
    /// </summary>
    Task<ServiceResult> UpdateAsync(NhapKhoCreateVm model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Xoa ban ghi khoi danh sach hien thi (soft delete).
    /// </summary>
    Task<ServiceResult> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
