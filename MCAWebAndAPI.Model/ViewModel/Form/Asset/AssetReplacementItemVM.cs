using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Control;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class AssetReplacementItemVM
    {
        public int Id { get; set; }

        [DisplayName("New")]
        public string NewAsset { get; set; }

        public string Item { get; set; }

        [DisplayName("PO Line Item")]
        public string PoLineItem { get; set; }

        public int AssetNo { get; set; }

        public int AssetSubNo { get; set; }

        public string AssetDescription { get; set; }

        [DisplayName("WBS ID")]
        public int WbsId { get; set; }

        [DisplayName("WBS Description")]
        public string WbsDesc { get; set; }

        [UIHint("Currency")]
        [DisplayName("Cost (IDR)")]
        public decimal? CostIdr { get; set; }

        [UIHint("Currency")]
        [DisplayName("Cost (USD)")]
        public decimal? CostUsd { get; set; }
    }
}
