using StockAPI.Domain.Entities;
using StockAPI.Domain.Repositories;
using System.Data;
using Dapper;

namespace StockAPI.Infrastructure.Repositories
{
    public class StockRepository : IStockRepository
    {
        private readonly IDbConnection _dbConnection;

        public StockRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<Stock> GetStockByTickerSymbolAsync(string tickerSymbol)
        {
            return await _dbConnection.QuerySingleOrDefaultAsync<Stock>(
                "SELECT * FROM Stocks WHERE TickerSymbol = @TickerSymbol",
                new { TickerSymbol = tickerSymbol });
        }

        public async Task<IEnumerable<Stock>> GetAllStocksAsync()
        {
            var stocks = await _dbConnection.QueryAsync<Stock>(
                "SELECT *, CurrentValue FROM Stocks");

            return stocks;
        }

        public async Task<Stock> AddStockAsync(Stock stock)
        {
            int insertedId = await _dbConnection.ExecuteScalarAsync<int>(
                "INSERT INTO Stocks (TickerSymbol) VALUES (@TickerSymbol); SELECT SCOPE_IDENTITY()",
                stock);
            stock.Id = insertedId;
            return stock;
        }

        public async Task UpdateStockAsync(Stock stock)
        {
            await _dbConnection.ExecuteAsync(
                "UPDATE Stocks SET TickerSymbol = @TickerSymbol WHERE Id = @Id",
                new { TickerSymbol = stock.TickerSymbol, Id = stock.Id });
        }

        public async Task UpdateStockValueAsync(string tickerSymbol, decimal currentValue)
        {
            await _dbConnection.ExecuteAsync(
                "UPDATE Stocks SET CurrentValue = @CurrentValue WHERE TickerSymbol = @TickerSymbol",
                new { TickerSymbol = tickerSymbol, CurrentValue = currentValue });
        }

        public async Task DeleteStockAsync(string tickerSymbol)
        {
            await _dbConnection.ExecuteAsync(
                "DELETE FROM Stocks WHERE TickerSymbol = @TickerSymbol",
                new { TickerSymbol = tickerSymbol });
        }
    }
}
