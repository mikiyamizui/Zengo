namespace Zengo
{
    public abstract class Config
    {
        public string FileNameFormat { get; set; } = "{0:yyyyMMddHHmmssfff}";

        public string NullString { get; set; } = "(NULL)";
    }
}