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

        AssetAcquisitionHeaderVM getPopulatedModel(int? id = null);
        AssetAcquisitionHeaderVM getHeader(int? ID);
        int createHeader(AssetAcquisitionHeaderVM header);
        bool updateHeader(AssetAcquisitionHeaderVM header);
        void createAssetAcquisitionItems(int? headerID, IEnumerable<AssetAcquisitionItemVM> items);
    }
}
