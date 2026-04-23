using BlazorApp1.Models.Common;

namespace BlazorApp1.Services.Interfaces;

/// <summary>
/// Cung cap ty gia tien te thuc te de quy doi giua VND va USD.
/// </summary>
public interface ICurrencyExchangeRateService
{
    /// <summary>
    /// Lay ty gia doi tien theo cap don vi tien.
    /// </summary>
    Task<ServiceResult<decimal>> GetExchangeRateAsync(
        string fromCurrency,
        string toCurrency,
        CancellationToken cancellationToken = default);
}
