using Microsoft.AspNetCore.Mvc;
using StockAPI.Application.Services;
using StockAPI.Domain.Entities;
using StockAPI.Domain.Exceptions;

namespace StockAPI.Controllers
{
    public class TradeController : ControllerBase
    {
        private readonly ITradeService _tradeService;
        private readonly Serilog.ILogger _logger;

        public TradeController(ITradeService tradeService, Serilog.ILogger logger)
        {
            _tradeService = tradeService ?? throw new ArgumentNullException(nameof(tradeService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        [Route("Trade")]
        public async Task<IActionResult> RecordTradeAsync([FromBody] Trade trade)
        {
            try
            {
                var recordedTrade = await _tradeService.RecordTradeAsync(trade);
                return Ok(recordedTrade);
            }
            catch (InvalidTradeException ite)
            {
                _logger.Error(ite, $"TradesController.RecordTradeAsync An error occurred while processing your request for Ticker Symbol {trade.TickerSymbol}. TradeID {trade.TradeId} has already been processed.");
                return StatusCode(500, $"TradesController.RecordTradeAsync An error occurred while processing your request for Ticker Symbol {trade.TickerSymbol}. TradeID {trade.TradeId} has already been processed.");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"TradesController.RecordTradeAsync An error occurred while processing your request for Trade {trade.TradeId}");
                return StatusCode(500, $"An error occurred while processing your request for trade {trade.TradeId}.");
            }
        }

        [HttpGet("{tickerSymbol}/trades")]
        public async Task<IActionResult> GetTradesByTickerSymbolAsync(string tickerSymbol)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tickerSymbol))
                {
                    _logger.Error( $"TradesController.GetTradesByTickerSymbolAsync invalid argument: {tickerSymbol}");
                    return BadRequest($"'{nameof(tickerSymbol)}' cannot be null or whitespace.");
                }

                var trades = await _tradeService.GetTradesByTickerSymbolAsync(tickerSymbol);
                return Ok(trades);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"TradesController.GetTradesByTickerSymbolAsync An error occurred while processing your request for Ticker Symbol {tickerSymbol}");
                return StatusCode(500, $"An error occurred while processing your request for Ticker Symbol {tickerSymbol}");
            }
        }

        [HttpGet("{tickerSymbol}/average-price")]
        public async Task<IActionResult> CalculateAverageStockPriceAsync(string tickerSymbol)
        {
            try
            {
                if (tickerSymbol is null)
                {
                    _logger.Error($"TradesController.CalculateAverageStockPriceAsync invalid argument: {tickerSymbol}");
                    return BadRequest($"'{nameof(tickerSymbol)}' cannot be null or whitespace.");
                }

                var averagePrice = await _tradeService.CalculateAverageStockPriceAsync(tickerSymbol);
                return Ok(averagePrice);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"TradesController.CalculateAverageStockPriceAsync An error occurred while processing your request for Ticker Symbol {tickerSymbol}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
