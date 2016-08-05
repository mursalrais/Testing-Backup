using MCAWebAndAPI.Model.ViewModel.Control;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using MCAWebAndAPI.Service.Asset;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;
using System.Linq;
using System.Web.Mvc;
using System;
using System.Data;
using System.Collections.Generic;
using System.Web;
using System.IO;
using MCAWebAndAPI.Service.Converter;
using MCAWebAndAPI.Service.Utils;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

namespace MCAWebAndAPI.Web.Controllers
{
    public class ASSAssetMasterController : Controller
    {
        IAssetMasterService _assetMasterService;

        public ASSAssetMasterController()
        {
            _assetMasterService = new AssetMasterService();
        }



        public JsonResult GetAssetMasters()
        {
            _assetMasterService.SetSiteUrl(ConfigResource.DefaultBOSiteUrl);
            var result = _assetMasterService.GetAssetMasters();

            return Json(result.Select(e => (new
            {
                e.ID,
                e.AssetDesc
            })), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAssetLocations()
        {
            _assetMasterService.SetSiteUrl(ConfigResource.DefaultBOSiteUrl);
            var result = _assetMasterService.GetAssetLocations();

            return Json(result.Select(e => (new
            {
                e.ID,
                e.Name
            })), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create(string site)
        {
            var viewModel = _assetMasterService.GetAssetMaster();
            return View(viewModel);
        }

        public ActionResult Edit(int ID, string site)
        {
            var viewModel = _assetMasterService.GetAssetMaster(ID);
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Submit(AssetMasterVM _data, string site)
        {
            //return View(new AssetMasterVM());
            _assetMasterService.CreateAssetMaster(_data);
            return new JavaScriptResult
            {
                Script = string.Format("window.parent.location.href = '{0}'", "https://eceos2.sharepoint.com/sites/ims/bo/Lists/AssetMaster/AllItems.aspx")
            };
        }

        public ActionResult Update(AssetMasterVM _data, string site)
        {
            //return View(new AssetMasterVM());
            _assetMasterService.UpdateAssetMaster(_data);
            return new JavaScriptResult
            {
                Script = string.Format("window.parent.location.href = '{0}'", "https://eceos2.sharepoint.com/sites/ims/bo/Lists/AssetMaster/AllItems.aspx")
            };
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

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditingPopup_Create([DataSourceRequest] DataSourceRequest request, AssetMasterVM _assetMasterVM)
        {
            if (_assetMasterVM != null && ModelState.IsValid)
            {
                //_assetMasterService.CreateAssetMasterItem_dummy(_assetMasterVM);
                //productService.Create(_assetMasterItem);
            }

            return Json(new[] { _assetMasterVM }.ToDataSourceResult(request, ModelState));
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
            DataTable createAssID = SessionManager.Get<DataTable>("CSVDataTable");
            var res = "";
            List<string> assetIDs = new List<string>();
            int x = 0;
            foreach (DataRow d in SessionManager.Get<DataTable>("CSVDataTable").Rows)
            {
                if(createAssID.Rows[x].ItemArray[0].ToString() == "")
                {
                    res = _assetMasterService.GetAssetIDForMainAsset(createAssID.Rows[x].ItemArray[2].ToString(), createAssID.Rows[x].ItemArray[3].ToString(), createAssID.Rows[x].ItemArray[4].ToString());
                    if (assetIDs.Contains(res))
                    {
                        //split
                        string[] breakk = res.Split('-');
                        int num = Convert.ToInt32(breakk[3]);
                        //FormatUtil.ConvertToDigitNumber(lastNumber, 4);
                        num = num + 1;
                        breakk[3] = FormatUtil.ConvertToDigitNumber(num, 4);
                        res = breakk[0] + "-" + breakk[1] + "-" + breakk[2] + "-" + breakk[3];
                        //get the number + 1

                    }
                    else
                    {
                        assetIDs.Add(res);
                    }
                }
                else
                {
                    //cek if assetID parent is exist
                    var assetIDss = _assetMasterService.GetAssetIDForSubAsset(createAssID.Rows[x].ItemArray[0].ToString());
                    if(assetIDss == "")
                    {
                        return JsonHelper.GenerateJsonErrorResponse(assetIDss);
                    }
                    else
                    {
                        res = assetIDss;
                    }
                }

                d["AssetID"] = res;
                x++;
            }
            var sessionVariables = SessionManager.Get<DataTable>("CSVDataTable") ?? new DataTable();
            var siteUrl = SessionManager.Get<string>("SiteUrl");

            try
            {
                CSVConverter.Instance.MassUpload(listName, sessionVariables, siteUrl);
            }
            catch (Exception e)
            {
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl);
        }

    }
}