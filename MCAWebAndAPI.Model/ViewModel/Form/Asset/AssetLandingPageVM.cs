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
        private AssetLandingPageFixedAssetVM _fixedAsset;
        private AssetLandingPageSmallValueAssetVM _smallValueAsset;


        public AssetLandingPageFixedAssetVM FixedAsset
        {
            get
            {
                if (_fixedAsset == null)
                    _fixedAsset = new AssetLandingPageFixedAssetVM();
                return _fixedAsset;
            }
            set
            {
                _fixedAsset = value;
            }
        }

        public AssetLandingPageSmallValueAssetVM SmallValueAsset
        {
            get
            {
                if (_smallValueAsset == null)
                    _smallValueAsset = new AssetLandingPageSmallValueAssetVM();
                return _smallValueAsset;
            }
            set
            {
                _smallValueAsset = value;
            }
        }
    }
}
