using Kendo.Mvc.UI;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class ClaimComponentDetailAXAVM : Item
    {

        public int? ProfessionalID { get; set; }
        public string ProfessionalName { get; set; }

        public int? DependentID { get; set; }
        public string DependentName { get; set; }

   
        [UIHint("Date")]
        [Required(ErrorMessage = "Receipt Date Field Is Required")]
        public DateTime? ReceiptDate { get; set; } = DateTime.UtcNow;


        [UIHint("Number")]
        public double MedicalExamination { get; set; } = 0;

        [UIHint("Number")]
        public double Laboratorium { get; set; } = 0;

        [UIHint("Number")]
        public double Prescription { get; set; } = 0;

        [UIHint("Number")]
        public double Other { get; set; } = 0;

        [UIHint("Number")]
        public double TotalAmount { get; set; } =0;


        [UIHint("TextArea")]
        public string Remarks { get; set; }

      
    }
}
