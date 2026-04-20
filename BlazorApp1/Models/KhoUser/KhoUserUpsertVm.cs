using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Models.KhoUser;

/// <summary>
/// Du lieu them/sua phan quyen kho-user.
/// </summary>
public sealed class KhoUserUpsertVm
{
    public int Kho_User_ID { get; set; }

    [Required(ErrorMessage = "Mã đăng nhập không được để trống.")]
    [StringLength(100, ErrorMessage = "Mã đăng nhập tối đa 100 ký tự.")]
    public string Ma_Dang_Nhap { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Kho không được để trống.")]
    public int Kho_ID { get; set; }
}
