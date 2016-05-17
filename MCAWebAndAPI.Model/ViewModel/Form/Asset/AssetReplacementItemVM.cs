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

        public string NewAsset { get; set; }

        public string Item { get; set; }

        public string PoLineItem { get; set; }

        public int AssetNo { get; set; }

        public int AssetSubNo { get; set; }

        public string AssetDescription { get; set; }

        public int WbsId { get; set; }

        public string WbsDescription { get; set; }

        public decimal CostIdr { get; set; }

        public decimal CostUsd { get; set; }
    }
}
