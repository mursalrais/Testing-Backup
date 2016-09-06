using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using MCAWebAndAPI.Service.Asset;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MCAWebAndAPI.Web.Controllers
{
    public class ASSReportController : Controller
    {
        IReportService _service;

        public ASSReportController()
        {
            _service = new ReportService();
        }

        public ActionResult ReportFixedAsset(string SiteUrl)
        {
            _service.SetSiteUrl(SiteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set("SiteUrl", SiteUrl ?? ConfigResource.DefaultBOSiteUrl);
            //var model = _service.GetReport(SiteUrl, "Fixed Asset");
            var emptyTable = _service.getTable("Fixed Asset");
            SessionManager.Set("CSVDataTable", emptyTable);

            var viewModel = new AssetReportVM();
            viewModel.dtDetails = emptyTable;
            return View(viewModel);
        }

        public ActionResult ReportSmallValueAsset(string SiteUrl)
        {
            _service.SetSiteUrl(SiteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set("SiteUrl", SiteUrl ?? ConfigResource.DefaultBOSiteUrl);
            //var model = _service.GetReport(SiteUrl, "Small Value Asset");
            var emptyTable = _service.getTable("Small Value Asset");
            SessionManager.Set("CSVDataTable", emptyTable);

            var viewModel = new AssetReportVM();
            viewModel.dtDetails = emptyTable;
            return View(viewModel);
        }

        public JsonResult Grid_ReadFA([DataSourceRequest] DataSourceRequest request)
        {
            // Get from existing session variable or create new if doesn't exist
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            var emptyTable = _service.getTable("Fixed Asset");
            SessionManager.Set("CSVDataTable", emptyTable);

            var viewModel = new AssetReportVM();
            viewModel.dtDetails = emptyTable;

            // Convert to Kendo DataSource
            DataSourceResult result = emptyTable.ToDataSourceResult(request);

            // Convert to Json
            var json = Json(result, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;
            return json;
        }

        public JsonResult Grid_ReadSVA([DataSourceRequest] DataSourceRequest request)
        {
            // Get from existing session variable or create new if doesn't exist
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            var emptyTable = _service.getTable("Small Value Asset");
            SessionManager.Set("CSVDataTable", emptyTable);

            var viewModel = new AssetReportVM();
            viewModel.dtDetails = emptyTable;

            // Convert to Kendo DataSource
            DataSourceResult result = emptyTable.ToDataSourceResult(request);

            // Convert to Json
            var json = Json(result, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;
            return json;
        }
    }
}