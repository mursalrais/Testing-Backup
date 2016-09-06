using MCAWebAndAPI.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class AssetLandingPageVM
    {
        private IEnumerable<AssetLandingPageFixedAssetVM> _fixedAsset;
        private IEnumerable<AssetLandingPageSmallValueAssetVM> _smallValueAsset;

        public IEnumerable<AssetLandingPageFixedAssetVM> FixedAsset
        {
            get
            {
                if (_fixedAsset == null)
                    _fixedAsset = new List<AssetLandingPageFixedAssetVM>();
                return _fixedAsset;
            }
            set
            {
                _fixedAsset = value;
            }
        }

        public IEnumerable<AssetLandingPageSmallValueAssetVM> SmallValueAsset
        {
            get
            {
                if (_smallValueAsset == null)
                    _smallValueAsset = new List<AssetLandingPageSmallValueAssetVM>();
                return _smallValueAsset;
            }
            set
            {
                _smallValueAsset = value;
            }
        }
    }
}
