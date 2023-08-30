using StockAPI.Domain.Entities;
using StockAPI.Domain.Repositories;

namespace StockAPI.Application.Services
{
    public class StockService : IStockService
    {
        private readonly IStockRepository _stockRepository;

        public StockService(IStockRepository stockRepository)
        {
            _stockRepository = stockRepository;
        }

        public async Task<IEnumerable<Stock>> GetAllStocksAsync()
        {
            return await _stockRepository.GetAllStocksAsync();
        }

        public async Task<Stock> GetStockByTickerSymbolAsync(string tickerSymbol)
        {
            return await _stockRepository.GetStockByTickerSymbolAsync(tickerSymbol);
        }

        public async Task UpdateStockPriceAsync(string tickerSymbol, decimal updatedAverageStockPrice)
        {
            await _stockRepository.UpdateStockValueAsync(tickerSymbol, updatedAverageStockPrice);
        }
    }
}
