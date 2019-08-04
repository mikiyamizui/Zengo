namespace Zengo.Interfaces
{
    public interface IConfig
    {
        string FileNameFormat { get; set; }

        string QuoteString { get; set; }

        string NullString { get; set; }

        string EmptyString { get; set; }
    }
}