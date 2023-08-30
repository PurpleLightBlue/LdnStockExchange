using StockAPI.Domain.Entities;
using System.Threading.Tasks;

namespace StockAPI.Domain.Repositories
{
    public interface IStockRepository
    {
        Task<Stock> GetStockByTickerSymbolAsync(string tickerSymbol);
        Task<IEnumerable<Stock>> GetAllStocksAsync();
        Task UpdateStockAsync(Stock stock);
        Task UpdateStockValueAsync(string tickerSymbol, decimal currentValue);
        Task<Stock> AddStockAsync(Stock stock);
        Task DeleteStockAsync(string tickerSymbol);
    }
}
