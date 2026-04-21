using System.Globalization;

namespace BlazorApp1.Models.Common;

/// <summary>
/// Quy uoc don vi tien te cho phieu nhap, phieu xuat va bao cao.
/// </summary>
public static class DonViTienOptions
{
    public const string Vnd = "VND";
    public const string Usd = "USD";

    private static readonly CultureInfo ViVn = CultureInfo.GetCultureInfo("vi-VN");

    public static IReadOnlyList<string> SupportedValues { get; } = [Vnd, Usd];

    public static string Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Vnd;
        }

        var normalized = value.Trim().ToUpperInvariant();
        return SupportedValues.Contains(normalized) ? normalized : Vnd;
    }

    public static bool IsSupported(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        return SupportedValues.Contains(value.Trim().ToUpperInvariant());
    }

    public static bool UsesDecimalAmount(string? donViTien)
    {
        return string.Equals(Normalize(donViTien), Usd, StringComparison.Ordinal);
    }

    public static decimal RoundAmount(decimal value, string? donViTien)
    {
        var scale = UsesDecimalAmount(donViTien) ? 2 : 0;
        return decimal.Round(value, scale, MidpointRounding.AwayFromZero);
    }

    public static string FormatAmount(decimal value, string? donViTien)
    {
        var rounded = RoundAmount(value, donViTien);
        return UsesDecimalAmount(donViTien)
            ? rounded.ToString("N2", ViVn)
            : rounded.ToString("N0", ViVn);
    }

    public static string FormatAmountInput(decimal value, string? donViTien)
    {
        var rounded = RoundAmount(value, donViTien);
        return UsesDecimalAmount(donViTien)
            ? rounded.ToString("0.00", CultureInfo.InvariantCulture)
            : rounded.ToString("0", CultureInfo.InvariantCulture);
    }

    public static string FormatQuantity(decimal value)
    {
        return decimal.Round(value, 0, MidpointRounding.AwayFromZero).ToString("N0", ViVn);
    }
}
