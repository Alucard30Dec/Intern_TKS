using BlazorApp1.Infrastructure.Data;
using BlazorApp1.Models.BaoCao;
using BlazorApp1.Models.Common;
using BlazorApp1.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp1.Services;

/// <summary>
/// Xu ly truy van bao cao chi tiet nhap, chi tiet xuat va xuat nhap ton.
/// </summary>
public sealed class BaoCaoService : IBaoCaoService
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
    private readonly ILogger<BaoCaoService> _logger;

    public BaoCaoService(
        IDbContextFactory<AppDbContext> dbContextFactory,
        ILogger<BaoCaoService> logger)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }

    public async Task<ServiceResult<IReadOnlyList<BaoCaoChiTietNhapItemVm>>> GetBaoCaoChiTietNhapAsync(
        BaoCaoDieuKienVm dieuKien,
        CancellationToken cancellationToken = default)
    {
        if (!TryNormalizeDateRange(dieuKien, out var tuNgayChuan, out var denNgayChuan, out var validationMessage))
        {
            return ServiceResult<IReadOnlyList<BaoCaoChiTietNhapItemVm>>.Fail(validationMessage);
        }

        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var items = await dbContext.NhapKhoRawDatas
                .AsNoTracking()
                .Where(x => x.Is_Active
                            && x.Nhap_Kho != null
                            && x.Nhap_Kho.Is_Active
                            && x.Nhap_Kho.Ngay_Nhap_Kho >= tuNgayChuan
                            && x.Nhap_Kho.Ngay_Nhap_Kho <= denNgayChuan)
                .OrderBy(x => x.Nhap_Kho!.Ngay_Nhap_Kho)
                .ThenBy(x => x.Nhap_Kho!.So_Phieu_Nhap_Kho)
                .ThenBy(x => x.Nhap_Kho_Raw_Data_ID)
                .Select(x => new BaoCaoChiTietNhapItemVm
                {
                    Ngay_Nhap = x.Nhap_Kho != null ? x.Nhap_Kho.Ngay_Nhap_Kho : tuNgayChuan,
                    So_Phieu_Nhap = x.Nhap_Kho != null ? x.Nhap_Kho.So_Phieu_Nhap_Kho : string.Empty,
                    Ten_NCC = x.Nhap_Kho != null && x.Nhap_Kho.NCC != null
                        ? x.Nhap_Kho.NCC.Ten_NCC
                        : string.Empty,
                    Ma_San_Pham = x.San_Pham != null ? x.San_Pham.Ma_San_Pham : string.Empty,
                    Ten_San_Pham = x.San_Pham != null ? x.San_Pham.Ten_San_Pham : string.Empty,
                    Don_Vi_Tien = x.Nhap_Kho != null ? x.Nhap_Kho.Don_Vi_Tien : DonViTienOptions.Vnd,
                    SL_Nhap = x.SL_Nhap,
                    Don_Gia = x.Don_Gia_Nhap
                })
                .ToListAsync(cancellationToken);

            return ServiceResult<IReadOnlyList<BaoCaoChiTietNhapItemVm>>.Ok(items);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Get report chi tiet nhap failed. TuNgay={TuNgay}, DenNgay={DenNgay}",
                tuNgayChuan,
                denNgayChuan);
            return ServiceResult<IReadOnlyList<BaoCaoChiTietNhapItemVm>>.Fail("Không thể tải báo cáo chi tiết hàng nhập.");
        }
    }

    public async Task<ServiceResult<IReadOnlyList<BaoCaoChiTietXuatItemVm>>> GetBaoCaoChiTietXuatAsync(
        BaoCaoDieuKienVm dieuKien,
        CancellationToken cancellationToken = default)
    {
        if (!TryNormalizeDateRange(dieuKien, out var tuNgayChuan, out var denNgayChuan, out var validationMessage))
        {
            return ServiceResult<IReadOnlyList<BaoCaoChiTietXuatItemVm>>.Fail(validationMessage);
        }

        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var items = await dbContext.XuatKhoRawDatas
                .AsNoTracking()
                .Where(x => x.Is_Active
                            && x.Xuat_Kho != null
                            && x.Xuat_Kho.Is_Active
                            && x.Xuat_Kho.Ngay_Xuat_Kho >= tuNgayChuan
                            && x.Xuat_Kho.Ngay_Xuat_Kho <= denNgayChuan)
                .OrderBy(x => x.Xuat_Kho!.Ngay_Xuat_Kho)
                .ThenBy(x => x.Xuat_Kho!.So_Phieu_Xuat_Kho)
                .ThenBy(x => x.Xuat_Kho_Raw_Data_ID)
                .Select(x => new BaoCaoChiTietXuatItemVm
                {
                    Ngay_Xuat = x.Xuat_Kho != null ? x.Xuat_Kho.Ngay_Xuat_Kho : tuNgayChuan,
                    So_Phieu_Xuat = x.Xuat_Kho != null ? x.Xuat_Kho.So_Phieu_Xuat_Kho : string.Empty,
                    Ma_San_Pham = x.San_Pham != null ? x.San_Pham.Ma_San_Pham : string.Empty,
                    Ten_San_Pham = x.San_Pham != null ? x.San_Pham.Ten_San_Pham : string.Empty,
                    Don_Vi_Tien = x.Xuat_Kho != null ? x.Xuat_Kho.Don_Vi_Tien : DonViTienOptions.Vnd,
                    SL_Xuat = x.SL_Xuat,
                    Don_Gia = x.Don_Gia_Xuat
                })
                .ToListAsync(cancellationToken);

            return ServiceResult<IReadOnlyList<BaoCaoChiTietXuatItemVm>>.Ok(items);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Get report chi tiet xuat failed. TuNgay={TuNgay}, DenNgay={DenNgay}",
                tuNgayChuan,
                denNgayChuan);
            return ServiceResult<IReadOnlyList<BaoCaoChiTietXuatItemVm>>.Fail("Không thể tải báo cáo chi tiết hàng xuất.");
        }
    }

    public async Task<ServiceResult<IReadOnlyList<BaoCaoXuatNhapTonItemVm>>> GetBaoCaoXuatNhapTonAsync(
        BaoCaoDieuKienVm dieuKien,
        CancellationToken cancellationToken = default)
    {
        if (!TryNormalizeDateRange(dieuKien, out var tuNgayChuan, out var denNgayChuan, out var validationMessage))
        {
            return ServiceResult<IReadOnlyList<BaoCaoXuatNhapTonItemVm>>.Fail(validationMessage);
        }

        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var nhapTruocKy = await dbContext.NhapKhoRawDatas
                .AsNoTracking()
                .Where(x => x.Is_Active
                            && x.Nhap_Kho != null
                            && x.Nhap_Kho.Is_Active
                            && x.Nhap_Kho.Ngay_Nhap_Kho < tuNgayChuan)
                .GroupBy(x => x.San_Pham_ID)
                .Select(x => new { San_Pham_ID = x.Key, SoLuong = x.Sum(y => y.SL_Nhap) })
                .ToDictionaryAsync(x => x.San_Pham_ID, x => x.SoLuong, cancellationToken);

            var xuatTruocKy = await dbContext.XuatKhoRawDatas
                .AsNoTracking()
                .Where(x => x.Is_Active
                            && x.Xuat_Kho != null
                            && x.Xuat_Kho.Is_Active
                            && x.Xuat_Kho.Ngay_Xuat_Kho < tuNgayChuan)
                .GroupBy(x => x.San_Pham_ID)
                .Select(x => new { San_Pham_ID = x.Key, SoLuong = x.Sum(y => y.SL_Xuat) })
                .ToDictionaryAsync(x => x.San_Pham_ID, x => x.SoLuong, cancellationToken);

            var nhapTrongKy = await dbContext.NhapKhoRawDatas
                .AsNoTracking()
                .Where(x => x.Is_Active
                            && x.Nhap_Kho != null
                            && x.Nhap_Kho.Is_Active
                            && x.Nhap_Kho.Ngay_Nhap_Kho >= tuNgayChuan
                            && x.Nhap_Kho.Ngay_Nhap_Kho <= denNgayChuan)
                .GroupBy(x => x.San_Pham_ID)
                .Select(x => new { San_Pham_ID = x.Key, SoLuong = x.Sum(y => y.SL_Nhap) })
                .ToDictionaryAsync(x => x.San_Pham_ID, x => x.SoLuong, cancellationToken);

            var xuatTrongKy = await dbContext.XuatKhoRawDatas
                .AsNoTracking()
                .Where(x => x.Is_Active
                            && x.Xuat_Kho != null
                            && x.Xuat_Kho.Is_Active
                            && x.Xuat_Kho.Ngay_Xuat_Kho >= tuNgayChuan
                            && x.Xuat_Kho.Ngay_Xuat_Kho <= denNgayChuan)
                .GroupBy(x => x.San_Pham_ID)
                .Select(x => new { San_Pham_ID = x.Key, SoLuong = x.Sum(y => y.SL_Xuat) })
                .ToDictionaryAsync(x => x.San_Pham_ID, x => x.SoLuong, cancellationToken);

            var productIds = nhapTruocKy.Keys
                .Concat(xuatTruocKy.Keys)
                .Concat(nhapTrongKy.Keys)
                .Concat(xuatTrongKy.Keys)
                .Distinct()
                .ToList();

            if (productIds.Count == 0)
            {
                return ServiceResult<IReadOnlyList<BaoCaoXuatNhapTonItemVm>>.Ok([]);
            }

            var sanPhams = await dbContext.SanPhams
                .AsNoTracking()
                .Where(x => productIds.Contains(x.San_Pham_ID))
                .Select(x => new
                {
                    x.San_Pham_ID,
                    x.Ma_San_Pham,
                    x.Ten_San_Pham
                })
                .ToListAsync(cancellationToken);

            var sanPhamDictionary = sanPhams.ToDictionary(x => x.San_Pham_ID);

            var items = productIds
                .Select(productId =>
                {
                    nhapTruocKy.TryGetValue(productId, out var nhapDauKy);
                    xuatTruocKy.TryGetValue(productId, out var xuatDauKy);
                    nhapTrongKy.TryGetValue(productId, out var nhapKyNay);
                    xuatTrongKy.TryGetValue(productId, out var xuatKyNay);

                    sanPhamDictionary.TryGetValue(productId, out var sanPham);

                    return new BaoCaoXuatNhapTonItemVm
                    {
                        San_Pham_ID = productId,
                        Ma_San_Pham = sanPham?.Ma_San_Pham ?? $"SP-{productId}",
                        Ten_San_Pham = sanPham?.Ten_San_Pham ?? "[Sản phẩm không còn tồn tại]",
                        SL_Dau_Ky = nhapDauKy - xuatDauKy,
                        SL_Nhap = nhapKyNay,
                        SL_Xuat = xuatKyNay
                    };
                })
                .OrderBy(x => x.Ma_San_Pham)
                .ThenBy(x => x.Ten_San_Pham)
                .ToList();

            return ServiceResult<IReadOnlyList<BaoCaoXuatNhapTonItemVm>>.Ok(items);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Get report xuat nhap ton failed. TuNgay={TuNgay}, DenNgay={DenNgay}",
                tuNgayChuan,
                denNgayChuan);
            return ServiceResult<IReadOnlyList<BaoCaoXuatNhapTonItemVm>>.Fail("Không thể tải báo cáo xuất nhập tồn.");
        }
    }

    private static bool TryNormalizeDateRange(
        BaoCaoDieuKienVm? dieuKien,
        out DateTime tuNgayChuan,
        out DateTime denNgayChuan,
        out string message)
    {
        tuNgayChuan = default;
        denNgayChuan = default;

        if (dieuKien is null)
        {
            message = "Điều kiện lọc báo cáo không hợp lệ.";
            return false;
        }

        if (!Enum.IsDefined(dieuKien.Kieu_Loc))
        {
            message = "Kiểu lọc báo cáo không hợp lệ.";
            return false;
        }

        var nam = dieuKien.Nam;
        if (nam < BusinessValidationRules.MinReportYear || nam > BusinessValidationRules.MaxReportYear)
        {
            message = $"Năm báo cáo chỉ được từ {BusinessValidationRules.MinReportYear} đến {BusinessValidationRules.MaxReportYear}.";
            return false;
        }

        switch (dieuKien.Kieu_Loc)
        {
            case BaoCaoKieuLoc.KhoangNgay:
                tuNgayChuan = dieuKien.Tu_Ngay.Date;
                denNgayChuan = dieuKien.Den_Ngay.Date;

                if (tuNgayChuan == default || denNgayChuan == default)
                {
                    message = "Từ ngày và đến ngày không được để trống.";
                    return false;
                }

                if (tuNgayChuan.Year < BusinessValidationRules.MinReportYear
                    || denNgayChuan.Year > BusinessValidationRules.MaxReportYear)
                {
                    message = $"Khoảng ngày chỉ được trong giai đoạn {BusinessValidationRules.MinReportYear} - {BusinessValidationRules.MaxReportYear}.";
                    return false;
                }
                break;

            case BaoCaoKieuLoc.Thang:
                if (dieuKien.Thang is < 1 or > 12)
                {
                    message = "Tháng báo cáo phải từ 1 đến 12.";
                    return false;
                }

                tuNgayChuan = new DateTime(nam, dieuKien.Thang, 1);
                denNgayChuan = tuNgayChuan.AddMonths(1).AddDays(-1);
                break;

            case BaoCaoKieuLoc.Quy:
                if (dieuKien.Quy is < 1 or > 4)
                {
                    message = "Quý báo cáo phải từ 1 đến 4.";
                    return false;
                }

                var startMonth = ((dieuKien.Quy - 1) * 3) + 1;
                tuNgayChuan = new DateTime(nam, startMonth, 1);
                denNgayChuan = tuNgayChuan.AddMonths(3).AddDays(-1);
                break;

            case BaoCaoKieuLoc.Nam:
                tuNgayChuan = new DateTime(nam, 1, 1);
                denNgayChuan = new DateTime(nam, 12, 31);
                break;

            default:
                message = "Kiểu lọc báo cáo không hợp lệ.";
                return false;
        }

        if (tuNgayChuan > denNgayChuan)
        {
            message = "Khoảng ngày không hợp lệ: Từ ngày phải nhỏ hơn hoặc bằng đến ngày.";
            return false;
        }

        if (tuNgayChuan > DateTime.Today || denNgayChuan > DateTime.Today)
        {
            message = $"Khoảng ngày báo cáo không được lớn hơn ngày hiện tại ({DateTime.Today:dd/MM/yyyy}).";
            return false;
        }

        if ((denNgayChuan - tuNgayChuan).TotalDays > 3662)
        {
            message = "Khoảng ngày báo cáo không được vượt quá 10 năm.";
            return false;
        }

        message = string.Empty;
        return true;
    }
}
