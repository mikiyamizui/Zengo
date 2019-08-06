namespace Zengo
{
    public abstract class Config
    {
        public string FileNameFormat { get; set; } = "{0:yyyyMMdd-HHmmss-ffff}";

        public string NullString { get; set; } = "(NULL)";
    }
}