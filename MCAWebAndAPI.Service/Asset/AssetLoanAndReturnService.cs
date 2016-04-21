using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using NLog;

namespace MCAWebAndAPI.Service.Asset
{
    public class AssetLoanAndReturnService : IAssetLoanAndReturnService
    {
        string _siteUrl = null;
        static Logger logger = LogManager.GetCurrentClassLogger();

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = siteUrl;
        }

        public AssetLoanAndReturnVM GetAssetLoanAndReturns()
        {
            throw new NotImplementedException();
        }

        public bool CreateAssetLoanAndReturn(AssetLoanAndReturnVM assetLoanAndReturn)
        {
            throw new NotImplementedException();
        }

        public bool UpdateAssetLoanAndReturn(AssetLoanAndReturnVM assetLoanAndReturn)
        {
            throw new NotImplementedException();
        }

        IEnumerable<AssetLoanAndReturnVM> IAssetLoanAndReturnService.GetAssetLoanAndReturns()
        {
            throw new NotImplementedException();
        }
    }
}
