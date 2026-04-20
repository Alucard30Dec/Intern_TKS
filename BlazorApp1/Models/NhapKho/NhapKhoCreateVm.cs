using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Models.NhapKho;

/// <summary>
/// Du lieu tao moi phieu nhap kho.
/// </summary>
public sealed class NhapKhoCreateVm
{
    public int Nhap_Kho_ID { get; set; }

    [Required(ErrorMessage = "Số phiếu nhập không được để trống.")]
    [StringLength(50, ErrorMessage = "Số phiếu nhập tối đa 50 ký tự.")]
    public string So_Phieu_Nhap_Kho { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Kho không được để trống.")]
    public int Kho_ID { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Nhà cung cấp không được để trống.")]
    public int NCC_ID { get; set; }

    [Required(ErrorMessage = "Ngày nhập kho không được để trống.")]
    public DateTime Ngay_Nhap_Kho { get; set; } = DateTime.Today;

    [StringLength(255, ErrorMessage = "Ghi chú tối đa 255 ký tự.")]
    public string? Ghi_Chu { get; set; }

    public List<NhapKhoRawDataUpsertVm> Chi_Tiets { get; set; } =
    [
        new NhapKhoRawDataUpsertVm()
    ];
}
