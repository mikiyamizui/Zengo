namespace Zengo.Abstracts
{
    public abstract class BaseLoggerConfig
    {
        /// <summary>
        /// FileName is decided by DateTime.Now.ToString($"{TableName}{FileNameFormat}{Extension}");
        /// </summary>
        public string FileNameFormat { get; set; } = "-yyyyMMddHHmmssfff";//"--yyyy-MM-dd--HH-mm-ss--fff";

        public string QuoteString { get; set; } = "\"";

        public string NullString { get; set; } = "(NULL)";

        public string EmptyString { get; set; } = "\"\"";
    }
}