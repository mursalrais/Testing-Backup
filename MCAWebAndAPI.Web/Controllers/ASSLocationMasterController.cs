using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using MCAWebAndAPI.Service.Asset;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Model.ViewModel.Control;
using MCAWebAndAPI.Web.Filters;
using MCAWebAndAPI.Web.Resources;
using System.Net;
using MCAWebAndAPI.Service.Resources;
using System.Web;
using System.IO;
using MCAWebAndAPI.Service.Converter;
using System.Data;

namespace MCAWebAndAPI.Web.Controllers
{
    public class ASSLocationMasterController : Controller
    {
        ILocationMasterService _locationMasterService;

        public ASSLocationMasterController()
        {
            _locationMasterService = new LocationMasterService();
        }

        public ActionResult CreateLocationMaster(string siteUrl = null)
        {
            // MANDATORY: Set Site URL
            _locationMasterService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            ViewBag.Action = "CreateLocationMaster";

            // Get blank ViewModel
            var viewModel = _locationMasterService.GetPopulatedModel(siteUrl);

            // Return to the name of the view and parse the model
            return View("Create", viewModel);
        }

        public ActionResult Edit(int ID, string siteUrl = null)
        {
            // MANDATORY: Set Site URL
            _locationMasterService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            ViewBag.Action = "Update";

            var viewModel = _locationMasterService.GetHeader(ID, siteUrl);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult CreateLocationMaster(FormCollection form, LocationMasterVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _locationMasterService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            int? headerID = null;
            try
            {
                headerID = _locationMasterService.CreateHeader(viewModel, viewModel.Province.Text, viewModel.OfficeName, viewModel.FloorName, viewModel.RoomName);
                if (headerID == 0)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return JsonHelper.GenerateJsonErrorResponse("This Location is Already Exist");
                }
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.LocationMaster);
        }

        public ActionResult Update(FormCollection form, LocationMasterVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _locationMasterService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            try
            {
                bool isExist = _locationMasterService.UpdateHeader(viewModel, viewModel.Province.Text, viewModel.OfficeName, viewModel.FloorName, viewModel.RoomName);
                if (isExist == false)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return JsonHelper.GenerateJsonErrorResponse("This Location is Already Exist");
                }
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.LocationMaster);
        }

        public JsonResult GetLocationMaster()
        {
            _locationMasterService.SetSiteUrl(SessionManager.Get<string>("SiteUrl"));

            var professionalmonthlyfee = GetFromLocationMasterExistingSession();
            return Json(professionalmonthlyfee.Select(e =>
                new
                {
                    e.ID,
                    e.Title
                }),
                JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<LocationMasterVM> GetFromLocationMasterExistingSession()
        {
            //Get existing session variable
            var sessionVariable = System.Web.HttpContext.Current.Session["LocationMaster"] as IEnumerable<LocationMasterVM>;
            var locationMaster = sessionVariable ?? _locationMasterService.GetLocationMaster();

            if (sessionVariable == null) // If no session variable is found
                System.Web.HttpContext.Current.Session["LocationMaster"] = locationMaster;
            return locationMaster;
        }

        public ActionResult UpdateProvince(string siteUrl = null)
        {
            // MANDATORY: Set Site URL
            _locationMasterService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            ViewBag.Action = "UpdateProvince";

            try
            {
                var province = _locationMasterService.UpdateProvince();
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.LocationMaster);
        }

        [HttpPost]
        public ActionResult UpdateProvince(FormCollection form, LocationMasterVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _locationMasterService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            try
            {
                var province = _locationMasterService.UpdateProvince();
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.LocationMaster);
        }


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

            try
            {
                int? upload = _locationMasterService.MassUpload("Location Master", sessionVariables, siteUrl);
                if (upload == 0)
                {
                    return JsonHelper.GenerateJsonErrorResponse("Failed One of Your Data is Invalid, Rolling Back");
                }
            }
            catch (Exception e)
            {
                return JsonHelper.GenerateJsonErrorResponse(e + " Invalid data, rolling back!");
            }

            //return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.AssetAcquisition);
            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.AssetAcquisition);
        }
    }
}