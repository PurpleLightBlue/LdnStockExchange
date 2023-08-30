using System.ComponentModel.DataAnnotations;

namespace StockAPI.Domain.Entities
{
    public class Trade
    {
        public int Id { get; set; }
        [Key]
        public Guid TradeId { get; set; }
        public string TickerSymbol { get; set; }
        public decimal Price { get; set; }
        public decimal Shares { get; set; }
        public int BrokerId { get; set; }
        public DateTime TradeTime { get; set; }
    }
}