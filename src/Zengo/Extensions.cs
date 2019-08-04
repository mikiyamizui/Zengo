using System.Data;
using System.Data.Common;

namespace Zengo
{
    public static class Extensions
    {
        public static IManager<TDataAdapter> Zengo<TDataAdapter>(this IDbConnection connection)
            where TDataAdapter : DbDataAdapter
            => new Manager<TDataAdapter>(connection);
    }
}