using StockAPI.Domain.Entities;

public interface ITradeRepository
{
    Task<Trade> AddTradeAsync(Trade trade);
    Task<List<Trade>> GetTradesByTickerSymbolAsync(string tickerSymbol);
    Task<bool> IsTradeIdProcessedAsync(Guid tradeId);
}
