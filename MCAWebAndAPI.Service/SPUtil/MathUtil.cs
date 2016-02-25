using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Service.SPUtil
{
    public class MathUtil
    {
        public static bool CompareDouble(double value1, double value2)
        {
            var difference = .1;
            return Math.Abs(value1 - value2) <= difference;
        }   
    }
}
