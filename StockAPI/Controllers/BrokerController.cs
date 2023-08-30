using Microsoft.AspNetCore.Mvc;
using StockAPI.Application.Services;

namespace StockAPI.Controllers
{
    [ApiController]
    [Route("api/brokers")]
    public class BrokerController : ControllerBase
    {
        private readonly IBrokerService _brokerService;
        private readonly Serilog.ILogger _logger;

        public BrokerController(IBrokerService brokerService, Serilog.ILogger logger)
        {
            _brokerService = brokerService ?? throw new ArgumentNullException(nameof(brokerService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));   
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBrokerByIdAsync(int id)
        {
            try
            {
                var broker = await _brokerService.GetBrokerByIdAsync(id);
                if (broker == null)
                {
                    return NotFound();
                }
                return Ok(broker);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"BrokersController.GetBrokerByIdAsync An error occurred while processing your request for BrokerID {id}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("byname/{brokerName}")]
        public async Task<IActionResult> GetBrokerByNameAsync(string brokerName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(brokerName))
                {
                    _logger.Error($"BrokersController.GetBrokerByNameAsync invalid argument: {brokerName}");
                    return BadRequest($"'{nameof(brokerName)}' cannot be null or whitespace.");
                }

                var broker = await _brokerService.GetBrokerByNameAsync(brokerName);
                if (broker == null)
                {
                    return NotFound();
                }
                return Ok(broker);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"BrokersController.GetBrokerByNameAsync An error occurred while processing your request for BrokerName {brokerName}");
                return  StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
