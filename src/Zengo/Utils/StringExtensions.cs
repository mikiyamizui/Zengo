using System.Text;

namespace Zengo.Utils
{
    internal static class StringExtensions
    {
        public static int GetByteCount(this string str)
            => Encoding.GetEncoding(932).GetByteCount(str);

        public static string Pad(this string str, bool right, int count, char c = ' ')
            => right
            ? str.PadLeft((count + str.Length - str.GetByteCount()), c)
            : str.PadRight((count + str.Length - str.GetByteCount()), c);
    }
}