using System.ComponentModel.DataAnnotations;
using BlazorApp1.Models.Common;

namespace BlazorApp1.Models.LoaiSanPham;

/// <summary>
/// Du lieu them/sua loai san pham.
/// </summary>
public sealed class LoaiSanPhamUpsertVm
{
    public int Loai_San_Pham_ID { get; set; }

    [Required(ErrorMessage = "Mã không được để trống.")]
    [StringLength(50, ErrorMessage = "Mã tối đa 50 ký tự.")]
    [RegularExpression(BusinessValidationRules.CodePattern, ErrorMessage = "Mã chỉ gồm chữ in hoa, số và các ký tự . _ / -.")]
    public string Ma_LSP { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tên không được để trống.")]
    [StringLength(120, ErrorMessage = "Tên tối đa 120 ký tự.")]
    public string Ten_LSP { get; set; } = string.Empty;

    [StringLength(255, ErrorMessage = "Ghi chú tối đa 255 ký tự.")]
    public string? Ghi_Chu { get; set; }
}
