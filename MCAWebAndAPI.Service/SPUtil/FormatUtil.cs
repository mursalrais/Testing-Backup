using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Service.SPUtil
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
            for(int i = 0; i< numberOfDigit - stringNumber.Length; i++)
            {
                stringNumber = "0" + stringNumber;
            }
            return stringNumber;
        }
    }
}
