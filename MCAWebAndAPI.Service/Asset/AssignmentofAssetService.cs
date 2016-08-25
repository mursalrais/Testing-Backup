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
using MCAWebAndAPI.Model.ViewModel.Control;
using System.Data;

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
            var caml = @"<View><Query>
                       <Where>
                          <Eq>
                            <FieldRef Name='ID' />
                             <Value Type='Counter'>" + (listItem["assetholder"] as FieldLookupValue).LookupId + @"</Value>
                          </Eq>
                       </Where>
                    </Query>
                    <ViewFields>
                       <FieldRef Name='Title' />
                       <FieldRef Name='Position' />
                    </ViewFields>
                    <QueryOptions /></View>";
            var sitehr = SiteUrl.Replace("/bo", "/hr");
            var infoAssetHolder = SPConnector.GetList("Professional Master", sitehr, caml);
            foreach (var info in infoAssetHolder)
            {
                viewModel.AssetHolder.Value = (listItem["assetholder"] as FieldLookupValue).LookupId.ToString();
                viewModel.AssetHolder.Text = info["Title"] + "-" + (info["Position"] as FieldLookupValue).LookupValue;
            }
            //viewModel.AccpMemo.Value = Convert.ToString(listItem["acceptancememono"]);
            viewModel.ProjectUnit = Convert.ToString(listItem["projectunit"]);
            viewModel.ContactNo = Convert.ToString(listItem["contactnumber"]);
            if (Convert.ToDateTime(listItem["transferdate"]) == DateTime.MinValue)
            {
                viewModel.Date = null;
            }
            else
            {
                viewModel.Date = Convert.ToDateTime(listItem["transferdate"]);
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
                        position = (item["Position"] as FieldLookupValue).LookupValue;
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

        public int? CreateHeader(AssignmentOfAssetVM viewmodel, string SiteUrl, string mode = null)
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
                if (mode == null)
                {
                    var breaks = viewmodel.AssetHolder.Value.Split('-');
                    var getInfo = GetProfMasterInfo(breaks[0], SiteUrl);
                    if (getInfo != null)
                    {
                        columnValues.Add("assetholder", new FieldLookupValue { LookupId = Convert.ToInt32(getInfo.ID) });
                        columnValues.Add("position", breaks[1]);
                        columnValues.Add("projectunit", getInfo.CurrentPosition.Text);
                        columnValues.Add("contactnumber", getInfo.MobileNumberOne);
                        columnValues.Add("completionstatus", viewmodel.CompletionStatus.Value);
                    }
                }
                else
                {
                    _siteUrl = SiteUrl;
                    var getInfo = GetProfMasterInfo(viewmodel.AssetHolder.Value, _siteUrl);
                    if (getInfo != null)
                    {
                        columnValues.Add("assetholder", new FieldLookupValue { LookupId = Convert.ToInt32(getInfo.ID) });
                        columnValues.Add("position", getInfo.Position);
                        columnValues.Add("projectunit", getInfo.CurrentPosition.Text);
                        columnValues.Add("contactnumber", getInfo.MobileNumberOne);
                        columnValues.Add("completionstatus", "In Progress");
                    }
                }

            }

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
            foreach (var doc in documents)
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
                var breaks = viewmodel.AssetHolder.Value.Split('-');
                var getInfo = GetProfMasterInfo(breaks[0], SiteUrl);
                if (getInfo != null)
                {
                    columnValues.Add("assetholder", new FieldLookupValue { LookupId = Convert.ToInt32(getInfo.ID) });
                    columnValues.Add("position", breaks[1]);
                    columnValues.Add("projectunit", getInfo.CurrentPosition.Text);
                    columnValues.Add("contactnumber", getInfo.MobileNumberOne);
                }
            }

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
                             <Value Type='Text'>" + fullname + @"</Value>
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
                if ((item["Position"] as FieldLookupValue) != null)
                {
                    viewmodel.Position = (item["Position"] as FieldLookupValue).LookupValue;
                }
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
                       <FieldRef Name='Title' />
                        <FieldRef Name='Province' />
                        <FieldRef Name='Floor' />
                        <FieldRef Name='Room' />
                        <FieldRef Name='Remarks' />
                    </ViewFields>
                    <QueryOptions /></View>";
            List<string> listProvince = new List<string>();
            foreach (var item in SPConnector.GetList("Location Master", _siteUrl, caml))
            {
                models.Add(ConvertToProvince(item));
                //var province = "";
                //if ((item["Province"] as FieldLookupValue) != null)
                //{
                //    province = (item["Province"] as FieldLookupValue).LookupValue;
                //}

                //if (listProvince.Count == 0)
                //{
                //    listProvince.Add(province);
                //    models.Add(ConvertToProvince(item));
                //}
                //else
                //{
                //    if (listProvince.Contains(province))
                //    {
                //        continue;
                //    }
                //    else
                //    {
                //        listProvince.Add(province);
                //        models.Add(ConvertToProvince(item));
                //    }
                //}
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
            viewModel.Province.Text = province;
            viewModel.OfficeName = Convert.ToString(item["Title"]);
            viewModel.FloorName = Convert.ToInt32(item["Floor"]);
            viewModel.RoomName = Convert.ToString(item["Room"]);
            viewModel.Remarks = Convert.ToString(item["Remarks"]);
            return viewModel;
        }

        public LocationMasterVM GetProvinceInfo(string province, string SiteUrl)
        {
            var caml = @"<View>
                        <Query>
                           <Where>
                              <Eq>
                                 <FieldRef Name='Province' />
                                 <Value Type='Lookup'>" + province + @"</Value>
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
            foreach (var item in list)
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
                var provinceinfo = SPConnector.GetListItem("Location Master", item.Province.Value.Value, _siteUrl);
                if ((getAssetID["assetsubasset"] as FieldLookupValue) != null)
                {
                    updatedValues.Add("assetsubasset", (getAssetID["assetsubasset"] as FieldLookupValue).LookupId);
                }

                if ((provinceinfo["Province"] as FieldLookupValue) != null)
                {
                    updatedValues.Add("province", (provinceinfo["Province"] as FieldLookupValue).LookupId);
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
            if (province == null)
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
                             <Value Type='Lookup'>" + province + @"</Value>
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

            foreach (var item in SPConnector.GetList("Location Master", SiteUrl, caml))
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
            var caml = @"<View><Query><Where><Eq><FieldRef Name='assetassignment' /><Value Type='Lookup'>" + headerID.ToString() + "</Value></Eq></Where></Query></View>";
            var details = new List<AssignmentOfAssetDetailsVM>();
            foreach (var item in SPConnector.GetList("Asset Assignment Detail", _siteUrl, caml))
            {
                details.Add(ConvertToDetails(item));
            }

            return details;
        }

        private AssignmentOfAssetDetailsVM ConvertToDetails(ListItem item)
        {
            var ListAssetSubAsset = SPConnector.GetListItem("Asset Master", (item["assetsubasset"] as FieldLookupValue).LookupId, _siteUrl);
            AjaxComboBoxVM _assetSubAsset = new AjaxComboBoxVM();
            _assetSubAsset.Value = (item["assetsubasset"] as FieldLookupValue).LookupId;
            _assetSubAsset.Text = Convert.ToString(ListAssetSubAsset["AssetID"]) + " - " + Convert.ToString(ListAssetSubAsset["Title"]);

            var province = (item["province"] as FieldLookupValue).LookupValue;

            var ListProvince = SPConnector.GetListItem("Location Master", (item["province"] as FieldLookupValue).LookupId, _siteUrl);
            AjaxComboBoxVM _province = new AjaxComboBoxVM();
            _province.Value = (item["province"] as FieldLookupValue).LookupId;
            _province.Text = (ListProvince["Province"] as FieldLookupValue).LookupValue +"-"+ ListProvince["Title"] + "-" +ListProvince["Floor"] + "-" +ListProvince["Room"] + "-" +ListProvince["Remarks"];


            return new AssignmentOfAssetDetailsVM
            {
                ID = Convert.ToInt32(item["ID"]),
                AssetSubAsset = AssignmentOfAssetDetailsVM.GetAssetSubAssetDefaultValue(_assetSubAsset),
                Province = AssignmentOfAssetDetailsVM.GetProvinceDefaultValue(_province),
                Status = Convert.ToString(item["Status"])
            };
        }

        public void UpdateDetails(int? headerID, IEnumerable<AssignmentOfAssetDetailsVM> items)
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
                var provinceinfo = SPConnector.GetListItem("Location Master", item.Province.Value.Value, _siteUrl);
                if ((getAssetID["assetsubasset"] as FieldLookupValue) != null)
                {
                    updatedValues.Add("assetsubasset", (getAssetID["assetsubasset"] as FieldLookupValue).LookupId);
                }

                //var provinceinfo = SPConnector.GetListItem("Location Master", item.Province.Value.Value, _siteUrl);

                //updatedValues.Add("assetsubasset", item.AssetSubAsset.Value.Value);
                if ((provinceinfo["Province"] as FieldLookupValue) != null)
                {
                    updatedValues.Add("province", (provinceinfo["Province"] as FieldLookupValue).LookupId);
                }
                updatedValues.Add("office", provinceinfo["Title"]);
                updatedValues.Add("floor", provinceinfo["Floor"]);
                updatedValues.Add("room", provinceinfo["Room"]);
                updatedValues.Add("remarks", provinceinfo["Remarks"]);
                updatedValues.Add("Status", "RUNNING");
                try
                {
                    if (Item.CheckIfUpdated(item))
                        SPConnector.UpdateListItem("Asset Assignment Detail", item.ID, updatedValues, _siteUrl);
                    else
                        SPConnector.AddListItem("Asset Assignment Detail", updatedValues, _siteUrl);
                }
                catch (Exception e)
                {
                    logger.Error(e);
                    throw new Exception(ErrorResource.SPInsertError);
                }
            }
        }

        public int? MassUploadHeaderDetail(string ListName, DataTable CSVDataTable, string SiteUrl = null)
        {
            int? latest = null;
            if (CSVDataTable.Columns.Count == 6)
            {
                foreach(DataRow  d in CSVDataTable.Rows)
                {
                    //header
                    var model = new AssignmentOfAssetVM();
                    model.AssetHolder.Text = Convert.ToString(d.ItemArray[2]);
                    var additionalDateTime = d.ItemArray[1] + " 00:00:00";
                    DateTime date;
                    //model.Date = DateTime.TryParse(d.ItemArray[6].ToString(), out date) ? date : (DateTime?)null;
                    model.Date = DateTime.TryParse(d.ItemArray[1].ToString(), out date) ? date : (DateTime?)null;

                    latest = CreateHeader(model, SiteUrl, "upload");
                }
                
            }
            else
            {
                foreach(DataRow d in CSVDataTable.Rows)
                {
                    //detail
                    var model = new AssignmentOfAssetDetailsVM();
                    //find id for assetsubasset from acquisition details ad province from location master
                    var camlasset = @"<View>
                        <Query>
                           <Where>
                              <Eq>
                                 <FieldRef Name='assetsubasset' />
                                 <Value Type='Text'>" + d.ItemArray[1] + @"</Value>
                              </Eq>
                           </Where>
                        </Query>
                        <ViewFields>
                           <FieldRef Name='assetsubasset' />
                        </ViewFields>
                        <QueryOptions /></View>";
                    //check province
                    var camlprovince = @"<View><Query>
                            <Where>
                                <And>
                                    <Eq>
                                    <FieldRef Name='Province' />
                                    <Value Type='Lookup'>" + d.ItemArray[2].ToString() + @"</Value>
                                    </Eq>
                                    <And>
                                    <Eq>
                                        <FieldRef Name='Title' />
                                        <Value Type='Text'>" + d.ItemArray[3].ToString() + @"</Value>
                                    </Eq>
                                    <And>
                                        <Eq>
                                            <FieldRef Name='Floor' />
                                            <Value Type='Text'>" + d.ItemArray[4].ToString() + @"</Value>
                                        </Eq>
                                        <Eq>
                                            <FieldRef Name='Room' />
                                            <Value Type='Text'>" + d.ItemArray[5].ToString() + @"</Value>
                                        </Eq>
                                    </And>
                                    </And>
                                </And>
                            </Where>
                        </Query>
                        <ViewFields>
                            <FieldRef Name='Province' />
                            <FieldRef Name='Title' />
                            <FieldRef Name='Floor' />
                            <FieldRef Name='Room' />
                            <FieldRef Name='Remarks' />
                        </ViewFields>
                        <QueryOptions /></View>";
                    var inforAsset = SPConnector.GetList("Asset Acquisition Details", SiteUrl, camlasset);
                    var infoLocationMaster = SPConnector.GetList("Location Master", SiteUrl, camlprovince);
                    foreach (var asset in inforAsset)
                    {
                        model.AssetSubAsset.Value = Convert.ToInt32(asset["ID"]);
                    }

                    foreach(var location in infoLocationMaster)
                    {
                        model.Province.Value = Convert.ToInt32(location["ID"]);
                        model.OfficeName = Convert.ToString(location["Title"]);
                        model.Floor = Convert.ToString(location["Floor"]);
                        model.Room = Convert.ToString(location["Room"]);
                        model.Remarks = Convert.ToString(location["Remarks"]);
                    }

                    CreateDetails(Convert.ToInt32(d.ItemArray[0]), model, SiteUrl);
                    latest = SPConnector.GetLatestListItemID("Asset Assignment Detail", SiteUrl);
                }
                

            }
            return latest;
        }

        public void RollbackParentChildrenUpload(string listNameHeader, int? latestIDHeader, string siteUrl)
        {
            throw new NotImplementedException();
        }

        public bool isExist(string listname, string caml, string site)
        {
            var list = SPConnector.GetList(listname, site, caml);
            if (list.Count == 0)
            {
                return false;
            }

            return true;
        }

        public IEnumerable<LocationMasterVM> GetFloorList(string SiteUrl, string office = null)
        {
            var model = new List<LocationMasterVM>();
            string caml = "";
            if (office == null)
            {
                caml = @"<View><Query>
                       <Where>
                          <IsNotNull>
                             <FieldRef Name='Floor' />
                          </IsNotNull>
                       </Where>
                       <OrderBy>
                          <FieldRef Name='Floor' Ascending='True' />
                       </OrderBy>
                    </Query>
                    <ViewFields>
                       <FieldRef Name='Floor' />
                    </ViewFields>
                    <QueryOptions /></View>";
            }
            else
            {
                caml = @"<View></View>";
            }

            List<int> listFloor= new List<int>();
            foreach (var item in SPConnector.GetList("Location Master", _siteUrl, caml))
            {
                var floorelement = 0;
                if (Convert.ToString(item["Floor"]) != null)
                {
                    floorelement = Convert.ToInt32(item["Floor"]);
                }

                if (listFloor.Count == 0)
                {
                    listFloor.Add(floorelement);
                    model.Add(ConvertToFloorList(item));
                }
                else
                {
                    if (listFloor.Contains(floorelement))
                    {
                        continue;
                    }
                    else
                    {
                        listFloor.Add(floorelement);
                        model.Add(ConvertToFloorList(item));
                    }
                }
            }

            return model;
        }

        private LocationMasterVM ConvertToFloorList(ListItem item)
        {
            var viewmodel = new LocationMasterVM();
            viewmodel.OfficeName = Convert.ToString(item["Floor"]);

            return viewmodel;

        }

        public IEnumerable<LocationMasterVM> GetRoomList(string SiteUrl, string floor = null)
        {
            var model = new List<LocationMasterVM>();
            string caml = "";
            if (floor == null)
            {
                caml = @"<View><Query>
                       <Where>
                          <IsNotNull>
                             <FieldRef Name='Room' />
                          </IsNotNull>
                       </Where>
                       <OrderBy>
                            <FieldRef Name='Room' Ascending='True' />
                        </OrderBy>
                    </Query>
                    <ViewFields>
                       <FieldRef Name='Room' />
                    </ViewFields>
                    <QueryOptions /></View>";
            }
            else
            {
                caml = @"<View></View>";
            }
            List<string> listRoom = new List<string>();
            foreach (var item in SPConnector.GetList("Location Master", _siteUrl, caml))
            {
                var roomname = "";
                if (Convert.ToString(item["Room"])  != null)
                {
                    roomname = Convert.ToString(item["Room"]);
                }

                if (listRoom.Count == 0)
                {
                    listRoom.Add(roomname);
                    model.Add(ConvertToRoomList(item));
                }
                else
                {
                    if (listRoom.Contains(roomname))
                    {
                        continue;
                    }
                    else
                    {
                        listRoom.Add(roomname);
                        model.Add(ConvertToRoomList(item));
                    }
                }
            }

            return model;
        }

        private LocationMasterVM ConvertToRoomList(ListItem item)
        {
            var viewmodel = new LocationMasterVM();

            //viewmodel.ID = Convert.ToInt32(item["ID"]);
            viewmodel.RoomName = Convert.ToString(item["Room"]);

            return viewmodel;

        }

        public void CreateDetails(int? headerID, AssignmentOfAssetDetailsVM item, string SiteUrl)
        {
            var updatedValues = new Dictionary<string, object>();
            updatedValues.Add("assetassignment", new FieldLookupValue { LookupId = Convert.ToInt32(headerID) });
            var getAssetID = SPConnector.GetListItem("Asset Acquisition Details", item.AssetSubAsset.Value.Value, _siteUrl);
            var provinceinfo = SPConnector.GetListItem("Location Master", item.Province.Value.Value, _siteUrl);
            if ((getAssetID["assetsubasset"] as FieldLookupValue) != null)
            {
                updatedValues.Add("assetsubasset", (getAssetID["assetsubasset"] as FieldLookupValue).LookupId);
            }

            if ((provinceinfo["Province"] as FieldLookupValue) != null)
            {
                updatedValues.Add("province", (provinceinfo["Province"] as FieldLookupValue).LookupId);
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
}
