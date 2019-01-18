using NPinyin;
using System.Text.RegularExpressions;

namespace MZBlog.Core.Extensions
{
    public static class StringExtensions
    {
        static StringExtensions()
        {
        }

        public static bool IsNullOrWhitespace(this string text)
        {
            return string.IsNullOrWhiteSpace(text);
        }

        public static string FormatWith(this string text, params object[] args)
        {
            return string.Format(text, args);
        }

        public static string ToSlug(this string value)
        {
            value = value.ToLowerInvariant();

            value = ConvertChineseToPY(value);

            value = value.Replace("#", "-sharp ").Replace("@", "-at ")
                         .Replace("$", "-dollar ").Replace("%", "-percent ")
                         .Replace("&", "-and ").Replace("||", "-or ");

            value = Regex.Replace(value.TrimEnd(), @"\s", "-", RegexOptions.Compiled);

            value = Regex.Replace(value, @"[^a-z0-9\s-_]", "", RegexOptions.Compiled);

            value = value.Trim('-', '_');

            value = Regex.Replace(value, @"([-_]){2,}", "$1", RegexOptions.Compiled);

            return value;
        }

        private static string ConvertChineseToPY(string value)
        {
            return Regex.Replace(value, "[\u4e00-\u9fa5]", (m) => string.Format(" {0} ", Pinyin.GetPinyin(m.Value).ToLower()));
        }
    }
}