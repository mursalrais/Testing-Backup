using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Model.HR.DataMaster;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using NLog;

namespace MCAWebAndAPI.Service.HR.Recruitment
{
    public class PSAManagementService : IPSAManagementService
    {
        string _siteUrl;
        static Logger logger = LogManager.GetCurrentClassLogger();
        const string SP_LIST_NAME = "PSA";

       
        public int CreatePSA(PSAManagementVM psaManagement)
        {
            var updatedValues = new Dictionary<string, object>();
            updatedValues.Add("Title", psaManagement.psaNumber);
            updatedValues.Add("isrenewal", psaManagement.IsRenewal);
            updatedValues.Add("renewalnumber", psaManagement.renewalnumber);
            updatedValues.Add("position", new FieldLookupValue { LookupId = Convert.ToInt32(psaManagement.Position.Value) });
            updatedValues.Add("professional", new FieldLookupValue { LookupId = Convert.ToInt32(psaManagement.Professional.Value) });
            updatedValues.Add("joindate", psaManagement.joindate);
            updatedValues.Add("dateofnewpsa", psaManagement.dateofnewpsa);
            updatedValues.Add("tenure", psaManagement.tenure);
            updatedValues.Add("psaexpirydate", psaManagement.psaexpirydate);

            try
            {
                SPConnector.AddListItem(SP_LIST_NAME, updatedValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }

            return SPConnector.GetInsertedItemID(SP_LIST_NAME, _siteUrl);
        }

        public IEnumerable<PSAMaster> GetPSAs()
        {
            var models = new List<PSAMaster>();

            foreach (var item in SPConnector.GetList(SP_LIST_NAME, _siteUrl))
            {
                models.Add(ConvertToPSAModel(item));
            }

            return models;
        }

        private PSAMaster ConvertToPSAModel(ListItem item)
        {
            return new PSAMaster
            {
                ID = item["professional_x003a_ID"] == null ? "" :
               Convert.ToString((item["professional_x003a_ID"] as FieldLookupValue).LookupValue),
                JoinDate = Convert.ToString(item["joindate"]),
                DateOfNewPSA = Convert.ToString(item["dateofnewpsa"]),
                PsaExpiryDate = Convert.ToString(item["psaexpirydate"]),
                ProjectOrUnit = Convert.ToString(item["ProjectOrUnit"]),
            };
        }

        /*
        public int CreateItem(int headerID, AssetTransactionItemVM item)
        {
            var updatedValues = new Dictionary<string, object>();
            updatedValues.Add("TransactionID", new FieldLookupValue { LookupId = headerID });
            updatedValues.Add("CostIDR", item.CostIDR);
            updatedValues.Add("CostUSD", item.CostUSD);

            SPConnector.AddListItem(SP_ITEMS_LIST_NAME, updatedValues, _siteUrl);

            return 1;

        }
        */

        /*
        public void DeleteItem(AssetTransactionItemVM item)
        {
            throw new NotImplementedException();
        }
        */

        /*
        public AssetTransactionHeaderVM GetHeader()
        {
            throw new NotImplementedException();
        }
        */

        /*
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
        */

        /*
        private AssetTransactionItemVM ConvertToAssetTransactionItemVM(ListItem item)
        {
            throw new NotImplementedException();
        }
        */

        public PSAManagementVM GetPopulatedModel(int? id = null)
        {
            var model = new PSAManagementVM();
            return model;
        }

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
        }

        /*
        public void UpdateHeader(AssetTransactionHeaderVM header)
        {

        }
        */

        /*
        public void UpdateItem(AssetTransactionItemVM item)
        {
            throw new NotImplementedException();
        }
        */

        /*
        public bool CreateItems(int headerID, IEnumerable<AssetTransactionItemVM> items)
        {
            foreach (var item in items)
            {
                try
                {
                    CreateItem(headerID, item);
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    return false;
                }
            }

            return true;
        }
        */
    }
}
