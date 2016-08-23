using System;
using System.Collections.Generic;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using MCAWebAndAPI.Service.Utils;
using NLog;
using Microsoft.SharePoint.Client;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using System.Web;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Service.Resources;

namespace MCAWebAndAPI.Service.Asset
{
    public class AssignmentOfAssetService : IAssignmentOfAssetService
    {
        string _siteUrl;
        static Logger logger = LogManager.GetCurrentClassLogger();

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = siteUrl;
        }


        public AssignmentOfAssetVM GetPopulatedModel(string SiteUrl)
        {
            var model = new AssignmentOfAssetVM();
            model.AssetHolder.Choices = GetFromListHR("Professional Master", "Title", "Position", SiteUrl);

            return model;
        }

        public AssignmentOfAssetVM GetHeader(int? ID, string SiteUrl)
        {
            var listItem = SPConnector.GetListItem("Asset Assignment", ID, SiteUrl);
            var viewModel = new AssignmentOfAssetVM();

            viewModel.TransactionType = Convert.ToString(listItem["Title"]);
            viewModel.AssetHolder.Choices = GetFromListHR("Professional Master", "Title", "Position", SiteUrl);
            if ((listItem["assetholder"] as FieldLookupValue) != null)
            {
                viewModel.AssetHolder.Value = (listItem["assetholder"] as FieldLookupValue).LookupId.ToString();
                viewModel.AssetHolder.Text = (listItem["assetholder"] as FieldLookupValue).LookupValue;
            }
            //viewModel.AccpMemo.Value = Convert.ToString(listItem["acceptancememono"]);
            viewModel.ProjectUnit = Convert.ToString(listItem["pono"]);
            viewModel.ContactNo = Convert.ToString(listItem["vendorid"]) + "-" + Convert.ToString(listItem["vendorname"]);
            if (Convert.ToDateTime(listItem["trasnferdate"]) == DateTime.MinValue)
            {
                viewModel.Date = null;
            }
            else
            {
                viewModel.Date = Convert.ToDateTime(listItem["trasnferdate"]).AddDays(1);
            }
            //viewModel.PurchaseDescription = Regex.Replace(Convert.ToString(listItem["purchasedescription"]), "<.*?>", string.Empty);
            viewModel.ID = ID;

            //viewModel.CancelURL = _siteUrl + UrlResource.AssetAcquisition;

            return viewModel;
        }

        private IEnumerable<string> GetFromListHR(string listname, string f1, string f2, string siteUrl)
        {
            var siteHr = siteUrl.Replace("/bo", "/hr");
            List<string> _choices = new List<string>();
            var listItems = SPConnector.GetList(listname, siteHr);
            foreach (var item in listItems)
            {
                if (f2 != null)
                {
                    var position = "";
                    if ((item["Position"] as FieldLookupValue) != null)
                    {
                        position =(item["Position"] as FieldLookupValue).LookupValue;
                    }
                    _choices.Add(item[f1] + "-" + position);
                }
                else
                {
                    _choices.Add(item[f1].ToString());
                }
                //var listProfMasBO = SPConnector.GetList(listname, siteUrl);
                //foreach(var pmbo in listProfMasBO)
                //{
                    
                //}
            }
            return _choices.ToArray();
        }

