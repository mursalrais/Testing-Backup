using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class AssignmentofAssetItemVM
    {
        public int Id { get; set; }

        public string NewAsset { get; set; }

        public string Item { get; set; }

        public int AssetNo { get; set; }

        public int SubAssetNo { get; set; }

        public string AssetDescription { get; set; }

        public string OfficeName { get; set; }

        public string FloorName { get; set; }

        public string RoomName { get; set; }
    }
}
