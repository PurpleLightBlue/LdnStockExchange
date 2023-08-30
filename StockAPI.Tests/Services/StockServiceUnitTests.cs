using Moq;
using StockAPI.Application.Services;
using StockAPI.Domain.Entities;
using StockAPI.Domain.Repositories;
using System.Collections.Generic;

namespace StockAPI.Tests.Services
{
    public class StockServiceTests
    {
        [Fact]
        public async void GetStockByTickerSymbol_ReturnsStock_WhenExists()
        {
            // Arrange
            string tickerSymbol = "AAPL";
            var expectedStock = new Stock { TickerSymbol = tickerSymbol };

            var mockStockRepository = new Mock<IStockRepository>();
            mockStockRepository.Setup(repo => repo.GetStockByTickerSymbolAsync(tickerSymbol)).ReturnsAsync(expectedStock);

            var stockService = new StockService(mockStockRepository.Object);

            // Act
            var actualStock = await stockService.GetStockByTickerSymbolAsync(tickerSymbol);

            // Assert
            Assert.Equal(expectedStock, actualStock);
        }

        [Fact]
        public async void GetStockByTickerSymbol_ReturnsNull_WhenNotFound()
        {
            // Arrange
            string tickerSymbol = "AAPL";

            var mockStockRepository = new Mock<IStockRepository>();
            mockStockRepository.Setup(repo => repo.GetStockByTickerSymbolAsync(tickerSymbol)).ReturnsAsync((Stock)null);

            var stockService = new StockService(mockStockRepository.Object);

            // Act
            var actualStock = await stockService.GetStockByTickerSymbolAsync(tickerSymbol);

            // Assert
            Assert.Null(actualStock);
        }

        [Fact]
        public async Task GetAllStocksAsync_Success()
        {
            // Arrange
            var mockStockRepository = new Mock<IStockRepository>();
            mockStockRepository.Setup(repo => repo.GetAllStocksAsync())
                .ReturnsAsync(new List<Stock> {  new Stock()
                    {
                        CurrentValue = 100,
                        Name = "Apple Inc.",
                        TickerSymbol = "AAPL",
                        Id= 1
                    },
                    new Stock ()
                    {
                        CurrentValue = 23.5m,
                        Name = "Microsoft",
                        TickerSymbol = "MSFT",
                        Id = 2
                    }
                });
            var service = new StockService(mockStockRepository.Object);

            // Act
            var result = await service.GetAllStocksAsync();

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Count() == 2);
            Assert.Equal("AAPL", result.First().TickerSymbol);
            Assert.Equal("MSFT", result.Last().TickerSymbol);
        }

        [Fact]
        public async Task GetAllStocksAsync_EmptyStocks()
        {
            // Arrange
            var emptyList = new List<Stock>();
            var mockStockRepository = new Mock<IStockRepository>();
            mockStockRepository.Setup(repo => repo.GetAllStocksAsync())
                .ReturnsAsync(emptyList);

            var service = new StockService(mockStockRepository.Object);

            // Act
            var result = await service.GetAllStocksAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllStocksAsync_Exception()
        {
            // Arrange
            var mockStockRepository = new Mock<IStockRepository>();
            mockStockRepository.Setup(repo => repo.GetAllStocksAsync())
                .ThrowsAsync(new Exception("Test exception"));
            var service = new StockService(mockStockRepository.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await service.GetAllStocksAsync());
        }

    }
}
