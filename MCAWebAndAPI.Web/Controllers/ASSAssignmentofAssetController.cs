using MCAWebAndAPI.Service.Asset;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MCAWebAndAPI.Web.Controllers
{
    public class ASSAssignmentofAssetController : Controller
    {

        //IAssignmentofAssetService _assignmentOfAssetService;

        //public ASSAssignmentofAssetController()
        //{
        //    _assignmentOfAssetService = new AssignmentofAssetService();
        //}


        //public ActionResult Create(string SiteUrl)
        //{
        //    _assignmentOfAssetService.SetSiteUrl(SiteUrl ?? ConfigResource.DefaultBOSiteUrl);
        //    SessionManager.Set("SiteUrl", SiteUrl ?? ConfigResource.DefaultBOSiteUrl);
        //    _assignmentOfAssetService.GetPopulatedModel(SiteUrl);
        //    return View();
        //}
        //[HttpPost]
        //public ActionResult Submit(string SiteUrl)
        //{
        //    SiteUrl = SessionManager.Get<string>("SiteUrl");
        //    _assignmentOfAssetService.SetSiteUrl(SiteUrl ?? ConfigResource.DefaultBOSiteUrl);
        //    _assignmentOfAssetService.GetPopulatedModel(SiteUrl);
        //    return View();
        //}
    }
}