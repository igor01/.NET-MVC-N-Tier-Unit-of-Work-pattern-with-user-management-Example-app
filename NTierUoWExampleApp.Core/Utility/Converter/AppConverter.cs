using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NTierUoWExampleApp.Core.Utility.Converter
{
    public static class AppConverter
    {
        public static long DateTimeToUnixTime(DateTime dateTime)
        {
            var timeSpan = (dateTime - new DateTime(1970, 1, 1, 0, 0, 0).ToUniversalTime());
            return (long)timeSpan.TotalSeconds;
        }

        public static string SplitCamelCase(string str)
        {
            return Regex.Replace(
                Regex.Replace(
                    str,
                    @"(\P{Ll})(\P{Ll}\p{Ll})",
                    "$1 $2"
                ),
                @"(\p{Ll})(\P{Ll})",
                "$1 $2"
            );
        }

        public static string MakeCamelCase(string str)
        {
            return Regex.Replace(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower()), @"\s+", "");
        }
    }
}
