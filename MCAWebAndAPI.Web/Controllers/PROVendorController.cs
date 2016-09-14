using MCAWebAndAPI.Model.ViewModel.Control;
using MCAWebAndAPI.Model.ViewModel.Form.Procurement;
using MCAWebAndAPI.Service.Converter;
using MCAWebAndAPI.Service.Procurement;
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
    public class PROVendorController : Controller
    {
        IVendorService _service;

        public PROVendorController()
        {
            _service = new VendorService();
        }

        // GET: PROVendor
        public ActionResult Create(string siteUrl)
        {
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            // Get blank ViewModel
            var viewModel = _service.getPopulated(siteUrl);

            // Return to the name of the view and parse the model
            return View("Create", viewModel);
        }

        public ActionResult Edit(int ID, string siteUrl)
        {
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            // Get blank ViewModel
            var viewModel = _service.GetVendorMaster(ID);

            // Return to the name of the view and parse the model
            return View("Edit", viewModel);
        }

        [HttpPost]
        public ActionResult Submit(VendorVM model, string siteUrl)
        {
            siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            // Get blank ViewModel
            try
            {
                var latestID = _service.CreateVendorMaster(model);
            }
            catch(Exception e)
            {
                Response.TrySkipIisCustomErrors = true;
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse("Failed To Save..");
            }

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.VendorMaster);
        }

        [HttpPost]
        public ActionResult Update(VendorVM model, string siteUrl)
        {
            siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            // Get blank ViewModel
            try
            {
                _service.UpdateVendorMaster(model);
            }
            catch (Exception e)
            {
                Response.TrySkipIisCustomErrors = true;
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse("Failed To Update..");
            }

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.VendorMaster);
        }

        public ActionResult GetProfMasterInfo(int ID)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            var profmasterinfo = _service.getProfMasterInfo("Professional Master", ID, siteUrl);

            return Json(profmasterinfo, JsonRequestBehavior.AllowGet);
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

        //[HttpPost]
        public ActionResult SubmitUpload(string listName)
        {
            var sessionVariables = SessionManager.Get<DataTable>("CSVDataTable") ?? new DataTable();
            var siteUrl = SessionManager.Get<string>("SiteUrl");

            // Get blank ViewModel
            try
            {
                var status = _service.MassUpload(listName, sessionVariables, siteUrl);
                if(status != 1)
                {
                    Response.TrySkipIisCustomErrors = true;
                    Response.TrySkipIisCustomErrors = true;
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return JsonHelper.GenerateJsonErrorResponse("No Professional ID is Found!");
                }
            }
            catch (Exception e)
            {
                Response.TrySkipIisCustomErrors = true;
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse("Failed To Upload CSV..");
            }

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.VendorMaster);
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
    }
}