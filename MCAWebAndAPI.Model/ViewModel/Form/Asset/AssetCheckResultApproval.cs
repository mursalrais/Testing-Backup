using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class AssetCheckResultApproval : Item
    {
        public int ID { get; set; }

        public string FormID { get; set; }

        public string CountDate { get; set; }

        public string ApprovalName { get; set; }

        public string ApprovalPosition { get; set; }

        public string CountedBy1 { get; set; }

        public string CountedBy2 { get; set; }

        public string CountedBy3 { get; set; }
    }
}
