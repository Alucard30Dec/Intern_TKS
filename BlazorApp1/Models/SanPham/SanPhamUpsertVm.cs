using System.ComponentModel.DataAnnotations;
using BlazorApp1.Models.Common;

namespace BlazorApp1.Models.SanPham;

/// <summary>
/// Du lieu them/sua san pham.
/// </summary>
public sealed class SanPhamUpsertVm
{
    public int San_Pham_ID { get; set; }

    [Required(ErrorMessage = "Mã sản phẩm không được để trống.")]
    [StringLength(50, ErrorMessage = "Mã sản phẩm tối đa 50 ký tự.")]
    [RegularExpression(BusinessValidationRules.CodePattern, ErrorMessage = "Mã sản phẩm chỉ gồm chữ in hoa, số và các ký tự . _ / -.")]
    public string Ma_San_Pham { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tên sản phẩm không được để trống.")]
    [StringLength(200, ErrorMessage = "Tên sản phẩm tối đa 200 ký tự.")]
    public string Ten_San_Pham { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Loại sản phẩm không được để trống.")]
    public int Loai_San_Pham_ID { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Đơn vị tính không được để trống.")]
    public int Don_Vi_Tinh_ID { get; set; }

    [StringLength(255, ErrorMessage = "Ghi chú tối đa 255 ký tự.")]
    public string? Ghi_Chu { get; set; }
}
