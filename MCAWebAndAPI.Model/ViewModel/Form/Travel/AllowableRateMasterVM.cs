using MCAWebAndAPI.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Form.Travel
{
    public class AllowableRateMasterVM : Item
    {
        public string Province { get; set; }

        public decimal PerDiem { get; set; }

        public decimal Lodging { get; set; }

        public decimal Taxi { get; set; }

        public decimal RentCar { get; set; }
            
    }
}
