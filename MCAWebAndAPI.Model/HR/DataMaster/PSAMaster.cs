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
    public class PSAMaster : Item
    {
        public string PSAID { get; set; }

        public string PSANumber { get; set; }

        public string ProfessionalID { get; set; }

        public string Position { get; set; }

        public string ProjectOrUnit { get; set; }

        public string PsaExpiryDate { get; set; }

        public string JoinDate { get; set; }

        public string DateOfNewPSA { get; set; }
    }
}
