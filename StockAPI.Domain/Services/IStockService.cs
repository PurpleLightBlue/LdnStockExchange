using StockAPI.Domain.Entities;

namespace StockAPI.Application.Services
{
    public interface IStockService
    {
        Task<Stock> GetStockByTickerSymbolAsync(string tickerSymbol);
        Task<IEnumerable<Stock>> GetAllStocksAsync();
        Task UpdateStockPriceAsync(string tickerSymbol, decimal updatedAverageStockPrice);
    }
}
