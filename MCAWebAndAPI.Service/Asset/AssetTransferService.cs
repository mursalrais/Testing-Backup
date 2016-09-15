using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using NLog;
using MCAWebAndAPI.Service.Utils;
using MCAWebAndAPI.Model.ViewModel.Form.Shared;
using Microsoft.SharePoint.Client;
using MCAWebAndAPI.Service.Resources;
using System.Web;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;
using System.Data;

namespace MCAWebAndAPI.Service.Asset
{
    public class AssetTransferService : IAssetTransferService
    {
        string _siteUrl;
        static Logger logger = LogManager.GetCurrentClassLogger();

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = siteUrl;
        }


        public AssetTransferVM GetPopulatedModel(string SiteUrl)
        {
            var model = new AssetTransferVM();
            model.CancelURL = _siteUrl + UrlResource.AssetTransfer;
            model.AssetHolder.Choices = GetFromListHR("Professional Master", "Title", "Position", SiteUrl);
            model.AssetHolderTo.Choices = GetFromListHR("Professional Master", "Title", "Position", SiteUrl);

            return model;
        }

        public AssetTransferVM GetHeader(int? ID, string SiteUrl)
        {
            var filename = SPConnector.GetAttachFileName("Asset Transfer", ID, _siteUrl);
            var listItem = SPConnector.GetListItem("Asset Transfer", ID, SiteUrl);
            var viewModel = new AssetTransferVM();
            viewModel.filename = filename;
            viewModel.positionFrom = listItem["positionfrom"].ToString();
            viewModel.nameOnlyFrom = listItem["assetholderfrom"].ToString();
            viewModel.positionTo = listItem["positionto"].ToString();
            viewModel.nameOnlyTo = listItem["assetholderto"].ToString();
            viewModel.TransactionType = Convert.ToString(listItem["Title"]);
            viewModel.AssetHolder.Choices = GetFromListHR("Professional Master", "Title", "Position", SiteUrl);
            viewModel.AssetHolderTo.Choices = GetFromListHR("Professional Master", "Title", "Position", SiteUrl);
            var caml = @"<View><Query>
                       <Where>
                          <Eq>
                            <FieldRef Name='ID' />
                             <Value Type='Counter'>" + (listItem["assetholderfrom"] as FieldLookupValue).LookupId + @"</Value>
                          </Eq>
                       </Where>
                    </Query>
                    <ViewFields>
                       <FieldRef Name='Title' />
                       <FieldRef Name='Position' />
                    </ViewFields>
                    <QueryOptions /></View>";
            var camlto = @"<View><Query>
                       <Where>
                          <Eq>
                            <FieldRef Name='ID' />
                             <Value Type='Counter'>" + (listItem["assetholderto"] as FieldLookupValue).LookupId + @"</Value>
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
            var infoAssetHolderTo = SPConnector.GetList("Professional Master", sitehr, camlto);
            for (int i = 0; i < infoAssetHolder.Count; i++)
            {
                viewModel.AssetHolder.Value = (listItem["assetholderfrom"] as FieldLookupValue).LookupId.ToString();
                viewModel.AssetHolder.Text = infoAssetHolder[i]["Title"] + "-" + (infoAssetHolder[i]["Position"] as FieldLookupValue).LookupValue;

                viewModel.AssetHolderTo.Value = (listItem["assetholderto"] as FieldLookupValue).LookupId.ToString();
                viewModel.AssetHolderTo.Text = infoAssetHolderTo[i]["Title"] + "-" + (infoAssetHolderTo[i]["Position"] as FieldLookupValue).LookupValue;
            }
            viewModel.ProjectUnit = Convert.ToString(listItem["projectunitfrom"]);
            viewModel.ContactNo = Convert.ToString(listItem["contactnumberfrom"]);
            viewModel.ProjectUnitTo = Convert.ToString(listItem["projectunitto"]);
            viewModel.ContactNoTo = Convert.ToString(listItem["contactnumberto"]);
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
                             <FieldRef Name='assettransfer' />
                             <Value Type='Lookup'>" + ID + @"</Value>
                          </Eq>
                       </Where>
                    </Query>
                    <ViewFields>
                       <FieldRef Name='assetsubasset' />
                    </ViewFields>
                    <QueryOptions /></View>";
            var getDetails = SPConnector.GetList("Asset Transfer Detail", _siteUrl, caml1);
            var combine = "";
            foreach (var det in getDetails)
            {
                if (combine == "")
                {
                    combine = (det["assetsubasset"] as FieldLookupValue).LookupValue;
                }
                else
                {
                    if (!combine.Contains((det["assetsubasset"] as FieldLookupValue).LookupValue))
                    {
                        combine = combine + ", " + (det["assetsubasset"] as FieldLookupValue).LookupValue;
                    }
                }
            }
            viewModel.AssetIDs = combine;
            viewModel.CancelURL = _siteUrl + UrlResource.AssetTransfer;

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

        public int? CreateHeader(AssetTransferVM viewmodel, string SiteUrl, string mode = null)
        {
            viewmodel.CancelURL = _siteUrl + UrlResource.AssetTransfer;
            var columnValues = new Dictionary<string, object>();
            //columnValues.add
            columnValues.Add("Title", "Asset Transfer");
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
                    var breaks1 = viewmodel.AssetHolderTo.Value.Split('-');
                    var getInfo = GetProfMasterInfo(breaks[0], SiteUrl);
                    var getInfo1 = GetProfMasterInfo(breaks1[0], SiteUrl);
                    if (getInfo != null && getInfo1 != null)
                    {
                        columnValues.Add("assetholderfrom", new FieldLookupValue { LookupId = Convert.ToInt32(getInfo.ID) });
                        columnValues.Add("positionfrom", breaks[1]);
                        columnValues.Add("projectunitfrom", getInfo.CurrentPosition.Text);
                        columnValues.Add("contactnumberfrom", getInfo.MobileNumberOne);

                        columnValues.Add("assetholderto", new FieldLookupValue { LookupId = Convert.ToInt32(getInfo1.ID) });
                        columnValues.Add("positionto", breaks1[1]);
                        columnValues.Add("projectunitto", getInfo1.CurrentPosition.Text);
                        columnValues.Add("contactnumberto", getInfo1.MobileNumberOne);

                        columnValues.Add("completionstatus", viewmodel.CompletionStatus.Value);
                    }
                }
                else
                {
                    _siteUrl = SiteUrl;
                    var getInfo = GetProfMasterInfo(viewmodel.AssetHolder.Value, _siteUrl);
                    var getInfo1 = GetProfMasterInfo(viewmodel.AssetHolderTo.Value, _siteUrl);
                    if (getInfo != null)
                    {
                        columnValues.Add("assetholderfrom", new FieldLookupValue { LookupId = Convert.ToInt32(getInfo.ID) });
                        columnValues.Add("positionfrom", getInfo.Position);
                        columnValues.Add("projectunitfrom", getInfo.CurrentPosition.Text);
                        columnValues.Add("contactnumberfrom", getInfo.MobileNumberOne);

                        columnValues.Add("assetholderto", new FieldLookupValue { LookupId = Convert.ToInt32(getInfo1.ID) });
                        columnValues.Add("positionto", getInfo1.Position);
                        columnValues.Add("projectunitto", getInfo1.CurrentPosition.Text);
                        columnValues.Add("contactnumberto", getInfo1.MobileNumberOne);

                        columnValues.Add("completionstatus", "In Progress");
                    }
                }

            }

