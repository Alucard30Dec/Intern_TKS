using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Models.XuatKho;

/// <summary>
/// Du lieu tao moi phieu xuat kho.
/// </summary>
public sealed class XuatKhoCreateVm
{
    public int Xuat_Kho_ID { get; set; }

    [Required(ErrorMessage = "Số phiếu xuất không được để trống.")]
    [StringLength(50, ErrorMessage = "Số phiếu xuất tối đa 50 ký tự.")]
    public string So_Phieu_Xuat_Kho { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Kho không được để trống.")]
    public int Kho_ID { get; set; }

    [Required(ErrorMessage = "Ngày xuất kho không được để trống.")]
    public DateTime Ngay_Xuat_Kho { get; set; } = DateTime.Today;

    [StringLength(255, ErrorMessage = "Ghi chú tối đa 255 ký tự.")]
    public string? Ghi_Chu { get; set; }

    public List<XuatKhoRawDataUpsertVm> Chi_Tiets { get; set; } =
    [
        new XuatKhoRawDataUpsertVm()
    ];
}
