using StockAPI.Domain.Entities;
using Microsoft.Data.Sqlite;
using Moq;
using Serilog;
using Dapper;
using StockAPI.Infrastructure.Repositories.Mappers;

namespace StockAPI.Tests.Repositories
{
    public class TradeRepositoryTests
    {
        private readonly SqliteConnection _connection;
        private readonly TradeRepository _tradeRepository;

        public TradeRepositoryTests()
        {

            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();
            InitializeDatabase(); // Create tables and schema
            _tradeRepository = new TradeRepository(_connection, new Mock<ILogger>().Object);
        }

        private void InitializeDatabase()
        {
            SqlMapper.AddTypeHandler(new TradeIdGuidMapper()); 
            SqlMapper.RemoveTypeMap(typeof(Guid));
            SqlMapper.RemoveTypeMap(typeof(Guid?));

            // Create necessary tables and schema
            var createTableSql = @"
            CREATE TABLE Trades (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                TradeId TEXT NOT NULL,
                TickerSymbol NVARCHAR(10) NOT NULL,
                Price DECIMAL(18, 2) NOT NULL,
                Shares DECIMAL(18, 2) NOT NULL,
                BrokerId INT NOT NULL,
                TradeTime DATETIME NOT NULL
            );
        ";

            using (var command = _connection.CreateCommand())
            {
                command.CommandText = createTableSql;
                command.ExecuteNonQuery();
            }
        }

        [Fact]
        public async Task AddTrade_Should_AddTrade()
        {
            // Arrange
            var trade = new Trade
            {
                TickerSymbol = "AAPL",
                Price = 150.25m,
                Shares = 100,
                BrokerId = 1,
                TradeTime = DateTime.Now
            };

            
            var insertSql = @"
                INSERT INTO Trades (TradeId, TickerSymbol, Price, Shares, BrokerId, TradeTime)
                VALUES (@TradeId, @TickerSymbol, @Price, @Shares, @BrokerId, @TradeTime);
            ";

            await _connection.ExecuteAsync(insertSql, trade);

            // Act & Assert
            var result = await _tradeRepository.GetTradesByTickerSymbolAsync("AAPL");
            Assert.Single(result);
            Assert.Equal(trade.TickerSymbol, result[0].TickerSymbol);
            Assert.Equal(trade.Price, result[0].Price);
            Assert.Equal(trade.Shares, result[0].Shares);
            Assert.Equal(trade.BrokerId, result[0].BrokerId);   
            Assert.Equal(trade.TradeTime, result[0].TradeTime); 
        }

        [Fact]
        public async void GetTradesByTickerSymbol_Should_ReturnMatchingTrades()
        {
            // Arrange
            var trade1 = new Trade
            {
                TickerSymbol = "AAPL",
                TradeId = new Guid(),
                Price = 100,
                Shares = 10,
                BrokerId = 1,
                TradeTime = DateTime.Now
            };

            var trade2 = new Trade
            {
                TickerSymbol = "AAPL",
                TradeId = new Guid(),
                Price = 110,
                Shares = 15,
                BrokerId = 1,
                TradeTime = DateTime.Now
            };
            // Act
            var insertSql = @"
                INSERT INTO Trades (TradeId, TickerSymbol, Price, Shares, BrokerId, TradeTime)
                VALUES (@TradeId, @TickerSymbol, @Price, @Shares, @BrokerId, @TradeTime);
            ";

            await _connection.ExecuteAsync(insertSql, trade1);
            await _connection.ExecuteAsync(insertSql, trade2);

            // Act
            var trades = await _tradeRepository.GetTradesByTickerSymbolAsync("AAPL");

            // Assert
            Assert.Equal(2, trades.Count);

            Assert.Contains(trades, item => item.Price == trade1.Price);
            Assert.Contains(trades, item => item.Shares == trade1.Shares);
            Assert.Contains(trades, item => item.BrokerId == trade1.BrokerId);
            Assert.Contains(trades, item => item.TradeTime == trade1.TradeTime);

            Assert.Contains(trades, item => item.Price == trade2.Price);
            Assert.Contains(trades, item => item.Shares == trade2.Shares);
            Assert.Contains(trades, item => item.BrokerId == trade2.BrokerId);
            Assert.Contains(trades, item => item.TradeTime == trade2.TradeTime);
            trades.ForEach(item => Assert.Equal("AAPL", item.TickerSymbol));
        }

        [Fact]
        public async Task GetTradesByTickerSymbol_Should_ReturnEmptyList_WhenNoMatchingTradesAsync()
        {
            // Arrange
            var trade = new Trade { TickerSymbol = "AAPL", Price = 100, Shares = 10 };
            var insertSql = @"
                INSERT INTO Trades (TradeId, TickerSymbol, Price, Shares, BrokerId, TradeTime)
                VALUES (@TradeId, @TickerSymbol, @Price, @Shares, @BrokerId, @TradeTime);
            "
            ;

            await _connection.ExecuteAsync(insertSql, trade);

            // Act
            var trades = await _tradeRepository.GetTradesByTickerSymbolAsync("GOOGL");

            // Assert

            Assert.Empty(trades);
        }

        // Dispose the connection after all tests
        public void Dispose()
        {
            _connection.Close();
            _connection.Dispose();
        }
    }
}
