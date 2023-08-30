using StockAPI.Domain.Entities;
using StockAPI.Domain.Repositories;

namespace StockAPI.Application.Services
{
    public class BrokerService : IBrokerService
    {
        private readonly IBrokerRepository _brokerRepository;

        public BrokerService(IBrokerRepository brokerRepository)
        {
            _brokerRepository = brokerRepository;
        }

        public async Task<Broker> GetBrokerByIdAsync(int brokerId)
        {
            return await _brokerRepository.GetBrokerByIdAsync(brokerId);
        }

        public async Task<Broker> GetBrokerByNameAsync(string brokerName)
        {
            return await _brokerRepository.GetBrokerByNameAsync(brokerName);
        }
    }
}
