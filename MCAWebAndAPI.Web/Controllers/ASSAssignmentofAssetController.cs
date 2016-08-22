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
    public class ASSAssignmentOfAssetController : Controller
    {

        IAssignmentOfAssetService _service;

        public ASSAssignmentOfAssetController()
        {
            _service = new AssignmentOfAssetService();
        }

        // GET: AssignmentOfAsset
        public ActionResult Create(string SiteUrl)
        {
            _service.SetSiteUrl(SiteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set("SiteUrl", SiteUrl ?? ConfigResource.DefaultBOSiteUrl);
            var viewModel = _service.GetPopulatedModel(SiteUrl);
            return View(viewModel);
        }

        public ActionResult Edit(int ID, string siteUrl)
        {
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            var viewModel = _service.GetHeader(ID);

            int? headerID = null;
            headerID = viewModel.ID;

            try
            {
                var viewdetails = _service.GetDetails(headerID);
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
        public ActionResult Submit(AssignmentOfAssetVM _data, string siteUrl)
        {
            siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            //return View(new AssetMasterVM());
            int? headerID = null;
            try
            {
                headerID = _service.CreateHeader(_data, siteUrl);
                //_service.CreateDocuments(headerID, _data.Attachment, siteUrl);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            try
            {
                _service.CreateDetails(headerID, _data.Details);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }
            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.AssetAcquisition);
            //return Redirect(string.Format("{0}/{1}", siteUrl ?? ConfigResource.DefaultBOSiteUrl, UrlResource.AssetAcquisition));
        }

        [HttpPost]
        public ActionResult Update(AssignmentOfAssetVM _data, string SiteUrl)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            try
            {
                //_service.UpdateHeader(_data);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            try
            {
                //update items
                //_service.UpdateDetails(_data.ID, _data.Details);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.AssetAcquisition);
        }

        public ActionResult GetProfMasterInfo(string fullname, string position)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            var profmasterinfo = _service.GetProfMasterInfo(fullname, siteUrl);

            //var professionals = GetFromExistingSession();
            return Json(
                new
                {
                    profmasterinfo.CurrentPosition,
                    profmasterinfo.MobileNumberOne
                }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAssetSubSAssetGrid()
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

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
            var positions = sessionVariable ?? _service.GetAssetSubAsset();

            if (sessionVariable == null) // If no session variable is found
                System.Web.HttpContext.Current.Session["Asset%Asset%20Acquisition%20Details"] = positions;
            return positions;
        }

        public JsonResult GetProvinceGrid()
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            var positions = GetProvinceExistingSession();

            return Json(positions.Select(e =>
                new
                {
                    Value = Convert.ToString(e.ID),
                    Text = e.Province.Text
                }),
                JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<LocationMasterVM> GetProvinceExistingSession()
        {
            //Get existing session variable
            var sessionVariable = System.Web.HttpContext.Current.Session["Location%20Master"] as IEnumerable<LocationMasterVM>;
            var positions = sessionVariable ?? _service.GetProvince();

            if (sessionVariable == null) // If no session variable is found
                System.Web.HttpContext.Current.Session["Location%20Master"] = positions;
            return positions;
        }

        public ActionResult GetProvinceInfo(string province, string SiteUrl)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            var info = _service.GetProvinceInfo(province, siteUrl);

            //var professionals = GetFromExistingSession();
            return Json(
                new
                {
                    info.ID,
                    info.OfficeName,
                    info.FloorName,
                    info.RoomName,
                    info.Remarks
                }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOfficeNameLists(string province, string SiteUrl)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");

            var officeName = _service.GetOfficeName(province, siteUrl);
            officeName = officeName.OrderBy(e => e.Province);

            return Json(officeName.Select(e =>
                new
                {
                    e.ID
                }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOfficeGrid(string province = null)
        {
            SessionManager.Set("Province", province);
            var pro = SessionManager.Get<string>("Province");
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            var positions = _service.GetOfficeName(siteUrl, province);

            return Json(positions.Select(e =>
                new
                {
                    Value = Convert.ToString(e.ID),
                    Text = e.OfficeName
                }),
                JsonRequestBehavior.AllowGet);
        }

    }
}