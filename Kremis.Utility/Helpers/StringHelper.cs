using System.Globalization;
using System.Text;
using System.Threading;

namespace Kremis.Utility.Helpers
{
    public static class StringHelper
    {
        /// <summary>
        /// onverts the string value to titecase excepts conjonction(of, at, for,..)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToTitleCase(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;

            return textInfo.ToTitleCase(value).Replace("Of", "of").Replace("At", "at")
                .Replace("Door", "door").Replace("And", "and").Replace("Or", "or").Replace("To", "to")
                .Replace("For", "for").Replace("But", "but")
                .Replace("De", "de").Replace("Du", "du").Replace("Où", "où").Replace("Ou", "ou")
                .Replace("Et", "et").Replace("Pour", "pour").Replace("Par", "par");
        }

        public static string RemoveSpecialCharacters(this string str)
        {
            StringBuilder sb = new();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_' || c == ' ')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString().Replace(" ", "_");
        }
    }
}
