using Moq;
using Serilog;
using StockAPI.Application.Services;
using StockAPI.Domain.Entities;
using StockAPI.Domain.Exceptions;
using StockAPI.Domain.Repositories;

namespace StockAPI.Tests.Application.Services
{
    public class TradeServiceTests
    {
        private readonly Mock<ITradeRepository> _tradeRepositoryMock;
        private readonly Mock<IStockService> _stockServiceMock;
        private readonly ITradeService _tradeService;

        public TradeServiceTests()
        {
            _tradeRepositoryMock = new Mock<ITradeRepository>();
            _stockServiceMock = new Mock<IStockService>();
            var loggerMock = new Mock<ILogger>();
            _tradeService = new TradeService(_tradeRepositoryMock.Object, _stockServiceMock.Object);
        }

        [Fact]
        public async Task RecordTradeAsync_ValidTrade_AddsTradeAndReturnsInsertedTradeWithId()
        {
            // Arrange
            var trade = new Trade { TradeId = Guid.NewGuid(), TickerSymbol = "AAPL", Price = 100, Shares = 10 };
            _tradeRepositoryMock.Setup(repo => repo.IsTradeIdProcessedAsync(It.IsAny<Guid>()))
                .ReturnsAsync(false);
            _tradeRepositoryMock.Setup(repo => repo.AddTradeAsync(It.IsAny<Trade>()))
                .ReturnsAsync(trade);
            _tradeRepositoryMock.Setup(repo => repo.GetTradesByTickerSymbolAsync("AAPL")).ReturnsAsync(new List<Trade> { trade });
            _stockServiceMock.Setup(x => x.UpdateStockPriceAsync(It.IsAny<string>(), It.IsAny<decimal>())).Returns(Task.CompletedTask);

            // Act
            var result = await _tradeService.RecordTradeAsync(trade);

            // Assert
            Assert.Equal(trade, result);
            _tradeRepositoryMock.Verify(repo => repo.AddTradeAsync(trade), Times.Once);
        }

        [Fact]
        public async Task RecordTradeAsync_TradeIdAlreadyProcessed_ThrowsExcpetion()
        {
            // Arrange
            var trade = new Trade { TradeId = Guid.NewGuid(), TickerSymbol = "AAPL", Price = 100, Shares = 10 };
            _tradeRepositoryMock.Setup(repo => repo.IsTradeIdProcessedAsync(It.IsAny<Guid>()))
                .ReturnsAsync(true);

            // act & Assert
            Assert.ThrowsAsync<InvalidTradeException>(() => _tradeService.RecordTradeAsync(trade));

        }

        [Fact]
        public async Task GetTradesByTickerSymbolAsync_InvalidTickerSymbol_ThrowsArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _tradeService.GetTradesByTickerSymbolAsync(null);
            });
        }

        [Fact]
        public async Task CalculateAverageStockPriceAsync_NoTrades_ReturnsZero()
        {
            // Arrange
            var tickerSymbol = "AAPL";
            _tradeRepositoryMock.Setup(repo => repo.GetTradesByTickerSymbolAsync(tickerSymbol))
                .ReturnsAsync(new List<Trade>());

            // Act
            var result = await _tradeService.CalculateAverageStockPriceAsync(tickerSymbol);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task CalculateAverageStockPriceAsync_ValidTickerSymbol_CalculatesAveragePrice()
        {
            // Arrange
            var tickerSymbol = "AAPL";
            var trades = new List<Trade>
            {
                new Trade { Price = 100, Shares = 5 },
                new Trade { Price = 110, Shares = 7 },
                new Trade { Price = 90, Shares = 3 }
            };
            _tradeRepositoryMock.Setup(repo => repo.GetTradesByTickerSymbolAsync(tickerSymbol))
                .ReturnsAsync(trades);

            // Act
            var result = await _tradeService.CalculateAverageStockPriceAsync(tickerSymbol);

            // Assert
            Assert.Equal(102.67m, result);
        }
    }
}
