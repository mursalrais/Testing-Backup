using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using MCAWebAndAPI.Service.Asset;
using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MCAWebAndAPI.Web.Controllers
{
    public class ASSAssetTransferController : Controller
    {
        IAssetTransferService assetTransferService;

        public ASSAssetTransferController()
        {
            assetTransferService = new AssetTransferService();
        }

        // GET: ASSAssetTransfer
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CreateAssetTransfer(string siteUrl = null)
        {
            // MANDATORY: Set Site URL
            assetTransferService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            // Get blank ViewModel
            var viewModel = assetTransferService.GetPopulatedModel();

            // Return to the name of the view and parse the model
            return View("CreateAssetTransfer", viewModel);
        }

        [HttpPost]
        public ActionResult SubmitAssetTransfer(FormCollection form, AssetTransferVM viewModel, string siteUrl)
        {
            siteUrl = SessionManager.Get<string>("SiteUrl");
            assetTransferService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            int? headerID = null;
            try
            {
                headerID = assetTransferService.CreateHeader(viewModel);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.Professional);
            //try
            //{
            //    viewModel.AssetTransferDetail = BindMonthlyFeeDetailDetails(form, viewModel.MonthlyFeeDetails);
            //    _hRPayrollService.CreateMonthlyFeeDetails(headerID, viewModel.MonthlyFeeDetails);
            //}
            //catch (Exception e)
            //{
            //    Response.StatusCode = (int)HttpStatusCode.BadRequest;
            //    return JsonHelper.GenerateJsonErrorResponse(e);
            //}            
        }



        public ActionResult GetAssetHolderFrom(int IDAssetHolderFrom)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            int IDAssetHolFrom = IDAssetHolderFrom;
            var assetHolFrom = assetTransferService.GetAssetHolderFromInfo(IDAssetHolFrom, siteUrl);

            //var professionals = GetFromExistingSession();
            return Json(
                new
                {
                    assetHolFrom.ID,
                    assetHolFrom.ProjectName,
                    assetHolFrom.ContactNo,
                    //assetHolFrom.VendorName,
                    //assetHolFrom.PoNo
                }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAssetHolderTo(int IDAssetHolderTo)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            int IDAssetHolFrom = IDAssetHolderTo;
            var assetHolFrom = assetTransferService.GetAssetHolderFromInfo(IDAssetHolFrom, siteUrl);

            //var professionals = GetFromExistingSession();
            return Json(
                new
                {
                    assetHolFrom.ID,
                    assetHolFrom.ProjectName,
                    assetHolFrom.ContactNo,
                    //assetHolFrom.VendorName,
                    //assetHolFrom.PoNo
                }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProvinceGrid()
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            assetTransferService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            var positions = GetFromPositionsExistingSession();

            return Json(positions.Select(e =>
                new
                {
                    Value = Convert.ToString(e.ID),
                    Text = e.RoomName
                }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLocationGrid()
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            assetTransferService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            var positions = GetFromPositionsExistingSession();

            return Json(positions.Select(e =>
                new
                {
                    Value = Convert.ToString(e.ID),
                    Text = e.RoomName
                }),
                JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<LocationMasterVM> GetFromPositionsExistingSession()
        {
            //Get existing session variable
            var sessionVariable = System.Web.HttpContext.Current.Session["Location%20Master"] as IEnumerable<LocationMasterVM>;
            var positions = sessionVariable ?? assetTransferService.GetProvince();

            if (sessionVariable == null) // If no session variable is found
                System.Web.HttpContext.Current.Session["Location%20Master"] = positions;
            return positions;
        }
    }
}