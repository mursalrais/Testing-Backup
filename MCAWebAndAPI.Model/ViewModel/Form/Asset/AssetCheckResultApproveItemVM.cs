using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class AssetCheckResultApproveItemVM
    {
        public int Id { get; set; }

        public string Item { get; set; }

        public int AssetNo { get; set; }

        public int SubAssetNo { get; set; }

        public string AssetDescription { get; set; }

        public int SerialNo { get; set; }

        public string LocationName { get; set; }

        public string Status { get; set; }

        public int SystemQty { get; set; }

        public int PhysicalQty { get; set; }

        public int DifferentQty { get; set; }

        public string Scrap { get; set; }

        public string Remarks { get; set; }
    }
}
