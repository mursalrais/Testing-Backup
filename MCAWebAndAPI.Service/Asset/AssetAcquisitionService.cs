using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using NLog;

namespace MCAWebAndAPI.Service.Asset
{
    public class AssetAcquisitionService : IAssetAcquisitionService
    {
        string _siteUrl = null;
        static Logger logger = LogManager.GetCurrentClassLogger();

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = siteUrl;
        }

        public AssetAcquisitionVM GetAssetAcquisitions()
        {
            throw new NotImplementedException();
        }

        public bool CreateAssetAcquisition(AssetAcquisitionVM assetAcquisition)
        {
            throw new NotImplementedException();
        }

        public bool UpdateAssetAcquisition(AssetAcquisitionVM assetAcquisition)
        {
            throw new NotImplementedException();
        }

        IEnumerable<AssetAcquisitionVM> IAssetAcquisitionService.GetAssetAcquisitions()
        {
            throw new NotImplementedException();
        }
    }
}
