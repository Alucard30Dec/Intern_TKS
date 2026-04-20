namespace BlazorApp1.Models.DonViTinh;

/// <summary>
/// Dong du lieu hien thi tren luoi danh sach don vi tinh.
/// </summary>
public sealed class DonViTinhListItemVm
{
    public int Id { get; set; }
    public string TenDonViTinh { get; set; } = string.Empty;
    public string? GhiChu { get; set; }
}
