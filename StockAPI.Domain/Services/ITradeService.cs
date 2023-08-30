using StockAPI.Domain.Entities;

namespace StockAPI.Application.Services
{
    public interface ITradeService
    {
        Task<Trade> RecordTradeAsync(Trade trade);
        Task<List<Trade>> GetTradesByTickerSymbolAsync(string tickerSymbol);
        Task<bool> IsTradeIdProcessedAsync(Guid tradeId);
        Task<decimal> CalculateAverageStockPriceAsync(string tickerSymbol);
    }
}
