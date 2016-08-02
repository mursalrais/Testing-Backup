using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using NLog;
using MCAWebAndAPI.Service.Utils;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Service.Resources;
using Microsoft.SharePoint.Client;

namespace MCAWebAndAPI.Service.Asset
{
    public class AssetAcquisitionService : IAssetAcquisitionService
    {
        string _siteUrl = "https://eceos2.sharepoint.com/sites/mca-dev/bo/";
        static Logger logger = LogManager.GetCurrentClassLogger();
        const string SP_ASSACQ_LIST_NAME = "Asset Acquisition";
        const string SP_ASSACQDetails_LIST_NAME = "Asset Acquisition Details";
        const string SP_ACC_MEMO_LIST_NAME = "Acceptance Memo";

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = siteUrl;
        }

        public AssetAcquisitionHeaderVM GetPopulatedModel(int? ID = default(int?))
        {
            var model = new AssetAcquisitionHeaderVM();
            model.TransactionType = Convert.ToString("Asset Acquisition");
            model.AccpMemo.Choices = GetChoicesFromList("ID","Title");

            return model;
        }

        private IEnumerable<string> GetChoicesFromList(string v1, string v2 = null)
        {
            List<string> _choices = new List<string>();
            var listItems = SPConnector.GetList(SP_ACC_MEMO_LIST_NAME, _siteUrl);
            foreach (var item in listItems)
            {
                _choices.Add(item[v1] + "-" + item[v2].ToString());
                //_choices.Add(item[v1] + "-" + item[v2].ToString());
            }
            return _choices.ToArray();
        }

        public bool CreateHeader(AssetAcquisitionHeaderVM viewmodel)
        {
            var columnValues = new Dictionary<string, object>();
            //columnValues.add
            columnValues.Add("Title", viewmodel.TransactionType);
            string[] memo = viewmodel.AccpMemo.Value.Split('-');
            //columnValues.Add("Acceptance_x0020_Memo_x0020_No", memo[1]);
            columnValues.Add("Acceptance_x0020_Memo_x0020_No", new FieldLookupValue { LookupId = Convert.ToInt32(memo[0])});
            columnValues.Add("Vendor", viewmodel.Vendor);
            columnValues.Add("PO_x0020_No", viewmodel.PoNo);
            columnValues.Add("Purchase_x0020_Date", viewmodel.PurchaseDate);
            columnValues.Add("Purchase_x0020_Description", viewmodel.PurchaseDescription);

            try
            {
                SPConnector.AddListItem(SP_ASSACQ_LIST_NAME, columnValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                return false;
            }
            var entitiy = new AssetAcquisitionHeaderVM();
            entitiy = viewmodel;
            return true;
        }

        public AssetAcquisitionHeaderVM GetHeader(int? ID)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AssetMasterVM> GetAssetSubAsset()
        {
            var models = new List<AssetMasterVM>();

            foreach (var item in SPConnector.GetList("Asset Master", _siteUrl))
            {
                models.Add(ConvertToModel(item));
            }

            return models;
        }

        private AssetMasterVM ConvertToModel(ListItem item)
        {
            var viewModel = new AssetMasterVM();

            viewModel.ID = Convert.ToInt32(item["ID"]);
            viewModel.AssetDesc = Convert.ToString(item["AssetDescription"]);
            return viewModel;
        }
    }
}
