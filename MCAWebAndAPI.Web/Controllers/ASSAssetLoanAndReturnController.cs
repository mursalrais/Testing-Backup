using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MCAWebAndAPI.Model.HR.DataMaster;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using MCAWebAndAPI.Service.Asset;
using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MCAWebAndAPI.Web.Controllers
{
    public class ASSAssetLoanAndReturnController : Controller
    {
        IAssetLoanAndReturnService assetLoanAndReturnService;

        public ASSAssetLoanAndReturnController()
        {
            assetLoanAndReturnService = new AssetLoanAndReturnService();
        }

        // GET: ASSAssetLoanAndReturn
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create(string siteUrl)
        {
            assetLoanAndReturnService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            var viewModel = assetLoanAndReturnService.GetPopulatedModel();

            return View("Create", viewModel);
        }

        public JsonResult GetAssetSubSAssetGrid()
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            assetLoanAndReturnService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

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
            var positions = sessionVariable ?? assetLoanAndReturnService.GetAssetSubAsset();

            if (sessionVariable == null) // If no session variable is found
                System.Web.HttpContext.Current.Session["AssetMaster"] = positions;
            return positions;
        }

        //public JsonResult GetProfessionalInfo()
        //{
        //    assetLoanAndReturnService.SetSiteUrl(SessionManager.Get<string>("SiteUrl"));

        //    var professionalInfo = GetFromProfessionalExistingSession();
        //    return Json(professionalInfo.Select(e =>
        //        new {
        //            e.ID,
        //            e.Name,
        //            e.Status,
        //            Desc = string.Format("{0} - {1}", e.Name, e.Status)
        //        }),
        //        JsonRequestBehavior.AllowGet);
        //}

        //private IEnumerable<ProfessionalMaster> GetFromProfessionalExistingSession()
        //{
        //    //Get existing session variable
        //    var sessionVariable = System.Web.HttpContext.Current.Session["ProfessionalMasterMonthlyFees"] as IEnumerable<ProfessionalMaster>;
        //    var professionalmonthlyfees = sessionVariable ?? assetLoanAndReturnService.GetProfessionalInfo();

        //    if (sessionVariable == null) // If no session variable is found
        //        System.Web.HttpContext.Current.Session["ProfessionalMasterMonthlyFees"] = professionalmonthlyfees;
        //    return professionalmonthlyfees;
        //}

        //public ActionResult GetProfessionalInfo(int IDProfessional)
        //{
        //    var siteUrl = SessionManager.Get<string>("SiteUrl");
        //    int? IDProf = IDProfessional;
        //    var professionalInfo = assetLoanAndReturnService.GetProfessionalInfo(IDProf, siteUrl);

        //    var professionals = GetFromExistingSession();
        //    return Json(
        //        new
        //        {
        //            professionalInfo.ID,
        //            professionalInfo.ProjectName,
        //            professionalInfo.ContactNo

        //        }, JsonRequestBehavior.AllowGet);
        //}

        [HttpPost]
        public ActionResult Submit(AssetLoanAndReturnHeaderVM _data, string siteUrl)
        {
            // Get existing session variable
            //var sessionVariables = SessionManager.Get<DataTable>("CSVDataTable") ?? new DataTable();
            //var siteUrl = SessionManager.Get<string>("SiteUrl");

            siteUrl = SessionManager.Get<string>("SiteUrl");
            assetLoanAndReturnService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            int? headerID = null;
            try
            {
                headerID = assetLoanAndReturnService.CreateHeader(_data);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

        //    //cek apakah header / item
        //    int? latestIDHeader = 0;
        //    int? latestIDDetail = 0;
        //    List<int> idsHeader = new List<int>();
        //    List<int> idsDetail = new List<int>();
        //    var TableHeader = new DataTable();
        //    var TableDetail = new DataTable();

        //    var listNameHeader = "Asset Loan And Return";
        //    var listNameDetail = "Asset Loan And Return Detail";
        //    var listAssetMaster = "Asset Master";
        //    var listProfessionalMAster = "Professional Master";

        //    foreach (DataRow d in SessionManager.Get<DataTable>("CSVDataTable").Rows)
        //    {
        //        if (d.ItemArray[0].ToString() == "Asset Loan And Return")
        //        {
        //            //try
        //            //{
        //            //    var IDProf = assetLoanAndReturnService.getListIDOfList(listProfessionalMAster, "ID", "Title", siteUrl);
        //            //    int myKey = 0;

        //            //    foreach (var id in IDProf)
        //            //    {
        //            //        try
        //            //        {
        //            //            if (id.Value == d.ItemArray[2].ToString())
        //            //            {
        //            //                myKey = id.Key;
        //            //                break;
        //            //            }
        //            //        }
        //            //        catch (Exception e)
        //            //        {
        //            //            return JsonHelper.GenerateJsonErrorResponse("No Lookup Value/s is Found!");
        //            //        }
        //            //    }

        //            TableHeader = new DataTable();
        //            TableHeader.Columns.Add("Title", typeof(string));
        //            TableHeader.Columns.Add("Professional", typeof(int));
        //            TableHeader.Columns.Add("Project_x002f_Unit", typeof(string));
        //            TableHeader.Columns.Add("Contact_x0020_No", typeof(string));
        //            TableHeader.Columns.Add("Loan_x0020_Date", typeof(string));
        //            TableHeader.Columns.Add("Purpose", typeof(string));

        //            DataRow row = TableHeader.NewRow();

        //            row["Title"] = d.ItemArray[0].ToString();
        //            row["Professional"] = myKey;
        //            if (d.ItemArray[3].ToString() == "-1")
        //            {
        //                row["Project_x002f_Unit"] = null;
        //            }
        //            else
        //            {
        //                row["Project_x002f_Unit"] = d.ItemArray[3].ToString();
        //            }

        //            row["Contact_x0020_No"] = d.ItemArray[4].ToString();
        //            row["Loan_x0020_Date"] = d.ItemArray[5].ToString();
        //            row["Purpose"] = d.ItemArray[6].ToString();

        //            TableHeader.Rows.InsertAt(row, 0);

        //            latestIDHeader = assetLoanAndReturnService.MassUploadHeaderDetail(listNameHeader, TableHeader, siteUrl);
        //            idsHeader.Add(Convert.ToInt32(latestIDHeader));

        //        }

        //            catch (Exception e)
        //        {
        //            if (idsHeader.Count > 0)
        //            {
        //                foreach (var id in idsHeader)
        //                {
        //                    //delete parent
        //                    assetLoanAndReturnService.RollbackParentChildrenUpload(listNameHeader, id, siteUrl);
        //                }
        //            }
        //            else if (idsDetail.Count > 0)
        //            {
        //                foreach (var id in idsDetail)
        //                {
        //                    //delete parent
        //                    assetLoanAndReturnService.RollbackParentChildrenUpload(listNameDetail, id, siteUrl);
        //                }

        //            }
        //            return JsonHelper.GenerateJsonErrorResponse("Invalid data, rolling back!");
        //        }

        //        //if (d.ItemArray[8].ToString() != "" && latestIDHeader != null)
        //        //{
        //        //    TableDetail = new DataTable();
        //        //    TableDetail.Columns.Add("Asset_x0020_Loan_x0020_Return_x0", typeof(string));
        //        //    TableDetail.Columns.Add("Asset_x002d_Sub_x0020_Asset", typeof(string));
        //        //    TableDetail.Columns.Add("Est_x0020_Return_x0020_Date", typeof(string));
        //        //    TableDetail.Columns.Add("Return_x0020_Date", typeof(string));
        //        //    TableDetail.Columns.Add("Status", typeof(string));

        //        //    DataRow row = TableDetail.NewRow();
        //        //    row["Asset_x0020_Loan_x0020_Return_x0"] = latestIDHeader;
        //        //    row["Asset_x002d_Sub_x0020_Asset"] = d.ItemArray[8].ToString();
        //        //}

        //    }

        //}
            return JsonHelper.GenerateJsonSuccessResponse(siteUrl);
        }

}
}