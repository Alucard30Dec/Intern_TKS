namespace BlazorApp1.Models.NhapKho;

/// <summary>
/// Du lieu hien thi danh sach phieu nhap kho.
/// </summary>
public sealed class NhapKhoListItemVm
{
    public int Nhap_Kho_ID { get; set; }
    public string So_Phieu_Nhap_Kho { get; set; } = string.Empty;
    public int Kho_ID { get; set; }
    public string Ten_Kho { get; set; } = string.Empty;
    public int NCC_ID { get; set; }
    public string Ten_NCC { get; set; } = string.Empty;
    public DateTime Ngay_Nhap_Kho { get; set; }
    public string Don_Vi_Tien { get; set; } = BlazorApp1.Models.Common.DonViTienOptions.Vnd;
    public string? Ghi_Chu { get; set; }
    public int So_Dong_Chi_Tiet { get; set; }
    public decimal Tong_Tri_Gia { get; set; }
    public bool Is_Active { get; set; }
}
