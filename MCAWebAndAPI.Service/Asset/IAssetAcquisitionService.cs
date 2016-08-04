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

        //create empty form
        AssetAcquisitionHeaderVM GetPopulatedModel(int? ID = null);
        AssetAcquisitionItemVM GetPopulatedModelItem(int? ID = null);

        int? CreateHeader(AssetAcquisitionHeaderVM viewmodel);

        void CreateDetails(int? headerID, IEnumerable<AssetAcquisitionItemVM> items);

        AssetAcquisitionHeaderVM GetHeader(int? ID);

        IEnumerable<AssetAcquisitionItemVM> GetDetails(int? headerID);

        IEnumerable<AssetMasterVM> GetAssetSubAsset();
    }
}