            try
            {
                SPConnector.AddListItem("Asset Transfer", columnValues, _siteUrl);
                var id = SPConnector.GetLatestListItemID("Asset Transfer", _siteUrl);
                if (viewmodel.CompletionStatus.Value == "Complete" && (viewmodel.attach.FileName != "" || viewmodel.attach.FileName != null))
                {
                    SPConnector.AttachFile("Asset Transfer", id, viewmodel.attach, _siteUrl);
                }
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }
            var entitiy = new AssetTransferVM();
            entitiy = viewmodel;
            return SPConnector.GetLatestListItemID("Asset Transfer", _siteUrl);
        }

        public void CreateDocuments(int? headerID, IEnumerable<HttpPostedFileBase> documents, string SiteUrl)
        {
            throw new NotImplementedException();
        }

        public bool UpdateHeader(AssetTransferVM viewmodel, string SiteUrl)
        {
            viewmodel.CancelURL = _siteUrl + UrlResource.AssetTransfer;
            var columnValues = new Dictionary<string, object>();
            var ID = Convert.ToInt32(viewmodel.ID);
            var oldData = SPConnector.GetListItem("Asset Transfer", ID, SiteUrl);
            //columnValues.add
            columnValues.Add("Title", "Asset Transfer");
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
                var breaks1 = viewmodel.AssetHolderTo.Value.Split('-');
                var getInfo = GetProfMasterInfo(breaks[0], SiteUrl);
                var getInfo1 = GetProfMasterInfo(breaks1[0], SiteUrl);
                if (getInfo != null && getInfo1 != null)
                {
                    columnValues.Add("assetholderfrom", new FieldLookupValue { LookupId = Convert.ToInt32(getInfo.ID) });
                    columnValues.Add("positionfrom", breaks[1]);
                    columnValues.Add("projectunitfrom", getInfo.CurrentPosition.Text);
                    columnValues.Add("contactnumberfrom", getInfo.MobileNumberOne);

                    columnValues.Add("assetholderto", new FieldLookupValue { LookupId = Convert.ToInt32(getInfo1.ID) });
                    columnValues.Add("positionto", breaks1[1]);
                    columnValues.Add("projectunitto", getInfo1.CurrentPosition.Text);
                    columnValues.Add("contactnumberto", getInfo1.MobileNumberOne);
                }
            }

            columnValues.Add("completionstatus", viewmodel.CompletionStatus.Value);

            try
            {
                SPConnector.UpdateListItem("Asset Transfer", ID, columnValues, _siteUrl);
                if (viewmodel.CompletionStatus.Value == "Complete" && (viewmodel.attach.FileName != "" || viewmodel.attach.FileName != null))
                {
                    SPConnector.AttachFile("Asset Transfer", ID, viewmodel.attach, _siteUrl);
                }
                var newData = SPConnector.GetListItem("Asset Transfer", ID, _siteUrl);
                if (viewmodel.CompletionStatus.Value == "Complete")
                {
                    if (Convert.ToBoolean(newData["Attachments"]) == false)
                    {
                        var oldcolumnValues = new Dictionary<string, object>();
                        oldcolumnValues.Add("Title", oldData["Title"]);
                        oldcolumnValues.Add("transferdate", oldData["transferdate"]);
                        oldcolumnValues.Add("assetholderfrom", oldData["assetholderfrom"]);
                        oldcolumnValues.Add("positionfrom", oldData["positionfrom"]);
                        oldcolumnValues.Add("projectunitfrom", oldData["projectunitfrom"]);
                        oldcolumnValues.Add("contactnumberfrom", oldData["contactnumberfrom"]);
                        oldcolumnValues.Add("assetholderto", oldData["assetholderto"]);
                        oldcolumnValues.Add("positionto", oldData["positionto"]);
                        oldcolumnValues.Add("projectunitto", oldData["projectunitto"]);
                        oldcolumnValues.Add("contactnumberto", oldData["contactnumberto"]);

                        oldcolumnValues.Add("completionstatus", oldData["completionstatus"]);

                        SPConnector.UpdateListItem("Asset Transfer", ID, oldcolumnValues, _siteUrl);
                        return false;
                    }
                }
                else
                {
                    if (Convert.ToBoolean(newData["Attachments"]) == false)
                    {
                        var oldcolumnValues = new Dictionary<string, object>();
                        oldcolumnValues.Add("Title", oldData["Title"]);
                        oldcolumnValues.Add("transferdate", oldData["transferdate"]);
                        oldcolumnValues.Add("assetholderfrom", oldData["assetholderfrom"]);
                        oldcolumnValues.Add("positionfrom", oldData["positionfrom"]);
                        oldcolumnValues.Add("projectunitfrom", oldData["projectunitfrom"]);
                        oldcolumnValues.Add("contactnumberfrom", oldData["contactnumberfrom"]);
                        oldcolumnValues.Add("assetholderto", oldData["assetholderto"]);
                        oldcolumnValues.Add("positionto", oldData["positionto"]);
                        oldcolumnValues.Add("projectunitto", oldData["projectunitto"]);
                        oldcolumnValues.Add("contactnumberto", oldData["contactnumberto"]);

                        oldcolumnValues.Add("completionstatus", oldData["completionstatus"]);

                        SPConnector.UpdateListItem("Asset Transfer", ID, oldcolumnValues, _siteUrl);
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }
            var entitiy = new AssetTransferVM();
            entitiy = viewmodel;
            return true;
        }

        public ProfessionalDataVM GetProfMasterInfo(string fullname, string SiteUrl)
        {
            var caml = @"<View><Query>
                       <Where>
                          <Contains>
                             <FieldRef Name='Title' />
                             <Value Type='Text'>" + fullname + @"</Value>
                          </Contains>
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

        public IEnumerable<AssignmentOfAssetDetailsVM> GetAssetSubAsset()
        {
            var models = new List<AssignmentOfAssetDetailsVM>();
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
            foreach (var item in SPConnector.GetList("Asset Assignment Detail", _siteUrl, caml))
            {
                models.Add(ConvertToModelAssetSubAsset(item));
            }

            return models;
        }

        private AssignmentOfAssetDetailsVM ConvertToModelAssetSubAsset(ListItem item)
        {
            var viewModel = new AssignmentOfAssetDetailsVM();

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

        public IEnumerable<Model.ViewModel.Form.Asset.LocationMasterVM> GetProvince()
        {
            var models = new List<Model.ViewModel.Form.Asset.LocationMasterVM>();
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
                        <FieldRef Name='city' />
                        <FieldRef Name='Floor' />
                        <FieldRef Name='Room' />
                        <FieldRef Name='Remarks' />
                    </ViewFields>
                    <QueryOptions /></View>";
            List<string> listProvince = new List<string>();
            foreach (var item in SPConnector.GetList("Location Master", _siteUrl, caml))
            {
                models.Add(ConvertToProvince(item));
            }

            return models;
        }

        private Model.ViewModel.Form.Asset.LocationMasterVM ConvertToProvince(ListItem item)
        {
            var viewModel = new Model.ViewModel.Form.Asset.LocationMasterVM();

            viewModel.ID = Convert.ToInt32(item["ID"]);
            var province = "";
            if ((item["Province"] as FieldLookupValue) != null)
            {
                province = Convert.ToString(item["city"]) + "," + (item["Province"] as FieldLookupValue).LookupValue;
            }
            viewModel.Province.Text = province;
            viewModel.OfficeName = Convert.ToString(item["Title"]);
            viewModel.FloorName = Convert.ToInt32(item["Floor"]);
            viewModel.RoomName = Convert.ToString(item["Room"]);
            viewModel.Remarks = Convert.ToString(item["Remarks"]);
            return viewModel;
        }

        public Model.ViewModel.Form.Asset.LocationMasterVM GetProvinceInfo(string province, string SiteUrl)
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
            var viewmodel = new Model.ViewModel.Form.Asset.LocationMasterVM();
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

        public void CreateDetails(int? headerID, IEnumerable<AssetTransferDetailVM> items)
        {
            foreach (var item in items)
            {
                if (Item.CheckIfSkipped(item)) continue;

                if (Item.CheckIfDeleted(item))
                {
                    try
                    {
                        SPConnector.DeleteListItem("Asset Transfer Detail", item.ID, _siteUrl);
                    }
                    catch (Exception e)
                    {
                        logger.Error(e);
                        throw e;
                    }
                    continue;
                }

                var updatedValues = new Dictionary<string, object>();
                updatedValues.Add("assettransfer", new FieldLookupValue { LookupId = Convert.ToInt32(headerID) });
                var getAssetID = SPConnector.GetListItem("Asset Assignment Detail", item.AssetSubAsset.Value.Value, _siteUrl);
                var provinceinfo = SPConnector.GetListItem("Location Master", item.Province.Value.Value, _siteUrl);
                var provinceinfo1 = SPConnector.GetListItem("Location Master", item.ProvinceTo.Value.Value, _siteUrl);
                if ((getAssetID["assetsubasset"] as FieldLookupValue) != null)
                {
                    updatedValues.Add("assetsubasset", (getAssetID["assetsubasset"] as FieldLookupValue).LookupId);
                }

                if ((provinceinfo["Province"] as FieldLookupValue) != null && (provinceinfo1["Province"] as FieldLookupValue) != null)
                {
                    updatedValues.Add("provincefrom", (provinceinfo["Province"] as FieldLookupValue).LookupId);
                    updatedValues.Add("provinceto", (provinceinfo1["Province"] as FieldLookupValue).LookupId);
                }
                updatedValues.Add("cityfrom", provinceinfo["city"]);
                updatedValues.Add("officefrom", provinceinfo["Title"]);
                updatedValues.Add("floorfrom", provinceinfo["Floor"]);
                updatedValues.Add("roomfrom", provinceinfo["Room"]);

                updatedValues.Add("cityto", provinceinfo1["city"]);
                updatedValues.Add("officeto", provinceinfo1["Title"]);
                updatedValues.Add("floorto", provinceinfo1["Floor"]);
                updatedValues.Add("roomto", provinceinfo1["Room"]);

                updatedValues.Add("Title", item.Remarks);
                updatedValues.Add("status", "RUNNING");
                try
                {
                    SPConnector.AddListItem("Asset Transfer Detail", updatedValues, _siteUrl);
                }
                catch (Exception e)
                {
                    logger.Error(e);
                    throw new Exception(ErrorResource.SPInsertError);
                }
            }
        }

        public IEnumerable<Model.ViewModel.Form.Asset.LocationMasterVM> GetOfficeName(string SiteUrl, string province)
        {
            var model = new List<Model.ViewModel.Form.Asset.LocationMasterVM>();
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

        private Model.ViewModel.Form.Asset.LocationMasterVM ConvertToOfficeName(ListItem item)
        {
            var viewmodel = new Model.ViewModel.Form.Asset.LocationMasterVM();

            viewmodel.ID = Convert.ToInt32(item["ID"]);
            viewmodel.OfficeName = Convert.ToString(item["Title"]);

            return viewmodel;

        }

        IEnumerable<AssetTransferDetailVM> IAssetTransferService.GetDetails(int? headerID)
        {
			var caml = @"<View><Query><Where><Eq><FieldRef Name='assettransfer' /><Value Type='Lookup'>" + headerID.ToString() + "</Value></Eq></Where></Query></View>";
            var details = new List<AssetTransferDetailVM>();

            foreach (var item in SPConnector.GetList("Asset Transfer Detail", _siteUrl, caml))
            {
                details.Add(ConvertToDetails(item));
            }

            return details;
        }

        IEnumerable<AssetTransferDetailVM> IAssetTransferService.GetDetailsPrint(int? headerID)
        {
            var caml = @"<View><Query><Where><Eq><FieldRef Name='assettransfer' /><Value Type='Lookup'>" + headerID.ToString() + "</Value></Eq></Where></Query></View>";
            var details = new List<AssetTransferDetailVM>();
            List<string> listAssetID = new List<string>();

            foreach (var item in SPConnector.GetList("Asset Transfer Detail", _siteUrl, caml))
            {
                if (listAssetID.Count != 0 && listAssetID.Contains((item["assetsubasset"] as FieldLookupValue).LookupValue))
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

            foreach (var d in details)
            {
                var quantityPerItem = 0;
                foreach (var l in listAssetID)
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

        private AssetTransferDetailVM ConvertToDetails(ListItem item)
        {
            var ListAssetSubAsset = SPConnector.GetListItem("Asset Master", (item["assetsubasset"] as FieldLookupValue).LookupId, _siteUrl);

            AjaxComboBoxVM _assetSubAsset = new AjaxComboBoxVM();
            _assetSubAsset.Value = (item["assetsubasset"] as FieldLookupValue).LookupId;
            _assetSubAsset.Text = Convert.ToString(ListAssetSubAsset["AssetID"]) + " - " + Convert.ToString(ListAssetSubAsset["Title"]);

            AjaxComboBoxVM _provincefrom = new AjaxComboBoxVM();
            AjaxComboBoxVM _provinceto = new AjaxComboBoxVM();
            var _officefrom = "";
            var _floorfrom = "";
            var _roomfrom = "";
            var _officeto = "";
            var _floorto = "";
            var _roomto = "";


            var provincefrom = (item["provincefrom"] as FieldLookupValue).LookupId;
            var provinceto = (item["provinceto"] as FieldLookupValue).LookupId;
            var infoitem = SPConnector.GetListItem("Asset Transfer Detail", Convert.ToInt32(item["ID"]), _siteUrl);
            var camlfrom = @"<View><Query>
                               <Where>
                                  <And>
                                     <Eq>
                                        <FieldRef Name='Province_x003a_ID' />
                                        <Value Type='Lookup'>" + provincefrom + @"</Value>
                                     </Eq>
                                     <And>
                                        <Eq>
                                           <FieldRef Name='city' />
                                           <Value Type='Text'>" + infoitem["cityfrom"] + @"</Value>
                                        </Eq>
                                        <And>
                                           <Eq>
                                              <FieldRef Name='Title' />
                                              <Value Type='Text'>" + infoitem["officefrom"] + @"</Value>
                                           </Eq>
                                           <And>
                                              <Eq>
                                                 <FieldRef Name='Floor' />
                                                 <Value Type='Text'>" + infoitem["floorfrom"] + @"</Value>
                                              </Eq>
                                              <Eq>
                                                 <FieldRef Name='Room' />
                                                 <Value Type='Text'>" + infoitem["roomfrom"] + @"</Value>
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
                               <FieldRef Name='Floor' />
                               <FieldRef Name='Room' />
                               <FieldRef Name='city' />
                            </ViewFields>
                            <QueryOptions /></View>";
            var camlto = @"<View><Query>
                           <Where>
                              <And>
                                 <Eq>
                                    <FieldRef Name='Province_x003a_ID' />
                                    <Value Type='Lookup'>" + provinceto + @"</Value>
                                 </Eq>
                                 <And>
                                    <Eq>
                                       <FieldRef Name='city' />
                                       <Value Type='Text'>" + infoitem["cityto"] + @"</Value>
                                    </Eq>
                                    <And>
                                       <Eq>
                                          <FieldRef Name='Title' />
                                          <Value Type='Text'>" + infoitem["officeto"] + @"</Value>
                                       </Eq>
                                       <And>
                                          <Eq>
                                             <FieldRef Name='Floor' />
                                             <Value Type='Text'>" + infoitem["floorto"] + @"</Value>
                                          </Eq>
                                          <Eq>
                                             <FieldRef Name='Room' />
                                             <Value Type='Text'>" + infoitem["roomto"] + @"</Value>
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
                           <FieldRef Name='Floor' />
                           <FieldRef Name='Room' />
                           <FieldRef Name='city' />
                        </ViewFields>
                        <QueryOptions /></View>";
            var ListProvince = SPConnector.GetList("Location Master", _siteUrl, camlfrom);
            var ListProvince1 = SPConnector.GetList("Location Master", _siteUrl, camlto);
            AjaxComboBoxVM _province = new AjaxComboBoxVM();
            for (int j = 0; j < ListProvince.Count; j++)
            {
                if (Convert.ToString(ListProvince[j]["Title"]) == Convert.ToString(item["officefrom"]) && Convert.ToString(ListProvince[j]["Floor"]) == Convert.ToString(item["floorfrom"]) && Convert.ToString(ListProvince[j]["Room"]) == Convert.ToString(item["roomfrom"]))
                {
                    _provincefrom.Value = (item["provincefrom"] as FieldLookupValue).LookupId;
                    _provincefrom.Text = Convert.ToString(ListProvince[j]["city"]) + "," + (ListProvince[j]["Province"] as FieldLookupValue).LookupValue + "-" + ListProvince[j]["Title"] + "-" + ListProvince[j]["Floor"] + "-" + ListProvince[j]["Room"];
                    _officefrom = Convert.ToString(ListProvince[j]["Title"]);
                    _floorfrom = Convert.ToString(ListProvince[j]["Floor"]);
                    _roomfrom = Convert.ToString(ListProvince[j]["Room"]);
                }

                if (Convert.ToString(ListProvince1[j]["Title"]) == Convert.ToString(item["officeto"]) && Convert.ToString(ListProvince1[j]["Floor"]) == Convert.ToString(item["floorto"]) && Convert.ToString(ListProvince1[j]["Room"]) == Convert.ToString(item["roomto"]))
                {
                    _provinceto.Value = (item["provinceto"] as FieldLookupValue).LookupId;
                    _provinceto.Text = Convert.ToString(ListProvince1[j]["city"]) + "," + (ListProvince1[j]["Province"] as FieldLookupValue).LookupValue + "-" + ListProvince1[j]["Title"] + "-" + ListProvince1[j]["Floor"] + "-" + ListProvince1[j]["Room"];
                    _officeto = Convert.ToString(ListProvince1[j]["Title"]);
                    _floorto = Convert.ToString(ListProvince1[j]["Floor"]);
                    _roomto = Convert.ToString(ListProvince1[j]["Room"]);
                }
            }

            return new AssetTransferDetailVM
            {
                ID = Convert.ToInt32(item["ID"]),
                textasset = Convert.ToString(ListAssetSubAsset["AssetID"]),
                description = Convert.ToString(ListAssetSubAsset["Title"]),
                quantity = 1,
                AssetSubAsset = AssetTransferDetailVM.GetAssetSubAssetDefaultValue(_assetSubAsset),
                Province = AssetTransferDetailVM.GetProvinceDefaultValue(_provincefrom),
                OfficeName = _officefrom,
                Floor = _floorfrom,
                Room = _roomfrom,
                ProvinceTo = AssetTransferDetailVM.GetProvinceDefaultValue(_provinceto),
                OfficeNameTo = _officeto,
                FloorTo = _floorto,
                RoomTo = _roomto,
                Remarks = Convert.ToString(item["Title"]),
                Status = Convert.ToString(item["status"])
            };
        }

        public void UpdateDetails(int? headerID, IEnumerable<AssetTransferDetailVM> items)
        {
            foreach (var item in items)
            {
                if (Item.CheckIfSkipped(item)) continue;

                if (Item.CheckIfDeleted(item))
                {
                    try
                    {
                        SPConnector.DeleteListItem("Asset Transfer Detail", item.ID, _siteUrl);
                    }
                    catch (Exception e)
                    {
                        logger.Error(e);
                        throw e;
                    }
                    continue;
                }

                var updatedValues = new Dictionary<string, object>();
                updatedValues.Add("assettransfer", new FieldLookupValue { LookupId = Convert.ToInt32(headerID) });
                var infoitem = SPConnector.GetListItem("Asset Transfer Detail", Convert.ToInt32(item.ID), _siteUrl);
                var camlfrom = @"<View><Query>
                               <Where>
                                  <And>
                                     <Eq>
                                        <FieldRef Name='Province_x003a_ID' />
                                        <Value Type='Lookup'>" + item.Province.Value.Value + @"</Value>
                                     </Eq>
                                     <And>
                                        <Eq>
                                           <FieldRef Name='city' />
                                           <Value Type='Text'>" + infoitem["cityfrom"] + @"</Value>
                                        </Eq>
                                        <And>
                                           <Eq>
                                              <FieldRef Name='Title' />
                                              <Value Type='Text'>" + infoitem["officefrom"] + @"</Value>
                                           </Eq>
                                           <And>
                                              <Eq>
                                                 <FieldRef Name='Floor' />
                                                 <Value Type='Text'>" + infoitem["floorfrom"] + @"</Value>
                                              </Eq>
                                              <Eq>
                                                 <FieldRef Name='Room' />
                                                 <Value Type='Text'>" + infoitem["roomfrom"] + @"</Value>
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
                               <FieldRef Name='Floor' />
                               <FieldRef Name='Room' />
                               <FieldRef Name='city' />
                            </ViewFields>
                            <QueryOptions /></View>";
                var camlto = @"<View><Query>
                           <Where>
                              <And>
                                 <Eq>
                                    <FieldRef Name='Province_x003a_ID' />
                                    <Value Type='Lookup'>" + item.ProvinceTo.Value.Value + @"</Value>
                                 </Eq>
                                 <And>
                                    <Eq>
                                       <FieldRef Name='city' />
                                       <Value Type='Text'>" + infoitem["cityto"] + @"</Value>
                                    </Eq>
                                    <And>
                                       <Eq>
                                          <FieldRef Name='Title' />
                                          <Value Type='Text'>" + infoitem["officeto"] + @"</Value>
                                       </Eq>
                                       <And>
                                          <Eq>
                                             <FieldRef Name='Floor' />
                                             <Value Type='Text'>" + infoitem["floorto"] + @"</Value>
                                          </Eq>
                                          <Eq>
                                             <FieldRef Name='Room' />
                                             <Value Type='Text'>" + infoitem["roomto"] + @"</Value>
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
                           <FieldRef Name='Floor' />
                           <FieldRef Name='Room' />
                           <FieldRef Name='city' />
                        </ViewFields>
                        <QueryOptions /></View>";
                //var getAssetID = SPConnector.GetListItem("Asset Assignment Detail", item.AssetSubAsset.Value.Value, _siteUrl);
                var provinceinfo = SPConnector.GetList("Location Master", _siteUrl, camlfrom);
                var provinceinfo1 = SPConnector.GetList("Location Master", _siteUrl, camlto);

                //if provinceinfo or provinceinfo1 null, means there is changes in province which Province / ProvinceTo ID is from Location Master
                if (provinceinfo.Count == 0)
                {
                    camlfrom = @"<View><Query>
                       <Where>
                          <Eq>
                            <FieldRef Name='ID' />
                             <Value Type='Counter'>" + item.Province.Value.Value + @"</Value>
                          </Eq>
                       </Where>
                    </Query>
                    <ViewFields>
                       <FieldRef Name='Province' />
                       <FieldRef Name='Title' />
                       <FieldRef Name='Floor' />
                       <FieldRef Name='Room' />
                       <FieldRef Name='city' />
                    </ViewFields>
                    <QueryOptions /></View>";

                    provinceinfo = SPConnector.GetList("Location Master", _siteUrl, camlfrom);

                }

                if (provinceinfo1.Count == 0)
                {
                    camlto = @"<View><Query>
                       <Where>
                          <Eq>
                                <FieldRef Name='ID' />
                             <Value Type='Counter'>" + item.ProvinceTo.Value.Value + @"</Value>
                          </Eq>
                       </Where>
                    </Query>
                    <ViewFields>
                       <FieldRef Name='Province' />
                       <FieldRef Name='Title' />
                       <FieldRef Name='Floor' />
                       <FieldRef Name='Room' />
                       <FieldRef Name='city' />
                    </ViewFields>
                    <QueryOptions /></View>";
                    provinceinfo1 = SPConnector.GetList("Location Master", _siteUrl, camlto);
                }

                updatedValues.Add("assetsubasset", item.AssetSubAsset.Value.Value);
                for (int i = 0; i < provinceinfo1.Count; i++)
                {
                    updatedValues.Add("provincefrom", (provinceinfo[i]["Province"] as FieldLookupValue).LookupId);
                    updatedValues.Add("cityfrom", provinceinfo[i]["city"]);
                    updatedValues.Add("officefrom", provinceinfo[i]["Title"]);
                    updatedValues.Add("floorfrom", provinceinfo[i]["Floor"]);
                    updatedValues.Add("roomfrom", provinceinfo[i]["Room"]);

                    updatedValues.Add("provinceto", (provinceinfo1[i]["Province"] as FieldLookupValue).LookupId);
                    updatedValues.Add("cityto", provinceinfo1[i]["city"]);
                    updatedValues.Add("officeto", provinceinfo1[i]["Title"]);
                    updatedValues.Add("floorto", provinceinfo1[i]["Floor"]);
                    updatedValues.Add("roomto", provinceinfo1[i]["Room"]);
                }

                updatedValues.Add("Title", item.Remarks);
                updatedValues.Add("status", "RUNNING");
                try
                {
                    if (Item.CheckIfUpdated(item))
                        SPConnector.UpdateListItem("Asset Transfer Detail", item.ID, updatedValues, _siteUrl);
                    else
                        SPConnector.AddListItem("Asset Transfer Detail", updatedValues, _siteUrl);
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
            if (CSVDataTable.Columns.Count == 4)
            {
                foreach (DataRow d in CSVDataTable.Rows)
                {
                    //header
                    var model = new AssetTransferVM();
                    model.AssetHolder.Text = Convert.ToString(d.ItemArray[2]);
                    model.AssetHolderTo.Text = Convert.ToString(d.ItemArray[3]);
                    var additionalDateTime = d.ItemArray[1] + " 00:00:00";
                    DateTime date;
                    model.Date = DateTime.TryParse(d.ItemArray[1].ToString(), out date) ? date : (DateTime?)null;

                    latest = CreateHeader(model, SiteUrl, "upload");
                }

            }
            else
            {
                foreach (DataRow d in CSVDataTable.Rows)
                {
                    //detail
                    var model = new AssetTransferDetailVM();

                    model.AssetSubAsset.Text = Convert.ToString(d.ItemArray[1]);
                    model.Province.Text = Convert.ToString(d.ItemArray[2]);
                    model.OfficeName = Convert.ToString(d.ItemArray[3]);
                    model.Floor = Convert.ToString(d.ItemArray[4]);
                    model.Room = Convert.ToString(d.ItemArray[5]);

                    model.ProvinceTo.Text = Convert.ToString(d.ItemArray[6]);
                    model.OfficeNameTo = Convert.ToString(d.ItemArray[7]);
                    model.FloorTo = Convert.ToString(d.ItemArray[8]);
                    model.RoomTo = Convert.ToString(d.ItemArray[9]);

                    model.Remarks = Convert.ToString(d.ItemArray[10]);

                    CreateDetails(Convert.ToInt32(d.ItemArray[0]), model, SiteUrl);
                    latest = SPConnector.GetLatestListItemID("Asset Transfer Detail", SiteUrl);
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

        public IEnumerable<Model.ViewModel.Form.Asset.LocationMasterVM> GetFloorList(string SiteUrl, string office = null)
        {
            var model = new List<Model.ViewModel.Form.Asset.LocationMasterVM>();
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

            List<int> listFloor = new List<int>();
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

        private Model.ViewModel.Form.Asset.LocationMasterVM ConvertToFloorList(ListItem item)
        {
            var viewmodel = new Model.ViewModel.Form.Asset.LocationMasterVM();
            viewmodel.OfficeName = Convert.ToString(item["Floor"]);

            return viewmodel;

        }

        public IEnumerable<Model.ViewModel.Form.Asset.LocationMasterVM> GetRoomList(string SiteUrl, string floor = null)
        {
            var model = new List<Model.ViewModel.Form.Asset.LocationMasterVM>();
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
                if (Convert.ToString(item["Room"]) != null)
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

        private Model.ViewModel.Form.Asset.LocationMasterVM ConvertToRoomList(ListItem item)
        {
            var viewmodel = new Model.ViewModel.Form.Asset.LocationMasterVM();

            //viewmodel.ID = Convert.ToInt32(item["ID"]);
            viewmodel.RoomName = Convert.ToString(item["Room"]);

            return viewmodel;

        }

        public void CreateDetails(int? headerID, AssetTransferDetailVM item, string SiteUrl)
        {
            var updatedValues = new Dictionary<string, object>();
            updatedValues.Add("assettransfer", new FieldLookupValue { LookupId = Convert.ToInt32(headerID) });
            var asset = @"<View>
                        <Query>
                           <Where>
                              <Eq>
                                 <FieldRef Name='assetsubasset' />
                                 <Value Type='Lookup'>" + item.AssetSubAsset.Text + @"</Value>
                              </Eq>
                           </Where>
                        </Query>
                        <ViewFields>
                           <FieldRef Name='assetsubasset' />
                           <FieldRef Name='assetsubasset_x003a_ID' />
                        </ViewFields>
                        <QueryOptions /></View>";
            var breakfrom = item.Province.Text.Split('-');
            var breakto = item.ProvinceTo.Text.Split('-');
            var provinceFrom = @"<View><Query>
                               <Where>
                                  <And>
                                     <Eq>
                                        <FieldRef Name='Province' />
                                        <Value Type='Lookup'>" + breakfrom[1] + @"</Value>
                                     </Eq>
                                     <And>
                                        <Eq>
                                           <FieldRef Name='city' />
                                           <Value Type='Text'>" + breakfrom[0] + @"</Value>
                                        </Eq>
                                        <And>
                                           <Eq>
                                              <FieldRef Name='Title' />
                                              <Value Type='Text'>" + item.OfficeName + @"</Value>
                                           </Eq>
                                           <And>
                                              <Eq>
                                                 <FieldRef Name='Floor' />
                                                 <Value Type='Text'>" + item.Floor + @"</Value>
                                              </Eq>
                                              <Eq>
                                                 <FieldRef Name='Room' />
                                                 <Value Type='Text'>" + item.Room + @"</Value>
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
                               <FieldRef Name='Floor' />
                               <FieldRef Name='Room' />
                               <FieldRef Name='city' />
                            </ViewFields>
                            <QueryOptions /></View>";
            var provinceTo = @"<View><Query>
                               <Where>
                                  <And>
                                     <Eq>
                                        <FieldRef Name='Province' />
                                        <Value Type='Lookup'>" + breakto[1] + @"</Value>
                                     </Eq>
                                     <And>
                                        <Eq>
                                           <FieldRef Name='city' />
                                           <Value Type='Text'>" + breakto[0] + @"</Value>
                                        </Eq>
                                        <And>
                                           <Eq>
                                              <FieldRef Name='Title' />
                                              <Value Type='Text'>" + item.OfficeNameTo + @"</Value>
                                           </Eq>
                                           <And>
                                              <Eq>
                                                 <FieldRef Name='Floor' />
                                                 <Value Type='Text'>" + item.FloorTo + @"</Value>
                                              </Eq>
                                              <Eq>
                                                 <FieldRef Name='Room' />
                                                 <Value Type='Text'>" + item.RoomTo + @"</Value>
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
                               <FieldRef Name='Floor' />
                               <FieldRef Name='Room' />
                               <FieldRef Name='city' />
                            </ViewFields>
                            <QueryOptions /></View>";
            var getAssetID = SPConnector.GetList("Asset Assignment Detail", _siteUrl, asset);
            var provinceinfo = SPConnector.GetList("Location Master", _siteUrl, provinceFrom);
            var provinceinfo1 = SPConnector.GetList("Location Master", _siteUrl, provinceFrom);
            for (int i = 0; i < getAssetID.Count; i++)
            {
                updatedValues.Add("assetsubasset", (getAssetID[i]["assetsubasset"] as FieldLookupValue).LookupId);

                updatedValues.Add("provincefrom", (provinceinfo[i]["Province"] as FieldLookupValue).LookupId);
                updatedValues.Add("provinceto", (provinceinfo1[i]["Province"] as FieldLookupValue).LookupId);

                updatedValues.Add("cityfrom", provinceinfo[i]["city"]);
                updatedValues.Add("officefrom", provinceinfo[i]["Title"]);
                updatedValues.Add("floorfrom", provinceinfo[i]["Floor"]);
                updatedValues.Add("roomfrom", provinceinfo[i]["Room"]);

                updatedValues.Add("cityto", provinceinfo1[i]["city"]);
                updatedValues.Add("officeto", provinceinfo1[i]["Title"]);
                updatedValues.Add("floorto", provinceinfo1[i]["Floor"]);
                updatedValues.Add("roomto", provinceinfo1[i]["Room"]);
            }
            updatedValues.Add("Title", item.Remarks);
            updatedValues.Add("status", "RUNNING");
            try
            {
                SPConnector.AddListItem("Asset Transfer Detail", updatedValues, _siteUrl);
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
            var lists = SPConnector.GetList("Asset Transfer", SiteUrl);
            foreach (var l in lists)
            {
                var camlfrom = @"<View><Query>
                           <Where>
                              <Eq>
                                 <FieldRef Name='ID' />
                                 <Value Type='Lookup'>" + (l["assetholderfrom"] as FieldLookupValue).LookupId + @"</Value>
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

                var camlto = @"<View><Query>
                           <Where>
                              <Eq>
                                 <FieldRef Name='ID' />
                                 <Value Type='Lookup'>" + (l["assetholderto"] as FieldLookupValue).LookupId + @"</Value>
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

                var getFromProfMas = SPConnector.GetList("Professional Master", sitehr, camlfrom);
                var getFromProfMas1 = SPConnector.GetList("Professional Master", sitehr, camlto);
                for (int i = 0; i < getFromProfMas.Count; i++)
                {
                    var model = new Dictionary<string, object>();
                    model.Add("projectunitfrom", Convert.ToString(getFromProfMas[i]["Project_x002f_Unit"]));
                    model.Add("contactnumberfrom", Convert.ToString(getFromProfMas[i]["mobilephonenr"]));
                    model.Add("positionfrom", (getFromProfMas[i]["Position"] as FieldLookupValue).LookupValue);

                    model.Add("projectunitto", Convert.ToString(getFromProfMas1[i]["Project_x002f_Unit"]));
                    model.Add("contactnumberto", Convert.ToString(getFromProfMas1[i]["mobilephonenr"]));
                    model.Add("positionto", (getFromProfMas1[i]["Position"] as FieldLookupValue).LookupValue);

                }
            }

            return true;
        }
    }
}
