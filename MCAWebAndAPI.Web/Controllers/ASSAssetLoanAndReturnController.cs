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
using MCAWebAndAPI.Service.JobSchedulers.Schedulers;
using Elmah;


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
        // GET: ASSAssetCheckForm
        public ActionResult Index(string siteUrl)
        {
            siteUrl = SessionManager.Get<string>("SiteUrl");
            assetLoanAndReturnService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
           // SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            String url = (siteUrl ?? ConfigResource.DefaultBOSiteUrl) + UrlResource.AssetLoanAndReturn;

            return Content("<script>window.top.location.href = '" + url + "';</script>");
        }

        public ActionResult Create(string siteUrl)
        {
            assetLoanAndReturnService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            var viewModel = assetLoanAndReturnService.GetPopulatedModel(siteUrl);

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
                    Text = e.AssetSubAsset.Text
                }),
                JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<AssetAcquisitionItemVM> GetFromPositionsExistingSession()
        {
            //Get existing session variable
            var sessionVariable = System.Web.HttpContext.Current.Session["Asset%Asset%20Acquisition%20Details"] as IEnumerable<AssetAcquisitionItemVM>;
            var positions = sessionVariable ?? assetLoanAndReturnService.GetAssetSubAsset();

            if (sessionVariable == null) // If no session variable is found
                System.Web.HttpContext.Current.Session["Asset%Asset%20Acquisition%20Details"] = positions;
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

        public ActionResult GetProfessionalInfo(int IDProfessional)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            int? IDProf = IDProfessional;
            var professionalInfo = assetLoanAndReturnService.GetProfessionalInfo(IDProf, siteUrl);

            //var professionals = GetFromExistingSession();
            return Json(
                new
                {
                    professionalInfo.ID,
                    professionalInfo.ProjectName,
                    professionalInfo.ContactNo

                }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Submit(FormCollection form, AssetLoanAndReturnHeaderVM _data, string siteUrl)
        {
            // Get existing session variable
            //var sessionVariables = SessionManager.Get<DataTable>("CSVDataTable") ?? new DataTable();
            //var siteUrl = SessionManager.Get<string>("SiteUrl");

            siteUrl = SessionManager.Get<string>("SiteUrl");
            assetLoanAndReturnService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            int? headerID = null;
            try
            {
                headerID = assetLoanAndReturnService.CreateHeader(_data, siteUrl);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            try
            {
                // assetLoanAndReturnService.CreateDetails(headerID, _data.AssetLoanAndReturnItem);
                _data.AssetLoanAndReturnItem = BindMonthlyFeeDetailEstReturnDateDetails(form, _data.AssetLoanAndReturnItem);
                _data.AssetLoanAndReturnItem = BindMonthlyFeeDetailReturnDateDetails(form, _data.AssetLoanAndReturnItem);
                assetLoanAndReturnService.CreateDetails(headerID, _data.AssetLoanAndReturnItem);
            }
            catch (Exception e)
            {   
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }


            //return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.AssetLoanAndReturn);
            return RedirectToAction("Index");
        }

        private IEnumerable<AssetLoanAndReturnItemVM> BindMonthlyFeeDetailEstReturnDateDetails(FormCollection form, IEnumerable<AssetLoanAndReturnItemVM> monthlyFeeDetails)
        {
            var array = monthlyFeeDetails.ToArray();

            for (int i = 0; i < array.Length; i++)
            {
                array[i].EstReturnDate = BindHelper.BindDateInGrid("AssetLoanAndReturnItem",
                    i, "EstReturnDate", form);
            }
            return array;
        }

        private IEnumerable<AssetLoanAndReturnItemVM> BindMonthlyFeeDetailReturnDateDetails(FormCollection form, IEnumerable<AssetLoanAndReturnItemVM> monthlyFeeDetails)
        {
            var array = monthlyFeeDetails.ToArray();

            for (int i = 0; i < array.Length; i++)
            {
                //if (array[i].ReturnDate == null)
                //{
                //    array[i].ReturnDate = null;
                //}
                //else
                //{
                    array[i].ReturnDate = BindHelper.BindDateInGrid("AssetLoanAndReturnItem",
                        i, "ReturnDate", form);
                //}

            }
            return array;
        }

        public ActionResult Edit(int ID, string siteUrl)
        {
            assetLoanAndReturnService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            var viewModel = assetLoanAndReturnService.GetHeader(ID, siteUrl);

            int? headerID = null;
            headerID = viewModel.ID;

            try
            {
                var viewdetails = assetLoanAndReturnService.GetDetails(headerID);
                viewModel.AssetLoanAndReturnItem = viewdetails;
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Update(FormCollection form, AssetLoanAndReturnHeaderVM viewModel, string SiteUrl)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            assetLoanAndReturnService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            assetLoanAndReturnService.UpdateHeader(viewModel);

            try
            {
                viewModel.AssetLoanAndReturnItem = BindMonthlyFeeDetailEstReturnDateDetails(form, viewModel.AssetLoanAndReturnItem);
                viewModel.AssetLoanAndReturnItem = BindMonthlyFeeDetailReturnDateDetails(form, viewModel.AssetLoanAndReturnItem);
                //viewModel.AssetLoanAndReturnItem = BindMonthlyFeeDetailDetails(form, viewModel.AssetLoanAndReturnItem);
                //assetLoanAndReturnService.CreateDetails(viewModel.ID, viewModel.AssetLoanAndReturnItem);

                assetLoanAndReturnService.UpdateDetails(viewModel.ID, viewModel.AssetLoanAndReturnItem);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            //return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.AssetLoanAndReturn);
            return RedirectToAction("Index");
        }

        public ActionResult GetProfMasterInfo(string fullname, string position)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            var profmasterinfo = assetLoanAndReturnService.GetProfMasterInfo(fullname, siteUrl);

            //var professionals = GetFromExistingSession();
            return Json(
                new
                {
                    profmasterinfo.CurrentPosition,
                    profmasterinfo.MobileNumberOne
                }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AssetSchedulerExpired(string siteUrl = null)
        {

            try
            {
                AssetManagementScheduler.DoNowPSAExpired_OnceEveryDay(siteUrl);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction("Index", "Error");
            }
            return RedirectToAction("Index", "Success");
        }


    }
}