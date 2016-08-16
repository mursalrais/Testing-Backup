using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
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
    public class ASSAssignmentofAssetController : Controller
    {

        IAssignmentofAssetService _assignmentOfAssetService;

        public ASSAssignmentofAssetController()
        {
            _assignmentOfAssetService = new AssignmentofAssetService();
        }

        public ActionResult Create(string siteUrl)
        {
            _assignmentOfAssetService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            var viewModel = _assignmentOfAssetService.GetPopulatedModel();
            return View(viewModel);
        }

        public ActionResult Edit(int ID, string siteUrl)
        {
            _assignmentOfAssetService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            var viewModel = _assignmentOfAssetService.GetHeader(ID);

            int? headerID = null;
            headerID = viewModel.ID;

            try
            {
                var viewdetails = _assignmentOfAssetService.GetDetails(headerID);
                viewModel.Details = viewdetails;
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult SubmitAsset(AssignmentofAssetVM _data, string siteUrl)
        {
            siteUrl = SessionManager.Get<string>("SiteUrl");
            _assignmentOfAssetService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            //return View(new AssetMasterVM());
            int? headerID = null;
            try
            {
                headerID = _assignmentOfAssetService.CreateHeader(_data);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            try
            {
                _assignmentOfAssetService.CreateDetails(headerID, _data.Details);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }
            //return JsonHelper.GenerateJsonSuccessResponse(siteUrl ?? ConfigResource.DefaultBOSiteUrl + UrlResource.AssignmentofAsset);
            return Redirect(string.Format("{0}/{1}", siteUrl ?? ConfigResource.DefaultBOSiteUrl, UrlResource.AssetAcquisition));
        }

        public ActionResult Update(AssignmentofAssetVM _data, string SiteUrl)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _assignmentOfAssetService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            try
            {
                _assignmentOfAssetService.UpdateHeader(_data);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            try
            {
                //update items
                _assignmentOfAssetService.UpdateDetails(_data.ID, _data.Details);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            //return JsonHelper.GenerateJsonSuccessResponse(string.Format("{0}/{1}", SiteUrl, UrlResource.AssignmentofAsset));
            return Redirect(string.Format("{0}/{1}", siteUrl ?? ConfigResource.DefaultBOSiteUrl, UrlResource.AssetAcquisition));
        }

        public JsonResult GetAssetSubSAssetGrid()
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _assignmentOfAssetService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

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
            var positions = sessionVariable ?? _assignmentOfAssetService.GetAssetSubAsset();

            if (sessionVariable == null) // If no session variable is found
                System.Web.HttpContext.Current.Session["AssetMaster"] = positions;
            return positions;
        }


        public JsonResult GetLocationMaster()
        {
            _assignmentOfAssetService.SetSiteUrl(SessionManager.Get<string>("SiteUrl"));

            var professionalmonthlyfee = GetFromLocationMasterExistingSession();
            return Json(professionalmonthlyfee.Select(e =>
                new
                {

                    Value = Convert.ToString(e.ID),
                    Text = e.Province.Text


                }),
                JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<LocationMasterVM> GetFromLocationMasterExistingSession()
        {
            //Get existing session variable
            var sessionVariable = System.Web.HttpContext.Current.Session["Location%20Master"] as IEnumerable<LocationMasterVM>;
            var locationMaster = sessionVariable ?? _assignmentOfAssetService.GetLocationMaster();

            if (sessionVariable == null) // If no session variable is found
                System.Web.HttpContext.Current.Session["Location%20Master"] = locationMaster;
            return locationMaster;
        }


        public JsonResult GetOfficeGrid()
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _assignmentOfAssetService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            var positions = GetFromOfficeExistingSession();

            return Json(positions.Select(e =>
                new
                {
                    Value = Convert.ToString(e.ID),
                    Text = e.OfficeName
                }),
                JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<LocationMasterVM> GetFromOfficeExistingSession()
        {
            //Get existing session variable
            var sessionVariable = System.Web.HttpContext.Current.Session["Location%20Master"] as IEnumerable<LocationMasterVM>;
            var positions = sessionVariable ?? _assignmentOfAssetService.GetLocationMaster();

            if (sessionVariable == null) // If no session variable is found
                System.Web.HttpContext.Current.Session["Location%20Master"] = positions;
            return positions;
        }

        public JsonResult GetFloorGrid()
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _assignmentOfAssetService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            var positions = GetFromFloorExistingSession();

            return Json(positions.Select(e =>
                new
                {
                    Value = Convert.ToString(e.ID),
                    Text = e.FloorName
                }),
                JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<LocationMasterVM> GetFromFloorExistingSession()
        {
            //Get existing session variable
            var sessionVariable = System.Web.HttpContext.Current.Session["Location%20Master"] as IEnumerable<LocationMasterVM>;
            var positions = sessionVariable ?? _assignmentOfAssetService.GetLocationMaster();

            if (sessionVariable == null) // If no session variable is found
                System.Web.HttpContext.Current.Session["Location%20Master"] = positions;
            return positions;
        }

        public JsonResult GetRoomGrid()
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _assignmentOfAssetService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            var positions = GetFromRoomExistingSession();

            return Json(positions.Select(e =>
                new
                {
                    Value = Convert.ToString(e.ID),
                    Text = e.RoomName
                }),
                JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<LocationMasterVM> GetFromRoomExistingSession()
        {
            //Get existing session variable
            var sessionVariable = System.Web.HttpContext.Current.Session["Location%20Master"] as IEnumerable<LocationMasterVM>;
            var positions = sessionVariable ?? _assignmentOfAssetService.GetLocationMaster();

            if (sessionVariable == null) // If no session variable is found
                System.Web.HttpContext.Current.Session["Location%20Master"] = positions;
            return positions;
        }

        public JsonResult GetRemarkGrid()
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _assignmentOfAssetService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            var positions = GetFromRemarkExistingSession();

            return Json(positions.Select(e =>
                new
                {
                    Value = Convert.ToString(e.ID),
                    Text = e.Remarks
                }),
                JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<LocationMasterVM> GetFromRemarkExistingSession()
        {
            //Get existing session variable
            var sessionVariable = System.Web.HttpContext.Current.Session["Location%20Master"] as IEnumerable<LocationMasterVM>;
            var positions = sessionVariable ?? _assignmentOfAssetService.GetLocationMaster();

            if (sessionVariable == null) // If no session variable is found
                System.Web.HttpContext.Current.Session["Location%20Master"] = positions;
            return positions;
        }

        // GET: ASSAsignmentofAsset
        public ActionResult Index()
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _assignmentOfAssetService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            //return Redirect(string.Format("{0}/{1}", siteUrl ?? ConfigResource.DefaultBOSiteUrl, UrlResource.AssignmentofAsset));
            return Redirect(string.Format("{0}/{1}", siteUrl ?? ConfigResource.DefaultBOSiteUrl, UrlResource.AssetAcquisition));
        }
    }

}