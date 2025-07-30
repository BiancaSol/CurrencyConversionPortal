namespace CurrencyConversionPortal.Api.Tests.Controllers
{
    using CurrencyConversionPortal.Api.Controllers;
    using CurrencyConversionPortal.Api.DTOs;
    using CurrencyConversionPortal.Core.Entities;
    using CurrencyConversionPortal.Core.Exceptions;
    using CurrencyConversionPortal.Core.Models;
    using CurrencyConversionPortal.Core.Services;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class CurrenciesControllerTests
    {
        private readonly Mock<ICurrencyService> _mockCurrencyService;
        private readonly CurrenciesController _controller;

        public CurrenciesControllerTests()
        {
            _mockCurrencyService = new Mock<ICurrencyService>();
            _controller = new CurrenciesController(_mockCurrencyService.Object);
        }

        [Fact]
        public async Task GetAll_WithValidCurrencies_ReturnsOkResultWithCurrencies()
        {
            var currencies = new List<Currency>
            {
                new Currency { Code = "USD", Description = "US Dollar" },
                new Currency { Code = "EUR", Description = "Euro" },
                new Currency { Code = "GBP", Description = "British Pound" }
            };

            _mockCurrencyService.Setup(x => x.GetCurrenciesAsync())
                              .ReturnsAsync(currencies);


            var result = await _controller.GetAll();


            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);

            var responseValue = okResult.Value;
            var currenciesProperty = responseValue.GetType().GetProperty("currencies");
            Assert.NotNull(currenciesProperty);

            var returnedCurrencies = currenciesProperty.GetValue(responseValue) as IEnumerable<CurrencyDto>;
            Assert.NotNull(returnedCurrencies);

            var currencyList = returnedCurrencies.ToList();
            Assert.Equal(3, currencyList.Count);
            Assert.Equal("USD", currencyList[0].Code);
            Assert.Equal("US Dollar", currencyList[0].Description);
            Assert.Equal("EUR", currencyList[1].Code);
            Assert.Equal("Euro", currencyList[1].Description);
            Assert.Equal("GBP", currencyList[2].Code);
            Assert.Equal("British Pound", currencyList[2].Description);
        }

        [Fact]
        public async Task GetAll_WithEmptyList_ReturnsOkResultWithEmptyArray()
        {
            var currencies = new List<Currency>();
            _mockCurrencyService.Setup(x => x.GetCurrenciesAsync())
                              .ReturnsAsync(currencies);


            var result = await _controller.GetAll();


            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);

            var responseValue = okResult.Value;
            var currenciesProperty = responseValue.GetType().GetProperty("currencies");
            Assert.NotNull(currenciesProperty);

            var returnedCurrencies = currenciesProperty.GetValue(responseValue) as IEnumerable<CurrencyDto>;
            Assert.NotNull(returnedCurrencies);
            Assert.Empty(returnedCurrencies);
        }

        [Fact]
        public async Task GetAll_WhenServiceThrowsExternalServiceException_ThrowsException()
        {
            _mockCurrencyService.Setup(x => x.GetCurrenciesAsync())
                              .ThrowsAsync(new ExternalServiceException("Failed to retrieve available currencies"));


            var exception = await Assert.ThrowsAsync<ExternalServiceException>(() => _controller.GetAll());
            Assert.Equal("Failed to retrieve available currencies", exception.Message);
        }

        [Fact]
        public async Task GetAll_VerifiesServiceMethodIsCalled()
        {
            var currencies = new List<Currency>();
            _mockCurrencyService.Setup(x => x.GetCurrenciesAsync())
                              .ReturnsAsync(currencies);


            await _controller.GetAll();


            _mockCurrencyService.Verify(x => x.GetCurrenciesAsync(), Times.Once);
        }

        [Fact]
        public async Task Convert_WithValidRequest_ReturnsOkResultWithConversionResponse()
        {
            var request = new ConversionRequestDto
            {
                Amount = 100m,
                SourceCurrency = "USD"
            };

            var conversionModel = new ConversionModel
            {
                SourceAmount = 100m,
                SourceCurrency = "USD",
                Results = new Dictionary<string, decimal>
                {
                    { "EUR", 85.50m },
                    { "GBP", 73.25m },
                    { "JPY", 11045.00m }
                }
            };

            _mockCurrencyService.Setup(x => x.ConvertAsync(request.Amount, request.SourceCurrency))
                              .ReturnsAsync(conversionModel);


            var result = await _controller.Convert(request);


            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ConversionResponseDto>(okResult.Value);

            Assert.Equal(100m, response.SourceAmount);
            Assert.Equal("USD", response.SourceCurrency);
            Assert.Equal(3, response.ConvertedAmounts.Count);
            Assert.Equal(85.50m, response.ConvertedAmounts["EUR"]);
            Assert.Equal(73.25m, response.ConvertedAmounts["GBP"]);
            Assert.Equal(11045.00m, response.ConvertedAmounts["JPY"]);
        }

        [Fact]
        public async Task Convert_WithMinimalAmount_ReturnsValidResponse()
        {
            var request = new ConversionRequestDto
            {
                Amount = 0.01m,
                SourceCurrency = "USD"
            };

            var conversionModel = new ConversionModel
            {
                SourceAmount = 0.01m,
                SourceCurrency = "USD",
                Results = new Dictionary<string, decimal>
                {
                    { "EUR", 0.0085m }
                }
            };

            _mockCurrencyService.Setup(x => x.ConvertAsync(request.Amount, request.SourceCurrency))
                              .ReturnsAsync(conversionModel);


            var result = await _controller.Convert(request);


            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ConversionResponseDto>(okResult.Value);

            Assert.Equal(0.01m, response.SourceAmount);
            Assert.Equal("USD", response.SourceCurrency);
            Assert.Single(response.ConvertedAmounts);
        }

        [Fact]
        public async Task Convert_WithLargeAmount_ReturnsValidResponse()
        {
            var request = new ConversionRequestDto
            {
                Amount = 999999.99m,
                SourceCurrency = "EUR"
            };

            var conversionModel = new ConversionModel
            {
                SourceAmount = 999999.99m,
                SourceCurrency = "EUR",
                Results = new Dictionary<string, decimal>
                {
                    { "USD", 1169999.99m }
                }
            };

            _mockCurrencyService.Setup(x => x.ConvertAsync(request.Amount, request.SourceCurrency))
                              .ReturnsAsync(conversionModel);


            var result = await _controller.Convert(request);


            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ConversionResponseDto>(okResult.Value);

            Assert.Equal(999999.99m, response.SourceAmount);
            Assert.Equal("EUR", response.SourceCurrency);
        }

        [Fact]
        public async Task Convert_WhenServiceThrowsValidationException_ThrowsException()
        {
            var request = new ConversionRequestDto
            {
                Amount = -10m,
                SourceCurrency = "USD"
            };

            _mockCurrencyService.Setup(x => x.ConvertAsync(request.Amount, request.SourceCurrency))
                              .ThrowsAsync(new ValidationException("Amount must be greater than zero"));


            var exception = await Assert.ThrowsAsync<ValidationException>(() => _controller.Convert(request));
            Assert.Equal("Amount must be greater than zero", exception.Message);
        }

        [Fact]
        public async Task Convert_WhenServiceThrowsValidationExceptionForInvalidCurrency_ThrowsException()
        {
            var request = new ConversionRequestDto
            {
                Amount = 100m,
                SourceCurrency = "INVALID"
            };

            _mockCurrencyService.Setup(x => x.ConvertAsync(request.Amount, request.SourceCurrency))
                              .ThrowsAsync(new ValidationException("Invalid source currency code"));


            var exception = await Assert.ThrowsAsync<ValidationException>(() => _controller.Convert(request));
            Assert.Equal("Invalid source currency code", exception.Message);
        }

        [Fact]
        public async Task Convert_WhenServiceThrowsExternalServiceException_ThrowsException()
        {
            var request = new ConversionRequestDto
            {
                Amount = 100m,
                SourceCurrency = "USD"
            };

            _mockCurrencyService.Setup(x => x.ConvertAsync(request.Amount, request.SourceCurrency))
                              .ThrowsAsync(new ExternalServiceException("Currency conversion service is temporarily unavailable"));


            var exception = await Assert.ThrowsAsync<ExternalServiceException>(() => _controller.Convert(request));
            Assert.Equal("Currency conversion service is temporarily unavailable", exception.Message);
        }
    }
}
