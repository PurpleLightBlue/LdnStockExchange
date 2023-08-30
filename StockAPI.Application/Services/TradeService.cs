using StockAPI.Domain.Entities;
using Serilog;
using StockAPI.Domain.Exceptions;

namespace StockAPI.Application.Services
{

    public class TradeService : ITradeService
    {
        private readonly ITradeRepository _tradeRepository;
        private readonly IStockService _stockService;
        private readonly ILogger _logger;

        public TradeService(ITradeRepository tradeRepository, IStockService stockService)
        {
            _tradeRepository = tradeRepository;
            _stockService = stockService;
        }

        public async Task<Trade> RecordTradeAsync(Trade trade)
        {
            if (trade is null)
            {
                throw new ArgumentNullException(nameof(trade));
            }

            if (!await IsTradeIdProcessedAsync(trade.TradeId))
            {
                var recordedTrade = await _tradeRepository.AddTradeAsync(trade);
                var updatedAverageStockPrice = await this.CalculateAverageStockPriceAsync(trade.TickerSymbol);
                await _stockService.UpdateStockPriceAsync(trade.TickerSymbol, updatedAverageStockPrice);
                return recordedTrade;
            }
            else 
            { 
                throw new InvalidTradeException($"Trade with Id {trade.TradeId} has already been processed.");
            }
        }

        public async Task<List<Trade>> GetTradesByTickerSymbolAsync(string tickerSymbol)
        {
            if (string.IsNullOrWhiteSpace(tickerSymbol))
            {
                throw new ArgumentException($"'{nameof(tickerSymbol)}' cannot be null or whitespace.", nameof(tickerSymbol));
            }

            return await _tradeRepository.GetTradesByTickerSymbolAsync(tickerSymbol);
        }

        public async Task<bool> IsTradeIdProcessedAsync(Guid tradeId)
        {
            return await _tradeRepository.IsTradeIdProcessedAsync(tradeId);
        }

        public async Task<decimal> CalculateAverageStockPriceAsync(string tickerSymbol)
        {
            if (string.IsNullOrWhiteSpace(tickerSymbol))
            {
                throw new ArgumentException($"'{nameof(tickerSymbol)}' cannot be null or whitespace.", nameof(tickerSymbol));
            }

            var trades = await GetTradesByTickerSymbolAsync(tickerSymbol);
            if (trades.Count == 0)
            {
                return 0;
            }

            var priceList = new List<decimal>();
            foreach (var trade in trades)
            {
                for (int i = 0; i < trade.Shares; i++)
                {
                    priceList.Add(trade.Price);
                }
            }

            var totalPrice = priceList.Sum(x => x);
            var totalShares = priceList.Count;
            return Math.Round((totalPrice / totalShares), 2);
        }
    }
}
