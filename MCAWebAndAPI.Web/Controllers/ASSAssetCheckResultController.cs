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
using System.IO;
using Elmah;
using MCAWebAndAPI.Service.Converter;

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
        public ActionResult Index(string siteUrl)
        {
            assetCheckResultService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            return Redirect((siteUrl ?? ConfigResource.DefaultBOSiteUrl)+ Service.Resources.UrlResource.AssetCheckResult);
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


            if (!string.IsNullOrEmpty(SubmitForApproval))
            {
                var viewModelSaveAsDraft = assetCheckResultService.GetPopulatedModelSave(data, true);
                return RedirectToAction("Index");
            }


            if (!string.IsNullOrEmpty(SaveAsDraft))
            {
                var viewModelSaveAsDraft = assetCheckResultService.GetPopulatedModelSave(data);
                return RedirectToAction("Index");
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

        
        public ActionResult Edit(string siteUrl,
            AssetCheckResultHeaderVM data,
            int? ID,
            string Calculate,
            string SubmitForApproval,
            string SaveAsDraft,
            string Cancel
        )
        {

            assetCheckResultService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            
            if(data.ID != null)
            {
                ID = data.ID;
            }

            if (!string.IsNullOrEmpty(Calculate))
            {
                var viewModelCalculate = assetCheckResultService.GetPopulatedModelCalculate(data, ID);
                return View(viewModelCalculate);
            }

            if (!string.IsNullOrEmpty(SubmitForApproval))
            {
                var viewModelSaveAsDraft = assetCheckResultService.GetPopulatedModelSave(data, true,ID);
                return RedirectToAction("Index");
            }

            if (!string.IsNullOrEmpty(SaveAsDraft))
            {
                var viewModelSaveAsDraft = assetCheckResultService.GetPopulatedModelSave(data, false, ID);
                return RedirectToAction("Index");
            }

            var viewModel = assetCheckResultService.GetPopulatedModel(ID, data.FormID.Value);
            return View(viewModel);
        }




        public ActionResult View(string siteUrl,
            AssetCheckResultHeaderVM data,
            int? ID,
            string Print,
            Boolean RequestApproval = false
        )
        {
            if (RequestApproval && ID != null)
            {
                return RedirectToAction("Approve", new { ID = ID });
            }

            assetCheckResultService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            if (data.ID != null)
            {
                ID = data.ID;
            }

            if (!string.IsNullOrEmpty(Print))
            {
                const string RelativePath = "~/Views/ASSAssetCheckResult/Print.cshtml";
                var view = ViewEngines.Engines.FindView(ControllerContext, RelativePath, null);

                var fileName = "_AssetCheckForm.pdf";
                byte[] pdfBuf = null;
                string content;

                ControllerContext.Controller.ViewData.Model = data;
                ViewData = ControllerContext.Controller.ViewData;
                TempData = ControllerContext.Controller.TempData;

                using (var writer = new StringWriter())
                {
                    var contextviewContext = new ViewContext(ControllerContext, view.View, ViewData, TempData, writer);
                    view.View.Render(contextviewContext, writer);
                    writer.Flush();
                    content = writer.ToString();

                    // Get PDF Bytes
                    try
                    {
                        pdfBuf = PDFConverter.Instance.ConvertFromHTML(fileName, content);
                    }
                    catch (Exception e)
                    {
                        ErrorSignal.FromCurrentContext().Raise(e);
                        RedirectToAction("Index", "Error");
                    }
                }
                if (pdfBuf == null)
                    return HttpNotFound();
                return File(pdfBuf, "application/pdf");
            }

            var viewModel = assetCheckResultService.GetPopulatedModel(ID, data.FormID.Value, data);
            return View(viewModel);
        }

        public ActionResult Approve(string siteUrl,
            AssetCheckResultHeaderVM data,
            int? ID,
            string Approve,
            string Reject
        )
        {

            assetCheckResultService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            
            if (!string.IsNullOrEmpty(Approve))
            {
                var viewModelSaveAsDraft = assetCheckResultService.Approve(ID);
                return RedirectToAction("Index");
            }

            if (!string.IsNullOrEmpty(Reject))
            {
                var viewModelSaveAsDraft = assetCheckResultService.Reject(ID);
                return RedirectToAction("Index");
            }

            var viewModel = assetCheckResultService.GetPopulatedModel(ID, data.FormID.Value, data);
            return View(viewModel);
        }


        //public ActionResult GetCheckInfo(int IDAssetCheck)
        //{
        //    var siteUrl = SessionManager.Get<string>("SiteUrl");
        //    int? IDcheck = IDAssetCheck;
        //    var CheckInfo = assetCheckResultService.GetCheckInfo(IDcheck, siteUrl);

        //    //var professionals = GetFromExistingSession();
        //    return Json(
        //        new
        //        {
        //            CheckInfo.ID,
        //            CheckInfo.CompletionStatus

        //        }, JsonRequestBehavior.AllowGet);
        //}

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