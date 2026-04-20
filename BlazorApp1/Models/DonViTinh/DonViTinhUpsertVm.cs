using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Models.DonViTinh;

/// <summary>
/// Du lieu nhap/xuat cua form them sua don vi tinh.
/// </summary>
public sealed class DonViTinhUpsertVm
{
    public int Don_Vi_Tinh_ID { get; set; }

    [Required(ErrorMessage = "Tên đơn vị tính không được để trống.")]
    [StringLength(100, ErrorMessage = "Tên đơn vị tính tối đa 100 ký tự.")]
    public string Ten_Don_Vi_Tinh { get; set; } = string.Empty;

    [StringLength(255, ErrorMessage = "Ghi chú tối đa 255 ký tự.")]
    public string? Ghi_Chu { get; set; }
}
