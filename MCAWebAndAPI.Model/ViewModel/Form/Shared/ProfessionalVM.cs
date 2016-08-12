using MCAWebAndAPI.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Form.Shared
{
    public class ProfessionalVM:Item
    {
        public int ID { get; set; }

        public string ProfesionalName { get; set; }

        public string ProjectID { get; set; }

        public string ProjectName { get; set; }

        public string ContactNo { get; set; }
    }
}
