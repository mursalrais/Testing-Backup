using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Service.Asset
{
    public interface IAssetAcquisitionService
    {
        void SetSiteUrl(string siteUrl);

        IEnumerable<AssetAcquisitionVM> GetAssetAcquisition();

        bool CreateAssetAcquisition(AssetAcquisitionVM assetAcquisition);

        bool UpdateAssetAcquisition(AssetAcquisitionVM assetAcquisition);

        bool CreateAssetAcquisition_Dummy(AssetAcquisitionItemVM assetAcquisition);

        bool UpdateAssetAcquisition_Dummy(AssetAcquisitionItemVM assetAcquisition);

        bool DestroyAssetAcquisition_Dummy(AssetAcquisitionItemVM assetAcquisition);

        AssetAcquisitionVM GetAssetAcquisitionItems_Dummy();
    }
}
