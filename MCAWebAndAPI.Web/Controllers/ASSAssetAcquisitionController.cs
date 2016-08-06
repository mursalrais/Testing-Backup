﻿using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using MCAWebAndAPI.Service.Asset;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;
using System;
using System.Net;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;
using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Model.ViewModel.Control;
using System.Data;
using System.Web;
using System.IO;
using MCAWebAndAPI.Service.Converter;
using MCAWebAndAPI.Service.Utils;

namespace MCAWebAndAPI.Web.Controllers
{
    public class ASSAssetAcquisitionController : Controller
    {
        IAssetAcquisitionService _assetAcquisitionService;

        public ASSAssetAcquisitionController()
        {
            _assetAcquisitionService = new AssetAcquisitionService();
        }

        public ActionResult Index()
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _assetAcquisitionService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            return Redirect(string.Format("{0}/{1}", siteUrl ?? ConfigResource.DefaultBOSiteUrl, UrlResource.AssetAcquisition));
        }

        public ActionResult Create(string siteUrl)
        {
            _assetAcquisitionService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            var viewModel = _assetAcquisitionService.GetPopulatedModel();
            return View(viewModel);
        }


        public ActionResult Edit(int ID, string siteUrl)
        {
            _assetAcquisitionService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            var viewModel = _assetAcquisitionService.GetHeader(ID);

            int? headerID = null;
            headerID = viewModel.ID;

            try
            {
                var viewdetails = _assetAcquisitionService.GetDetails(headerID);
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
        public ActionResult Submit(AssetAcquisitionHeaderVM _data, string siteUrl)
        {
            siteUrl = SessionManager.Get<string>("SiteUrl");
            _assetAcquisitionService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            //return View(new AssetMasterVM());
            int? headerID = null;
            try
            {
                headerID = _assetAcquisitionService.CreateHeader(_data);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            try
            {
                _assetAcquisitionService.CreateDetails(headerID, _data.Details);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }
            return JsonHelper.GenerateJsonSuccessResponse(siteUrl ?? ConfigResource.DefaultBOSiteUrl + UrlResource.AssetAcquisition);
            //return Redirect(string.Format("{0}/{1}", siteUrl ?? ConfigResource.DefaultBOSiteUrl, UrlResource.AssetAcquisition));
        }

        public ActionResult Update(AssetAcquisitionHeaderVM _data, string SiteUrl)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _assetAcquisitionService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            try
            {
                _assetAcquisitionService.UpdateHeader(_data);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            try
            {
                //update items
                _assetAcquisitionService.UpdateDetails(_data.ID, _data.Details);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            return JsonHelper.GenerateJsonSuccessResponse(string.Format("{0}/{1}", SiteUrl, UrlResource.AssetAcquisition));
        }

        public JsonResult GetAssetSubSAssetGrid()
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _assetAcquisitionService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            var positions = GetFromPositionsExistingSession();

            return Json(positions.Select(e =>
                new
                {
                    Value = Convert.ToString(e.ID),
                    Text = e.AssetNoAssetDesc.Value + " - " + e.AssetDesc
                }),
                JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<AssetMasterVM> GetFromPositionsExistingSession()
        {
            //Get existing session variable
            var sessionVariable = System.Web.HttpContext.Current.Session["AssetMaster"] as IEnumerable<AssetMasterVM>;
            var positions = sessionVariable ?? _assetAcquisitionService.GetAssetSubAsset();

            if (sessionVariable == null) // If no session variable is found
                System.Web.HttpContext.Current.Session["AssetMaster"] = positions;
            return positions;
        }

        public JsonResult GetWBSGrid()
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _assetAcquisitionService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            var positions = GetFromWBSExistingSession();

            return Json(positions.Select(e =>
                new
                {
                    Value = Convert.ToString(e.ID),
                    Text = e.WBSID.Text + " - " + e.WBSDesc
                }),
                JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<WBSMaterVM> GetFromWBSExistingSession()
        {
            //Get existing session variable
            var sessionVariable = System.Web.HttpContext.Current.Session["WBSMaster"] as IEnumerable<WBSMaterVM>;
            var positions = sessionVariable ?? _assetAcquisitionService.GetWBS();

            if (sessionVariable == null) // If no session variable is found
                System.Web.HttpContext.Current.Session["WBSMaster"] = positions;
            return positions;
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

        public ActionResult Submit(string listName)
        {
            // Get existing session variable
            var sessionVariables = SessionManager.Get<DataTable>("CSVDataTable") ?? new DataTable();
            var siteUrl = SessionManager.Get<string>("SiteUrl");

            //cek apakah header / item
            int x = 0;
            int? latestIDHeader = 0;
            foreach (DataRow d in SessionManager.Get<DataTable>("CSVDataTable").Rows)
            {
                if (d.ItemArray[0].ToString() == "Asset Acquisition")
                {
                    try
                    {
                        var listNameHeader = "Asset Acquisition";
                        var listNameHeaderMemo = "Acceptance Memo";
                        var IDMemo = _assetAcquisitionService.getListIDOfList(listNameHeaderMemo, "ID" , "Title", siteUrl);
                        var myKey = IDMemo.FirstOrDefault(v => v.Value == d.ItemArray[2].ToString()).Key;

                        var TableHeader = new DataTable();
                        TableHeader.Columns.Add("Title", typeof(string));
                        TableHeader.Columns.Add("Acceptance_x0020_Memo_x0020_No", typeof(int));
                        TableHeader.Columns.Add("Vendor", typeof(string));
                        TableHeader.Columns.Add("PO_x0020_No", typeof(string));
                        TableHeader.Columns.Add("Purchase_x0020_Date", typeof(string));
                        TableHeader.Columns.Add("Purchase_x0020_Description", typeof(string));

                        DataRow row = TableHeader.NewRow();

                        row["Title"] = d.ItemArray[0].ToString();
                        row["Acceptance_x0020_Memo_x0020_No"] = myKey;
                        row["Vendor"] = d.ItemArray[4].ToString();
                        row["PO_x0020_No"] = d.ItemArray[5].ToString();
                        row["Purchase_x0020_Date"] = d.ItemArray[6].ToString();
                        row["Purchase_x0020_Description"] = d.ItemArray[7].ToString();

                        TableHeader.Rows.InsertAt(row, 0);

                        latestIDHeader = _assetAcquisitionService.MassUploadHeaderDetail(listNameHeader, TableHeader, siteUrl);   
                    }
                    catch (Exception e)
                    {
                        return JsonHelper.GenerateJsonErrorResponse(e);
                    }
                }
                else
                {
                    try
                    {
                        var listNameDetail = "Asset Acquisition Details";
                        var listAssetMaster = "Asset Master";
                        var listWBSMaster = "WBS Master";

                        //var IDAssetMaster = _assetAcquisitionService.getListIDOfList(listNameHeaderMemo, "ID", "Title", siteUrl);
                        //var myKeyAssetMaster = IDMemo.FirstOrDefault(v => v.Value == d.ItemArray[2].ToString()).Key;

                        //var IDWBSMaster = _assetAcquisitionService.getListIDOfList(listNameHeaderMemo, "ID", "Title", siteUrl);
                        //var myKeyWBSMaster = IDMemo.FirstOrDefault(v => v.Value == d.ItemArray[2].ToString()).Key;

                        var TableDetail = new DataTable();
                        TableDetail.Columns.Add("Asset_x0020_Acquisition", typeof(string));
                        TableDetail.Columns.Add("PO_x0020_Line_x0020_Item", typeof(string));
                        TableDetail.Columns.Add("Asset_x002d_Sub_x0020_Asset", typeof(string));
                        TableDetail.Columns.Add("WBS", typeof(string));
                        TableDetail.Columns.Add("Cost_x0020_IDR", typeof(string));
                        TableDetail.Columns.Add("Cost_x0020_USD", typeof(string));
                        TableDetail.Columns.Add("Remarks", typeof(string));
                        TableDetail.Columns.Add("Status", typeof(string));

                        DataRow row = TableDetail.NewRow();

                        row["Asset_x0020_Acquisition"] = latestIDHeader;
                        row["PO_x0020_Line_x0020_Item"] = d.ItemArray[8].ToString();
                        //cek if assetid ada pada table asset master
                        //FXA-PC-OE-0001 - Laptop Lenovo
                        var splitAssetID = d.ItemArray[9].ToString().Split('-');
                        var resultAssetID="";
                        var resultDesc="";
                        if (splitAssetID.Length == 5)
                        {
                            resultAssetID = splitAssetID[0] + "-" + splitAssetID[1] + "-" + splitAssetID[2] + "-" + splitAssetID[3];
                            resultDesc = splitAssetID[4];
                        }
                        else
                        {
                            resultAssetID = splitAssetID[0] + "-" + splitAssetID[1] + "-" + splitAssetID[2] + "-" + splitAssetID[3] + "-" + splitAssetID[4];
                            resultDesc = splitAssetID[5];
                        }
                        var splitWBS = d.ItemArray[10].ToString().Split('-');
                        var camlAssetID = @"<View><Query>
                                    <Where>
                                        <And>
                                            <Eq>
                                                <FieldRef Name='AssetID' />
                                                <Value Type='Text'>"+ resultAssetID + @"</Value>
                                            </Eq>
                                            <And>
                                            <Eq>
                                                <FieldRef Name='Title' />
                                                <Value Type='Text'>"+resultDesc+@"</Value>
                                            </Eq>
                                            </And>
                                        </And>
                                    </Where>
                                    </Query></View>";
                        var camlWBS = @"<View><Query>
                                    <Where>
                                        <And>
                                            <Eq>
                                                <FieldRef Name='Title' />
                                                <Value Type='Text'>" + splitWBS[0] + @"</Value>
                                            </Eq>
                                            <And>
                                            <Eq>
                                                <FieldRef Name='WBSDesc' />
                                                <Value Type='Text'>" + splitWBS[1] + @"</Value>
                                            </Eq>
                                            </And>
                                        </And>
                                    </Where>
                                    </Query></View>";

                        try
                        {
                            bool isAssetIDExist = _assetAcquisitionService.isValueOfColumnExist("Asset Master", siteUrl, camlAssetID);
                            bool isWBSExist = _assetAcquisitionService.isValueOfColumnExist("WBS Master", siteUrl, camlWBS);
                            if (isAssetIDExist == true && isWBSExist == true)
                            {
                                row["Asset_x002d_Sub_x0020_Asset"] = d.ItemArray[9].ToString();
                                row["WBS"] = d.ItemArray[10].ToString();
                            }

                        }
                        catch(Exception e)
                        {
                            return JsonHelper.GenerateJsonErrorResponse(e);
                        }
                        //cek if wbs id ada pada table wbs master
                        row["Cost_x0020_IDR"] = Convert.ToInt32(d.ItemArray[11]);
                        row["Cost_x0020_USD"] = Convert.ToInt32(d.ItemArray[12].ToString());
                        row["Remarks"] = d.ItemArray[13].ToString();
                        row["Status"] = d.ItemArray[14].ToString();

                        TableDetail.Rows.InsertAt(row, 0);

                        _assetAcquisitionService.MassUploadHeaderDetail(listNameDetail, TableDetail, siteUrl);
                    }
                    catch (Exception e)
                    {
                        return JsonHelper.GenerateJsonErrorResponse(e);
                    }
                }
                x++;
            }
            return JsonHelper.GenerateJsonSuccessResponse(siteUrl);
        }

    }
}