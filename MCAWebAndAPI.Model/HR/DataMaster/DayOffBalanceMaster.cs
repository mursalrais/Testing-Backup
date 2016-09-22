using MCAWebAndAPI.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.HR.DataMaster
{
    public class DayOffBalanceMaster : Item
    {
        public string PSATitle { get; set; }

        public DateTime DateOfNewPsa { get; set; }

        public DateTime EndOfContract { get; set; }

        public DateTime LastWorkingDate { get; set; }

        public int Entitlement { get; set; }

        public int DayOffBrought { get; set; }

        public int Deduction { get; set; }

        public int Draft { get; set; }

        public int PendingApproval { get; set; }

        public int Approved { get; set; }

        public int Rejected { get; set; }

        public int EntitlementTotal { get; set; }

        public int Balance { get; set; }

        public int PSA { get; set; }

        public int PSARenewal { get; set; }

        public string Professional { get; set; }

        public string ProfessionalID { get; set; }

        public string PSAStatus { get; set; }

        public string DayOffName { get; set; }

        public int FinalBalance { get; set; }

        public DateTime FirstDate { get; set; }
    }
}
