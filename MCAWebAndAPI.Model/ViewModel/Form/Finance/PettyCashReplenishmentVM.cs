using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using MCAWebAndAPI.Model.Form.Finance;
using MCAWebAndAPI.Model.ViewModel.Control;

namespace MCAWebAndAPI.Model.ViewModel.Form.Finance
{
    /// <summary>
    /// FIN14: Petty Cash Replenishment
    /// </summary>

    public class PettyCashReplenishmentVM : PettyCashTransactionItem
    {
        public PettyCashReplenishmentVM()
        {
            this.TransactionType = Shared.PettyCashTranscationType_PettyCashReplenishment;
        }
        [Required]
        public string Remarks { get; set; }

        [UIHint("MultiFileUploader")]
        public IEnumerable<HttpPostedFileBase> Documents { get; set; } = new List<HttpPostedFileBase>();

        public string DocumentUrl { get; set; }
    }
}
