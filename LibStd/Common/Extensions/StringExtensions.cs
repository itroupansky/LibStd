using System;
using System.Text.RegularExpressions;

namespace LibStd.Common.Extensions
{
    public static class StringExtensions
    {
        public static string Right(this string input, int numOfChars)
        {
            return input?.Substring(Math.Max(0, input.Length - numOfChars));
        }

        public static string Left(this string input, int numOfChars)
        {
            return input?.Substring(0, Math.Min(input.Length, numOfChars));
        }

        public static string EndWith(this string input, string endText)
        {
            return (input == null) ? null : (input.EndsWith(endText ?? string.Empty)) ? input : input + endText;
        }

        public static bool IsEmail(this string input)
        {
            return  Regex.IsMatch(input,
                        @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                        @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                        RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }
    }
}
