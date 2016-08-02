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

        bool CreateHeader(AssetAcquisitionHeaderVM viewmodel);

        AssetAcquisitionHeaderVM GetHeader(int? ID);

        IEnumerable<AssetMasterVM> GetAssetSubAsset();
    }
}
