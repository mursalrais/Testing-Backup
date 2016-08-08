using MCAWebAndAPI.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.HR.DataMaster
{
    public class MonthlyFeeMaster : Item
    {
        public int ProfessionalID { get; set; }

        public DateTime DateOfNewFee { get; set; }

        public DateTime EndDate { get; set; }

        public double MonthlyFee { get; set; }


    }
}
