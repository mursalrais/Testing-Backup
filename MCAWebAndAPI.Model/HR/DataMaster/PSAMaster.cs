using MCAWebAndAPI.Model.Common;
using System;

namespace MCAWebAndAPI.Model.HR.DataMaster
{
    public class PSAMaster : Item
    {
        public string PSAID { get; set; }

        public string PSANumber { get; set; }

        public string ProfessionalID { get; set; }

        public string Position { get; set; }

        public string ProjectOrUnit { get; set; }

        public string PsaExpiryDateString { get; set; }

        public DateTime PSAExpiryDate { get; set; }

        public string JoinDateString { get; set; }

        public DateTime JoinDate { get; set; }


        public string DateOfNewPSAString { get; set; }

        public DateTime DateOfNewPSA { get; set; }

        public DateTime LastWorkingDate { get; set; }

    }
}
