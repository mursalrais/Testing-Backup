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
        public string Candidate { get; set; }

        public string Candidatemail { get; set; }

        public string CV { get; set; }

        [UIHint("MultiFileUploader")]
        [Required]
        public IEnumerable<HttpPostedFileBase> Documents { get; set; } = new List<HttpPostedFileBase>();

        public string DocumentUrl { get; set; }

        public string GetStat { get; set; }

        /// <summary>
        /// statusaplication
        /// </summary>
        [UIHint("InGridComboBox")]
        public InGridComboBoxVM Status { get; set; }  = new InGridComboBoxVM();

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

        public static InGridComboBoxVM GetStatusDefaultValue(InGridComboBoxVM model = null)
        {
            var options = GetStatusOptions();
            if (model == null || string.IsNullOrEmpty(model.Text))
                return options.FirstOrDefault();

            return options.FirstOrDefault(e =>
                e.Value == model.Value || e.Text == model.Text);
        }

        public string Remarks { get; set; }
    }
}
