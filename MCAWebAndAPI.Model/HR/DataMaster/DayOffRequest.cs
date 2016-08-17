using MCAWebAndAPI.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.HR.DataMaster
{
    public class DayOffRequest : Item
    {
        public string DayOffType { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int ProfessionalID { get; set; }

        public int TotalDays { get; set; }

        public string ApprovalStatus { get; set; }



    }
}
