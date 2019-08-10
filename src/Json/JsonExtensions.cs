using Zengo.Json;

namespace Zengo
{
    public static class JsonExtensions
    {
        public static IBuilder SaveAsJson(this IBuilder self, string tableName, string filterSql = null)
        {
            self.Tables[tableName] = filterSql;
            self.Loggers.Add(new JsonLogger());
            return self;
        }
    }
}
