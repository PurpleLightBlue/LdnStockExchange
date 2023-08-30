using System.Data;
using Dapper;
using Microsoft.Data.Sqlite;
using StockAPI.Domain.Entities;
using StockAPI.Infrastructure.Repositories;

namespace StockAPI.Tests.Repositories
{
    public class BrokerRepositoryTests : IDisposable
    {
        private readonly IDbConnection _connection;
        private readonly BrokerRepository _brokerRepository;

        public BrokerRepositoryTests()
        {
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();
            InitializeDatabase(); // Create tables and schema
            _brokerRepository = new BrokerRepository(_connection);
        }

        private void InitializeDatabase()
        {
            // Create necessary tables and schema for Brokers
            var createTableSql = @"
                CREATE TABLE Brokers (
                    BrokerId INTEGER PRIMARY KEY,
                    BrokerName NVARCHAR(255) NOT NULL
                );
            ";

            using (var command = _connection.CreateCommand())
            {
                command.CommandText = createTableSql;
                command.ExecuteNonQuery();
            }
        }

        [Fact]
        public async Task GetBrokerByIdAsync_Should_ReturnMatchingBroker()
        {
            // Arrange
            var expectedBroker = new Broker { BrokerId = 1, BrokerName = "ABC Brokers" };
            await _connection.ExecuteAsync("INSERT INTO Brokers (BrokerId, BrokerName) VALUES (@BrokerId, @BrokerName)",
                                           new { expectedBroker.BrokerId, expectedBroker.BrokerName });

            // Act
            var result = await _brokerRepository.GetBrokerByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedBroker.BrokerId, result.BrokerId);
            Assert.Equal(expectedBroker.BrokerName, result.BrokerName);
        }

        [Fact]
        public async Task GetBrokerByIdAsync_Should_ReturnNull_WhenNoMatchingBroker()
        {
            // Act
            var result = await _brokerRepository.GetBrokerByIdAsync(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetBrokerByNameAsync_Should_ReturnMatchingBroker()
        {
            // Arrange
            var expectedBroker = new Broker { BrokerId = 1, BrokerName = "ABC Brokers" };
            await _connection.ExecuteAsync("INSERT INTO Brokers (BrokerId, BrokerName) VALUES (@BrokerId, @BrokerName)",
                                           new { expectedBroker.BrokerId, expectedBroker.BrokerName });

            // Act
            var result = await _brokerRepository.GetBrokerByNameAsync("ABC Brokers");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedBroker.BrokerId, result.BrokerId);
            Assert.Equal(expectedBroker.BrokerName, result.BrokerName);
        }

        [Fact]
        public async Task GetBrokerByNameAsync_Should_ReturnNull_WhenNoMatchingBroker()
        {
            // Act
            var result = await _brokerRepository.GetBrokerByNameAsync("XYZ Brokers");

            // Assert
            Assert.Null(result);
        }

        // Dispose the connection after all tests
        public void Dispose()
        {
            _connection.Close();
            _connection.Dispose();
        }
    }
}
