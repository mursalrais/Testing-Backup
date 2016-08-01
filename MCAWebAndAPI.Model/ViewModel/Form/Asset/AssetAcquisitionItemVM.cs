using MCAWebAndAPI.Model.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class AssetAcquisitionItemVM : Item
    {
        [DisplayName("Cost (USD)")]
        [UIHint("Currency")]
        public decimal? CostUSD { get; set; }

        public int Id { get; set; }

        [DisplayName("Cost (IDR)")]
        [UIHint("Currency")]
        public decimal? CostIDR { get; set; }

        public string WBSDescription { get; set; }
        public string WBSNo { get; set; }
        public string AssetDescription { get; set; }
        public string AssetSubAsset { get; set; }
        public string AssetNo { get; set; }
        public string POLineItem { get; set; }
        public string Remakrs { get; set; }
        public string Status { get; set; }
    }
}
