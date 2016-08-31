using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Control;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MCAWebAndAPI.Model.Common;

namespace MCAWebAndAPI.Model.HR.DataMaster
{
    public class ProfessionalMaster : Item
    {
        public string Name { get; set; }

        public string FirstMiddleName { get; set; }
        
        public string Position { get; set; }

        public int PositionId { get; set; }

        public string Status { get; set; }

        public string Project_Unit { get; set; }

        public string UserLogin { get; set; }

        public string OfficeEmail { get; set; }

        public string PSANumber { get; set; }

        public string Desc { get; set; }

        [UIHint("Date")]
        public DateTime JoinDate { get; set; } = DateTime.Now;

        public string JoinDateTemp { get; set; }

        public string PersonalMail { get; set; }

        public string InsuranceAccountNumber { get; set; }

        public string MobileNumber { get; set; }

        #region Used in Payroll

        public string BankAccountName { get; set; }

        public string Currency { get; set; }

        public string BankAccountNumber { get; set; }

        public string BankBranchOffice { get; set; }

        public string BankName { get; set; }

        #endregion

    }
}
