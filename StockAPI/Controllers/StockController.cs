using Microsoft.AspNetCore.Mvc;
using StockAPI.Application.Services;

namespace StockAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockController : ControllerBase
    {
        private readonly IStockService _stockService;
        private readonly Serilog.ILogger _logger;

        public StockController(IStockService stockService, Serilog.ILogger logger)
        {
            _stockService = stockService ?? throw new ArgumentNullException(nameof(stockService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("{tickerSymbol}")]
        public async Task<IActionResult> GetStockByTickerSymbolAsync(string tickerSymbol)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tickerSymbol))
                {
                    _logger.Error($"StocksController.GetStockByTickerSymbolAsync '{nameof(tickerSymbol)}' cannot be null or whitespace.");
                    return BadRequest($"'{nameof(tickerSymbol)}' cannot be null or whitespace.");
                }

                var stock = await _stockService.GetStockByTickerSymbolAsync(tickerSymbol);

                if (stock == null )
                {
                    return NotFound();
                }

                return Ok(stock);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"StocksController.GetStockByTickerSymbolAsync An error occurred while processing the request for tickerSymbol {tickerSymbol}");
                return StatusCode(500, $"StocksController.GetStockByTickerSymbolAsync An error occurred while processing your request for tickerSymbol {tickerSymbol}.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllStocksAsync()
        {
            try
            {

                var stock = await _stockService.GetAllStocksAsync();

                if (stock == null || !stock.Any())
                {
                    return NotFound();
                }

                return Ok(stock);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "StocksController.GetAllStocksAsync. An error occurred while fetching all stocks.");
                return StatusCode(500, "StocksController.GetAllStocksAsync. An error occurred while fetching all stocks.");
            }
        }
    }
}
