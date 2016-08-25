using MCAWebAndAPI.Model.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class AssetDisposalVM : Item
    {


        public IEnumerable<AssetDisposalDetailVM> Details { get; set; } = new List<AssetDisposalDetailVM>();

      
        public string CancelURL { get; set; }

        public DateTime? _date = DateTime.Now;

        public string TransactionType { get; set; }

        [UIHint("MultiFileUploader")]
        [DisplayName("Attachment")]
        public IEnumerable<HttpPostedFileBase> Documents { get; set; } = new List<HttpPostedFileBase>();


        public string DocumentUrl { get; set; }

        [DisplayName("Date")]
        [UIHint("Date")]
        public DateTime? Date
        {
            get
            {
                return _date;
            }
            set
            {
                _date = value;
            }
        }
    }
}
