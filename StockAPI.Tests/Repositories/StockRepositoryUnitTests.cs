using StockAPI.Domain.Entities;
using Microsoft.Data.Sqlite;
using Dapper;
using StockAPI.Infrastructure.Repositories;

namespace StockAPI.Tests.Repositories
{
    public class StockRepositoryTests
    {
        private readonly SqliteConnection _connection;
        private readonly StockRepository _stockRepository;

        public StockRepositoryTests()
        {
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();
            InitializeDatabase(); // Create tables and schema
            _stockRepository = new StockRepository(_connection);
        }

        private async void InitializeDatabase()
        {
            // Create necessary tables and schema for Stocks
            var createTableSql = @"
            CREATE TABLE Stocks (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                TickerSymbol NVARCHAR(10) NOT NULL,
                CurrentValue DECIMAL(18, 2) DEFAULT 0
            );
        ";

            using (var command = _connection.CreateCommand())
            {
                command.CommandText = createTableSql;
                command.ExecuteNonQuery();
            }
        }

        [Fact]
        public async Task AddStock_Should_AddStock_ToStockListAsync()
        {
            // Arrange
            var stock = new Stock { TickerSymbol = "AAPL", CurrentValue = 123.45m };

            var insertSql = @"
                INSERT INTO Stocks (TickerSymbol, CurrentValue)
                VALUES (@TickerSymbol, @CurrentValue);
            ";

            await _connection.ExecuteAsync(insertSql, stock);

            // Assert
            var result = await _stockRepository.GetStockByTickerSymbolAsync("AAPL");
            Assert.NotNull(result);
            Assert.Equal(stock.TickerSymbol, result.TickerSymbol);
        }

        [Fact]
        public async void GetStockByTickerSymbol_Should_ReturnNull_WhenNoMatchingStock()
        {
            // Act
            var result = await _stockRepository.GetStockByTickerSymbolAsync("GOOGL");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async void UpdateStock_Should_UpdateStock_TickerSymbol()
        {
            // Arrange
            // Arrange
            var stock = new Stock { TickerSymbol = "BT", CurrentValue = 123.45m };

            var insertSql = @"
                INSERT INTO Stocks (TickerSymbol, CurrentValue)
                VALUES (@TickerSymbol, @CurrentValue);
            ";

            await _connection.ExecuteAsync(insertSql, stock);

            var getStockResult = await _stockRepository.GetStockByTickerSymbolAsync("BT");

            // Update the stock
            getStockResult.TickerSymbol = "GOOGL";
            await _stockRepository.UpdateStockAsync(getStockResult);

            // Act
            var result = await _stockRepository.GetStockByTickerSymbolAsync("GOOGL");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(getStockResult.TickerSymbol, result.TickerSymbol);
        }

        [Fact]
        public async void DeleteStock_Should_RemoveStock_FromStockList()
        {
            // Arrange
            var stock = new Stock { TickerSymbol = "STHNW", CurrentValue = 123.45m };

            var insertSql = @"
                INSERT INTO Stocks (TickerSymbol, CurrentValue)
                VALUES (@TickerSymbol, @CurrentValue);
            ";

            await _connection.ExecuteAsync(insertSql, stock);

            // Act
            var preDeleteResult = await _stockRepository.GetStockByTickerSymbolAsync("STHNW");
            Assert.NotNull(preDeleteResult);
            await _stockRepository.DeleteStockAsync("STHNW");
            var result = await _stockRepository.GetStockByTickerSymbolAsync("STHNW");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateStockValueAsync_Should_UpdateCurrentValue()
        {
            // Arrange
            var initialStock = new Stock { TickerSymbol = "AAPL", CurrentValue = 100 };
            await _connection.ExecuteAsync("INSERT INTO Stocks (TickerSymbol, CurrentValue) VALUES (@TickerSymbol, @CurrentValue)",
                                           new { initialStock.TickerSymbol, initialStock.CurrentValue });

            var updatedValue = 120;

            // Act
            await _stockRepository.UpdateStockValueAsync("AAPL", updatedValue);
            var result = await _stockRepository.GetStockByTickerSymbolAsync("AAPL");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(updatedValue, result.CurrentValue);
        }

        [Fact]
        public async Task GetAllStocksAsync_Success()
        {
            // Arrange
            var stock = new Stock { TickerSymbol = "AAPL", CurrentValue = 123.5m, Id = 1, Name = "Apple Inc." };
            var stock2 = new Stock { TickerSymbol = "MSFT", CurrentValue = 144.5m, Id = 1, Name = "Microsoft Inc." };
            var insertSql = @"
                INSERT INTO Stocks (TickerSymbol, CurrentValue)
                VALUES (@TickerSymbol, @CurrentValue);
            ";

            await _connection.ExecuteAsync(insertSql, stock);
            await _connection.ExecuteAsync(insertSql, stock2);


            // Act
            var stocks = await _stockRepository.GetAllStocksAsync();

            // Assert
            Assert.True(stocks.Count() == 2);
            Assert.Equal("AAPL", stocks.First().TickerSymbol);
            Assert.Equal("MSFT", stocks.Last().TickerSymbol);
        }


        // Dispose the connection after all tests
        public void Dispose()
        {
            _connection.Close();
            _connection.Dispose();
        }
    }
}
