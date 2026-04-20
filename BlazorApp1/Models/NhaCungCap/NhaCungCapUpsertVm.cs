using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Models.NhaCungCap;

/// <summary>
/// Du lieu them/sua nha cung cap.
/// </summary>
public sealed class NhaCungCapUpsertVm
{
    public int NCC_ID { get; set; }

    [Required(ErrorMessage = "Mã nhà cung cấp không được để trống.")]
    [StringLength(50, ErrorMessage = "Mã nhà cung cấp tối đa 50 ký tự.")]
    public string Ma_NCC { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tên nhà cung cấp không được để trống.")]
    [StringLength(150, ErrorMessage = "Tên nhà cung cấp tối đa 150 ký tự.")]
    public string Ten_NCC { get; set; } = string.Empty;

    [StringLength(255, ErrorMessage = "Ghi chú tối đa 255 ký tự.")]
    public string? Ghi_Chu { get; set; }
}
