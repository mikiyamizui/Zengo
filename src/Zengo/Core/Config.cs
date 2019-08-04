using Zengo.Interfaces;

namespace Zengo.Core
{
    public abstract class Config : IConfig
    {
        public string FileNameFormat { get; set; } = "yyyyMMddHHmmssfff";

        public string QuoteString { get; set; } = "\"";

        public string NullString { get; set; } = "(NULL)";

        public string EmptyString { get; set; } = "\"\"";
    }
}