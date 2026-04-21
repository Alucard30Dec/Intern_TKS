namespace BlazorApp1.Models.Common;

/// <summary>
/// Tong tri gia theo tung don vi tien de tranh cong gop khac loai tien te.
/// </summary>
public sealed class TongTienTheoDonViVm
{
    public string Don_Vi_Tien { get; set; } = DonViTienOptions.Vnd;
    public decimal Tong_Tri_Gia { get; set; }
}
