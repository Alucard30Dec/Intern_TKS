using System.ComponentModel.DataAnnotations;
using BlazorApp1.Models.Common;

namespace BlazorApp1.Models.XuatKho;

/// <summary>
/// Du lieu cap nhat 1 dong chi tiet phieu xuat kho (bai 13).
/// </summary>
public sealed class XuatKhoDetailUpdateVm
{
    [Range(1, int.MaxValue, ErrorMessage = "ID dòng chi tiết không hợp lệ.")]
    public int Xuat_Kho_Raw_Data_ID { get; set; }

    [Range(typeof(decimal), "1", BusinessValidationRules.MaxQuantityText, ErrorMessage = "Số lượng xuất phải từ 1 đến giới hạn hệ thống.")]
    public decimal SL_Xuat { get; set; }

    [Range(typeof(decimal), "0.01", BusinessValidationRules.MaxAmountText, ErrorMessage = "Đơn giá xuất phải lớn hơn 0 và trong giới hạn hệ thống.")]
    public decimal Don_Gia_Xuat { get; set; }
}
