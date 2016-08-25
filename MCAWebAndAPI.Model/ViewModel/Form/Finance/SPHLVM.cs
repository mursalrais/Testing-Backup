using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace MCAWebAndAPI.Model.ViewModel.Form.Finance.SPHL
{
    /// <summary>
    /// Wireframe FIN16: SPHL
    ///     i.e.: Surat Pengesahan Hibah Langsung
    /// </summary>

    public class SPHLVM
    {
        public int ID { get; set; }

        [Required]
        [DisplayName("SPHL No.")]
        public string No { get; set; }

        [Required]
        [DisplayName("SPHL Date")]
        [UIHint("Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
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
