using System.Data.Common;
using System.Text;
using Zengo.Interfaces;

namespace Zengo.Text
{
    public static class Extensions
    {
        public static ILoggerManager<TDataAdapter> AsText<TDataAdapter>(
            this ILoggerManager<TDataAdapter> self, TextConfig config = null)
            where TDataAdapter : DbDataAdapter
        {
            var manager = self.ConfigManager;
            config = config ?? new TextConfig();
            var logger = new TextLogger<TDataAdapter>(manager.Connection, config, self.TableName, self.FilterSql);

            self.AddLogger(logger);

            return self;
        }

        public static int GetByteCount(this string str)
            => Encoding.GetEncoding(932).GetByteCount(str);

        public static string Pad(this string str, bool right, int count, char c = ' ')
            => right
            ? str.PadLeft((count + str.Length - str.GetByteCount()), c)
            : str.PadRight((count + str.Length - str.GetByteCount()), c);
    }
}