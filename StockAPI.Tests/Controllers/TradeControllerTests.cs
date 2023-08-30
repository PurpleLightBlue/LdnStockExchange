using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Serilog;
using StockAPI.Application.Services;
using StockAPI.Controllers;
using StockAPI.Domain.Entities;
using Xunit;

namespace StockAPI.Tests.Controllers
{
    public class TradeControllerTests
    {
        [Fact]
        public async Task RecordTradeAsync_ValidTrade_ReturnsOk()
        {
            // Arrange
            var trade = new Trade()
            {
                
                BrokerId = 1,
                Price = 100,
                Shares = 10,
                TickerSymbol = "APPL",
                TradeId = new Guid(),
                TradeTime = DateTime.Now
            };  


            var tradeServiceMock = new Mock<ITradeService>();
            tradeServiceMock.Setup(service => service.RecordTradeAsync(It.IsAny<Trade>()))
                .ReturnsAsync(trade);

            var loggerMock = new Mock<ILogger>();

            var controller = new TradeController(tradeServiceMock.Object, loggerMock.Object);

            // Act
            var result = await controller.RecordTradeAsync(new Trade());

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okObjectResult = result as OkObjectResult;
            var recordedTrade = okObjectResult.Value as Trade;

            Assert.Equal(trade.Id, recordedTrade.Id);
            Assert.Equal(trade.BrokerId, recordedTrade.BrokerId);
            Assert.Equal(trade.Price, recordedTrade.Price);
            Assert.Equal(trade.Shares, recordedTrade.Shares);
            Assert.Equal(trade.TickerSymbol, recordedTrade.TickerSymbol);
            Assert.Equal(trade.TradeId, recordedTrade.TradeId);
            Assert.Equal(trade.TradeTime, recordedTrade.TradeTime);
        }

        [Fact]
        public async Task RecordTradeAsync_TradeServiceThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var tradeServiceMock = new Mock<ITradeService>();
            tradeServiceMock.Setup(service => service.RecordTradeAsync(It.IsAny<Trade>()))
                .ThrowsAsync(new Exception("Some error message"));

            var loggerMock = new Mock<ILogger>();

            var controller = new TradeController(tradeServiceMock.Object, loggerMock.Object);

            // Act
            var result = await controller.RecordTradeAsync(new Trade());

            // Assert
            var objectResult = result as ObjectResult;
            Assert.Equal(500, objectResult.StatusCode);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task GetTradesByTickerSymbolAsync_InvalidTickerSymbol_ReturnsBadRequest(string tickerSymbol)
        {
            // Arrange
            var tradeServiceMock = new Mock<ITradeService>();
            var loggerMock = new Mock<ILogger>();

            var controller = new TradeController(tradeServiceMock.Object, loggerMock.Object);

            // Act
            var result = await controller.GetTradesByTickerSymbolAsync(tickerSymbol);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal($"'{nameof(tickerSymbol)}' cannot be null or whitespace.", badRequestResult.Value);
        }

        [Fact]
        public async Task GetTradesByTickerSymbolAsync_TradeServiceThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var tradeServiceMock = new Mock<ITradeService>();
            tradeServiceMock.Setup(service => service.GetTradesByTickerSymbolAsync("AAPL"))
                .ThrowsAsync(new Exception("Some error message"));

            var loggerMock = new Mock<ILogger>();

            var controller = new TradeController(tradeServiceMock.Object, loggerMock.Object);

            // Act
            var result = await controller.GetTradesByTickerSymbolAsync("AAPL");

            // Assert
            var objectResult = result as ObjectResult;
            Assert.Equal(500, objectResult.StatusCode);
        }

        [Fact]
        public async Task CalculateAverageStockPriceAsync_ValidTickerSymbol_ReturnsOk()
        {
            // Arrange
            var tradeServiceMock = new Mock<ITradeService>();
            tradeServiceMock.Setup(service => service.CalculateAverageStockPriceAsync("AAPL"))
                .ReturnsAsync(100.0m);

            var loggerMock = new Mock<ILogger>();

            var controller = new TradeController(tradeServiceMock.Object, loggerMock.Object);

            // Act
            var result = await controller.CalculateAverageStockPriceAsync("AAPL");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(100.0m, okResult.Value);
        }

        [Fact]
        public async Task CalculateAverageStockPriceAsync_TickerSymbolIsNull_ReturnsBadRequest()
        {
            // Arrange
            var tradeServiceMock = new Mock<ITradeService>();
            var loggerMock = new Mock<ILogger>();

            var controller = new TradeController(tradeServiceMock.Object, loggerMock.Object);

            // Act
            var result = await controller.CalculateAverageStockPriceAsync(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal($"'tickerSymbol' cannot be null or whitespace.", badRequestResult.Value);
        }

        [Fact]
        public async Task CalculateAverageStockPriceAsync_TradeServiceThrowsException_ReturnsErrorStatus500()
        {
            // Arrange
            var tradeServiceMock = new Mock<ITradeService>();
            tradeServiceMock.Setup(service => service.CalculateAverageStockPriceAsync("AAPL"))
                .ThrowsAsync(new Exception("Some error message"));

            var loggerMock = new Mock<ILogger>();

            var controller = new TradeController(tradeServiceMock.Object, loggerMock.Object);

            // Act
            var result = await controller.CalculateAverageStockPriceAsync("AAPL");

            // Assert
            var objectResult = result as ObjectResult;
            Assert.Equal(500, objectResult.StatusCode);
        }
    }
}
