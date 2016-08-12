using MCAWebAndAPI.Model.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MCAWebAndAPI.Model.ViewModel.Form.Finance.SPHL
{
    public class SPHLVM : Item
    {

        [Required]
        [DisplayName("SPHL No.")]
        public string No { get; set; }

        [UIHint("Date")]
        [Required]
        [DisplayName("SPHL Date")]
        public DateTime Date { get; set; } = DateTime.Now;

        [UIHint("Currency")]
        [Required]
        [DisplayName("Amount (IDR)")]
        public decimal? AmountIDR { get; set; }

        [UIHint("TextArea")]
        public string Remarks { get; set; }

        [UIHint("MultiFileUploader")]
        [DisplayName("Attachment")]
        public IEnumerable<HttpPostedFileBase> Documents { get; set; } = new List<HttpPostedFileBase>();

        public string DocumentUrl { get; set; }
    }
}
