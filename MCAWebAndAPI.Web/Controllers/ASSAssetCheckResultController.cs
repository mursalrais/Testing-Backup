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
using Microsoft.SharePoint.Client;

namespace MCAWebAndAPI.Web.Controllers
{
    public class ASSAssetCheckResultController : Controller
    {
        IAssetCheckResultService assetCheckResultService;

        public ASSAssetCheckResultController()
        {
            assetCheckResultService = new AssetCheckResultService();
        }

        // GET: AssetCheckResult
        public ActionResult Index()
        {
            return View();
        }

        //[HttpPost]
        public ActionResult Create(string siteUrl,
            AssetCheckResultHeaderVM data,
            string GetData,
            string Calculate,
            string SubmitForApproval,
            string SaveAsDraft,
            string Cancel
            )
        {
            assetCheckResultService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            if (!string.IsNullOrEmpty(GetData))
            {
                var viewModelGetData = assetCheckResultService.GetPopulatedModelGetData(Convert.ToInt32(data.FormID.Value));
                return View(viewModelGetData);
            }

            if (!string.IsNullOrEmpty(Calculate))
            {
                var viewModelCalculate = assetCheckResultService.GetPopulatedModelCalculate(data);
                return View(viewModelCalculate);
            }

            var viewModel = assetCheckResultService.GetPopulatedModel(null, data.FormID.Value);
            return View(viewModel);
        }

        public ActionResult GetProfessionalInfo(int IDProfessional)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            int? IDProf = IDProfessional;
            var professionalInfo = assetCheckResultService.GetProfessionalInfo(IDProf, siteUrl);

            //var professionals = GetFromExistingSession();
            return Json(
                new
                {
                    professionalInfo.ID,
                    professionalInfo.ProfessionalName,
                    professionalInfo.Posision

                }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCheckInfo(int IDAssetCheck)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            int? IDcheck = IDAssetCheck;
            var CheckInfo = assetCheckResultService.GetCheckInfo(IDcheck, siteUrl);

            //var professionals = GetFromExistingSession();
            return Json(
                new
                {
                    CheckInfo.ID,
                    CheckInfo.CompletionStatus

                }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Search()
        {
            var viewModel = new AssetCheckResultVM();
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditingPopup_Create([DataSourceRequest] DataSourceRequest request, AssetCheckResultItemVM _AssetCheckResultItemVM)
        {
            if (_AssetCheckResultItemVM != null && ModelState.IsValid)
            {
                assetCheckResultService.CreateAssetCheckResult_Dummy(_AssetCheckResultItemVM);
            }

            return Json(new[] { _AssetCheckResultItemVM }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditingPopup_Update()
        {
            var viewModel = new AssetCheckResultVM();

            return View(viewModel);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditingPopup_Destroy()
        {
            var viewModel = new AssetCheckResultVM();

            return View(viewModel);
        }
    }
}