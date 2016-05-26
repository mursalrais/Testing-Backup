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

        public string Position { get; set; }

        public string Status { get; set; }

    }
}
