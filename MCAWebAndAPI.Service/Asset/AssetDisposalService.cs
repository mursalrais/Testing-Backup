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
using MCAWebAndAPI.Model.ViewModel.Control;
using System.Data;
using System.Text.RegularExpressions;

namespace MCAWebAndAPI.Service.Asset
{
    public class AssetDisposalService : IAssetDisposalService
    {
        string _siteUrl;
        const string SP_ASSACQDetails_LIST_NAME = "Asset Disposal Detail";
        const string SP_MON_FEE_LIST_NAME = "Asset Disposal";
        static Logger logger = LogManager.GetCurrentClassLogger();

        public bool CreateAssetTransfer(AssetDisposalVM assetTransfer)
        {
            throw new NotImplementedException();
        }

        public int? CreateHeader(AssetDisposalVM header, string SiteUrl = null)
        {
            var columnValues = new Dictionary<string, object>();
           // columnValues.Add("ID", header.ID);
            columnValues.Add("Title", header.TransactionType);
            columnValues.Add("date", header.Date);
            

            try
            {
                SPConnector.AddListItem(SP_MON_FEE_LIST_NAME, columnValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }

            return SPConnector.GetLatestListItemID(SP_MON_FEE_LIST_NAME, _siteUrl);

         
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
            model.CancelURL = _siteUrl + UrlResource.AssetDisposal;

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
        public void CreateDetails(int? headerID, IEnumerable<AssetDisposalDetailVM> items)
        {
            foreach (var item in items)
            {
                if (Item.CheckIfSkipped(item)) continue;
                if (Item.CheckIfDeleted(item))
                {
                    try
                    {
                        SPConnector.DeleteListItem(SP_ASSACQDetails_LIST_NAME, item.ID, _siteUrl);
                    }
                    catch (Exception e)
                    {
                        logger.Error(e);
                        throw e;
                    }
                    continue;
                }


                var updatedValues = new Dictionary<string, object>();
                updatedValues.Add("assetdisposal", new FieldLookupValue { LookupId = Convert.ToInt32(headerID) });
                updatedValues.Add("assetsubasset", new FieldLookupValue { LookupId = Convert.ToInt32(item.AssetSubAsset.Value.Value) });
              
                updatedValues.Add("remarks", item.Remarks);
                updatedValues.Add("status", "Retired");
                try
                {
                    SPConnector.AddListItem(SP_ASSACQDetails_LIST_NAME, updatedValues, _siteUrl);
                }
                catch (Exception e)
                {
                    logger.Error(e);
                    throw new Exception(ErrorResource.SPInsertError);
                }
            }
        }

        public AssetDisposalVM GetHeader(int? ID)
        {
            var listItem = SPConnector.GetListItem(SP_MON_FEE_LIST_NAME, ID, _siteUrl);
            var viewModel = new AssetDisposalVM();

            viewModel.TransactionType = Convert.ToString(listItem["Title"]);
           
            //viewModel.AccpMemo.Value = Convert.ToString(listItem["acceptancememono"]);
           

            viewModel.Date= Convert.ToDateTime(listItem["date"]);
            //viewModel.Spesifications = Regex.Replace(listItem["Spesifications"].ToString(), "<.*?>", string.Empty);

            viewModel.CancelURL = _siteUrl + UrlResource.AssetDisposal;
            viewModel.ID = ID;

            return viewModel;
        }

        public IEnumerable<AssetDisposalDetailVM> GetDetails(int? headerID)
        {
            var caml = @"<View><Query><Where><Eq><FieldRef Name='assetdisposal' /><Value Type='Lookup'>"+headerID.ToString()+"</Value></Eq></Where></Query></View>";
           
            var details = new List<AssetDisposalDetailVM>();
            foreach (var item in SPConnector.GetList(SP_ASSACQDetails_LIST_NAME, _siteUrl, caml))
            {
                details.Add(ConvertToDetails(item));
            }
                
            return details;
        }

        private AssetDisposalDetailVM ConvertToDetails(ListItem item)
        {

            var ListAssetSubAsset = SPConnector.GetListItem("Asset Master", (item["assetsubasset"] as FieldLookupValue).LookupId, _siteUrl);
            AjaxComboBoxVM _assetSubAsset = new AjaxComboBoxVM();
            _assetSubAsset.Value = (item["assetsubasset"] as FieldLookupValue).LookupId;
            _assetSubAsset.Text = Convert.ToString(ListAssetSubAsset["AssetID"]) + " - " + Convert.ToString(ListAssetSubAsset["Title"]);

            //var ListWBS = SPConnector.GetListItem("WBS Master", (item["wbs"] as FieldLookupValue).LookupId, _siteUrl);
            //AjaxComboBoxVM _wbs = new AjaxComboBoxVM();
            //_wbs.Value = (item["wbs"] as FieldLookupValue).LookupId;
            //_wbs.Text = Convert.ToString(ListWBS["Title"]) + " - " + Convert.ToString(ListWBS["WBSDesc"]);

            return new AssetDisposalDetailVM
            {
                ID = Convert.ToInt32(item["ID"]),         
                AssetSubAsset = AssetDisposalDetailVM.GetAssetSubAssetDefaultValue(_assetSubAsset),
  
                Remarks = Convert.ToString(item["remarks"]),
                Status = Convert.ToString(item["status"])
            };
        }

        public bool UpdateHeader(AssetDisposalVM viewmodel)
        {
            viewmodel.CancelURL = _siteUrl + UrlResource.AssetDisposal;
            var columnValues = new Dictionary<string, object>();
            var ID = Convert.ToInt32(viewmodel.ID);
            //columnValues.add
            columnValues.Add("Title", "Asset Disposal");
            
            columnValues.Add("date", Convert.ToDateTime(viewmodel.Date));
          

            try
            {
                SPConnector.UpdateListItem(SP_MON_FEE_LIST_NAME, ID, columnValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }
            var entitiy = new AssetDisposalVM();
            entitiy = viewmodel;
            return true;
        }

        public void UpdateDetails(int? headerID, IEnumerable<AssetDisposalDetailVM> items)
        {
            foreach (var item in items)
            {
                if (Item.CheckIfSkipped(item)) continue;

                if (Item.CheckIfDeleted(item))
                {
                    try
                    {
                        SPConnector.DeleteListItem(SP_ASSACQDetails_LIST_NAME, item.ID, _siteUrl);
                    }
                    catch (Exception e)
                    {
                        logger.Error(e);
                        throw e;
                    }
                    continue;
                }

                var updatedValues = new Dictionary<string, object>();
                updatedValues.Add("assetdisposal", new FieldLookupValue { LookupId = Convert.ToInt32(headerID) });
                updatedValues.Add("assetsubasset", new FieldLookupValue { LookupId = Convert.ToInt32(item.AssetSubAsset.Value.Value) });
                updatedValues.Add("remarks", item.Remarks);
                updatedValues.Add("status", "RETIRED");
                try
                {
                    if (Item.CheckIfUpdated(item))
                        SPConnector.UpdateListItem(SP_ASSACQDetails_LIST_NAME, item.ID, updatedValues, _siteUrl);
                    else
                        SPConnector.AddListItem(SP_ASSACQDetails_LIST_NAME, updatedValues, _siteUrl);
                }
                catch (Exception e)
                {
                    logger.Error(e);
                    throw new Exception(ErrorResource.SPUpdateError);
                }
            }
        }

        public IEnumerable<AssetAcquisitionItemVM> GetAssetSubAsset()
        {
            var models = new List<AssetAcquisitionItemVM>();
            var caml = @"<View><Query>
                           <Where>
                              <IsNotNull>
                                 <FieldRef Name='assetsubasset' />
                              </IsNotNull>
                           </Where>
                        </Query>
                        <ViewFields>
                           <FieldRef Name='assetsubasset' />
                        </ViewFields>
                        <QueryOptions /></View>";
            foreach (var item in SPConnector.GetList("Asset Acquisition Details", _siteUrl, caml))
            {
                models.Add(ConvertToModelAssetSubAsset(item));
            }

            return models;
        }

        private AssetAcquisitionItemVM ConvertToModelAssetSubAsset(ListItem item)
        {
            var viewModel = new AssetAcquisitionItemVM();

            viewModel.ID = Convert.ToInt32(item["ID"]);
            var assetID = "";
            //getInfo Asset Master
            if ((item["assetsubasset"] as FieldLookupValue) != null)
            {
                assetID = (item["assetsubasset"] as FieldLookupValue).LookupValue;
            }
            var info = GetInfoAssetMaster("Asset Master", assetID, _siteUrl);
            viewModel.AssetSubAsset.Text = Convert.ToString(info.AssetNoAssetDesc.Text) + "-" + Convert.ToString(info.AssetDesc);
            return viewModel;
        }

        private AssetMasterVM GetInfoAssetMaster(string listname, string assetID, string _siteUrl)
        {
            var caml = @"<View>
                        <Query>
                           <Where>
                              <Eq>
                                 <FieldRef Name='AssetID' />
                                 <Value Type='Text'>" + assetID + @"</Value>
                              </Eq>
                           </Where>
                        </Query>
                        <ViewFields>
                           <FieldRef Name='AssetID' />
                           <FieldRef Name='ID' />
                           <FieldRef Name='Title' />
                        </ViewFields>
                        <QueryOptions /></View>";
            var list = SPConnector.GetList(listname, _siteUrl, caml);
            var model = new AssetMasterVM();
            foreach (var item in list)
            {
                model.ID = Convert.ToInt32(item["ID"]);
                model.AssetNoAssetDesc.Text = Convert.ToString(item["AssetID"]);
                model.AssetDesc = Convert.ToString(item["Title"]);
            }

            return model;
        }
    }
    }
