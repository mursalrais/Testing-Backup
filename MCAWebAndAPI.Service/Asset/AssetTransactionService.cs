using System;
using System.Collections.Generic;
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
        const string SP_ITEMS_LIST_NAME = "Asset Transaction Detail", 
            SP_HEADER_LIST_NAME = "Asset Transaction Header"; 

        public int CreateHeader(AssetTransactionHeaderVM header)
        {
            var updatedValues = new Dictionary<string, object>();
            updatedValues.Add("Title", header.TransactionType);
            updatedValues.Add("AssignmentDate", header.Date);
            updatedValues.Add("HolderID", new FieldLookupValue { LookupId  = Convert.ToInt32(header.AssetHolderFrom.Value) } );
            updatedValues.Add("HolderIDTo", new FieldLookupValue { LookupId = Convert.ToInt32(header.AssetHolderTo.Value) });

            try
            {
                SPConnector.AddListItem(SP_HEADER_LIST_NAME, updatedValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }
            
            return SPConnector.GetInsertedItemID(SP_HEADER_LIST_NAME, _siteUrl);
        }

        public int CreateItem(int headerID, AssetTransactionItemVM item)
        {
            var updatedValues = new Dictionary<string, object>();
            updatedValues.Add("TransactionID", new FieldLookupValue { LookupId = headerID });
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
            //TODO: Put filter get items having FK = header ID
            var caml = "";

            var listItems = SPConnector.GetList(SP_ITEMS_LIST_NAME, _siteUrl, caml);
            var viewModels = new List<AssetTransactionItemVM>();
            foreach (var item in listItems)
            {
                viewModels.Add(ConvertToAssetTransactionItemVM(item));
            }

            return viewModels;
        }

        private AssetTransactionItemVM ConvertToAssetTransactionItemVM(ListItem item)
        {
            throw new NotImplementedException();
        }

        public AssetTransactionVM GetPopulatedModel(int? id = null)
        {
            var model = new AssetTransactionVM();
            return model;
        }

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
        }

        public void UpdateHeader(AssetTransactionHeaderVM header)
        {
            
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
