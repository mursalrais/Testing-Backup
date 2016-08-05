using Kendo.Mvc.Extensions;
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
            //var viewModelItem = _assetAcquisitionService.GetPopulatedModelItem();
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
                new {
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
                new {
                    Value = Convert.ToString(e.ID),
                    Text = e.WBSID + " - " + e.WBDDesc
                }),
                JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<WBSMaterVM> GetFromWBSExistingSession()
        {
            //Get existing session variable
            var sessionVariable = System.Web.HttpContext.Current.Session["AssetAcquisition"] as IEnumerable<WBSMaterVM>;
            var positions = sessionVariable ?? _assetAcquisitionService.GetWBS();

            if (sessionVariable == null) // If no session variable is found
                System.Web.HttpContext.Current.Session["AssetAcquisition"] = positions;
            return positions;
        }
    }
}