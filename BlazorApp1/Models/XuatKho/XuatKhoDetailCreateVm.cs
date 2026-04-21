using System.ComponentModel.DataAnnotations;
using BlazorApp1.Models.Common;

namespace BlazorApp1.Models.XuatKho;

/// <summary>
/// Du lieu them moi 1 dong chi tiet phieu xuat kho (bai 13).
/// </summary>
public sealed class XuatKhoDetailCreateVm
{
    [Range(1, int.MaxValue, ErrorMessage = "Sản phẩm không được để trống.")]
    public int San_Pham_ID { get; set; }

    [Range(typeof(decimal), "1", BusinessValidationRules.MaxQuantityText, ErrorMessage = "Số lượng xuất phải từ 1 đến giới hạn hệ thống.")]
    public decimal SL_Xuat { get; set; }

    [Range(typeof(decimal), "0.01", BusinessValidationRules.MaxAmountText, ErrorMessage = "Đơn giá xuất phải lớn hơn 0 và trong giới hạn hệ thống.")]
    public decimal Don_Gia_Xuat { get; set; }
}
