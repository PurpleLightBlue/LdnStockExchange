using StockAPI.Domain.Entities;
using System.Threading.Tasks;

namespace StockAPI.Domain.Repositories
{
    public interface IBrokerRepository
    {
        Task<Broker> GetBrokerByIdAsync(int brokerId);
        Task<Broker> GetBrokerByNameAsync(string brokerName);
    }
}
