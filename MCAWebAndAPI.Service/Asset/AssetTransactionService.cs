using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using NLog;

namespace MCAWebAndAPI.Service.Asset
{
    public class AssetTransactionService : IAssetTransactionService
    {
        string _siteUrl;
        static Logger logger = LogManager.GetCurrentClassLogger();
        const string SP_ITEMS_LIST_NAME = "AssetTrxItem", SP_HEADER_LIST_NAME = "AssetTrxHeader"; 

        public int CreateHeader(AssetTransactionHeaderVM header)
        {
            throw new NotImplementedException();
        }

        public int CreateItem(int headerID, AssetTransactionItemVM item)
        {
            var updatedValues = new Dictionary<string, object>();
            updatedValues.Add("TransactionID", new FieldLookupValue { LookupId = headerID });
            updatedValues.Add("LocationID", new FieldLookupValue { LookupId = item.LocationFrom.ID });
            updatedValues.Add("LocationIDTo", new FieldLookupValue { LookupId = item.LocationTo.ID });
            updatedValues.Add("CostIDR", item.CostIDR);
            updatedValues.Add("CostUSD", item.CostUSD);


            SPConnector.AddListItem(SP_ITEMS_LIST_NAME, updatedValues, _siteUrl);

            return 1;

        }

        public void DeleteItem(AssetTransactionItemVM item)
        {
            throw new NotImplementedException();
        }

        public AssetTransactionHeaderVM GetHeader()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AssetTransactionItemVM> GetItems(int headerID)
        {
            var caml = "";

            var listItems = SPConnector.GetList(SP_ITEMS_LIST_NAME, _siteUrl, caml);
            var viewModels = new List<AssetTransactionItemVM>();
            foreach (var item in listItems)
            {
                viewModels.Add(ConvertListItemToAssetTransactionItemVM(item));
            }

            return viewModels;
        }

        private AssetTransactionItemVM ConvertListItemToAssetTransactionItemVM(ListItem item)
        {
            throw new NotImplementedException();
        }

        public AssetTransactionVM GetPopulatedModel(int? id = null)
        {
            var model = new AssetTransactionVM();
            var header = new AssetTransactionHeaderVM();
            header.AssetHolderFrom = ModelMappingUtil.ConfigAjaxComboBoxVM("HRDataMaster", "GetProfessionals", "ProfessionalID", "ProfessionalDesc");
            header.AssetHolderTo = ModelMappingUtil.ConfigAjaxComboBoxVM("HRDataMaster", "GetProfessionals", "ProfessionalID", "ProfessionalDesc");

            // Edit Mode
            if (id != null)
            {
                
            }

            model.Header = header;
            return model;
        }

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = siteUrl;
        }

        public void UpdateHeader(AssetTransactionItemVM header)
        {
            throw new NotImplementedException();
        }

        public void UpdateItem(AssetTransactionItemVM item)
        {
            throw new NotImplementedException();
        }

        public bool CreateItems(int headerID, IEnumerable<AssetTransactionItemVM> items)
        {
            foreach(var item in items)
            {
                try
                {
                    CreateItem(headerID, item);
                }catch(Exception e)
                {
                    logger.Error(e.Message);
                    return false;
                }
            }

            return true;
        }
    }
}
