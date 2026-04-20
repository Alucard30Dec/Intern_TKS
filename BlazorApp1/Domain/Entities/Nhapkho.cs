using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Domain.Entities;

/// <summary>
/// Thuc the phieu nhap kho trong bai 7.
/// </summary>
public sealed class NhapKho
{
    public int Nhap_Kho_ID { get; set; }

    [Required]
    [MaxLength(50)]
    public string So_Phieu_Nhap_Kho { get; set; } = string.Empty;

    [Required]
    public int Kho_ID { get; set; }

    [Required]
    public int NCC_ID { get; set; }

    [Required]
    public DateTime Ngay_Nhap_Kho { get; set; }

    [MaxLength(255)]
    public string? Ghi_Chu { get; set; }

    public bool Is_Active { get; set; } = true;

    public Kho? Kho { get; set; }
    public NhaCungCap? NCC { get; set; }
    public ICollection<NhapKhoRawData> Nhap_Kho_Raw_Datas { get; set; } = new List<NhapKhoRawData>();
}
