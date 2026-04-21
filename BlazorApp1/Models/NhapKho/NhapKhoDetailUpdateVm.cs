using System.ComponentModel.DataAnnotations;
using BlazorApp1.Models.Common;

namespace BlazorApp1.Models.NhapKho;

/// <summary>
/// Du lieu cap nhat 1 dong chi tiet phieu nhap kho (bai 9).
/// </summary>
public sealed class NhapKhoDetailUpdateVm
{
    [Range(1, int.MaxValue, ErrorMessage = "ID dòng chi tiết không hợp lệ.")]
    public int Nhap_Kho_Raw_Data_ID { get; set; }

    [Range(typeof(decimal), "1", BusinessValidationRules.MaxQuantityText, ErrorMessage = "Số lượng nhập phải từ 1 đến giới hạn hệ thống.")]
    public decimal SL_Nhap { get; set; }

    [Range(typeof(decimal), "0.01", BusinessValidationRules.MaxAmountText, ErrorMessage = "Đơn giá nhập phải lớn hơn 0 và trong giới hạn hệ thống.")]
    public decimal Don_Gia_Nhap { get; set; }
}
