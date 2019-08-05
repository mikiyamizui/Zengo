using System.Data;

namespace Zengo
{
    public static class Extensions
    {
        public static IBuilder Zengo<TConnection>(this TConnection connection)
            where TConnection : IDbConnection
            => new Builder(connection);
    }
}