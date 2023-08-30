using StockAPI.Domain.Entities;
using StockAPI.Domain.Repositories;
using System.Data;
using Dapper;

namespace StockAPI.Infrastructure.Repositories
{
    public class BrokerRepository : IBrokerRepository
    {
        private readonly IDbConnection _dbConnection;

        public BrokerRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<Broker> GetBrokerByIdAsync(int brokerId)
        {
            string query = "SELECT * FROM Brokers WHERE BrokerId = @BrokerId";
            return await _dbConnection.QuerySingleOrDefaultAsync<Broker>(query, new { BrokerId = brokerId });
        }

        public async Task<Broker> GetBrokerByNameAsync(string brokerName)
        {
            string query = "SELECT * FROM Brokers WHERE BrokerName = @BrokerName";
            return await _dbConnection.QuerySingleOrDefaultAsync<Broker>(query, new { BrokerName = brokerName });
        }
    }
}
