using MCAWebAndAPI.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class ProfessionalsVM : Item
    {
        public int ID { get; set; }

        public string ProfessionalName { get; set; }

        public string ProjectID { get; set; }

        public string ProjectName { get; set; }

        public string ContactNo { get; set; }

        public string Posision { get; set; }
        
    }
}
