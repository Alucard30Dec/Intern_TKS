using BlazorApp1.Models.BaoCao;
using BlazorApp1.Models.Common;

namespace BlazorApp1.Services.Interfaces;

/// <summary>
/// Dich vu bao cao bai 15, 16, 17.
/// </summary>
public interface IBaoCaoService
{
    /// <summary>
    /// Lay bao cao chi tiet hang nhap theo khoang ngay.
    /// </summary>
    Task<ServiceResult<IReadOnlyList<BaoCaoChiTietNhapItemVm>>> GetBaoCaoChiTietNhapAsync(
        BaoCaoDieuKienVm dieuKien,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lay bao cao chi tiet hang xuat theo khoang ngay.
    /// </summary>
    Task<ServiceResult<IReadOnlyList<BaoCaoChiTietXuatItemVm>>> GetBaoCaoChiTietXuatAsync(
        BaoCaoDieuKienVm dieuKien,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lay bao cao xuat nhap ton theo khoang ngay.
    /// </summary>
    Task<ServiceResult<IReadOnlyList<BaoCaoXuatNhapTonItemVm>>> GetBaoCaoXuatNhapTonAsync(
        BaoCaoDieuKienVm dieuKien,
        CancellationToken cancellationToken = default);
}
