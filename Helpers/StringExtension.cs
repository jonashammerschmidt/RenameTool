using System.Text.RegularExpressions;

namespace Contract.Renamer
{
    public static partial class StringExtension
    {
        public static string PascalToKebabCase(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return Regex.Replace(
                value,
                "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])",
                "-$1",
                RegexOptions.Compiled)
                .Trim()
                .ToLower();
        }

        public static string LowerFirstChar(this string text)
        {
            return char.ToLower(text[0]) + text[1..];
        }
    }
}