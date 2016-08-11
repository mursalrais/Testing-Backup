using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;

namespace MCAWebAndAPI.Service.Asset
{
    public class AssetDisposalService : IAssetDisposalService
    {
        string _siteUrl = "https://eceos2.sharepoint.com/sites/mca-dev/bo";

        public bool CreateAssetTransfer(AssetDisposalVM assetTransfer)
        {
            throw new NotImplementedException();
        }

        public int CreateHeader(AssetDisposalVM header)
        {
            //var columnValues = new Dictionary<string, object>();
            //columnValues.Add("professional", new FieldLookupValue { LookupId = Convert.ToInt32(header.ProfessionalName.Value) });
            //columnValues.Add("ProjectOrUnit", header.Documents);
            //columnValues.Add("date", header.Date);
          
            //try
            //{
            //    SPConnector.AddListItem(SP_MON_FEE_LIST_NAME, columnValues, _siteUrl);
            //}
            //catch (Exception e)
            //{
            //    logger.Error(e.Message);
            //}

            //return SPConnector.GetLatestListItemID(SP_MON_FEE_LIST_NAME, _siteUrl);

            throw new NotImplementedException();
        }

        public AssetDisposalVM GetAssetHolderFromInfo(int? ID, string siteUrl)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AssetDisposalVM> GetAssetTransfers()
        {
            throw new NotImplementedException();
        }

        public AssetDisposalVM GetPopulatedModel(int? id = default(int?))
        {
            var model = new AssetDisposalVM();
            model.TransactionType = Convert.ToString("Asset Disposal");
           
            return model;
        }

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = siteUrl;
        }

        public bool UpdateAssetTransfer(AssetDisposalVM assetTransfer)
        {
            throw new NotImplementedException();
        }
    }
}
