using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using NLog;

namespace MCAWebAndAPI.Service.Asset
{
    public class AssetCheckFormService : IAssetCheckFormService
    {
        string _siteUrl = null;
        static Logger logger = LogManager.GetCurrentClassLogger();

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = siteUrl;
        }

        public AssetCheckFormVM GetAssetCheckForms()
        {
            throw new NotImplementedException();
        }

        public bool CreateAssetCheckForm(AssetCheckFormVM assetCheckForm)
        {
            throw new NotImplementedException();
        }

        public bool UpdateAssetCheckForm(AssetCheckFormVM assetCheckForm)
        {
            throw new NotImplementedException();
        }

        IEnumerable<AssetCheckFormVM> IAssetCheckFormService.GetAssetCheckForms()
        {
            throw new NotImplementedException();
        }
    }
}
