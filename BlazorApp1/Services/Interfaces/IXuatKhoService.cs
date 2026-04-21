using BlazorApp1.Models.Common;
using BlazorApp1.Models.XuatKho;

namespace BlazorApp1.Services.Interfaces;

/// <summary>
/// Dich vu xu ly nghiep vu phieu xuat kho (bai 11 den bai 13).
/// </summary>
public interface IXuatKhoService
{
    /// <summary>
    /// Lay danh sach phieu xuat kho de hien thi tren man hinh quan ly.
    /// </summary>
    Task<IReadOnlyList<XuatKhoListItemVm>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Lay danh sach chi tiet cua mot phieu xuat kho.
    /// </summary>
    Task<ServiceResult<IReadOnlyList<XuatKhoDetailListItemVm>>> GetDetailsAsync(int xuatKhoId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tao moi phieu xuat kho.
    /// </summary>
    Task<ServiceResult> CreateAsync(XuatKhoCreateVm model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Hieu chinh thong tin phieu xuat kho (phan header - bai 12).
    /// </summary>
    Task<ServiceResult> UpdateHeaderAsync(XuatKhoCreateVm model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Them dong chi tiet vao phieu xuat kho (bai 13).
    /// </summary>
    Task<ServiceResult> AddDetailAsync(int xuatKhoId, XuatKhoDetailCreateVm model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Hieu chinh dong chi tiet phieu xuat kho (chi cho phep sua so luong, don gia - bai 13).
    /// </summary>
    Task<ServiceResult> UpdateDetailAsync(XuatKhoDetailUpdateVm model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Xoa mem dong chi tiet phieu xuat kho (bai 13).
    /// </summary>
    Task<ServiceResult> DeleteDetailAsync(int xuatKhoRawDataId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Xoa ban ghi khoi danh sach hien thi (soft delete).
    /// </summary>
    Task<ServiceResult> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
