using MCAWebAndAPI.Model.ViewModel.Form.Asset;
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
    public class ASSAssetCheckFormController : Controller
    {
        IAssetCheckFormService assetCheckFormService;

        
        public ASSAssetCheckFormController()
        {
            assetCheckFormService = new AssetCheckFormService();
        }

        // GET: ASSAssetCheckForm
        public ActionResult Index()
        {
            return View();
        }

        //public ActionResult Create()
        //{
        //    var viewModel = new AssetCheckFormVM();

        //    return View(viewModel);
        //}
        
        public ActionResult Create(
            string siteUrl, 
            AssetCheckFormHeaderVM data, 
            string save, 
            string cancel)
        {
            assetCheckFormService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            
            if (!string.IsNullOrEmpty(save))
            {
                int? formid = assetCheckFormService.save(data);
                return RedirectToAction("Create", "ASSAssetCheckForm", new { });
            }

            if(!string.IsNullOrEmpty(cancel))
            {
                return RedirectToAction("Create", "ASSAssetCheckForm", new { });
            }

            string office = data.Office.Value;
            string floor = data.Floor.Value;
            string room = data.Room.Value;

            

            var viewModel = assetCheckFormService.GetPopulatedModel(null, office, floor, room);

            return View(viewModel);
        }

    }
}