using System.ComponentModel.DataAnnotations;
using BlazorApp1.Models.Common;

namespace BlazorApp1.Models.BaoCao;

/// <summary>
/// Dieu kien loc bao cao cho bai 15, 16, 17.
/// </summary>
public sealed class BaoCaoDieuKienVm : IValidatableObject
{
    public int? Kho_ID { get; set; }
    public BaoCaoKieuLoc Kieu_Loc { get; set; } = BaoCaoKieuLoc.KhoangNgay;
    public DateTime Tu_Ngay { get; set; } = DateTime.Today.AddDays(-30);
    public DateTime Den_Ngay { get; set; } = DateTime.Today;

    [Range(1, 12, ErrorMessage = "Tháng phải từ 1 đến 12.")]
    public int Thang { get; set; } = DateTime.Today.Month;

    [Range(1, 4, ErrorMessage = "Quý phải từ 1 đến 4.")]
    public int Quy { get; set; } = ((DateTime.Today.Month - 1) / 3) + 1;

    public int Nam { get; set; } = DateTime.Today.Year;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Kho_ID.HasValue && Kho_ID.Value <= 0)
        {
            yield return new ValidationResult(
                "Kho không hợp lệ.",
                [nameof(Kho_ID)]);
        }

        if (Nam < BusinessValidationRules.MinReportYear || Nam > BusinessValidationRules.MaxReportYear)
        {
            yield return new ValidationResult(
                $"Năm báo cáo chỉ được từ {BusinessValidationRules.MinReportYear} đến {BusinessValidationRules.MaxReportYear}.",
                [nameof(Nam)]);
        }

        if (Kieu_Loc != BaoCaoKieuLoc.KhoangNgay)
        {
            yield break;
        }

        var tuNgay = Tu_Ngay.Date;
        var denNgay = Den_Ngay.Date;
        if (tuNgay == default || denNgay == default)
        {
            yield return new ValidationResult(
                "Từ ngày và đến ngày không được để trống.",
                [nameof(Tu_Ngay), nameof(Den_Ngay)]);
            yield break;
        }

        if (tuNgay > denNgay)
        {
            yield return new ValidationResult(
                "Khoảng ngày không hợp lệ: Từ ngày phải nhỏ hơn hoặc bằng đến ngày.",
                [nameof(Tu_Ngay), nameof(Den_Ngay)]);
        }

        if (denNgay > DateTime.Today)
        {
            yield return new ValidationResult(
                $"Khoảng ngày báo cáo không được lớn hơn ngày hiện tại ({DateTime.Today:dd/MM/yyyy}).",
                [nameof(Den_Ngay)]);
        }
    }
}
