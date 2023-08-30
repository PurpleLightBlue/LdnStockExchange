using Dapper;
using System.Data;

namespace StockAPI.Infrastructure.Repositories.Mappers
{
    public class TradeIdGuidMapper : SqlMapper.TypeHandler<Guid>
    {
        public override void SetValue(IDbDataParameter parameter, Guid guid)
        {
            parameter.Value = guid.ToString();
        }

        public override Guid Parse(object value)
        {
            return new Guid((string)value);
        }
    }
}
