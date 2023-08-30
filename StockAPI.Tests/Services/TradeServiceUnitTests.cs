using Moq;
using StockAPI.Application.Services;
using StockAPI.Domain.Entities;

namespace StockAPI.Tests.Services
{
    public class TradeServiceUnitTests
    {

        [Fact]
        public async void CalculateAverageStockPrice_ShouldReturnAveragePrice_WhenTradesExist()
        {
            // Arrange
            string tickerSymbol = "AAPL";
            var trades = new List<Trade>
        {
            new Trade { TickerSymbol = tickerSymbol, Price = 150, Shares = 10 },
            new Trade { TickerSymbol = tickerSymbol, Price = 155, Shares = 5 }
        };

            var mockTradeRepository = new Mock<ITradeRepository>();
            mockTradeRepository.Setup(repo => repo.GetTradesByTickerSymbolAsync(tickerSymbol)).ReturnsAsync(trades);

            var tradeService = new TradeService(mockTradeRepository.Object, new Mock<IStockService>().Object);

            // Act
            var averagePrice = await tradeService.CalculateAverageStockPriceAsync(tickerSymbol);

            // Assert
            Assert.Equal(151.67M, averagePrice);
        }

        [Fact]
        public async Task CalculateAverageStockPrice_ShouldReturnZero_WhenNoTradesExistAsync()
        {
            // Arrange
            var mockTradeRepository = new Mock<ITradeRepository>();
            mockTradeRepository.Setup(x => x.GetTradesByTickerSymbolAsync("AAPL")).ReturnsAsync(new List<Trade>() { });

            var tradeService = new TradeService(mockTradeRepository.Object, new Mock<IStockService>().Object);

            // Act
            var averagePrice = await tradeService.CalculateAverageStockPriceAsync("AAPL");

            // Assert
            Assert.Equal(0, averagePrice);
        }
    }
}
