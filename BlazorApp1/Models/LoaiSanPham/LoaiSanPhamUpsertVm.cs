using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Models.LoaiSanPham;

/// <summary>
/// Du lieu them/sua loai san pham.
/// </summary>
public sealed class LoaiSanPhamUpsertVm
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Mã không được để trống.")]
    [StringLength(50, ErrorMessage = "Mã tối đa 50 ký tự.")]
    public string MaLsp { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tên không được để trống.")]
    [StringLength(120, ErrorMessage = "Tên tối đa 120 ký tự.")]
    public string TenLsp { get; set; } = string.Empty;

    [StringLength(255, ErrorMessage = "Ghi chú tối đa 255 ký tự.")]
    public string? GhiChu { get; set; }
}

