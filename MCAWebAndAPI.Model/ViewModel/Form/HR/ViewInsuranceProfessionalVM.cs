using Kendo.Mvc.UI;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Web;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class ViewInsuranceProfessionalVM : Item
    {
     // public IEnumerable<ClaimComponentDetailAXAVM> ClaimComponentAXADetails { get; set; } = new List<ClaimComponentDetailAXAVM>();

        public DataTable dtDetails { get; set; } = new DataTable();

    
        public string Email { get; set; }
        public string URL { get; set; }

    }
}
