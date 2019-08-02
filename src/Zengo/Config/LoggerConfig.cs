namespace Zengo.Config
{
    public abstract class LoggerConfig
    {
        public string FileName { get; set; } = "-yyyyMMddHHmmssfff";//"--yyyy-MM-dd--HH-mm-ss--fff";
        public string Null { get; set; } = "(NULL)";
        public string Empty { get; set; } = "''";
    }
}