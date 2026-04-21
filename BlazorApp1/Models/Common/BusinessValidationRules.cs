using System.Text.RegularExpressions;

namespace BlazorApp1.Models.Common;

/// <summary>
/// Quy tac validation nghiep vu dung chung cho toan bo module.
/// </summary>
public static class BusinessValidationRules
{
    public const string CodePattern = "^[A-Z0-9][A-Z0-9._/-]{0,49}$";
    public const string LoginPattern = "^[A-Z0-9][A-Z0-9._@-]{0,99}$";
    public const string CurrencyPattern = "^(VND|USD)$";

    public const string MaxQuantityText = "999999999999999999";
    public const string MaxAmountText = "9999999999999999.99";

    public const decimal MaxQuantity = 999999999999999999m;
    public const decimal MaxAmount = 9999999999999999.99m;

    public const int MinReportYear = 2000;
    public static int MaxReportYear => DateTime.Today.Year + 1;
    public static DateTime MinVoucherDate => new(MinReportYear, 1, 1);
    public static DateTime MaxVoucherDate => DateTime.Today;

    private static readonly Regex CodeRegex = new(CodePattern, RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly Regex LoginRegex = new(LoginPattern, RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static bool IsValidCode(string value)
    {
        return !string.IsNullOrWhiteSpace(value) && CodeRegex.IsMatch(value);
    }

    public static bool IsValidLogin(string value)
    {
        return !string.IsNullOrWhiteSpace(value) && LoginRegex.IsMatch(value);
    }

    public static bool IsValidAmount(decimal value)
    {
        return value > 0m && value <= MaxAmount;
    }

    public static bool IsValidQuantity(decimal value)
    {
        return value > 0m && value <= MaxQuantity;
    }

    public static bool IsValidVoucherDate(DateTime value)
    {
        var normalized = value.Date;
        return normalized >= MinVoucherDate && normalized <= MaxVoucherDate;
    }

    public static bool HasValidAmountScale(decimal value, string? donViTien)
    {
        var scale = GetDecimalScale(value);
        return string.Equals(DonViTienOptions.Normalize(donViTien), DonViTienOptions.Usd, StringComparison.Ordinal)
            ? scale <= 2
            : scale == 0;
    }

    private static int GetDecimalScale(decimal value)
    {
        var bits = decimal.GetBits(value);
        return (bits[3] >> 16) & 0x7F;
    }
}
