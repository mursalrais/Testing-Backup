using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MCAWebAndAPI.Model.ViewModel.Control;
using MCAWebAndAPI.Service.Asset;
using MCAWebAndAPI.Service.Converter;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MCAWebAndAPI.Web.Controllers
{
    public class CSVController : Controller
    {
        IAssetTransactionService _assetTransactionService;
        public CSVController()
        {
            _assetTransactionService = new AssetTransactionService();
        }

        public ActionResult Create(IEnumerable<HttpPostedFileBase> files)
        {
            var viewModel = new CSVVM();
            return View(viewModel);
        }

        public ActionResult Save(IEnumerable<HttpPostedFileBase> files)
        {
            StreamReader reader = null;
            // The Name of the Upload component is "files"
            if (files != null)
            {
                foreach (var file in files)
                {
                    // Some browsers send file names with full path.
                    // We are only interested in the file name.
                    var fileName = Path.GetFileName(file.FileName);
                    var tess = file.InputStream;
                    
                    var physicalPath = Path.Combine(Server.MapPath("~/App_Data"), fileName);

                    using (reader = new StreamReader(tess))
                    {
                        var CSVDataTable = CSVConverter.Instance.ToDataTable(reader);
                        Session.Add("CSVDataTable", CSVDataTable);
                    }

                    // The files are not actually saved in this demo
                    // file.SaveAs(physicalPath);
                }
            }

            // Return an empty string to signify success
            return Content("");
        }

        public ActionResult Remove(string[] fileNames)
        {
            // The parameter of the Remove action must be called "fileNames"

            if (fileNames != null)
            {
                foreach (var fullName in fileNames)
                {
                    var fileName = Path.GetFileName(fullName);
                    var physicalPath = Path.Combine(Server.MapPath("~/App_Data"), fileName);

                    if (System.IO.File.Exists(physicalPath))
                    {
                        // The files are not actually removed in this demo
                        // System.IO.File.Delete(physicalPath);
                    }
                }
            }

            // Return an empty string to signify success
            return Content("");
        }

        [HttpPost]
        public ActionResult Submit(CSVVM _data, string siteUrl = null)
        {
            var data = _data;
            System.Data.DataTable CSVDataTable = Session["CSVDataTable"] as System.Data.DataTable;
            data.DataTable = CSVDataTable;
            return View(data);
        }

        [HttpPost]
        public ActionResult Upload(CSVVM _data, string siteUrl = null)
        {
            var data = _data;
            string ListName = data.ListName;
            System.Data.DataTable CSVDataTable = Session["CSVDataTable"] as System.Data.DataTable;

            // Clear Existing Session Variables if any
            if (System.Web.HttpContext.Current.Session.Keys.Count > 0)
                System.Web.HttpContext.Current.Session.Clear();

            // MANDATORY: Set Site URL
            _assetTransactionService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            System.Web.HttpContext.Current.Session["SiteUrl"] = siteUrl ?? ConfigResource.DefaultBOSiteUrl;
            string SiteUrl = Session["SiteUrl"] as string;

            //Insert to SPList
            CSVConverter.Instance.MassUpload(ListName,CSVDataTable, SiteUrl);

            // Get blank ViewModel
            var viewModel = new CSVVM();          
            

            // Return to the name of the view and parse the model
            return View("Create",viewModel);
        }

    }
}