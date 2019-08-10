using System.Text;
namespace Zengo.Csv
{
    public class CsvConfig
    {
        public Encoding Encoding { get; set; } = Encoding.Default;
        public bool OutputColumnLine { get; set; } = true;
        public bool ForceQuote { get; set; } = false;
    }
}
