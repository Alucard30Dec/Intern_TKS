using System.ComponentModel.DataAnnotations;
using BlazorApp1.Models.Common;

namespace BlazorApp1.Models.XuatKho;

/// <summary>
/// Du lieu tao moi phieu xuat kho.
/// </summary>
public sealed class XuatKhoCreateVm : IValidatableObject
{
    public int Xuat_Kho_ID { get; set; }

    [Required(ErrorMessage = "Số phiếu xuất không được để trống.")]
    [StringLength(50, ErrorMessage = "Số phiếu xuất tối đa 50 ký tự.")]
    [RegularExpression(BusinessValidationRules.CodePattern, ErrorMessage = "Số phiếu xuất chỉ gồm chữ in hoa, số và các ký tự . _ / -.")]
    public string So_Phieu_Xuat_Kho { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Kho không được để trống.")]
    public int Kho_ID { get; set; }

    [Required(ErrorMessage = "Ngày xuất kho không được để trống.")]
    public DateTime Ngay_Xuat_Kho { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Đơn vị tiền không được để trống.")]
    [StringLength(3, ErrorMessage = "Đơn vị tiền không hợp lệ.")]
    [RegularExpression(BusinessValidationRules.CurrencyPattern, ErrorMessage = "Đơn vị tiền chỉ chấp nhận VND hoặc USD.")]
    public string Don_Vi_Tien { get; set; } = BlazorApp1.Models.Common.DonViTienOptions.Vnd;

    [StringLength(255, ErrorMessage = "Ghi chú tối đa 255 ký tự.")]
    public string? Ghi_Chu { get; set; }

    [MinLength(1, ErrorMessage = "Phiếu xuất phải có ít nhất 1 dòng chi tiết.")]
    public List<XuatKhoRawDataUpsertVm> Chi_Tiets { get; set; } =
    [
        new XuatKhoRawDataUpsertVm()
    ];

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!BusinessValidationRules.IsValidVoucherDate(Ngay_Xuat_Kho))
        {
            yield return new ValidationResult(
                $"Ngày xuất kho phải trong khoảng {BusinessValidationRules.MinVoucherDate:dd/MM/yyyy} đến {BusinessValidationRules.MaxVoucherDate:dd/MM/yyyy}.",
                [nameof(Ngay_Xuat_Kho)]);
        }

        if (!DonViTienOptions.IsSupported(Don_Vi_Tien))
        {
            yield return new ValidationResult(
                "Đơn vị tiền chỉ chấp nhận VND hoặc USD.",
                [nameof(Don_Vi_Tien)]);
        }

        if (Chi_Tiets is null || Chi_Tiets.Count == 0)
        {
            yield break;
        }

        var productIds = new HashSet<int>();
        for (var i = 0; i < Chi_Tiets.Count; i++)
        {
            var line = Chi_Tiets[i];
            var lineNo = i + 1;

            if (line.San_Pham_ID > 0 && !productIds.Add(line.San_Pham_ID))
            {
                yield return new ValidationResult(
                    $"Dòng {lineNo}: Sản phẩm đã bị trùng trong cùng phiếu xuất.",
                    [nameof(Chi_Tiets)]);
            }

            if (line.SL_Xuat > BusinessValidationRules.MaxQuantity)
            {
                yield return new ValidationResult(
                    $"Dòng {lineNo}: Số lượng xuất vượt quá giới hạn cho phép của hệ thống.",
                    [nameof(Chi_Tiets)]);
            }

            if (line.Don_Gia_Xuat > BusinessValidationRules.MaxAmount)
            {
                yield return new ValidationResult(
                    $"Dòng {lineNo}: Đơn giá xuất vượt quá giới hạn cho phép của hệ thống.",
                    [nameof(Chi_Tiets)]);
            }

            if (line.Don_Gia_Xuat > 0 && !BusinessValidationRules.HasValidAmountScale(line.Don_Gia_Xuat, Don_Vi_Tien))
            {
                var message = DonViTienOptions.UsesDecimalAmount(Don_Vi_Tien)
                    ? $"Dòng {lineNo}: Đơn giá xuất tối đa 2 chữ số thập phân khi đơn vị tiền là USD."
                    : $"Dòng {lineNo}: Đơn giá xuất không được có phần thập phân khi đơn vị tiền là VND.";

                yield return new ValidationResult(message, [nameof(Chi_Tiets)]);
            }
        }
    }
}
