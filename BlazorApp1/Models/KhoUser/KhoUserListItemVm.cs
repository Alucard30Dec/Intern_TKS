namespace BlazorApp1.Models.KhoUser;

/// <summary>
/// Du lieu hien thi danh sach phan quyen kho-user.
/// </summary>
public sealed class KhoUserListItemVm
{
    public int Kho_User_ID { get; set; }
    public string Ma_Dang_Nhap { get; set; } = string.Empty;
    public int Kho_ID { get; set; }
    public string Ten_Kho { get; set; } = string.Empty;
    public bool Is_Active { get; set; }
}
