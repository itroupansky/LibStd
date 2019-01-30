using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LibStd.Common
{
    /// <summary>
    /// class to encapsulate common validations
    /// </summary>
    public class Validations
    {
        /// <summunctionary>
        /// validate Email using RegEx, returns true/false
        /// </summary>
        /// <param name="email"></param>
        public static bool ValidateEmail(string email)
        {
            return Regex.IsMatch(email,
                        @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                        @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                        RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }

        /// <summunctionary>
        /// Validate SSN or PIC ID without dashes-dashes need to be removed prior to calling this function
        /// </summary>
        /// <param name="ssn"></param>
        public static bool ValidateSSN(string ssn)
        {
            string pattern = "^[0-9a-zA-Z]{1}[0-9]{8}$";
            return Regex.IsMatch(ssn, pattern);
        }

        /// <summary>
        /// Validate zip code - 5 numbers
        /// </summary>
        /// <param name="zip"></param>
        /// <returns></returns>
        public static bool ValidateZipCode(string zip)
        {
            string pattern = "^[0-9]{5}$";
            return Regex.IsMatch(zip, pattern);

        }

        /// <summary>
        /// Validate zip4 code - 4 numbers
        /// </summary>
        /// <param name="zip"></param>
        /// <returns></returns>
        public static bool ValidateZip4(string zip)
        {
            string pattern = "^[0-9]{4}$";
            return Regex.IsMatch(zip, pattern);

        }
    }
}
