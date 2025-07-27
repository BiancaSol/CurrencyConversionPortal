namespace CurrencyConversionPortal.Api.Controllers
{
    using CurrencyConversionPortal.Api.DTOs;
    using CurrencyConversionPortal.Core.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Linq;
    using System.Threading.Tasks;

    [ApiController]
    [Authorize(Policy = "StandardUser")]
    [Route("api/[controller]")]
    public class CurrenciesController : ControllerBase
    {
        private readonly ICurrencyService _currencyService;

        public CurrenciesController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var currencies = await _currencyService.GetCurrenciesAsync();

            var dtos = currencies.Select(c => new CurrencyDto { Code = c.Code, Description = c.Description });

            return Ok(new { currencies = dtos });
        }

        [HttpPost("convert")]
        public async Task<IActionResult> Convert([FromBody] ConversionRequestDto request)
        {
            var conversionModel = await _currencyService.ConvertAsync(request.Amount, request.SourceCurrency);

            var response = new ConversionResponseDto
            {
                SourceAmount = conversionModel.SourceAmount,
                SourceCurrency = conversionModel.SourceCurrency,
                ConvertedAmounts = conversionModel.Results
            };

            return Ok(response);
        }
    }
}
