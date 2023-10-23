using ForeignExchange.Api.Models;
using ForeignExchange.Api.Repositories;

namespace ForeignExchange.Api.Tests.Unit;

public class FakeRatesRepository : IRatesRepository
{
    public async Task<FxRate?> GetRateAsync(string baseCurrency, string quoteCurrency)
    {
        if (baseCurrency == "GBP" && quoteCurrency == "USD")
        {
            return new FxRate
            {
                FromCurrency = baseCurrency,
                ToCurrency = quoteCurrency,
                Rate = 1.6m
            };
        }

        return default;
    }
}
