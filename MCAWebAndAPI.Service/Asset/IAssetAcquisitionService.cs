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

        IEnumerable<AssetAcquisitionVM> GetAssetAcquisitions();

        bool CreateAssetAcquisition(AssetAcquisitionVM assetAcquisition);

        bool CreateAssetAcquisition_dummy(AssetAcquisitionVM assetAcquisition);

        bool UpdateAssetAcquisition(AssetAcquisitionVM assetAcquisition);

        AssetAcquisitionVM GetAssetAcquisition_Dummy();

        bool CreateAssetAcquisitionItem_dummy(AssetAcquisitionItemVM assetAcquisition);
       
    }
}
