using System;
using System.Collections.Generic;
using System.Text;

namespace Common.ExtensionMethods
{
    public static class ExtensionMethods
    {
        public static string USPhoneFormat(this string phoneInput)
        {
            string format = string.Format("{0:(###) ###-####}", long.Parse(phoneInput));
            return format;
        }
    }
}
