using System;
using System.Text.RegularExpressions;

namespace LibStd.Common
{
    public static class Utilities
    {
        public static string FormatPhoneNumber(string phoneNum)
        {
            Regex regExObj = new Regex(@"[^\d]");
            var phoneFrmttd = "";
            phoneFrmttd = regExObj.Replace(phoneNum, "");
            if (phoneFrmttd.Length == 10)
            {
                phoneFrmttd = string.Format("{0:###-###-####}", Convert.ToInt64(phoneFrmttd.ToString()));
                return phoneFrmttd;
            }
            else if (phoneFrmttd.Length == 11 && phoneFrmttd[0] == '1')
            {
                phoneFrmttd = Convert.ToInt64(phoneFrmttd.ToString()).ToString("###-###-####");
                return phoneFrmttd;
            }
            else
            {
                return phoneNum; //return original [some phone db fields in TrackerWeb allow text, extensions, etc.]
            }
        }
    }
}
