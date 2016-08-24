﻿using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MCAWebAndAPI.Model.ViewModel.Control;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using MCAWebAndAPI.Service.Asset;
using MCAWebAndAPI.Service.Converter;
using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MCAWebAndAPI.Web.Controllers
{
    public class ASSAssignmentOfAssetController : Controller
    {

        IAssignmentOfAssetService _service;

        public ASSAssignmentOfAssetController()
        {
            _service = new AssignmentOfAssetService();
        }

        // GET: AssignmentOfAsset
        public ActionResult Create(string SiteUrl)
        {
            _service.SetSiteUrl(SiteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set("SiteUrl", SiteUrl ?? ConfigResource.DefaultBOSiteUrl);
            var viewModel = _service.GetPopulatedModel(SiteUrl);
            return View(viewModel);
        }

        public ActionResult Edit(int ID, string SiteUrl)
        {
            _service.SetSiteUrl(SiteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set("SiteUrl", SiteUrl ?? ConfigResource.DefaultBOSiteUrl);

            var viewModel = _service.GetHeader(ID, SiteUrl);

            int? headerID = null;
            headerID = viewModel.ID;

            try
            {
                var viewdetails = _service.GetDetails(headerID);
                viewModel.Details = viewdetails;
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Submit(AssignmentOfAssetVM _data, string siteUrl)
        {
            siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            //return View(new AssetMasterVM());
            int? headerID = null;
            try
            {
                headerID = _service.CreateHeader(_data, siteUrl);
                //_service.CreateDocuments(headerID, _data.Attachment, siteUrl);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            try
            {
                _service.CreateDetails(headerID, _data.Details);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }
            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.AssetAcquisition);
            //return Redirect(string.Format("{0}/{1}", siteUrl ?? ConfigResource.DefaultBOSiteUrl, UrlResource.AssetAcquisition));
        }

        [HttpPost]
        public ActionResult Update(AssignmentOfAssetVM _data, string SiteUrl)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            try
            {
                _service.UpdateHeader(_data, siteUrl);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            try
            {
                //update items
                _service.UpdateDetails(_data.ID, _data.Details);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.AssetAcquisition);
        }

        public ActionResult GetProfMasterInfo(string fullname, string position)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            var profmasterinfo = _service.GetProfMasterInfo(fullname, siteUrl);

            //var professionals = GetFromExistingSession();
            return Json(
                new
                {
                    profmasterinfo.CurrentPosition,
                    profmasterinfo.MobileNumberOne
                }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAssetSubSAssetGrid()
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            var positions = GetFromPositionsExistingSession();

            return Json(positions.Select(e =>
                new
                {
                    Value = Convert.ToString(e.ID),
                    Text = e.AssetSubAsset.Text
                }),
                JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<AssetAcquisitionItemVM> GetFromPositionsExistingSession()
        {
            //Get existing session variable
            var sessionVariable = System.Web.HttpContext.Current.Session["Asset%Asset%20Acquisition%20Details"] as IEnumerable<AssetAcquisitionItemVM>;
            var positions = sessionVariable ?? _service.GetAssetSubAsset();

            if (sessionVariable == null) // If no session variable is found
                System.Web.HttpContext.Current.Session["Asset%Asset%20Acquisition%20Details"] = positions;
            return positions;
        }

        public JsonResult GetProvinceGrid()
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            var positions = GetProvinceExistingSession();

            return Json(positions.Select(e =>
                new
                {
                    Value = Convert.ToString(e.ID),
                    Text = e.Province.Text
                }),
                JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<LocationMasterVM> GetProvinceExistingSession()
        {
            //Get existing session variable
            var sessionVariable = System.Web.HttpContext.Current.Session["Location%20Master"] as IEnumerable<LocationMasterVM>;
            var positions = sessionVariable ?? _service.GetProvince();

            if (sessionVariable == null) // If no session variable is found
                System.Web.HttpContext.Current.Session["Location%20Master"] = positions;
            return positions;
        }

        public ActionResult GetProvinceInfo(string province, string SiteUrl)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            var info = _service.GetProvinceInfo(province, siteUrl);

            //var professionals = GetFromExistingSession();
            return Json(
                new
                {
                    info.ID,
                    info.OfficeName,
                    info.FloorName,
                    info.RoomName,
                    info.Remarks
                }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOfficeNameLists(string province, string SiteUrl)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");

            var officeName = _service.GetOfficeName(province, siteUrl);
            officeName = officeName.OrderBy(e => e.Province);

            return Json(officeName.Select(e =>
                new
                {
                    e.ID
                }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOfficeGrid(string province = null)
        {
            SessionManager.Set("Province", province);
            var pro = SessionManager.Get<string>("Province");
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            var positions = _service.GetOfficeName(siteUrl, province);

            return Json(positions.Select(e =>
                new
                {
                    Value = Convert.ToString(e.ID),
                    Text = e.OfficeName
                }),
                JsonRequestBehavior.AllowGet);
        }

        ////////////////////////////////////UPLOAD
        [HttpGet]
        public ActionResult Upload(string siteUrl = null, string listName = null)
        {
            if (siteUrl == null || listName == null)
                return RedirectToAction("Index", "Error", new { errorMessage = "Parameter cannot be null" });


            SessionManager.Set("SiteUrl", siteUrl);

            var emptyTable = GenerateEmptyDataTable();
            SessionManager.Set("CSVDataTable", emptyTable);

            var viewModel = new CSVVM();
            viewModel.ListName = listName;
            viewModel.DataTable = emptyTable;

            return View(viewModel);
        }

        private DataTable GenerateEmptyDataTable()
        {
            DataTable table = new DataTable();

            var column = new DataColumn("ID", typeof(int));
            table.Columns.Add(column);
            table.PrimaryKey = new DataColumn[] { column };
            table.Columns.Add("Title", typeof(string));

            table.Rows.Add(0, string.Empty);
            return table;
        }

        //Upload
        public ActionResult Save(IEnumerable<HttpPostedFileBase> CSVFile)
        {
            StreamReader reader = null;
            // The Name of the Upload component is "files"
            if (CSVFile != null)
            {
                foreach (var file in CSVFile)
                {
                    // Some browsers send file names with full path.
                    // We are only interested in the file name.
                    var fileName = Path.GetFileName(file.FileName);
                    var inputStream = file.InputStream;

                    var physicalPath = Path.Combine(Server.MapPath("~/App_Data"), fileName);

                    using (reader = new StreamReader(inputStream))
                    {
                        var CSVDataTable = CSVConverter.Instance.ToDataTable(reader);
                        Session.Add("CSVDataTable", CSVDataTable);
                    }
                }
            }

            // Return an empty string to signify success
            return Content("");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public JsonResult Grid_Read([DataSourceRequest] DataSourceRequest request)
        {
            // Get from existing session variable or create new if doesn't exist
            var dataTable = SessionManager.Get<DataTable>("CSVDataTable") ?? GenerateEmptyDataTable();

            if (request.Aggregates.Any())
            {
                request.Aggregates.Each(agg =>
                {
                    agg.Aggregates.Each(a =>
                    {
                        a.MemberType = dataTable.Columns[agg.Member].DataType;
                    });
                });
            }

            // Convert to Kendo DataSource
            DataSourceResult result = dataTable.ToDataSourceResult(request);

            // Convert to Json
            var json = Json(result, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;
            return json;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Grid_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]DataTable viewModel)
        {
            // Get existing session variable if any otherwise create new object
            var sessionVariables = SessionManager.Get<DataTable>("CSVDataTable") ?? new DataTable();
            foreach (var item in viewModel.AsEnumerable())
            {
                // Store in session variable
                sessionVariables.Rows.Add(item);
            }

            // Overwrite existing session variable
            SessionManager.Set("CSVDataTable", sessionVariables);

            // Return JSON
            DataSourceResult result = sessionVariables.ToDataSourceResult(request);
            var json = Json(result, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;
            return json;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Grid_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]DataTable viewModel)
        {
            // Get existing session variable
            var sessionVariables = SessionManager.Get<DataTable>("CSVDataTable") ?? new DataTable();

            foreach (var item in viewModel.AsEnumerable())
            {
                var obj = sessionVariables.AsEnumerable().FirstOrDefault(e => e["ID"] == item["ID"]);
                obj = item;
            }

            // Overwrite existing session variable
            SessionManager.Set("CSVDataTable", sessionVariables);

            // Return JSON
            DataSourceResult result = sessionVariables.ToDataSourceResult(request);
            var json = Json(result, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;
            return json;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Grid_Destroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]DataTable viewModel)
        {
            // Get existing session variable
            var sessionVariables = SessionManager.Get<DataTable>("CSVDataTable") ?? new DataTable();

            for (int i = sessionVariables.Rows.Count - 1; i >= 0; i--)
            {
                var row = sessionVariables.Rows[i];
                if (row["ID"] == viewModel.Rows[i]["ID"])
                    row.Delete();
            }

            // Overwrite existing session variable
            SessionManager.Set("CSVDataTable", sessionVariables);

            // Return JSON
            DataSourceResult result = sessionVariables.ToDataSourceResult(request);
            var json = Json(result, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;
            return json;
        }

        public ActionResult DisplayGrid()
        {
            return PartialView("_DisplayGrid");
        }

        //[HttpPost]
        public ActionResult SubmitUpload(string listName)
        {
            // Get existing session variable
            var sessionVariables = SessionManager.Get<DataTable>("CSVDataTable") ?? new DataTable();
            var siteUrl = SessionManager.Get<string>("SiteUrl");

            //cek apakah header / item
            int? latestIDHeader = 0;
            int? latestIDDetail = 0;
            List<int> idsHeader = new List<int>();
            List<int> idsDetail = new List<int>();
            var TableHeader = new DataTable();
            var TableDetail = new DataTable();

            var listNameHeader = "Asset Acquisition";
            var listNameHeaderMemo = "Acceptance Memo";
            var listNameDetail = "Asset Acquisition Details";
            var listAssetMaster = "Asset Master";
            var listWBSMaster = "WBS Master";
            foreach (DataRow d in SessionManager.Get<DataTable>("CSVDataTable").Rows)
            {
                if (d.ItemArray[0].ToString().Contains("Assignment of Asset"))
                {
                    try
                    {
                        TableHeader = new DataTable();
                        TableHeader.Columns.Add("Title", typeof(string));
                        TableHeader.Columns.Add("transferdate", typeof(DateTime));
                        TableHeader.Columns.Add("assetholder", typeof(string));
                        TableHeader.Columns.Add("position", typeof(string));
                        TableHeader.Columns.Add("projectunit", typeof(string));
                        TableHeader.Columns.Add("contactnumber", typeof(string));

                        //check assetholder in Professional Master HR
                        var caml = @"<View><Query>
                                   <Where>
                                      <Eq>
                                         <FieldRef Name='Title' />
                                         <Value Type='Text'>"+ d.ItemArray[1].ToString() + @"</Value>
                                      </Eq>
                                   </Where>
                                </Query>
                                <ViewFields>
                                     <FieldRef Name='Position' />
                                    <FieldRef Name='Title' />
                                    <FieldRef Name='mobilephonenr' />
                                    <FieldRef Name='Project_x002f_Unit' />
                                </ViewFields>
                                <QueryOptions /></View>";
                        var isAssetHolderExist = _service.isExist("Professional Master", caml, siteUrl);
                        if(isAssetHolderExist == true)
                        {
                            DataRow row = TableHeader.NewRow();
                            row["Title"] = d.ItemArray[0].ToString();

                            TableHeader.Rows.InsertAt(row, 0);

                            latestIDHeader = _service.MassUploadHeaderDetail(listNameHeader, TableHeader, siteUrl);
                            idsHeader.Add(Convert.ToInt32(latestIDHeader));
                        }
                        else
                        {
                            return JsonHelper.GenerateJsonErrorResponse("No Lookup Value/s is Found For Asset Holder!");
                        }
                    }
                    catch (Exception e)
                    {
                        if (idsHeader.Count > 0)
                        {
                            foreach (var id in idsHeader)
                            {
                                //delete parent
                                _service.RollbackParentChildrenUpload(listNameHeader, id, siteUrl);
                            }
                        }
                        else if (idsDetail.Count > 0)
                        {
                            foreach (var id in idsDetail)
                            {
                                //delete parent
                                _service.RollbackParentChildrenUpload(listNameDetail, id, siteUrl);
                            }

                        }
                        return JsonHelper.GenerateJsonErrorResponse("Invalid data, rolling back!");
                    }
                }

                //if (d.ItemArray[8].ToString() != "" && latestIDHeader != null)
                //{
                //    TableDetail = new DataTable();
                //    TableDetail.Columns.Add("assetacquisition", typeof(string));
                //    TableDetail.Columns.Add("polineitem", typeof(string));
                //    TableDetail.Columns.Add("assetsubasset", typeof(string));
                //    TableDetail.Columns.Add("wbs", typeof(string));
                //    TableDetail.Columns.Add("costidr", typeof(string));
                //    TableDetail.Columns.Add("costusd", typeof(string));
                //    TableDetail.Columns.Add("remarks", typeof(string));
                //    TableDetail.Columns.Add("status", typeof(string));

                //    DataRow row = TableDetail.NewRow();

                //    row["assetacquisition"] = latestIDHeader;
                //    row["polineitem"] = d.ItemArray[5].ToString();
                //    //cek if assetid ada pada table asset master
                //    //FXA-PC-OE-0001 - Laptop Lenovo
                //    var splitAssetID = d.ItemArray[8].ToString().Split('-');
                //    var resultAssetID = "";
                //    var resultDesc = "";
                //    var WBSDesc = "";
                //    if (splitAssetID.Length == 5)
                //    {
                //        resultAssetID = splitAssetID[0] + "-" + splitAssetID[1] + "-" + splitAssetID[2] + "-" + splitAssetID[3];
                //        resultDesc = splitAssetID[4];
                //        resultDesc = Regex.Replace(resultDesc, @"\t|\n|\r", "");
                //    }
                //    else
                //    {
                //        resultAssetID = splitAssetID[0] + "-" + splitAssetID[1] + "-" + splitAssetID[2] + "-" + splitAssetID[3] + "-" + splitAssetID[4];
                //        resultDesc = splitAssetID[5];
                //        resultDesc = Regex.Replace(resultDesc, @"\t|\n|\r", "");
                //    }
                //    var splitWBS = d.ItemArray[9].ToString().Split('-');
                //    WBSDesc = splitWBS[1] + "-" + splitWBS[2];
                //    WBSDesc = Regex.Replace(WBSDesc, @"\t|\n|\r", "");
                //    var camlAssetID = @"<View><Query><Where>
                //                            <Eq><FieldRef Name='AssetID' /><Value Type='Text'>" + resultAssetID.Trim() + @"</Value></Eq>
                //                            <And>
                //                            <Eq><FieldRef Name='Title' /><Value Type='Text'>" + resultDesc.Trim() + @"</Value></Eq>
                //                            </And>
                //                    </Where></Query></View>";
                //    var camlWBS = @"<View><Query><Where>
                //                            <Eq><FieldRef Name='Title' /><Value Type='Text'>" + splitWBS[0].Trim() + @"</Value></Eq>
                //                            <And>
                //                            <Eq><FieldRef Name='WBSDesc' /><Value Type='Text'>" + WBSDesc.Trim() + @"</Value></Eq>
                //                            </And>
                //                    </Where>
                //                    </Query></View>";
                //    try
                //    {
                //        int? idAssetIDExist = _assetAcquisitionService.getIdOfColumn("Asset Master", siteUrl, camlAssetID);
                //        int? idWBSExist = _assetAcquisitionService.getIdOfColumn("WBS Master", siteUrl, camlWBS);
                //        if (idAssetIDExist != 0 && idAssetIDExist != 0)
                //        {
                //            row["assetsubasset"] = idAssetIDExist;
                //            row["wbs"] = idWBSExist;
                //        }
                //        else
                //        {
                //            return JsonHelper.GenerateJsonErrorResponse("Invalid data, rolling back!");
                //        }

                //    }
                //    catch (Exception e)
                //    {
                //        if (idsHeader.Count > 0)
                //        {
                //            foreach (var id in idsHeader)
                //            {
                //                //delete parent
                //                _assetAcquisitionService.RollbackParentChildrenUpload(listNameHeader, id, siteUrl);
                //            }
                //        }
                //        else if (idsDetail.Count > 0)
                //        {
                //            foreach (var id in idsDetail)
                //            {
                //                //delete parent
                //                _assetAcquisitionService.RollbackParentChildrenUpload(listNameDetail, id, siteUrl);
                //            }

                //        }
                //        return JsonHelper.GenerateJsonErrorResponse("Invalid data, rolling back!");
                //    }
                //    //cek if wbs id ada pada table wbs master
                //    row["costidr"] = Convert.ToInt32(d.ItemArray[10]);
                //    row["costusd"] = Convert.ToInt32(d.ItemArray[11].ToString());
                //    row["remarks"] = d.ItemArray[12].ToString();
                //    row["status"] = d.ItemArray[13].ToString();

                //    TableDetail.Rows.InsertAt(row, 0);

                //    latestIDDetail = _assetAcquisitionService.MassUploadHeaderDetail(listNameDetail, TableDetail, siteUrl);
                //}
            }
            //return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.AssetAcquisition);
            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.AssetAcquisition);
        }

    }
}