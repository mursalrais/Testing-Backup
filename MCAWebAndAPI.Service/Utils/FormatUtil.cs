﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Service.Utils
{
    public class FormatUtil
    {
        /// <summary>
        /// Generate digit format string from given value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="numberOfDigit"></param>
        /// <returns></returns>
        public static string ConvertToDigitNumber(int value, int numberOfDigit)
        {
            var stringNumber = value + string.Empty;
            while (stringNumber.Length < numberOfDigit)
            {
                stringNumber = "0" + stringNumber;
            }
            return stringNumber;
        }

        public static string ConvertToCleanSiteUrl(string siteUrl)
        {
            var result = string.Empty;
            if (siteUrl != null)
            {
                result = siteUrl;
                result = result.Replace("\"", "");
                result = result.Replace("\'", "");
            }

            return result;
        }

    }
}