        public int? CreateHeader(AssignmentOfAssetVM viewmodel, string SiteUrl)
        {
            //viewmodel.CancelURL = _siteUrl + UrlResource.AssetAcquisition;
            var columnValues = new Dictionary<string, object>();
            //columnValues.add
            columnValues.Add("Title", "Assignment Of Asset");
            if (viewmodel.Date.HasValue)
            {
                columnValues.Add("transferdate", viewmodel.Date);
            }
            else
            {
                columnValues.Add("transferdate", null);
            }

            if (viewmodel.AssetHolder.Value == null)
            {
                return 0;
            }
            else
            {
                var breaks = viewmodel.AssetHolder.Value.Split('-');
                var getInfo = GetProfMasterInfo(breaks[0], SiteUrl);
                if(getInfo != null)
                {
                    columnValues.Add("assetholder", new FieldLookupValue { LookupId = Convert.ToInt32(getInfo.ID) });
                    columnValues.Add("projectunit", getInfo.CurrentPosition.Text);
                    columnValues.Add("contactnumber", getInfo.MobileNumberOne);
                }
            }

            //attachment later
            columnValues.Add("completionstatus", viewmodel.CompletionStatus.Value);
            

            try
            {
                SPConnector.AddListItem("Asset Assignment", columnValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }
            var entitiy = new AssignmentOfAssetVM();
            entitiy = viewmodel;
            return SPConnector.GetLatestListItemID("Asset Assignment", _siteUrl);
        }

        public void CreateDocuments(int? headerID, IEnumerable<HttpPostedFileBase> documents, string SiteUrl)
        {
            foreach(var doc in documents)
            {
                var updateValues = new Dictionary<string, object>();
                updateValues.Add("assetassignmentid", new FieldLookupValue { LookupId = Convert.ToInt32(headerID) });
                try
                {
                    SPConnector.UploadDocument("Asset Assignmment Documents", updateValues, doc.FileName, doc.InputStream, SiteUrl);
                }
                catch (Exception e)
                {

                }
            }
            throw new NotImplementedException();
        }

        public bool UpdateHeader(AssignmentOfAssetVM viewmodel, string SiteUrl)
        {
            //viewmodel.CancelURL = _siteUrl + UrlResource.AssetAcquisition;
            var columnValues = new Dictionary<string, object>();
            var ID = Convert.ToInt32(viewmodel.ID);
            //columnValues.add
            columnValues.Add("Title", "Assignment Of Asset");
            if (viewmodel.Date.HasValue)
            {
                columnValues.Add("transferdate", viewmodel.Date);
            }
            else
            {
                columnValues.Add("transferdate", null);
            }

            if (viewmodel.AssetHolder.Value == null)
            {
                return false;
            }
            else
            {
                columnValues.Add("assetholder", new FieldLookupValue { LookupId = Convert.ToInt32(viewmodel.AssetHolder.Value) });
                var breaks = viewmodel.AssetHolder.Value.Split('-');
                var getInfo = GetProfMasterInfo(breaks[0], SiteUrl);
                if (getInfo != null)
                {
                    columnValues.Add("projectunit", getInfo.CurrentPosition);
                    columnValues.Add("contactnumber", getInfo.MobileNumberOne);
                }
            }

            //attachment later
            columnValues.Add("completionstatus", viewmodel.CompletionStatus.Value);

            try
            {
                SPConnector.UpdateListItem("Asset Assignment", ID, columnValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }
            var entitiy = new AssignmentOfAssetVM();
            entitiy = viewmodel;
            return true;
        }

        public ProfessionalDataVM GetProfMasterInfo(string fullname, string SiteUrl)
        {
            var caml = @"<View><Query>
                       <Where>
                          <Eq>
                             <FieldRef Name='Title' />
                             <Value Type='Text'>"+fullname+ @"</Value>
                          </Eq>
                       </Where>
                    </Query>
                    <ViewFields>
                       <FieldRef Name='Title' />
                        <FieldRef Name='ID' />   
                       <FieldRef Name='Position' />
                       <FieldRef Name='Project_x002f_Unit' />
                       <FieldRef Name='mobilephonenr' />
                    </ViewFields>
                    <QueryOptions /></View>";
            var siteHr = SiteUrl.Replace("/bo", "/hr");
            var list = SPConnector.GetList("Professional Master", siteHr, caml);
            var viewmodel = new ProfessionalDataVM();
            foreach (var item in list)
            {
                viewmodel.ID = Convert.ToInt32(item["ID"]);
                viewmodel.CurrentPosition.Text = Convert.ToString(item["Project_x002f_Unit"]);
                viewmodel.MobileNumberOne = Convert.ToString(item["mobilephonenr"]);
            }

            return viewmodel;
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
                                 <Value Type='Text'>"+ assetID + @"</Value>
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
            foreach(var item in list)
            {
                model.ID = Convert.ToInt32(item["ID"]);
                model.AssetNoAssetDesc.Text = Convert.ToString(item["AssetID"]);
                model.AssetDesc = Convert.ToString(item["Title"]);
            }

            return model;
        }

        public IEnumerable<LocationMasterVM> GetProvince()
        {
            var models = new List<LocationMasterVM>();
            var caml = @"<View><Query>
                       <Where>
                          <IsNotNull>
                             <FieldRef Name='Province' />
                          </IsNotNull>
                       </Where>
                       <OrderBy>
                          <FieldRef Name='Province' Ascending='True' />
                       </OrderBy>
                    </Query>
                    <ViewFields>
                       <FieldRef Name='Province' />
                    </ViewFields>
                    <QueryOptions /></View>";
            List<string> listProvince = new List<string>();
            foreach (var item in SPConnector.GetList("Location Master", _siteUrl, caml))
            {
                var province = "";
                if ((item["Province"] as FieldLookupValue) != null)
                {
                    province = (item["Province"] as FieldLookupValue).LookupValue;
                }

                if(listProvince.Count == 0)
                {
                    listProvince.Add(province);
                    models.Add(ConvertToProvince(item));
                }
                else
                {
                    if(listProvince.Contains(province))
                    {
                        continue;
                    }
                    else
                    {
                        listProvince.Add(province);
                        models.Add(ConvertToProvince(item));
                    }
                }
            }

            return models;
        }

        private LocationMasterVM ConvertToProvince(ListItem item)
        {
            var viewModel = new LocationMasterVM();

            viewModel.ID = Convert.ToInt32(item["ID"]);
            var province = "";
            if ((item["Province"] as FieldLookupValue) != null)
            {
                province = (item["Province"] as FieldLookupValue).LookupValue;
            }
            //var info = GetInfoProvince("Province", province, _siteUrl);
            viewModel.Province.Text = province;
            //viewModel.OfficeName = Convert.ToString(item["Title"]);
            //viewModel.FloorName = Convert.ToInt32(item["Floor"]);
            //viewModel.RoomName = Convert.ToString(item["Room"]);
            //viewModel.Remarks = Convert.ToString(item["Remarks"]);
            return viewModel;
        }

        public LocationMasterVM GetProvinceInfo(string province, string SiteUrl)
        {
            var caml = @"<View>
                        <Query>
                           <Where>
                              <Eq>
                                 <FieldRef Name='Province' />
                                 <Value Type='Lookup'>"+province+@"</Value>
                              </Eq>
                           </Where>
                        </Query>
                        <ViewFields>
                           <FieldRef Name='Province' />
                           <FieldRef Name='Floor' />
                           <FieldRef Name='Room' />
                           <FieldRef Name='Remarks' />
                           <FieldRef Name='Title' />
                        </ViewFields>
                        <QueryOptions /></View>";
            var list = SPConnector.GetList("Location Master", SiteUrl, caml);
            var viewmodel = new LocationMasterVM();
            foreach(var item in list)
            {
                viewmodel.ID = Convert.ToInt32(item["ID"]);
                viewmodel.OfficeName = Convert.ToString(item["Title"]);
                viewmodel.FloorName = Convert.ToInt32(item["Floor"]);
                viewmodel.RoomName = Convert.ToString(item["Room"]);
                viewmodel.Remarks = Convert.ToString(item["Remarks"]);
            }

            return viewmodel;
        }

        public void CreateDetails(int? headerID, IEnumerable<AssignmentOfAssetDetailsVM> items)
        {
            foreach (var item in items)
            {
                if (Item.CheckIfSkipped(item)) continue;

                if (Item.CheckIfDeleted(item))
                {
                    try
                    {
                        SPConnector.DeleteListItem("Assignment Asset Detail", item.ID, _siteUrl);
                    }
                    catch (Exception e)
                    {
                        logger.Error(e);
                        throw e;
                    }
                    continue;
                }

                var updatedValues = new Dictionary<string, object>();
                updatedValues.Add("assetassignment", new FieldLookupValue { LookupId = Convert.ToInt32(headerID) });
                var getAssetID = SPConnector.GetListItem("Asset Acquisition Details", item.AssetSubAsset.Value.Value, _siteUrl);
                var getProvince = SPConnector.GetListItem("Province", item.Province.Value.Value, _siteUrl);
                var provinceinfo = SPConnector.GetListItem("Location Master", item.Province.Value.Value, _siteUrl);
                if ((getAssetID["assetsubasset"] as FieldLookupValue) != null)
                {
                    updatedValues.Add("assetsubasset", (getAssetID["assetsubasset"] as FieldLookupValue).LookupId);
                }

                if ((getProvince["Province"] as FieldLookupValue) != null)
                {
                    updatedValues.Add("province", (getProvince["Province"] as FieldLookupValue).LookupId);
                }
                //updatedValues.Add("assetsubasset", getAssetID["AssetID"]);
                //updatedValues.Add("province", getProvince["Title"]);
                updatedValues.Add("office", provinceinfo["Title"]);
                updatedValues.Add("floor", provinceinfo["Floor"]);
                updatedValues.Add("room", provinceinfo["Room"]);
                updatedValues.Add("remarks", provinceinfo["Remarks"]);
                updatedValues.Add("Status", "RUNNING");
                try
                {
                    SPConnector.AddListItem("Asset Assignment Detail", updatedValues, _siteUrl);
                }
                catch (Exception e)
                {
                    logger.Error(e);
                    throw new Exception(ErrorResource.SPInsertError);
                }
            }
        }

        public IEnumerable<LocationMasterVM> GetOfficeName(string SiteUrl, string province)
        {
            var model = new List<LocationMasterVM>();
            string caml = "";
            if(province == null)
            {
                caml = @"<View><Query>
                       <Where>
                          <IsNotNull>
                             <FieldRef Name='Province' />
                          </IsNotNull>
                       </Where>
                       <OrderBy>
                          <FieldRef Name='Province' Ascending='True' />
                       </OrderBy>
                    </Query>
                    <ViewFields>
                       <FieldRef Name='Title' />
                    </ViewFields>
                    <QueryOptions /></View>";
            }
            else
            {
                caml = @"<View><Query>
                       <Where>
                          <Eq>
                             <FieldRef Name='Province' />
                             <Value Type='Lookup'>"+ province +@"</Value>
                          </Eq>
                       </Where>
                       <OrderBy>
                          <FieldRef Name='Province' Ascending='True' />
                       </OrderBy>
                    </Query>
                    <ViewFields>
                       <FieldRef Name='Title' />
                    </ViewFields>
                    <QueryOptions /></View>";
            }

            foreach(var item in SPConnector.GetList("Location Master", SiteUrl, caml))
            {
                model.Add(ConvertToOfficeName(item));
            }

            return model;
        }

        private LocationMasterVM ConvertToOfficeName(ListItem item)
        {
            var viewmodel = new LocationMasterVM();

            viewmodel.ID = Convert.ToInt32(item["ID"]);
            viewmodel.OfficeName = Convert.ToString(item["Title"]);

            return viewmodel;

        }

        IEnumerable<AssignmentOfAssetDetailsVM> IAssignmentOfAssetService.GetDetails(int? headerID)
        {
            var caml = @"<View><Query><Where><Eq><FieldRef Name='assetacquisition' /><Value Type='Lookup'>" + headerID.ToString() + "</Value></Eq></Where></Query></View>";
            var details = new List<AssignmentOfAssetDetailsVM>();
            foreach (var item in SPConnector.GetList("Assignment Of Asset", _siteUrl, caml))
            {
                details.Add(ConvertToDetails(item));
            }

            return details;
        }

        private AssignmentOfAssetDetailsVM ConvertToDetails(ListItem item)
        {
            //var ListAssetSubAsset = SPConnector.GetListItem("Asset Master", (item["assetsubasset"] as FieldLookupValue).LookupId, _siteUrl);
            //AjaxComboBoxVM _assetSubAsset = new AjaxComboBoxVM();
            //_assetSubAsset.Value = (item["assetsubasset"] as FieldLookupValue).LookupId;
            //_assetSubAsset.Text = Convert.ToString(ListAssetSubAsset["AssetID"]) + " - " + Convert.ToString(ListAssetSubAsset["Title"]);

            //var ListWBS = SPConnector.GetListItem("WBS Master", (item["wbs"] as FieldLookupValue).LookupId, _siteUrl);
            //AjaxComboBoxVM _wbs = new AjaxComboBoxVM();
            //_wbs.Value = (item["wbs"] as FieldLookupValue).LookupId;
            //_wbs.Text = Convert.ToString(ListWBS["Title"]) + " - " + Convert.ToString(ListWBS["WBSDesc"]);

            return new AssignmentOfAssetDetailsVM
            {
                //ID = Convert.ToInt32(item["ID"]),
                //POLineItem = Convert.ToString(item["polineitem"]),
                //AssetSubAsset = AssetAcquisitionItemVM.GetAssetSubAssetDefaultValue(_assetSubAsset),
                //WBS = AssetAcquisitionItemVM.GetWBSDefaultValue(_wbs),
                //CostIDR = Convert.ToInt32(item["costidr"]),
                //CostUSD = Convert.ToInt32(item["costusd"]),
                //Remarks = Convert.ToString(item["remarks"]),
                //Status = Convert.ToString(item["status"])
            };
        }
    }
}
