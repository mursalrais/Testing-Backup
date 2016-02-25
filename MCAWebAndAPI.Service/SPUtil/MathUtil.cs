using System;

namespace MCAWebAndAPI.Service.SPUtil
{
    public class MathUtil
    {
        public static bool CompareDouble(double value1, double value2)
        {
            // 100 % = 1
            // 0.1% = .0001
            var difference = .0001;
            return Math.Abs(value1 - value2) <= difference;
        }   
    }
}
