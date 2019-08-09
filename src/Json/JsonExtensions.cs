using Zengo.Json;

namespace Zengo
{
    public static class JsonExtensions
    {
        public static IBuilder SaveAsJson(
            this IBuilder self,
            string tableName, string filterSql = null, JsonConfig config = null)
        {
            self.Tables[tableName] = filterSql;
            self.Loggers.Add(new JsonLogger(config ?? new JsonConfig()));
            return self;
        }
    }
}