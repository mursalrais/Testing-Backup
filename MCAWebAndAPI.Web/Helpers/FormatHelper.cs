using System.Linq;

namespace MCAWebAndAPI.Web.Helpers
{
    public class FormatHelper
    {
        public string ConvertToDigit(int value, int numberOfDigit)
        {
            var result = value + string.Empty;
            for(int index = 0; index < numberOfDigit; index++)
            {
                result += 0;
            }

            result = result.Reverse().ToString();
            return result;
        }
    }
}