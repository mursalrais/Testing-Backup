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
    public class ProfessionalMaster
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string FirstMiddleName { get; set; }

        public string Position { get; set; }

        public int PositionId { get; set; }

        public string Status { get; set; }

        public string Project_Unit { get; set; }

        public string UserLogin { get; set; }

        public string OfficeEmail { get; set; }

        public string PSANumber { get; set; }

        public DateTime JoinDate { get; set; } = DateTime.Now;

    }
}
