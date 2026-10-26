using System.Globalization;
using TSID.Creator.NET;

namespace SettlingEngine.Utils
{
    public class Util
    {
        public static string GenerateId(string prefix = "")
        {
            return $"{prefix}{TsidCreator.GetTsid().ToLong()}";
        }

        public static DateTime DateStringToDateTime(string dateStr, string format = "yyyyMMdd")
        {
            if (DateTime.TryParseExact(dateStr, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                return date;
            }

            return DateTime.MinValue;
        }
    }
}
