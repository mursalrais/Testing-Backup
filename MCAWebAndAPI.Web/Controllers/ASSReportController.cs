﻿using Kendo.Mvc.Extensions;
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
            var emptyTable = _service.getTable("Fixed Asset", "empty");
            SessionManager.Set("CSVDataTable", emptyTable);

            var viewModel = new AssetReportVM();
            viewModel.dtDetails = emptyTable;
            return View("ReportFixedAsset");
        }

        public ActionResult ReportSmallValueAsset(string SiteUrl)
        {
            _service.SetSiteUrl(SiteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set("SiteUrl", SiteUrl ?? ConfigResource.DefaultBOSiteUrl);
            //var model = _service.GetReport(SiteUrl, "Small Value Asset");
            var emptyTable = _service.getTable("Small Value Asset", "empty");
            SessionManager.Set("CSVDataTable", emptyTable);

            var viewModel = new AssetReportVM();
            viewModel.dtDetails = emptyTable;
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult GridWorksheet_Read([DataSourceRequest] DataSourceRequest request)
        {
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


        [HttpPost]
        public ActionResult GridWorksheet_ExportExcel(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            fileName = string.Format("ReportFixedAsset.xlsx");

            // Store the file on the session variable
            var fileContent = fileContents;
            //SessionManager.Set("PayrollWorksheet_ExcelFile", fileContents);

            return File(fileContents, contentType, fileName);
        }

        public JsonResult Grid_ReadFA([DataSourceRequest] DataSourceRequest request)
        {
            // Get from existing session variable or create new if doesn't exist
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            var dataTable = SessionManager.Get<System.Data.DataTable>("CSVDataTable");
            var emptyTable = new DataTable();
            if (dataTable.Rows.Count == 0)
            {
                emptyTable = _service.getTable("Fixed Asset");
                SessionManager.Set("CSVDataTable", emptyTable);
            }
            else
            {
                emptyTable = dataTable;
            }

            var viewModel = new AssetReportVM();
            viewModel.dtDetails = emptyTable;

            // Convert to Kendo DataSource
            DataSourceResult result = emptyTable.ToDataSourceResult(request);

            // Convert to Json
            var json = Json(result, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;
            return json;
        }

        [HttpPost]
        public ActionResult ReportFixedAsset(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            fileName = string.Format("ReportFixedAsset.xlsx");

            // Store the file on the session variable
            var fileContent = fileContents;

            return File(fileContents, contentType, fileName);
        }

        public JsonResult Grid_ReadSVA([DataSourceRequest] DataSourceRequest request)
        {
            // Get from existing session variable or create new if doesn't exist
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            var dataTable = SessionManager.Get<System.Data.DataTable>("CSVDataTable");
            var emptyTable = new DataTable();
            if (dataTable.Rows.Count == 0)
            {
                emptyTable = _service.getTable("Small Value Asset");
                SessionManager.Set("CSVDataTable", emptyTable);
            }
            else
            {
                emptyTable = dataTable;
            }

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