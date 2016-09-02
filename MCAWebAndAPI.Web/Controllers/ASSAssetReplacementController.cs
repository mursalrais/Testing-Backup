using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using MCAWebAndAPI.Service.Asset;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using MCAWebAndAPI.Web.Resources;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Service.Resources;
using System.Net;

namespace MCAWebAndAPI.Web.Controllers
{
    public class ASSAssetReplacementController : Controller
    {
        IAssetReplacementService _service;

        public ASSAssetReplacementController()
        {
            _service = new AssetReplacementService();
        }
        // GET: ASSAssetReplacement
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create(string siteUrl)
        {
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            var viewModel = _service.GetPopulatedModel();

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Submit(AssetReplacementHeaderVM _data, string siteUrl)
        {
            siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            if (_data.Details.Count() == 0)
            {
                Response.TrySkipIisCustomErrors = true;
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse("Details should not empty");
            }

            //return View(new AssetMasterVM());
            int? headerID = null;
            try
            {
                headerID = _service.CreateHeader(_data);
            }
            catch (Exception e)
            {
                Response.TrySkipIisCustomErrors = true;
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse("Failed To Save Header..");
            }

            try
            {
                _service.CreateDetails(headerID, _data.Details);
            }
            catch (Exception e)
            {
                _service.RollbackParentChildrenUpload("Asset Replacement", headerID, siteUrl);
                Response.TrySkipIisCustomErrors = true;
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse("Failed To Save Detail..");
            }
            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.AssetReplacement);
            //return Redirect(string.Format("{0}/{1}", siteUrl ?? ConfigResource.DefaultBOSiteUrl, UrlResource.AssetAcquisition));
        }

        public ActionResult Edit(int ID, string siteUrl)
        {
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            var viewModel = _service.GetHeader(ID);

            int? headerID = null;
            headerID = viewModel.Id;

            try
            {
                var viewdetails = _service.GetDetails(headerID);
                viewModel.Details = viewdetails;
            }
            catch (Exception e)
            {
                Response.TrySkipIisCustomErrors = true;
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse("Failed To Show Data For Update");
            }

            return View(viewModel);
        }

        public ActionResult View(int ID, string siteUrl)
        {
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            var viewModel = _service.GetHeader(ID);

            int? headerID = null;
            headerID = viewModel.Id;

            try
            {
                var viewdetails = _service.GetDetails(headerID);
                viewModel.Details = viewdetails;
            }
            catch (Exception e)
            {
                Response.TrySkipIisCustomErrors = true;
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse("Failed To Show Data For Update");
            }

            return View(viewModel);
        }

        public ActionResult Sync(string siteUrl)
        {
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            var viewModel = _service.GetPopulatedModel();
            return View(viewModel);
        }

        public ActionResult Syncronize(string siteUrl)
        {
            siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            try
            {
                var viewModel = _service.Syncronize(siteUrl);
            }
            catch (Exception e)
            {
                Response.TrySkipIisCustomErrors = true;
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse("Failed To Syncronize..");
            }

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.AssetAcquisition);
        }

        public ActionResult Update(AssetReplacementHeaderVM _data, string SiteUrl)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            try
            {
                _service.UpdateHeader(_data);
            }
            catch (Exception e)
            {
                Response.TrySkipIisCustomErrors = true;
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse("Failed To Update Header");
            }

            try
            {
                //update items
                _service.UpdateDetails(_data.Id, _data.Details);
            }
            catch (Exception e)
            {
                return JsonHelper.GenerateJsonErrorResponse("Failed To Update Detail");
            }

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.AssetAcquisition);
        }

        public ActionResult GetInfoFromAcquisitin(int id)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            int IDacquisition = id;
            var acquisition = _service.GetInfoFromAcquisitin(IDacquisition, siteUrl);
            //var details = _service.GetInfoFromAcquisitinDetail(IDacquisition, siteUrl);

            //var professionals = GetFromExistingSession();
            return Json(
                new
                {
                    acquisition.Vendor,
                    acquisition.Pono,
                    acquisition.purchasedatetext,
                    acquisition.purchaseDescription

                }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAll(int id, string SiteUrl)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            int IDacquisition = id;
            var acquisition = _service.GetInfoFromAcquisitin(IDacquisition, siteUrl);

            return View(acquisition);
        }

        public JsonResult GetAssetSubSAssetGrid()
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            var assetsubasset = GetFromPositionsExistingSession();

            return Json(assetsubasset.Select(e =>
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
            var positions = sessionVariable ?? _service.GetAssetSubAsset();

            if (sessionVariable == null) // If no session variable is found
                System.Web.HttpContext.Current.Session["AssetMaster"] = positions;
            return positions;
        }
    }
}