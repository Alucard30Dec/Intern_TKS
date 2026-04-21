using BlazorApp1.Models.Common;
using BlazorApp1.Models.NhapKho;

namespace BlazorApp1.Services.Interfaces;

/// <summary>
/// Dich vu xu ly nghiep vu phieu nhap kho (bai 7 den bai 10).
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
    /// Lay du lieu in phieu nhap kho (bai 10).
    /// </summary>
    Task<ServiceResult<NhapKhoPrintVm>> GetPrintDataAsync(int nhapKhoId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tao moi phieu nhap kho.
    /// </summary>
    Task<ServiceResult> CreateAsync(NhapKhoCreateVm model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Hieu chinh thong tin phieu nhap kho (phan header).
    /// </summary>
    Task<ServiceResult> UpdateHeaderAsync(NhapKhoCreateVm model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Them dong chi tiet vao phieu nhap kho (bai 9).
    /// </summary>
    Task<ServiceResult> AddDetailAsync(int nhapKhoId, NhapKhoDetailCreateVm model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Hieu chinh dong chi tiet phieu nhap kho (chi cho phep sua so luong, don gia - bai 9).
    /// </summary>
    Task<ServiceResult> UpdateDetailAsync(NhapKhoDetailUpdateVm model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Xoa mem dong chi tiet phieu nhap kho (bai 9).
    /// </summary>
    Task<ServiceResult> DeleteDetailAsync(int nhapKhoRawDataId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Xoa ban ghi khoi danh sach hien thi (soft delete).
    /// </summary>
    Task<ServiceResult> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
