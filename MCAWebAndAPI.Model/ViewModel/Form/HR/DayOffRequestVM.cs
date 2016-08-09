using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace MCAWebAndAPI.Model
{
    public class DayOffRequestVM : Item
    {
        public string Professional { get; set; }

        public string ProjectUnit { get; set; }

        public string Position { get; set; }

        [UIHint("MultiFileUploader")]
        [DisplayName("Medicial Certificate (Max. 2MB)")]
        public IEnumerable<HttpPostedFileBase> Documents { get; set; } = new List<HttpPostedFileBase>();
    }
}
