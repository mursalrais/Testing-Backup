using MCAWebAndAPI.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class PayrollWorksheetDraftVM : Item
    {
        public string FileName { get; set; }

        public DateTime Period { get; set; }

        public DateTime RunOn { get; set; }

        public string UrlToDownload { get; set; }

    }
}
