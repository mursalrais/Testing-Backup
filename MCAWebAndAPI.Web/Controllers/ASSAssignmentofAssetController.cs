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
using System.Net;
using MCAWebAndAPI.Service.Resources;

namespace MCAWebAndAPI.Web.Controllers
{
    public class ASSAssignmentofAssetController : Controller
    {
        AssignmentofAssetService assignmentofAssetService;

        public ASSAssignmentofAssetController()
        {
            assignmentofAssetService = new AssignmentofAssetService();
        }

        // GET: ASSAssignmentofAsset
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create(string siteUrl = null)
        {
            assignmentofAssetService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            var viewModel = assignmentofAssetService.GetPopulatedModel();

            return View(viewModel);
        }


        [HttpPost]
        public ActionResult SubmitMonthlyFee(FormCollection form, AssignmentofAssetVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            assignmentofAssetService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            int? headerID = null;
            try
            {
                headerID = assignmentofAssetService.CreateHeader(viewModel);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            try
            {
                viewModel.AssignmentofAssets = BindMonthlyFeeDetailDetails(form, viewModel.AssignmentofAssets);
                //assignmentofAssetService.CreateMonthlyFeeDetails(headerID, viewModel.MonthlyFeeDetails);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + Url);
        }

        private IEnumerable<AssignmentofAssetDetailVM> BindMonthlyFeeDetailDetails(FormCollection form, IEnumerable<AssignmentofAssetDetailVM> assignmentofAssets)
        {
            throw new NotImplementedException();
        }
    }

   

    //public ActionResult Edit()
    //    {
    //        var viewModel = new AssignmentofAssetVM();

    //        return View(viewModel);
    //    }

    //    [AcceptVerbs(HttpVerbs.Post)]
    //    public ActionResult EditingPopup_Create([DataSourceRequest] DataSourceRequest request, AssignmentofAssetItemVM _AssignmentofAssetItemVM)
    //    {
    //        if (_AssignmentofAssetItemVM != null && ModelState.IsValid)
    //        {
    //            assignmentofAssetService.CreateAssignmentofAsset_Dummy(_AssignmentofAssetItemVM);
    //        }

    //        return Json(new[] { _AssignmentofAssetItemVM }.ToDataSourceResult(request, ModelState));
    //    }

    //    [AcceptVerbs(HttpVerbs.Post)]
    //    public ActionResult EditingPopup_Update()
    //    {
    //        var viewModel = new AssetScrappingVM();

    //        return View(viewModel);
    //    }

    //    [AcceptVerbs(HttpVerbs.Post)]
    //    public ActionResult EditingPopup_Destroy()
    //    {
    //        var viewModel = new AssetScrappingVM();

    //        return View(viewModel);
    //    }
    }
