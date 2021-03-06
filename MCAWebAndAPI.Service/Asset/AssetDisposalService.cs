﻿using System;
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

                var id = SPConnector.GetLatestListItemID("Asset Disposal", _siteUrl);

                if (header.attach.FileName != "" || header.attach.FileName != null)
                {
                    SPConnector.AttachFile("Asset Disposal", id, header.attach, _siteUrl);
                }

                //var id = SPConnector.GetLatestListItemID("Asset Disposal", _siteUrl);
                //var info = SPConnector.GetListItem("Asset Disposal", id, _siteUrl);

                //if (Convert.ToBoolean(info["Attachments"]) == false)
                //    {
                //        SPConnector.DeleteListItem("Asset Disposal", id, _siteUrl);
                //        return 0;
                //    }  

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
                var caml = @"<View><Query>
                           <Where>
                              <Eq>
                                 <FieldRef Name='ID' />
                                 <Value Type='Counter'>"+item.AssetSubAsset.Value.Value+@"</Value>
                              </Eq>
                           </Where>
                        </Query>
                        <ViewFields>
                           <FieldRef Name='assetsubasset' />
                           <FieldRef Name='Asset_x0020_Sub_x0020_Asset_x003' />
                        </ViewFields>
                        <QueryOptions /></View>";
                var infoAcquisition = SPConnector.GetList("Asset Acquisition Details", _siteUrl, caml);
                var assetID = 0;
                foreach(var i in infoAcquisition)
                {
                    if (i["assetsubasset"] as FieldLookupValue != null)
                    {
                        assetID = (i["assetsubasset"] as FieldLookupValue).LookupId;
                    }
                }
                updatedValues.Add("assetsubasset", new FieldLookupValue { LookupId = assetID });
              
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

        public AssetDisposalVM GetHeader(int? ID, string SiteUrl)
        {

            var ff = SPConnector.GetListItem("Asset Disposal", ID, _siteUrl);

            var FV = Convert.ToString(ff["Attachments"]);
            var filename = "";

            if (FV == "true")
            {
                filename = SPConnector.GetAttachFileName("Asset Disposal", ID, _siteUrl);
            }

            var listItem = SPConnector.GetListItem(SP_MON_FEE_LIST_NAME, ID, _siteUrl);
            var viewModel = new AssetDisposalVM();
            viewModel.filename = filename;

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
            var oldData = SPConnector.GetListItem("Asset Disposal", ID, _siteUrl);
            //columnValues.add
            columnValues.Add("Title", "Asset Disposal");
            
            columnValues.Add("date", Convert.ToDateTime(viewmodel.Date));

            try
            {
                SPConnector.UpdateListItem("Asset Disposal", ID, columnValues, _siteUrl);

                var id = SPConnector.GetLatestListItemID("Asset Disposal", _siteUrl);
                if (viewmodel.attach.FileName != "" || viewmodel.attach.FileName != null)
                {
                    SPConnector.AttachFile("Asset Disposal", id, viewmodel.attach, _siteUrl);
                }

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
                        SPConnector.DeleteListItem("Asset Disposal Detail", item.ID, _siteUrl);
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

                var caml = @"<View><Query>
                           <Where>
                              <Eq>
                                 <FieldRef Name='Asset_x0020_Sub_x0020_Asset_x003' />
                                 <Value Type='Lookup'>" + item.AssetSubAsset.Value.Value + @"</Value>
                              </Eq>
                           </Where>
                        </Query>
                        <ViewFields>
                           <FieldRef Name='assetsubasset' />
                        </ViewFields>
                        <QueryOptions /></View>";

                var getAssetID = SPConnector.GetList("Asset Acquisition Details", _siteUrl, caml);
                //getInformation From  Asset Assignment Detail
                
                if ( getAssetID.Count != 0)
                {
                    foreach (var info in getAssetID)
                    {
                        if ((info["assetsubasset"] as FieldLookupValue) != null)
                        {
                            updatedValues.Add("assetsubasset", (info["assetsubasset"] as FieldLookupValue).LookupId);
                        }
                        if (getAssetID.Count > 1)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    var assetID = SPConnector.GetListItem("Asset Acquisition Details", item.AssetSubAsset.Value.Value, _siteUrl);
                   
                    if ((assetID["assetsubasset"] as FieldLookupValue) != null)
                    {
                        updatedValues.Add("assetsubasset", (assetID["assetsubasset"] as FieldLookupValue).LookupId);
                    }

                    
                }

                updatedValues.Add("remarks", item.Remarks);
                updatedValues.Add("status", "RETIRED");

                try
                {
                    if (Item.CheckIfUpdated(item))
                        SPConnector.UpdateListItem("Asset Disposal Detail", item.ID, updatedValues, _siteUrl);
                    else
                        SPConnector.AddListItem("Asset Disposal Detail", updatedValues, _siteUrl);
                }
                catch (Exception e)
                {
                    logger.Error(e);
                    throw new Exception(ErrorResource.SPInsertError);
                }
            }
        }

        //public void UpdateDetails(int? headerID, IEnumerable<AssetDisposalDetailVM> items)
        //{
        //    foreach (var item in items)
        //    {
        //         if (Item.CheckIfSkipped(item)) continue;

        //        if (Item.CheckIfDeleted(item))
        //        {
        //            try
        //            {
        //                SPConnector.DeleteListItem("Asset Dispoal Detail", item.ID, _siteUrl);
        //            }
        //            catch (Exception e)
        //            {
        //                logger.Error(e);
        //                throw e;
        //            }
        //            continue;
        //        }

        //        var updatedValues = new Dictionary<string, object>();
        //        updatedValues.Add("assetdisposal", new FieldLookupValue { LookupId = Convert.ToInt32(headerID) });


        //        var caml = @"<View><Query>
        //                   <Where>
        //                      <Eq>
        //                         <FieldRef Name='Asset_x0020_Sub_x0020_Asset_x003' />
        //                         <Value Type='Lookup'>" + item.AssetSubAsset.Value.Value + @"</Value>
        //                      </Eq>
        //                   </Where>
        //                </Query>
        //                <ViewFields>
        //                   <FieldRef Name='assetsubasset' />
        //                </ViewFields>
        //                <QueryOptions /></View>";

        //        var getAssetID = SPConnector.GetList("Asset Acquisition Details", _siteUrl, caml);



        //        foreach (var info in getAssetID)
        //        {
        //            if ((info["assetsubasset"] as FieldLookupValue) != null)
        //            {
        //                updatedValues.Add("assetsubasset", (info["assetsubasset"] as FieldLookupValue).LookupId);
        //            }
        //            if (getAssetID.Count > 1)
        //            {
        //                break;
        //            }
        //        }
        //        // updatedValues.Add("assetsubasset", new FieldLookupValue { LookupId = Convert.ToInt32(item.AssetSubAsset.Value.Value) });
        //        //updatedValues.Add("assetsubasset", new FieldLookupValue { LookupId = assetID });

        //        updatedValues.Add("remarks", item.Remarks);
        //        updatedValues.Add("status", "RETIRED");
        //        try
        //        {
        //            if (Item.CheckIfUpdated(item))
        //                SPConnector.UpdateListItem("Asset Disposal Detail", item.ID, updatedValues, _siteUrl);
        //            else
        //                SPConnector.AddListItem("Asset Disposal Detail", updatedValues, _siteUrl);
        //        }
        //        catch (Exception e)
        //        {
        //            logger.Error(e);
        //            throw new Exception(ErrorResource.SPInsertError);
        //        }
        //    }
        //}

        public IEnumerable<AssetAcquisitionItemVM> GetAssetSubAsset()
        {
            var models = new List<AssetAcquisitionItemVM>();
            var caml = @"<View><Query>
                        <Where>
                            <Eq>
                                <FieldRef Name='assetsubasset' />
                           </Eq>
                        </Where></Query>
                    <ViewFields> 
                        <FieldRef Name='Title' />
                        <FieldRef Name='AssetID' />
                    </ViewFields><QueryOptions /></View>";
            foreach (var item in SPConnector.GetList("Asset Acquisition Details", _siteUrl))
            {
                models.Add(ConvertToModelAssetSubAsset(item));
            }

            foreach (var item in SPConnector.GetList("Asset Replacement Detail", _siteUrl))
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
