using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Control;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MCAWebAndAPI.Model.HR.DataMaster
{
    public class PSAMaster
    {
        public string Professional { get; set; }

        public string ID { get; set; }

        public string ProjectOrUnit { get; set; }

        public string PsaExpiryDate { get; set; }

        public string JoinDate { get; set; }

        public string DateOfNewPSA { get; set; }
    }
}
