using System.Text.RegularExpressions;

namespace Dapper.Extensions
{
    internal static class StringExtensions
    {
        internal static string ToUnderscore(this string input)
        {
            return Regex.Replace(input, "(?<=.)([A-Z])", "_$0", RegexOptions.Compiled).ToLowerInvariant();
        }
    }
}