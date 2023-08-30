using System;

namespace StockAPI.Domain.Exceptions
{
    public class InvalidTradeException : Exception
    {
        public InvalidTradeException(string message) : base(message)
        {
        }

        public InvalidTradeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
