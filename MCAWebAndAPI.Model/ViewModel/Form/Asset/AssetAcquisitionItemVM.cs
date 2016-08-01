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
