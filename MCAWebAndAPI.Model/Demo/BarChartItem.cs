using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.Demo
{
    public class BarChartItem
    {
        public string Name { get; set; }
        public int Budget { get; set; }
        public int Actual { get; set; } 
        public int category { get; set; }
        public DateTime Date { get; set; }
    }
}
