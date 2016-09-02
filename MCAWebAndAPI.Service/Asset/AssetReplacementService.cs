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
using MCAWebAndAPI.Model.Common;

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
            viewmodel.CancelURL = _siteUrl + UrlResource.AssetReplacement;
            var columnValues = new Dictionary<string, object>();
            //columnValues.add
            //var camlam2 = @"<View></View>";
            columnValues.Add("Title", "Asset Replacement");
            columnValues.Add("oldtransactionid", viewmodel.OldTransactionId.Value);
            columnValues.Add("acceptancememono", viewmodel.AccMemoNo);
            columnValues.Add("vendor", viewmodel.Vendor);
            columnValues.Add("pono", viewmodel.Pono);
            columnValues.Add("purchasedate", Convert.ToDateTime(viewmodel.purchasedatetext));
            //Regex.Replace(Convert.ToString(listItem["purchasedescription"]), "<.*?>", string.Empty);
            columnValues.Add("purchasedescription", viewmodel.purchaseDescription);

            try
            {
                SPConnector.AddListItem("Asset Replacement", columnValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }
            var entitiy = new AssetReplacementHeaderVM();
            entitiy = viewmodel;
            return SPConnector.GetLatestListItemID("Asset Replacement", _siteUrl);
        }

        public bool UpdateHeader(AssetReplacementHeaderVM viewmodel)
        {
            viewmodel.CancelURL = _siteUrl + UrlResource.AssetReplacement;
            var columnValues = new Dictionary<string, object>();
            var ID = Convert.ToInt32(viewmodel.Id);
            //columnValues.add
            //var camlam2 = @"<View></View>";
            columnValues.Add("Title", "Asset Replacement");
            columnValues.Add("oldtransactionid", viewmodel.OldTransactionId.Value);
            columnValues.Add("acceptancememono", viewmodel.AccMemoNo);
            columnValues.Add("vendor", viewmodel.Vendor);
            columnValues.Add("pono", viewmodel.Pono);
            columnValues.Add("purchasedate", Convert.ToDateTime(viewmodel.purchasedatetext));
            columnValues.Add("purchasedescription", viewmodel.purchaseDescription);

            try
            {
                SPConnector.AddListItem("Asset Replacement", columnValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }
            var entitiy = new AssetReplacementHeaderVM();
            entitiy = viewmodel;
            return true;
        }

        public bool Syncronize(string SiteUrl)
        {
            //Sync Header
            var lists = SPConnector.GetList("Asset Replacement", SiteUrl);
            foreach (var l in lists)
            {
                var getAsset = SPConnector.GetListItem("Asset Acquisition",(l["oldtransactionid"] as FieldLookupValue).LookupId, SiteUrl);

                var model = new Dictionary<string, object>();
                model.Add("acceptancememono", (getAsset["acceptancememono"] as FieldLookupValue).LookupValue);
                model.Add("vendor", Convert.ToString(getAsset["vendorname"]));
                model.Add("pono", Convert.ToString(getAsset["pono"]));
                model.Add("purchasedate", Convert.ToDateTime(getAsset["purchasedate"]));
                //Regex.Replace(Convert.ToString(getAsset["purchasedescription"]), "<.*?>", string.Empty);
                model.Add("purchasedescription", Regex.Replace(Convert.ToString(getAsset["purchasedescription"]), "<.*?>", string.Empty));

                SPConnector.UpdateListItem("Asset Replacement", Convert.ToInt32(l["ID"]), model, SiteUrl);
            }

            return true;
        }

        public void CreateDetails(int? headerID, IEnumerable<AssetReplacementItemVM> items)
        {
            foreach (var item in items)
            {
                if (Item.CheckIfSkipped(item)) continue;

                if (Item.CheckIfDeleted(item))
                {
                    try
                    {
                        SPConnector.DeleteListItem("Asset Replacement Detail", item.ID, _siteUrl);
                    }
                    catch (Exception e)
                    {
                        logger.Error(e);
                        throw e;
                    }
                    continue;
                }

                var updatedValues = new Dictionary<string, object>();
                updatedValues.Add("assetreplacement", new FieldLookupValue { LookupId = Convert.ToInt32(headerID) });
                updatedValues.Add("assetsubasset", new FieldLookupValue { LookupId = Convert.ToInt32(item.AssetSubAsset.Value.Value) });
                updatedValues.Add("wbs", item.Wbs);
                updatedValues.Add("costidr", item.CostIdr);
                updatedValues.Add("costusd", item.CostUsd);
                updatedValues.Add("remarks", item.remarks);
                updatedValues.Add("status", "RUNNING");
                try
                {
                    if (Item.CheckIfUpdated(item))
                        SPConnector.UpdateListItem("Asset Replacement Detail", item.ID, updatedValues, _siteUrl);
                    else
                        SPConnector.AddListItem("Asset Replacement Detail", updatedValues, _siteUrl);
                }
                catch (Exception e)
                {
                    logger.Error(e);
                    throw new Exception(ErrorResource.SPUpdateError);
                }
            }
        }

        public void UpdateDetails(int? headerID, IEnumerable<AssetReplacementItemVM> items)
        {
            foreach (var item in items)
            {
                if (Item.CheckIfSkipped(item)) continue;

                if (Item.CheckIfDeleted(item))
                {
                    try
                    {
                        SPConnector.DeleteListItem("Asset Replacement Detail", item.ID, _siteUrl);
                    }
                    catch (Exception e)
                    {
                        logger.Error(e);
                        throw e;
                    }
                    continue;
                }

                var updatedValues = new Dictionary<string, object>();
                updatedValues.Add("assetreplacement", new FieldLookupValue { LookupId = Convert.ToInt32(headerID) });
                updatedValues.Add("assetsubasset", new FieldLookupValue { LookupId = Convert.ToInt32(item.AssetSubAsset.Value.Value) });
                updatedValues.Add("wbs", item.Wbs);
                updatedValues.Add("costidr", item.CostIdr);
                updatedValues.Add("costusd", item.CostUsd);
                updatedValues.Add("remarks", item.remarks);
                updatedValues.Add("status", "RUNNING");
                try
                {
                    //if (Item.CheckIfUpdated(item))
                        SPConnector.UpdateListItem("Asset Replacement Detail", item.ID, updatedValues, _siteUrl);
                    //else
                    //    SPConnector.AddListItem("Asset Replacement Detail", updatedValues, _siteUrl);
                }
                catch (Exception e)
                {
                    logger.Error(e);
                    throw new Exception(ErrorResource.SPUpdateError);
                }
            }
        }

        public AssetReplacementHeaderVM GetHeader(int? ID)
        {
            var listItem = SPConnector.GetListItem("Asset Replacement", ID, _siteUrl);
            var viewModel = new AssetReplacementHeaderVM();

            viewModel.TransactionType = Convert.ToString(listItem["Title"]);
            viewModel.OldTransactionId.Choices = GetChoicesFromListLookUpValue("Asset Acquisition", "ID", _siteUrl);
            viewModel.OldTransactionId.Value = (listItem["oldtransactionid"] as FieldLookupValue).LookupValue;
            viewModel.OldTransactionId.Text = (listItem["oldtransactionid"] as FieldLookupValue).LookupValue;
            viewModel.AccMemoNo = Convert.ToString(listItem["acceptancememono"]);
            viewModel.Pono = Convert.ToString(listItem["pono"]);
            viewModel.Vendor = Convert.ToString(listItem["vendor"]);

            viewModel.purchasedatetext = Convert.ToString(listItem["purchasedate"]);
            viewModel.PurchaseDate = Convert.ToDateTime(listItem["purchasedate"]);
            viewModel.purchaseDescription = Regex.Replace(Convert.ToString(listItem["purchasedescription"]), "<.*?>", string.Empty);
            viewModel.Id = Convert.ToInt32(ID);

            viewModel.CancelURL = _siteUrl + UrlResource.AssetReplacement;

            return viewModel;
        }

        public IEnumerable<AssetReplacementItemVM> GetDetails(int? headerID)
        {
            var caml = @"<View><Query><Where><Eq><FieldRef Name='assetreplacement' /><Value Type='Lookup'>" + headerID.ToString() + "</Value></Eq></Where></Query></View>";
            var details = new List<AssetReplacementItemVM>();
            foreach (var item in SPConnector.GetList("Asset Replacement Detail", _siteUrl, caml))
            {
                details.Add(ConvertToDetailsReplacement(item));
            }

            return details;
        }

        private AssetReplacementItemVM ConvertToDetailsReplacement(ListItem item)
        {
            var ListAssetSubAsset = SPConnector.GetListItem("Asset Master", (item["assetsubasset"] as FieldLookupValue).LookupId, _siteUrl);
            AjaxComboBoxVM _assetSubAsset = new AjaxComboBoxVM();
            _assetSubAsset.Value = (item["assetsubasset"] as FieldLookupValue).LookupId;
            _assetSubAsset.Text = Convert.ToString(ListAssetSubAsset["AssetID"]) + " - " + Convert.ToString(ListAssetSubAsset["Title"]);

            var model = new AssetReplacementItemVM();
            model.ID = Convert.ToInt32(item["ID"]);
            model.AssetSubAsset = AssetReplacementItemVM.GetAssetSubAssetDefaultValue(_assetSubAsset);
            model.Wbs = Convert.ToString(item["wbs"]);
            model.CostIdr = Convert.ToInt32(item["costidr"]);
            model.CostUsd = Convert.ToInt32(item["costusd"]);
            model.remarks = Convert.ToString(item["remarks"]);
            model.status = Convert.ToString(item["status"]);


            return model;
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
            SPConnector.DeleteListItem(listNameHeader, latestIDHeader, siteUrl);
        }

        public AssetReplacementHeaderVM GetInfoFromAcquisitin(int? ID, string SiteUrl)
        {
            _siteUrl = SiteUrl;
            var list = SPConnector.GetListItem("Asset Acquisition", ID, SiteUrl);
            var viewmodel = new AssetReplacementHeaderVM();
            viewmodel.OldTransactionId.Choices = GetChoicesFromListLookUpValue("Asset Acquisition", "ID", _siteUrl);
            viewmodel.OldTransactionId.Value = Convert.ToString(ID);
            viewmodel.OldTransactionId.Text = Convert.ToString(ID);
            viewmodel.CancelURL = _siteUrl + UrlResource.AssetReplacement;
            viewmodel.AccMemoNo = (list["acceptancememono"] as FieldLookupValue).LookupValue;
            viewmodel.Vendor = Convert.ToString(list["vendorname"]);
            viewmodel.Pono = Convert.ToString(list["pono"]);
            viewmodel.purchasedatetext = Convert.ToDateTime(list["purchasedate"]).ToShortDateString();
            viewmodel.PurchaseDate = Convert.ToDateTime(viewmodel.purchasedatetext);
            viewmodel.purchaseDescription = Regex.Replace(Convert.ToString(list["purchasedescription"]), "<.*?>", string.Empty);

            var caml = @"<View><Query><Where><Eq><FieldRef Name='assetacquisition' /><Value Type='Lookup'>" + ID.ToString() + "</Value></Eq></Where></Query></View>";
            var details = new List<AssetReplacementItemVM>();
            foreach (var item in SPConnector.GetList("Asset Acquisition Details", SiteUrl, caml))
            {
                details.Add(ConvertToDetails(item));
            }

            viewmodel.Details = details;

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

            var model = new AssetReplacementItemVM();
            model.AssetSubAsset = AssetReplacementItemVM.GetAssetSubAssetDefaultValue(_assetSubAsset);
            model.Wbs = _wbs.Text;
            model.CostIdr = Convert.ToInt32(item["costidr"]);
            model.CostUsd = Convert.ToInt32(item["costusd"]);
            model.remarks = "";
            model.status = "";
            
            return model;
        }
    }
}
