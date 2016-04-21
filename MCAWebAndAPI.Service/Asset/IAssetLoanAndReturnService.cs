using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Service.Asset
{
    public interface IAssetLoanAndReturnService
    {
        void SetSiteUrl(string siteUrl);

        IEnumerable<AssetLoanAndReturnVM> GetAssetLoanAndReturns();

        bool CreateAssetLoanAndReturn(AssetLoanAndReturnVM assetLoanAndReturn);

        bool UpdateAssetLoanAndReturn(AssetLoanAndReturnVM assetLoanAndReturn);
    }
}
