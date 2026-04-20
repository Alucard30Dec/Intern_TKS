namespace BlazorApp1.Models.DonViTinh;

/// <summary>
/// Dong du lieu hien thi tren luoi danh sach don vi tinh.
/// </summary>
public sealed class DonViTinhListItemVm
{
    public int Don_Vi_Tinh_ID { get; set; }
    public string Ten_Don_Vi_Tinh { get; set; } = string.Empty;
    public string? Ghi_Chu { get; set; }
    public bool Is_Active { get; set; }
}
