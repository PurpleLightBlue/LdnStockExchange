namespace StockAPI.Domain.Entities
{
    public class Stock
    {
        public int Id { get; set; }
        public string TickerSymbol { get; set; }
        public decimal CurrentValue { get; set; }
        public string Name { get; set; }
    }
}