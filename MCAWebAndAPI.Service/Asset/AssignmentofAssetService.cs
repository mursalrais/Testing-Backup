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
using System.IO;

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
            model.CancelURL = _siteUrl + UrlResource.AssetAssignment;
            model.AssetHolder.Choices = GetFromListHR("Professional Master", "Title", "Position", SiteUrl);

            return model;
        }

        public AssignmentOfAssetVM GetHeader(int? ID, string SiteUrl)
        {
            var listItem = SPConnector.GetListItem("Asset Assignment", ID, SiteUrl);
            var filename = "";
            if (Convert.ToBoolean(listItem["Attachments"]) != false)
            {
                 filename = SPConnector.GetAttachFileName("Asset Assignment", ID, _siteUrl);
            }
            
            var viewModel = new AssignmentOfAssetVM();
            viewModel.filename = filename;
            viewModel.position = listItem["position"].ToString();
            viewModel.nameOnly = listItem["assetholder"].ToString();
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
            viewModel.CompletionStatus.Value = Convert.ToString(listItem["completionstatus"]);
            viewModel.ID = ID;
            var caml1 = @"<View><Query>
                       <Where>
                          <Eq>
                             <FieldRef Name='assetassignment' />
                             <Value Type='Lookup'>"+ID+@"</Value>
                          </Eq>
                       </Where>
                    </Query>
                    <ViewFields>
                       <FieldRef Name='assetsubasset' />
                    </ViewFields>
                    <QueryOptions /></View>";
            var getDetails = SPConnector.GetList("Asset Assignment Detail", _siteUrl, caml1);
            var combine = "";
            foreach (var det in getDetails)
            {
                if(combine == "")
                {
                    combine = (det["assetsubasset"] as FieldLookupValue).LookupValue;
                }
                else
                {
                    if(!combine.Contains((det["assetsubasset"] as FieldLookupValue).LookupValue))
                    {
                        combine = combine + ", " + (det["assetsubasset"] as FieldLookupValue).LookupValue;
                    }
                }
            }
            viewModel.AssetIDs = combine;
            viewModel.CancelURL = _siteUrl + UrlResource.AssetAssignment;

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
            viewmodel.CancelURL = _siteUrl + UrlResource.AssetAssignment;
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
                var id = SPConnector.GetLatestListItemID("Asset Assignment", _siteUrl);
                if (viewmodel.CompletionStatus.Value == "Complete" && viewmodel.attach != null)
                {
                    SPConnector.AttachFile("Asset Assignment", id, viewmodel.attach, _siteUrl);
                }
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
            viewmodel.CancelURL = _siteUrl + UrlResource.AssetAssignment;
            var columnValues = new Dictionary<string, object>();
            var ID = Convert.ToInt32(viewmodel.ID);
            var oldData = SPConnector.GetListItem("Asset Assignment", ID, SiteUrl);
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
                if (viewmodel.CompletionStatus.Value == "Complete" && viewmodel.attach != null)
                {
                    SPConnector.AttachFile("Asset Assignment", ID, viewmodel.attach, _siteUrl);
                }

                var newData = SPConnector.GetListItem("Asset Assignment", ID, _siteUrl);
                if (viewmodel.CompletionStatus.Value == "Complete")
                {
                    if (Convert.ToBoolean(newData["Attachments"]) == false)
                    {
                        var oldcolumnValues = new Dictionary<string, object>();
                        oldcolumnValues.Add("Title", oldData["Title"]);
                        oldcolumnValues.Add("transferdate", oldData["transferdate"]);
                        oldcolumnValues.Add("assetholder",oldData["assetholder"]);
                        oldcolumnValues.Add("position", oldData["position"]);
                        oldcolumnValues.Add("projectunit", oldData["projectunit"]);
                        oldcolumnValues.Add("contactnumber", oldData["contactnumber"]);
                        oldcolumnValues.Add("completionstatus", oldData["completionstatus"]);

                        SPConnector.UpdateListItem("Asset Assignment", ID, oldcolumnValues, _siteUrl);
                        return false;
                    }
                }
                else
                {
                    if (Convert.ToBoolean(newData["Attachments"]) == true)
                    {
                        var oldcolumnValues = new Dictionary<string, object>();
                        oldcolumnValues.Add("Title", oldData["Title"]);
                        oldcolumnValues.Add("transferdate", oldData["transferdate"]);
                        oldcolumnValues.Add("assetholder", oldData["assetholder"]);
                        oldcolumnValues.Add("position", oldData["position"]);
                        oldcolumnValues.Add("projectunit", oldData["projectunit"]);
                        oldcolumnValues.Add("contactnumber", oldData["contactnumber"]);
                        oldcolumnValues.Add("completionstatus", oldData["completionstatus"]);

                        SPConnector.UpdateListItem("Asset Assignment", ID, oldcolumnValues, _siteUrl);
                        return false;
                    }
                }
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
                        <FieldRef Name='city' />
                    </ViewFields>
                    <QueryOptions /></View>";
            List<string> listProvince = new List<string>();
            foreach (var item in SPConnector.GetList("Location Master", _siteUrl, caml))
            {
                models.Add(ConvertToProvince(item));
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
                province = Convert.ToString(item["city"]) +","+(item["Province"] as FieldLookupValue).LookupValue;
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
                        SPConnector.DeleteListItem("Asset Assignment Detail", item.ID, _siteUrl);
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
                updatedValues.Add("city", provinceinfo["city"]);
                updatedValues.Add("office", provinceinfo["Title"]);
                updatedValues.Add("floor", provinceinfo["Floor"]);
                updatedValues.Add("room", provinceinfo["Room"]);
                updatedValues.Add("remarks", item.Remarks);
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

        IEnumerable<AssignmentOfAssetDetailsVM> IAssignmentOfAssetService.GetDetailsPrint(int? headerID)
        {
            var caml = @"<View><Query><Where><Eq><FieldRef Name='assetassignment' /><Value Type='Lookup'>" + headerID.ToString() + "</Value></Eq></Where></Query></View>";
            var details = new List<AssignmentOfAssetDetailsVM>();
            List<string> listAssetID = new List<string>();

            foreach (var item in SPConnector.GetList("Asset Assignment Detail", _siteUrl, caml))
            {
                if(listAssetID.Count != 0 && listAssetID.Contains((item["assetsubasset"] as FieldLookupValue).LookupValue))
                {
                    listAssetID.Add((item["assetsubasset"] as FieldLookupValue).LookupValue);
                    continue;
                }
                else
                {
                    listAssetID.Add((item["assetsubasset"] as FieldLookupValue).LookupValue);
                    details.Add(ConvertToDetails(item));
                }
            }

            foreach(var d in details)
            {
                var quantityPerItem = 0;
                foreach(var l in listAssetID)
                {
                    if (d.textasset == l)
                    {
                        quantityPerItem++;
                    }
                }
                d.quantity = quantityPerItem;
            }

            return details;
        }

        private AssignmentOfAssetDetailsVM ConvertToDetails(ListItem item)
        {            
            var ListAssetSubAsset = SPConnector.GetListItem("Asset Master", (item["assetsubasset"] as FieldLookupValue).LookupId, _siteUrl);

            AjaxComboBoxVM _assetSubAsset = new AjaxComboBoxVM();
            _assetSubAsset.Value = (item["assetsubasset"] as FieldLookupValue).LookupId;
            _assetSubAsset.Text = Convert.ToString(ListAssetSubAsset["AssetID"]) + " - " + Convert.ToString(ListAssetSubAsset["Title"]);

            var province = (item["province"] as FieldLookupValue).LookupId;
            var caml = @"<View><Query>
                       <Where>
                          <And>
                             <Eq>
                                <FieldRef Name='Province_x003a_ID' />
                                <Value Type='Lookup'>"+province+@"</Value>
                             </Eq>
                             <And>
                                <Eq>
                                   <FieldRef Name='city' />
                                   <Value Type='Text'>"+Convert.ToString(item["city"])+@"</Value>
                                </Eq>
                                <And>
                                   <Eq>
                                      <FieldRef Name='Title' />
                                      <Value Type='Text'>"+Convert.ToString(item["office"])+@"</Value>
                                   </Eq>
                                   <And>
                                      <Eq>
                                         <FieldRef Name='Room' />
                                         <Value Type='Text'>"+ Convert.ToString(item["room"]) + @"</Value>
                                      </Eq>
                                      <Eq>
                                         <FieldRef Name='Floor' />
                                         <Value Type='Text'>"+ Convert.ToString(item["floor"]) + @"</Value>
                                      </Eq>
                                   </And>
                                </And>
                             </And>
                          </And>
                       </Where>
                    </Query>
                    <ViewFields>
                       <FieldRef Name='Province' />
                       <FieldRef Name='Title' />
                        <FieldRef Name='city' />
                       <FieldRef Name='Floor' />
                       <FieldRef Name='Room' />
                       <FieldRef Name='Remarks' />
                    </ViewFields>
                    <QueryOptions /></View>";
            var ListProvince = SPConnector.GetList("Location Master", _siteUrl, caml);
            AjaxComboBoxVM _province = new AjaxComboBoxVM();
            foreach (var x in ListProvince)
            {
                if(Convert.ToString(x["Title"]) == Convert.ToString(item["office"]) && Convert.ToString(x["Floor"]) == Convert.ToString(item["floor"]) && Convert.ToString(x["Room"]) == Convert.ToString(item["room"]))
                {
                    _province.Value = (item["province"] as FieldLookupValue).LookupId;
                    _province.Text = Convert.ToString(x["city"])+","+(x["Province"] as FieldLookupValue).LookupValue + "-" + x["Title"] + "-" + x["Floor"] + "-" + x["Room"];
                }
            }

            return new AssignmentOfAssetDetailsVM
            {
                ID = Convert.ToInt32(item["ID"]),
                textasset = Convert.ToString(ListAssetSubAsset["AssetID"]),
                description = Convert.ToString(ListAssetSubAsset["Title"]),
                quantity = 1,
                AssetSubAsset = AssignmentOfAssetDetailsVM.GetAssetSubAssetDefaultValue(_assetSubAsset),
                Province = AssignmentOfAssetDetailsVM.GetProvinceDefaultValue(_province),
                Remarks = Convert.ToString(item["remarks"]),
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
                        SPConnector.DeleteListItem("Asset Assignment Detail", item.ID, _siteUrl);
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
                var caml = @"<View><Query>
                           <Where>
                              <Eq>
                                 <FieldRef Name='Asset_x0020_Sub_x0020_Asset_x003' />
                                 <Value Type='Lookup'>"+item.AssetSubAsset.Value.Value+ @"</Value>
                              </Eq>
                           </Where>
                        </Query>
                        <ViewFields>
                           <FieldRef Name='assetsubasset' />
                        </ViewFields>
                        <QueryOptions /></View>";

                var getAssetID = SPConnector.GetList("Asset Acquisition Details",  _siteUrl, caml);
                //getInformation From  Asset Assignment Detail
                var camld = @"<View><Query>
                           <Where>
                              <Eq>
                                 <FieldRef Name='ID' />
                                 <Value Type='Counter'>"+item.ID+ @"</Value>
                              </Eq>
                           </Where>
                        </Query>
                        <ViewFields>
                           <FieldRef Name='room' />
                            <FieldRef Name='city' />
                           <FieldRef Name='floor' />
                           <FieldRef Name='office' />
                        </ViewFields>
                        <QueryOptions /></View>";
                var getInfoProvinceFromDetails = SPConnector.GetList("Asset Assignment Detail", _siteUrl, camld);
                if(getInfoProvinceFromDetails.Count != 0 && getAssetID.Count != 0)
                {
                    foreach (var d in getInfoProvinceFromDetails)
                    {
                        var camlx = @"<View><Query>
                                   <Where>
                                      <And>
                                         <Eq>
                                            <FieldRef Name='Province_x003a_ID' />
                                            <Value Type='Lookup'>" + item.Province.Value.Value + @"</Value>
                                         </Eq>
                                         <And>
                                            <Eq>
                                               <FieldRef Name='Title' />
                                               <Value Type='Text'>" + d["office"] + @"</Value>
                                            </Eq>
                                            <And>
                                               <Eq>
                                                  <FieldRef Name='Floor' />
                                                  <Value Type='Text'>" + d["floor"] + @"</Value>
                                               </Eq>
                                               <Eq>
                                                  <FieldRef Name='Room' />
                                                  <Value Type='Text'>" + d["room"] + @"</Value>
                                               </Eq>
                                            </And>
                                         </And>
                                      </And>
                                   </Where>
                                </Query>
                                <ViewFields>
                                <FieldRef Name='Title' />
                                <FieldRef Name='Province' />
                                <FieldRef Name='city' />
                                <FieldRef Name='Floor' />
                                <FieldRef Name='Room' />
                                </ViewFields>
                                <QueryOptions /></View>";
                        var provinceinfo = SPConnector.GetList("Location Master", _siteUrl, camlx);
                        foreach (var pro in provinceinfo)
                        {
                            if ((pro["Province"] as FieldLookupValue) != null)
                            {
                                updatedValues.Add("province", (pro["Province"] as FieldLookupValue).LookupId);
                            }
                            updatedValues.Add("city", pro["city"]);
                            updatedValues.Add("office", pro["Title"]);
                            updatedValues.Add("floor", pro["Floor"]);
                            updatedValues.Add("room", pro["Room"]);
                        }
                    }

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
                    var provinceinfo = SPConnector.GetListItem("Location Master", item.Province.Value.Value, _siteUrl);
                    if ((assetID["assetsubasset"] as FieldLookupValue) != null)
                    {
                        updatedValues.Add("assetsubasset", (assetID["assetsubasset"] as FieldLookupValue).LookupId);
                    }

                    if ((provinceinfo["Province"] as FieldLookupValue) != null)
                    {
                        updatedValues.Add("province", (provinceinfo["Province"] as FieldLookupValue).LookupId);
                    }
                    updatedValues.Add("city", provinceinfo["city"]);
                    updatedValues.Add("office", provinceinfo["Title"]);
                    updatedValues.Add("floor", provinceinfo["Floor"]);
                    updatedValues.Add("room", provinceinfo["Room"]);
                }
                
                updatedValues.Add("remarks", item.Remarks);
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
            SPConnector.DeleteListItem(listNameHeader, latestIDHeader, siteUrl);
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
            updatedValues.Add("city", provinceinfo["city"]);
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

        public bool Syncronize(string SiteUrl)
        {
            var sitehr = SiteUrl.Replace("/bo", "/hr");
            var lists = SPConnector.GetList("Asset Assignment", SiteUrl);
            foreach (var l in lists)
            {
                var caml = @"<View><Query>
                           <Where>
                              <Eq>
                                 <FieldRef Name='ID' />
                                 <Value Type='Lookup'>"+ (l["assetholder"] as FieldLookupValue).LookupId + @"</Value>
                              </Eq>
                           </Where>
                        </Query>
                        <ViewFields>
                        <FieldRef Name='Title' />
                        <FieldRef Name='Project_x002f_Unit' />
                        <FieldRef Name='mobilephonenr' />
                        <FieldRef Name='Position' />
                        </ViewFields>
                        <QueryOptions /></View>";
                var getFromProfMas = SPConnector.GetList("Professional Master", sitehr, caml);
                foreach (var profMas in getFromProfMas)
                {
                    var model = new Dictionary<string, object>();
                    model.Add("projectunit", Convert.ToString(profMas["Project_x002f_Unit"]));
                    model.Add("contactnumber", Convert.ToString(profMas["mobilephonenr"]));
                    model.Add("position", (profMas["Position"] as FieldLookupValue).LookupValue);

                    SPConnector.UpdateListItem("Asset Assignment", Convert.ToInt32(l["ID"]), model, SiteUrl);
                }
            }

            return true;
        }
    }
}
