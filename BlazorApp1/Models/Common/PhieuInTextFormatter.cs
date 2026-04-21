namespace BlazorApp1.Models.Common;

/// <summary>
/// Dinh dang noi dung text cho phieu in nhap/xuat kho.
/// </summary>
public static class PhieuInTextFormatter
{
    private const string DefaultPlaceholder = ".......................................................";

    public static string FormatDateLong(DateTime date)
    {
        return $"Ngày {date:dd} tháng {date:MM} năm {date:yyyy}";
    }

    public static string FormatSignatureDate(DateTime date)
    {
        return FormatDateLong(date);
    }

    public static string DisplayOrPlaceholder(string? value, string placeholder = DefaultPlaceholder)
    {
        return string.IsNullOrWhiteSpace(value) ? placeholder : value.Trim();
    }

    public static string ConvertMoneyToWords(decimal value, string? donViTien)
    {
        var normalizedCurrency = DonViTienOptions.Normalize(donViTien);
        if (string.Equals(normalizedCurrency, DonViTienOptions.Usd, StringComparison.Ordinal))
        {
            return ConvertUsdMoneyToWords(value);
        }

        var roundedValue = decimal.Round(value, 0, MidpointRounding.AwayFromZero);
        var amount = (long)roundedValue;
        if (amount == 0)
        {
            return "Không đồng.";
        }

        var words = ConvertNumberToVietnamese(amount).Trim();
        var suffix = value == roundedValue ? " đồng chẵn." : " đồng.";
        return $"{char.ToUpper(words[0])}{words[1..]}{suffix}";
    }

    private static string ConvertUsdMoneyToWords(decimal value)
    {
        var roundedValue = decimal.Round(value, 2, MidpointRounding.AwayFromZero);
        var absoluteValue = decimal.Abs(roundedValue);

        var dollars = (long)decimal.Truncate(absoluteValue);
        var cents = (int)decimal.Round((absoluteValue - dollars) * 100m, 0, MidpointRounding.AwayFromZero);
        if (cents == 100)
        {
            dollars++;
            cents = 0;
        }

        var dollarsText = dollars == 0
            ? "không"
            : ConvertNumberToVietnamese(dollars).Trim();
        var signedPrefix = roundedValue < 0 ? "Âm " : string.Empty;

        if (cents == 0)
        {
            return ToSentenceCase($"{signedPrefix}{dollarsText} đô la Mỹ chẵn.");
        }

        var centsText = ConvertNumberToVietnamese(cents).Trim();
        return ToSentenceCase($"{signedPrefix}{dollarsText} đô la Mỹ và {centsText} xu.");
    }

    private static string ConvertNumberToVietnamese(long number)
    {
        if (number == 0)
        {
            return "không";
        }

        var units = new[] { "", " nghìn", " triệu", " tỷ", " nghìn tỷ", " triệu tỷ" };
        var parts = new List<string>();
        var unitIndex = 0;

        while (number > 0)
        {
            var chunk = (int)(number % 1000);
            if (chunk > 0)
            {
                var prefix = ReadThreeDigits(chunk, unitIndex > 0 && number / 1000 > 0);
                parts.Insert(0, $"{prefix}{units[unitIndex]}");
            }

            number /= 1000;
            unitIndex++;
        }

        return string.Join(" ", parts.Where(x => !string.IsNullOrWhiteSpace(x)))
            .Replace("  ", " ")
            .Trim();
    }

    private static string ReadThreeDigits(int number, bool full)
    {
        var hundreds = number / 100;
        var tens = (number % 100) / 10;
        var ones = number % 10;
        var words = new List<string>();

        if (full || hundreds > 0)
        {
            words.Add($"{DigitWord(hundreds)} trăm");
        }

        if (tens > 1)
        {
            words.Add($"{DigitWord(tens)} mươi");
            if (ones == 1)
            {
                words.Add("mốt");
            }
            else if (ones == 5)
            {
                words.Add("lăm");
            }
            else if (ones > 0)
            {
                words.Add(DigitWord(ones));
            }
        }
        else if (tens == 1)
        {
            words.Add("mười");
            if (ones == 5)
            {
                words.Add("lăm");
            }
            else if (ones > 0)
            {
                words.Add(DigitWord(ones));
            }
        }
        else if (ones > 0)
        {
            if (hundreds > 0 || full)
            {
                words.Add("lẻ");
            }

            words.Add(ones == 5 && (hundreds > 0 || full) ? "năm" : DigitWord(ones));
        }

        return string.Join(" ", words).Trim();
    }

    private static string DigitWord(int digit)
    {
        return digit switch
        {
            0 => "không",
            1 => "một",
            2 => "hai",
            3 => "ba",
            4 => "bốn",
            5 => "năm",
            6 => "sáu",
            7 => "bảy",
            8 => "tám",
            9 => "chín",
            _ => string.Empty
        };
    }

    private static string ToSentenceCase(string value)
    {
        var trimmed = value.Trim();
        if (trimmed.Length == 0)
        {
            return string.Empty;
        }

        return char.ToUpper(trimmed[0]) + trimmed[1..];
    }
}
