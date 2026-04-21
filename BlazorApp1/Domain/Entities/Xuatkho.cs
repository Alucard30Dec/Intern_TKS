using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Domain.Entities;

/// <summary>
/// Thuc the phieu xuat kho trong bai 11.
/// </summary>
public sealed class XuatKho
{
    public int Xuat_Kho_ID { get; set; }

    [Required]
    [MaxLength(50)]
    public string So_Phieu_Xuat_Kho { get; set; } = string.Empty;

    [Required]
    public int Kho_ID { get; set; }

    [Required]
    public DateTime Ngay_Xuat_Kho { get; set; }

    [Required]
    [MaxLength(3)]
    public string Don_Vi_Tien { get; set; } = Models.Common.DonViTienOptions.Vnd;

    [MaxLength(255)]
    public string? Ghi_Chu { get; set; }

    public bool Is_Active { get; set; } = true;

    public Kho? Kho { get; set; }
    public ICollection<XuatKhoRawData> Xuat_Kho_Raw_Datas { get; set; } = new List<XuatKhoRawData>();
}
