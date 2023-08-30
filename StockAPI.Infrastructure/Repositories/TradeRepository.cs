using System.Data;
using Dapper;
using StockAPI.Domain.Entities;
using Serilog;

public class TradeRepository : ITradeRepository
{
    private readonly IDbConnection _dbConnection;
    private readonly ILogger _logger;

    public TradeRepository(IDbConnection dbConnection, ILogger logger)
    {
        _dbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Trade> AddTradeAsync(Trade trade)
    {
        try
        {
            string sql = "INSERT INTO Trades (TradeId, TickerSymbol, Price, Shares, BrokerId, TradeTime) " +
                         "VALUES (@TradeId, @TickerSymbol, @Price, @Shares, @BrokerId, @TradeTime);" +
                         "SELECT SCOPE_IDENTITY()";

            var id = await _dbConnection.ExecuteScalarAsync<int>(sql, trade);
            trade.Id = id;
            return trade; // Return the same instance with auto-generated ID assigned
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error adding trade");
            throw;
        }
    }

    public async Task<List<Trade>> GetTradesByTickerSymbolAsync(string tickerSymbol)
    {
        try
        {
            string sql = "SELECT * FROM Trades WHERE TickerSymbol = @TickerSymbol";
            var trades = await _dbConnection.QueryAsync<Trade>(sql, new { TickerSymbol = tickerSymbol });
            return trades.AsList();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error getting trades by ticker symbol");
            throw;
        }
    }

    public async Task<bool> IsTradeIdProcessedAsync(Guid tradeId)
    {
        try
        {
            string sql = "SELECT COUNT(*) FROM Trades WHERE TradeId = @TradeId";
            int count = await _dbConnection.ExecuteScalarAsync<int>(sql, new { TradeId = tradeId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error checking if trade ID is processed");
            throw;
        }
    }
}
