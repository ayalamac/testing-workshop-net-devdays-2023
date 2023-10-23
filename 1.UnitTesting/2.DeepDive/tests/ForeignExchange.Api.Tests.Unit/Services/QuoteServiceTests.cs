using Castle.DynamicProxy;
using FluentAssertions;
using ForeignExchange.Api.Database;
using ForeignExchange.Api.Logging;
using ForeignExchange.Api.Models;
using ForeignExchange.Api.Repositories;
using ForeignExchange.Api.Services;
using ForeignExchange.Api.Validation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Xunit;

namespace ForeignExchange.Api.Tests.Unit.Services;

public class QuoteServiceTests
{
    private readonly IRatesRepository _ratesRepository = Substitute.For<IRatesRepository>();
    private readonly ILoggerAdapter<QuoteService> _logger = Substitute.For<ILoggerAdapter<QuoteService>>();
    private readonly QuoteService _sut;

    public QuoteServiceTests()
    {
        _sut = new QuoteService(_ratesRepository, _logger);
    }
    
    [Fact]
    public async Task GetQuoteAsync_ShouldReturnQuote_WhenCurrenciesAreSupported()
    {
        // Arrange
        var fromCurrency = "GBP";
        var toCurrency = "USD";
        var amount = 100;

        var expectedQuote = new ConversionQuote
        {
            BaseCurrency = fromCurrency,
            QuoteCurrency = toCurrency,
            BaseAmount = amount,
            QuoteAmount = 160m
        };

        _ratesRepository.GetRateAsync(fromCurrency, toCurrency)
            .Returns(new FxRate
        {
            FromCurrency = fromCurrency,
            ToCurrency = toCurrency,
            Rate = 1.6m
        });

        // Act
        var result = await _sut.GetQuoteAsync(fromCurrency, toCurrency, amount);

        // Assert
        result.Should().BeEquivalentTo(expectedQuote);
    }

    [Fact]
    public async Task GetQuoteAsync_ShouldThrowException_WhenSameCurrenciesAreUsed()
    {
        // Arrange
        var fromCurrency = "GBP";
        var toCurrency = "GBP";
        var amount = 100;
        
        // Act
        var resultAction = () => _sut.GetQuoteAsync(fromCurrency, toCurrency, amount);

        // Assert
        await resultAction.Should().ThrowAsync<SameCurrencyException>()
            .WithMessage($"You cannot convert currency {fromCurrency} to itself");
    }

    [Fact]
    public async Task GetQuoteAsync_ShouldLogAppropriateMessage_WhenInvoked()
    {
        // Arrange
        var fromCurrency = "GBP";
        var toCurrency = "USD";
        var amount = 100;

        _ratesRepository.GetRateAsync(fromCurrency, toCurrency)
            .Returns(new FxRate
            {
                FromCurrency = fromCurrency,
                ToCurrency = toCurrency,
                Rate = 1.6m
            });

        // Act
        await _sut.GetQuoteAsync(fromCurrency, toCurrency, amount);

        // Assert
        _logger.Received(1).LogInformation(
            "Retrieved quote for currencies {FromCurrency}->{ToCurrency} in {ElapsedMilliseconds}ms",
            Arg.Is<object[]>(x => 
                x[0].ToString() == "GBP" &&
                x[1].ToString() == "USD"));
    }
}
