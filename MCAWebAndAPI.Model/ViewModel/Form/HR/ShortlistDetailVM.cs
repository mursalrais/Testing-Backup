using System;
using MCAWebAndAPI.Model.Common;
using System.Collections.Generic;
using MCAWebAndAPI.Model.ViewModel.Control;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Linq;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class ShortlistDetailVM : Item
    {
        public string SiteUrl { get; set; }

        [Editable(false)]
        public string Candidate { get; set; }

        public string Candidatemail { get; set; }

        public string CV { get; set; }

        [UIHint("MultiFileUploader")]
        [Required]
        public IEnumerable<HttpPostedFileBase> Documents { get; set; } = new List<HttpPostedFileBase>();

        [Editable(false)]
        public string DocumentUrl { get; set; }

        public string CandidateUrl { get; set; }

        [DisplayName("Status")]
        public string GetStat { get; set; }

        [DisplayName("Need next interview")]
        public Boolean neednextintv { get; set; }

        [DisplayName("Need next interview")]
        public string neednexttext { get; set; }

        /// <summary>
        /// statusaplication
        /// </summary>
        [UIHint("InGridAjaxComboBox")]
        public AjaxComboBoxVM Status { get; set; }  = new AjaxComboBoxVM();

        public static IEnumerable<InGridComboBoxVM> GetStatusOptions()
        {
            var index = 0;
            var options = new string[] {
                "New",
                "Shortlisted",
                "Declined"};

            return options.Select(e =>
                new InGridComboBoxVM
                {
                    Value = ++index,
                    Text = e
                });
        }
        
        public static AjaxComboBoxVM GetStatusDefaultValue(AjaxComboBoxVM model = null)
        {
            if (model == null)
            {
                return new AjaxComboBoxVM();
            }
            else
            {
                return model;
            }
        }
        public string Remarks { get; set; }
    }
}
