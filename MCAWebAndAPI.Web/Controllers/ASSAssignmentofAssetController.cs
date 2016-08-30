using Elmah;
using Kendo.Mvc.Extensions;
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
                return JsonHelper.GenerateJsonErrorResponse("Failed To Get Data");
            }

            return View(viewModel);
        }

        public ActionResult View(int ID, string SiteUrl)
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
                return JsonHelper.GenerateJsonErrorResponse("Failed To Get Data");
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
                if(headerID == 0)
                {
                    return JsonHelper.GenerateJsonErrorResponse("Have To Attach File to Change Completion Status into Complete");
                }
                //_service.CreateDocuments(headerID, _data.Attachment, siteUrl);
            }
            catch (Exception e)
            {
                return JsonHelper.GenerateJsonErrorResponse("Failed To Save Header");
            }

            try
            {
                _service.CreateDetails(headerID, _data.Details);
            }
            catch (Exception e)
            {
                //rollback parent
                _service.RollbackParentChildrenUpload("Asset Assignment", headerID, siteUrl);
                return JsonHelper.GenerateJsonErrorResponse("Failed To Save Detail");
            }
            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.AssetAssignment);
        }

        [HttpPost]
        public ActionResult Update(AssignmentOfAssetVM _data, string SiteUrl)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            try
            {
                var update = _service.UpdateHeader(_data, siteUrl);
                if (update == false)
                {
                    return JsonHelper.GenerateJsonErrorResponse("Have To Attach File to Change Completion Status into Complete");
                }
            }
            catch (Exception e)
            {
                return JsonHelper.GenerateJsonErrorResponse("Failed To Update Header");
            }

            try
            {
                //update items
                _service.UpdateDetails(_data.ID, _data.Details);
            }
            catch (Exception e)
            {
                return JsonHelper.GenerateJsonErrorResponse("Failed To Update Detail");
            }

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.AssetAssignment);
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
                    Text = e.Province.Text+"-"+e.OfficeName+"-"+e.FloorName+"-"+e.RoomName
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
                    Value = Convert.ToString(e.OfficeName),
                    Text = e.OfficeName
                }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFloorGrid(string province = null)
        {
            SessionManager.Set("Province", province);
            var pro = SessionManager.Get<string>("Province");
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            var positions = _service.GetFloorList(siteUrl, province);

            return Json(positions.Select(e =>
                new
                {
                    Value = Convert.ToString(e.FloorName),
                    Text = e.FloorName
                }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRoomGrid(string province = null)
        {
            SessionManager.Set("Province", province);
            var pro = SessionManager.Get<string>("Province");
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            var positions = _service.GetRoomList(siteUrl, province);

            return Json(positions.Select(e =>
                new
                {
                    Value = Convert.ToString(e.RoomName),
                    Text = e.RoomName
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

            var listNameHeader = "Asset Assignment";
            var listNameDetail = "Asset Assignment Detail";
            foreach (DataRow d in SessionManager.Get<DataTable>("CSVDataTable").Rows)
            {
                var type = "assignment of asset";
                if (d.ItemArray[0].ToString().ToLower() == type)
                {
                    try
                    {
                        type = "Assignment Of Asset";

                        TableHeader = new DataTable();
                        TableHeader.Columns.Add("Title", typeof(string));
                        TableHeader.Columns.Add("transferdate", typeof(string));
                        TableHeader.Columns.Add("assetholder", typeof(string));
                        TableHeader.Columns.Add("position", typeof(string));
                        TableHeader.Columns.Add("projectunit", typeof(string));
                        TableHeader.Columns.Add("contactnumber", typeof(string));

                        //check assetholder in Professional Master HR
                        var caml = @"<View><Query>
                                   <Where>
                                      <Eq>
                                         <FieldRef Name='Title' />
                                         <Value Type='Text'>"+ d.ItemArray[2].ToString() + @"</Value>
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
                        var sitehr = siteUrl.Replace("/bo", "/hr");
                        var isAssetHolderExist = _service.isExist("Professional Master", caml, sitehr);
                        if(isAssetHolderExist == true)
                        {
                            DataRow row = TableHeader.NewRow();
                            row["Title"] = type;
                            row["transferdate"] = Convert.ToString(d.ItemArray[1]);
                            row["assetholder"] = Convert.ToString(d.ItemArray[2]);
                            row["position"] = "";
                            row["projectunit"] = "";
                            row["contactnumber"] = "";

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

                if (d.ItemArray[6].ToString() != "" && latestIDHeader != null)
                {
                    try
                    {
                        TableDetail = new DataTable();
                        TableDetail.Columns.Add("assignmentofasset", typeof(int));
                        TableDetail.Columns.Add("assetsubasset", typeof(string));
                        TableDetail.Columns.Add("province", typeof(string));
                        TableDetail.Columns.Add("office", typeof(string));
                        TableDetail.Columns.Add("floor", typeof(string));
                        TableDetail.Columns.Add("room", typeof(string));
                        TableDetail.Columns.Add("remarks", typeof(string));

                        //check assetsubasset
                        //check province -office - floor - room
                        //FXA-GP-FF-0001-GPS
                        var breakAsset = d.ItemArray[6].ToString().Split('-');
                        var assetID = breakAsset[0] + "-" + breakAsset[1] + "-" + breakAsset[2] + "-" + breakAsset[3];
                        if(breakAsset.Length > 5)
                        {
                            //if breakAsset.Length > 5 (Sub Asset)
                            assetID = breakAsset[0] + "-" + breakAsset[1] + "-" + breakAsset[2] + "-" + breakAsset[3] + "-" + breakAsset[4];
                        }
                        var camlasset = @"<View>
                        <Query>
                           <Where>
                              <Eq>
                                 <FieldRef Name='assetsubasset' />
                                 <Value Type='Lookup'>" + assetID + @"</Value>
                              </Eq>
                           </Where>
                           <OrderBy>
                              <FieldRef Name='assetsubasset' Ascending='True' />
                           </OrderBy>
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
                                    <Value Type='Lookup'>"+d.ItemArray[7].ToString()+@"</Value>
                                    </Eq>
                                    <And>
                                    <Eq>
                                        <FieldRef Name='Title' />
                                        <Value Type='Text'>"+d.ItemArray[8].ToString()+@"</Value>
                                    </Eq>
                                    <And>
                                        <Eq>
                                            <FieldRef Name='Floor' />
                                            <Value Type='Text'>"+d.ItemArray[9].ToString()+@"</Value>
                                        </Eq>
                                        <Eq>
                                            <FieldRef Name='Room' />
                                            <Value Type='Text'>"+d.ItemArray[10].ToString()+@"</Value>
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
                        var isAssetExist = _service.isExist("Asset Acquisition Details", camlasset, siteUrl);
                        var isProvinceExist = _service.isExist("Location Master", camlprovince, siteUrl);

                        if (isAssetExist == true && isProvinceExist == true)
                        {
                            DataRow row = TableDetail.NewRow();

                            row["assignmentofasset"] = latestIDHeader;
                            row["assetsubasset"] = assetID;
                            row["province"] = d.ItemArray[7].ToString();
                            row["office"] = d.ItemArray[8].ToString();
                            row["floor"] = d.ItemArray[9].ToString();
                            row["room"] = d.ItemArray[10].ToString();
                            row["remarks"] = d.ItemArray[11].ToString();

                            TableDetail.Rows.InsertAt(row, 0);

                            latestIDDetail = _service.MassUploadHeaderDetail(listNameDetail, TableDetail, siteUrl);
                            idsDetail.Add(Convert.ToInt32(latestIDDetail));
                        }
                        else
                        {
                            return JsonHelper.GenerateJsonErrorResponse("No Lookup Value/s is Found For Asset ID / Province!");
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
            }
            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.AssetAssignment);
        }

        [HttpPost]
        public ActionResult Print(FormCollection form, AssignmentOfAssetVM viewModel, string SiteUrl)
        {
            SiteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(SiteUrl ?? ConfigResource.DefaultBOSiteUrl);

            const string RelativePath = "~/Views/ASSAssignmentOfAsset/Print.cshtml";
            var view = ViewEngines.Engines.FindView(ControllerContext, RelativePath, null);
            var nm = viewModel.AssetHolder.Value.Split('-');
            viewModel.nameOnly = nm[0];
            viewModel.position = nm[1];
            viewModel.Details = _service.GetDetailsPrint(viewModel.ID);
            var fileName = nm[0] + "_AssignmentOfAsset.pdf";
            byte[] pdfBuf = null;
            string content;

            // ControllerContext context = new ControllerContext();
            ControllerContext.Controller.ViewData.Model = viewModel;
            ViewData = ControllerContext.Controller.ViewData;
            TempData = ControllerContext.Controller.TempData;

            using (var writer = new StringWriter())
            {
                var contextviewContext = new ViewContext(ControllerContext, view.View, ViewData, TempData, writer);
                view.View.Render(contextviewContext, writer);
                writer.Flush();
                content = writer.ToString();

                // Get PDF Bytes
                try
                {
                    pdfBuf = PDFConverter.Instance.ConvertFromHTML(fileName, content);
                }
                catch (Exception e)
                {
                    ErrorSignal.FromCurrentContext().Raise(e);
                    RedirectToAction("Index", "Error");
                }
            }
            if (pdfBuf == null)
                return HttpNotFound();
            return File(pdfBuf, "application/pdf");
        }

        public ActionResult Sync(string siteUrl)
        {
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            var viewModel = _service.GetPopulatedModel(siteUrl);
            return View(viewModel);
        }

        public ActionResult Syncronize(string siteUrl)
        {
            siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            try
            {
                var viewModel = _service.Syncronize(siteUrl);
            }
            catch (Exception e)
            {
                return JsonHelper.GenerateJsonErrorResponse("Failed To Syncronize");
            }

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.AssetAssignment);
        }

    }
}