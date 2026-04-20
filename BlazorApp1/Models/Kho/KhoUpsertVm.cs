using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Models.Kho;

/// <summary>
/// Du lieu them/sua kho.
/// </summary>
public sealed class KhoUpsertVm
{
    public int Kho_ID { get; set; }

    [Required(ErrorMessage = "Tên kho không được để trống.")]
    [StringLength(150, ErrorMessage = "Tên kho tối đa 150 ký tự.")]
    public string Ten_Kho { get; set; } = string.Empty;

    [StringLength(255, ErrorMessage = "Ghi chú tối đa 255 ký tự.")]
    public string? Ghi_Chu { get; set; }
}
