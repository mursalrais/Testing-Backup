using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.Common
{
    public static class DoubleExtentions
    {
        public static double ConvertInfinityOrNanToZero(this double input)
        {
            if (Double.IsInfinity(input) || Double.IsNaN(input))
                return 0d;
            return input;
        }
    }
}
