using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using NLog;
using MCAWebAndAPI.Service.Utils;
using MCAWebAndAPI.Service.Resources;
using Microsoft.SharePoint.Client;
using System.Text.RegularExpressions;
using MCAWebAndAPI.Model.ViewModel.Control;

namespace MCAWebAndAPI.Service.Asset
{
    public class AssetReplacementService : IAssetReplacementService
    {
        string _siteUrl = null;
        static Logger logger = LogManager.GetCurrentClassLogger();

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = siteUrl;
        }

        public AssetReplacementHeaderVM GetPopulatedModel(int? ID = default(int?))
        {
            var model = new AssetReplacementHeaderVM();
            model.TransactionType = Convert.ToString("Asset Replacement");
            model.OldTransactionId.Choices = GetChoicesFromListLookUpValue("Asset Acquisition", "ID", _siteUrl);
            model.CancelURL = _siteUrl + UrlResource.AssetReplacement;

            return model;
        }

        private IEnumerable<string> GetChoicesFromListLookUpValue(string listname, string fLookup1, string siteUrl, string v2 = null)
        {
            List<string> _choices = new List<string>();
            var listItems = SPConnector.GetList(listname, _siteUrl);
            foreach (var item in listItems)
            {
                if (v2 != null)
                {
                    _choices.Add(Convert.ToString(item[fLookup1]) + "-" + item[v2].ToString());
                }
                else
                {
                    _choices.Add(Convert.ToString(item[fLookup1]));
                }
            }
            return _choices.ToArray();
        }

        public int? CreateHeader(AssetReplacementHeaderVM viewmodel, string mode = null, string SiteUrl = null)
        {
            throw new NotImplementedException();
        }

        public bool UpdateHeader(AssetReplacementHeaderVM viewmodel)
        {
            throw new NotImplementedException();
        }

        public bool Syncronize(string SiteUrl)
        {
            throw new NotImplementedException();
        }

        public void CreateDetails(int? headerID, IEnumerable<AssetReplacementItemVM> items)
        {
            throw new NotImplementedException();
        }

        public void UpdateDetails(int? headerID, IEnumerable<AssetReplacementItemVM> items)
        {
            throw new NotImplementedException();
        }

        public AssetReplacementHeaderVM GetHeader(int? ID)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AssetReplacementItemVM> GetDetails(int? headerID)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AssetMasterVM> GetAssetSubAsset()
        {
            var models = new List<AssetMasterVM>();
            var caml = @"<View><Query>
                       <Where>
                          <Geq>
                             <FieldRef Name='AssetID' />
                             <Value Type='Text'>14</Value>
                          </Geq>
                       </Where>
                       <OrderBy>
                          <FieldRef Name='AssetID' Ascending='True' />
                       </OrderBy>
                    </Query>
                    <ViewFields>
                       <FieldRef Name='Title' />
                       <FieldRef Name='AssetID' />
                    </ViewFields>
                    <QueryOptions /></View>";
            foreach (var item in SPConnector.GetList("Asset Master", _siteUrl, caml))
            {
                models.Add(ConvertToModelAssetSubAsset(item));
            }

            return models;
        }

        private AssetMasterVM ConvertToModelAssetSubAsset(ListItem item)
        {
            var viewModel = new AssetMasterVM();

            viewModel.ID = Convert.ToInt32(item["ID"]);
            viewModel.AssetNoAssetDesc.Value = Convert.ToString(item["AssetID"]);
            viewModel.AssetDesc = Convert.ToString(item["Title"]);
            return viewModel;
        }

        public void RollbackParentChildrenUpload(string listNameHeader, int? latestIDHeader, string siteUrl)
        {
            throw new NotImplementedException();
        }

        public AssetReplacementHeaderVM GetInfoFromAcquisitin(int? ID, string SiteUrl)
        {
            var list = SPConnector.GetListItem("Asset Acquisition", ID, SiteUrl);
            var viewmodel = new AssetReplacementHeaderVM();
            viewmodel.Vendor = Convert.ToString(list["vendorname"]);
            viewmodel.Pono = Convert.ToString(list["pono"]);
            viewmodel.purchasedatetext = Convert.ToDateTime(list["purchasedate"]).ToShortDateString();
            viewmodel.purchaseDescription = Regex.Replace(Convert.ToString(list["purchasedescription"]), "<.*?>", string.Empty);

            return viewmodel;
        }

        public IEnumerable<AssetReplacementItemVM> GetInfoFromAcquisitinDetail(int? ID, string SiteUrl)
        {
            _siteUrl = SiteUrl;
            var caml = @"<View><Query><Where><Eq><FieldRef Name='assetacquisition' /><Value Type='Lookup'>" + ID.ToString() + "</Value></Eq></Where></Query></View>";
            var details = new List<AssetReplacementItemVM>();
            foreach (var item in SPConnector.GetList("Asset Acquisition Details", SiteUrl, caml))
            {
                details.Add(ConvertToDetails(item));
            }

            return details;
        }

        private AssetReplacementItemVM ConvertToDetails(ListItem item)
        {
            var ListAssetSubAsset = SPConnector.GetListItem("Asset Master", (item["assetsubasset"] as FieldLookupValue).LookupId, _siteUrl);
            AjaxComboBoxVM _assetSubAsset = new AjaxComboBoxVM();
            _assetSubAsset.Value = (item["assetsubasset"] as FieldLookupValue).LookupId;
            _assetSubAsset.Text = Convert.ToString(ListAssetSubAsset["AssetID"]) + " - " + Convert.ToString(ListAssetSubAsset["Title"]);

            var ListWBS = SPConnector.GetListItem("WBS Master", (item["wbs"] as FieldLookupValue).LookupId, _siteUrl);
            AjaxComboBoxVM _wbs = new AjaxComboBoxVM();
            _wbs.Value = (item["wbs"] as FieldLookupValue).LookupId;
            _wbs.Text = Convert.ToString(ListWBS["Title"]) + " - " + Convert.ToString(ListWBS["WBSDesc"]);

            return new AssetReplacementItemVM
            {
                AssetSubAsset = AssetAcquisitionItemVM.GetAssetSubAssetDefaultValue(_assetSubAsset),
                Wbs = _wbs.Text,
                CostIdr = Convert.ToInt32(item["costidr"]),
                CostUsd = Convert.ToInt32(item["costusd"]),
                remarks = Convert.ToString(item["remarks"]),
                status = Convert.ToString(item["status"])
            };
        }
    }
}
