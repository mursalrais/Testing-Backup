using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Chart
{
    public class OverallRIAChartVM
    {
        public string Name { get; set; }

        public List<int> Data = new List<int>();

        public string Color { get; set; }
    }
}
