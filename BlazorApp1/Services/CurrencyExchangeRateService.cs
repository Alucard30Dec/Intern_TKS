using System.Net.Http.Json;
using BlazorApp1.Models.Common;
using BlazorApp1.Services.Interfaces;

namespace BlazorApp1.Services;

/// <summary>
/// Lay ty gia tien te tu nguon du lieu cong khai va cache ngan han trong bo nho.
/// </summary>
public sealed class CurrencyExchangeRateService : ICurrencyExchangeRateService
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);
    private static readonly SemaphoreSlim CacheLock = new(1, 1);
    private static readonly Dictionary<string, (DateTimeOffset ExpiresAt, decimal Rate)> RateCache = new();

    private readonly HttpClient _httpClient;
    private readonly ILogger<CurrencyExchangeRateService> _logger;

    public CurrencyExchangeRateService(
        HttpClient httpClient,
        ILogger<CurrencyExchangeRateService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ServiceResult<decimal>> GetExchangeRateAsync(
        string fromCurrency,
        string toCurrency,
        CancellationToken cancellationToken = default)
    {
        var from = DonViTienOptions.Normalize(fromCurrency);
        var to = DonViTienOptions.Normalize(toCurrency);

        if (string.Equals(from, to, StringComparison.Ordinal))
        {
            return ServiceResult<decimal>.Ok(1m);
        }

        var cacheKey = $"{from}->{to}";
        var now = DateTimeOffset.UtcNow;

        await CacheLock.WaitAsync(cancellationToken);
        try
        {
            if (RateCache.TryGetValue(cacheKey, out var cached) && cached.ExpiresAt > now)
            {
                return ServiceResult<decimal>.Ok(cached.Rate);
            }
        }
        finally
        {
            CacheLock.Release();
        }

        try
        {
            var endpoint = $"v6/latest/{from}";
            var response = await _httpClient.GetFromJsonAsync<ExchangeRateApiResponse>(endpoint, cancellationToken);
            if (response is null || response.Rates is null || !response.Rates.TryGetValue(to, out var rate))
            {
                return ServiceResult<decimal>.Fail("Không lấy được tỉ giá hiện tại. Vui lòng thử lại sau.");
            }

            if (rate <= 0m)
            {
                return ServiceResult<decimal>.Fail("Tỉ giá hiện tại không hợp lệ. Vui lòng thử lại sau.");
            }

            await CacheLock.WaitAsync(cancellationToken);
            try
            {
                RateCache[cacheKey] = (DateTimeOffset.UtcNow.Add(CacheDuration), rate);
            }
            finally
            {
                CacheLock.Release();
            }

            return ServiceResult<decimal>.Ok(rate);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Fetch exchange rate failed. From={FromCurrency}, To={ToCurrency}",
                from,
                to);
            return ServiceResult<decimal>.Fail("Không thể cập nhật tỉ giá thực tế từ hệ thống. Vui lòng thử lại.");
        }
    }

    private sealed class ExchangeRateApiResponse
    {
        public string? Result { get; set; }
        public Dictionary<string, decimal>? Rates { get; set; }
    }
}
