using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using NLog;

namespace MCAWebAndAPI.Service.Asset
{
    public class AssetTransferService : IAssetTransferService
    {
        string _siteUrl = null;
        static Logger logger = LogManager.GetCurrentClassLogger();

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = siteUrl;
        }

        public AssetTransferVM GetAssetTransfers()
        {
            throw new NotImplementedException();
        }

        public bool CreateAssetTransfer(AssetTransferVM assetTransfer)
        {
            throw new NotImplementedException();
        }

        public bool UpdateAssetTransfer(AssetTransferVM assetTransfer)
        {
            throw new NotImplementedException();
        }

        IEnumerable<AssetTransferVM> IAssetTransferService.GetAssetTransfers()
        {
            throw new NotImplementedException();
        }
    }
}
