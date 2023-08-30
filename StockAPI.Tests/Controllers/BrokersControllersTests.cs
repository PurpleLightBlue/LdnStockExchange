using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Serilog;
using StockAPI.Application.Services;
using StockAPI.Controllers;
using StockAPI.Domain.Entities;

namespace StockAPI.Tests.Controllers
{
    public class BrokersControllerTests
    {
        [Fact]
        public async Task GetBrokerByIdAsync_ValidId_ReturnsOkResultWithBroker()
        {
            // Arrange
            var mockBrokerService = new Mock<IBrokerService>();
            var expectedBroker = new Broker { BrokerId = 1, BrokerName = "Broker1" };
            mockBrokerService.Setup(service => service.GetBrokerByIdAsync(1))
                             .ReturnsAsync(expectedBroker);

            var controller = new BrokerController(mockBrokerService.Object, new Mock<ILogger>().Object);

            // Act
            var actionResult = await controller.GetBrokerByIdAsync(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var actualBroker = Assert.IsType<Broker>(okResult.Value);
            Assert.Equal(expectedBroker.BrokerId, actualBroker.BrokerId);
            Assert.Equal(expectedBroker.BrokerName, actualBroker.BrokerName);
        }

        [Fact]
        public async Task GetBrokerByIdAsync_InvalidId_ReturnsNotFoundResult()
        {
            // Arrange
            var mockBrokerService = new Mock<IBrokerService>();
            mockBrokerService.Setup(service => service.GetBrokerByIdAsync(1))
                             .ReturnsAsync((Broker)null);

            var controller = new BrokerController(mockBrokerService.Object, new Mock<ILogger>().Object);

            // Act
            var actionResult = await controller.GetBrokerByIdAsync(1);

            // Assert
            Assert.IsType<NotFoundResult>(actionResult);
        }

        [Fact]
        public async Task GetBrokerByNameAsync_ValidName_ReturnsOkResultWithBroker()
        {
            // Arrange
            var mockBrokerService = new Mock<IBrokerService>();
            var expectedBroker = new Broker { BrokerId = 1, BrokerName = "Broker1" };
            mockBrokerService.Setup(service => service.GetBrokerByNameAsync("Broker1"))
                             .ReturnsAsync(expectedBroker);

            var controller = new BrokerController(mockBrokerService.Object, new Mock<ILogger>().Object);

            // Act
            var actionResult = await controller.GetBrokerByNameAsync("Broker1");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var actualBroker = Assert.IsType<Broker>(okResult.Value);
            Assert.Equal(expectedBroker.BrokerId, actualBroker.BrokerId);
            Assert.Equal(expectedBroker.BrokerName, actualBroker.BrokerName);
        }

        [Fact]
        public async Task GetBrokerByNameAsync_InvalidName_ReturnsNotFoundResult()
        {
            // Arrange
            var mockBrokerService = new Mock<IBrokerService>();
            mockBrokerService.Setup(service => service.GetBrokerByNameAsync(It.IsAny<string>()))
                             .ReturnsAsync((Broker)null);

            var controller = new BrokerController(mockBrokerService.Object, new Mock<ILogger>().Object);

            // Act
            var actionResult = await controller.GetBrokerByNameAsync("NonExistentBroker");

            // Assert
            Assert.IsType<NotFoundResult>(actionResult);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task GetBrokerByNameAsync_InvalidBrokerName_ThrowsArgumentException(string brokerName)
        {
            // Arrange
            var brokerServiceMock = new Mock<IBrokerService>();
            var controller = new BrokerController(brokerServiceMock.Object, new Mock<ILogger>().Object);

            // Act & Assert
            var actionResult = await controller.GetBrokerByNameAsync(brokerName);
            Assert.IsType<BadRequestObjectResult>(actionResult);
        }



        [Fact]
        public async Task GetBrokerByNameAsync_ThrowsException_ReturnsObjectResult_With500ErrorStatus()
        {
            // Arrange
            var mockBrokerService = new Mock<IBrokerService>();
            mockBrokerService.Setup(service => service.GetBrokerByNameAsync(It.IsAny<string>()))
                             .Throws<Exception>();

            var controller = new BrokerController(mockBrokerService.Object, new Mock<ILogger>().Object);

            // Act
            var actionResult = await controller.GetBrokerByNameAsync("NonExistentBroker");

            // Assert
            var objectResult = actionResult as ObjectResult;
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
        }
    }
}
