using System.Text;
namespace Zengo
{
    public class CsvConfig : Config
    {
        public Encoding Encoding { get; set; } = Encoding.Default;
        public bool OutputColumnLine { get; set; } = true;
        public bool ForceQuote { get; set; } = false;
    }
}
