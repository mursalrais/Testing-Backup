using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.HR.DataMaster
{
    public class AdjustmentMaster
    {
        public string ProfessionalID { get; set; }

        public DateTime AdjustmentPeriod { get; set; }

        public string Professional { get; set; }

        public string AdjustmentType { get; set; }

        public double AdjustmentAmount { get; set; }

        public string Currency { get; set; }

        public string DebitOrCredit { get; set; }
    }
}
