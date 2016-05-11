using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class AssetScrappingItemVM
    {
        public int Id { get; set; }

        public string NewAsset { get; set; }

        public string Item { get; set; }

        public int AssetNo { get; set; }

        public int SubAssetNo { get; set; }

        public string AssetDescription { get; set; }

        public string Remarks { get; set; }
    }
}
