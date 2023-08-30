using StockAPI.Domain.Entities;
using System.Threading.Tasks;

namespace StockAPI.Application.Services
{
    public interface IBrokerService
    {
        Task<Broker> GetBrokerByIdAsync(int brokerId);
        Task<Broker> GetBrokerByNameAsync(string brokerName);
    }
}
