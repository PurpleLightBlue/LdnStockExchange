using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Serilog;
using StockAPI.Application.Services;
using StockAPI.Controllers;
using StockAPI.Domain.Entities;

namespace StockAPI.Tests.Controllers
{
    public class StockControllerTests
    {
        [Fact]
        public async Task GetStockByTickerSymbolAsync_ValidTickerSymbol_ReturnsOk()
        {
            // Arrange
            var stockServiceMock = new Mock<IStockService>();
            stockServiceMock.Setup(service => service.GetStockByTickerSymbolAsync("AAPL"))
                .ReturnsAsync(new Stock { TickerSymbol = "AAPL", Name = "Apple Inc." });

            var loggerMock = new Mock<ILogger>();

            var controller = new StockController(stockServiceMock.Object, loggerMock.Object);

            // Act
            var result = await controller.GetStockByTickerSymbolAsync("AAPL");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var stock = Assert.IsType<Stock>(okResult.Value);
            Assert.Equal("AAPL", stock.TickerSymbol);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task GetStockByTickerSymbolAsync_InvalidTickerSymbol_ReturnsBadRequest(string tickerSymbol)
        {
            // Arrange
            var stockServiceMock = new Mock<IStockService>();
            var loggerMock = new Mock<ILogger>();

            var controller = new StockController(stockServiceMock.Object, loggerMock.Object);

            // Act
            var result = await controller.GetStockByTickerSymbolAsync(tickerSymbol);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal($"'{nameof(tickerSymbol)}' cannot be null or whitespace.", badRequestResult.Value);
        }

        [Fact]
        public async Task GetStockByTickerSymbolAsync_StockServiceThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var stockServiceMock = new Mock<IStockService>();
            stockServiceMock.Setup(service => service.GetStockByTickerSymbolAsync("AAPL"))
                .ThrowsAsync(new Exception("Some error message"));

            var loggerMock = new Mock<ILogger>();

            var controller = new StockController(stockServiceMock.Object, loggerMock.Object);

            // Act
            var actionResult = await controller.GetStockByTickerSymbolAsync("AAPL");

            // Assert
            var objectResult = actionResult as ObjectResult;
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
        }

        [Fact]
        public async Task GetAllStocksAsync_Success()
        {
            // Arrange
            var mockLogger = new Mock<ILogger>();

            var mockStockService = new Mock<IStockService>();
            mockStockService.Setup(service => service.GetAllStocksAsync())
                .ReturnsAsync(new List<Stock> {
                    new Stock()
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

            var controller = new StockController(mockStockService.Object, mockLogger.Object);

            // Act
            var result = await controller.GetAllStocksAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var stocks = Assert.IsAssignableFrom<IEnumerable<Stock>>(okResult.Value);
            Assert.True(stocks.Count() == 2);
            Assert.Equal("AAPL", stocks.First().TickerSymbol);
            Assert.Equal("MSFT", stocks.Last().TickerSymbol);
        }



        [Fact]
        public async Task GetAllStocksAsync_EmptyStocks()
        {
            // Arrange
            var mockLogger = new Mock<ILogger>();

            var mockStockService = new Mock<IStockService>();
            mockStockService.Setup(service => service.GetAllStocksAsync())
                .ReturnsAsync((IEnumerable<Stock>)null);

            var controller = new StockController(mockStockService.Object, mockLogger.Object);

            // Act
            var result = await controller.GetAllStocksAsync();

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetAllStocksAsync_StockServiceGetAllStockAsync_ThrowsException_Status500Returned()
        {
            // Arrange
            var mockStockService = new Mock<IStockService>();
            mockStockService.Setup(service => service.GetAllStocksAsync())
                .ThrowsAsync(new Exception("Test exception"));
            var mockLogger = new Mock<ILogger>();
            var controller = new StockController(mockStockService.Object, mockLogger.Object);

            // Act
            var actionResult = await controller.GetAllStocksAsync();

            // Assert
            var objectResult = actionResult as ObjectResult;
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
        }
    }
}
