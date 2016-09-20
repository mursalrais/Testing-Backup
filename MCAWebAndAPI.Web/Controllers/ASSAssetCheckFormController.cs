using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using MCAWebAndAPI.Service.Asset;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Elmah;
using MCAWebAndAPI.Service.Converter;
using MCAWebAndAPI.Service.Resources;

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
        public ActionResult Index(string siteUrl)
        {
            var siteUrlSession = SessionManager.Get<string>("SiteUrl");

            if (siteUrlSession == null)
            {
                assetCheckFormService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
                SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            }
            else
            {
                assetCheckFormService.SetSiteUrl(siteUrlSession ?? ConfigResource.DefaultBOSiteUrl);
            }

            String url = (siteUrl ?? ConfigResource.DefaultBOSiteUrl) + UrlResource.AssetCheckForm;

            return Content("<script>window.top.location.href = '" + url + "';</script>");
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
            var siteUrlSession = SessionManager.Get<string>("SiteUrl");

            if (siteUrlSession == null)
            {
                assetCheckFormService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
                SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            }else
            {
                assetCheckFormService.SetSiteUrl(siteUrlSession ?? ConfigResource.DefaultBOSiteUrl);
            }
            

            
            if (!string.IsNullOrEmpty(save))
            {
                int? formid = assetCheckFormService.save(data);
                return RedirectToAction("Index");
            }

            if(!string.IsNullOrEmpty(cancel))
            {
                return RedirectToAction("Index");
            }

            string office = data.Office.Value;
            string floor = data.Floor.Value;
            string room = data.Room.Value;

            

            var viewModel = assetCheckFormService.GetPopulatedModel(null, office, floor, room);

            return View(viewModel);
        }

        public ActionResult Edit(
            int? ID,
            string siteUrl,
            AssetCheckFormHeaderVM data,
            string save,
            string cancel)
        {
            var siteUrlSession = SessionManager.Get<string>("SiteUrl");

            if (siteUrlSession == null)
            {
                assetCheckFormService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
                SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            }
            else
            {
                assetCheckFormService.SetSiteUrl(siteUrlSession ?? ConfigResource.DefaultBOSiteUrl);
            }

            if (!string.IsNullOrEmpty(save))
            {
                int? formid = assetCheckFormService.EditSave(data);
                return RedirectToAction("Index");
            }

            if (!string.IsNullOrEmpty(cancel))
            {
                return RedirectToAction("Index");
            }

            var viewModel = assetCheckFormService.EditView(ID);

            return View(viewModel);
        }

        public ActionResult View(
            string siteUrl,
            AssetCheckFormHeaderVM data,
            int? ID,
            string print,
            string calculate)
        {
            var siteUrlSession = SessionManager.Get<string>("SiteUrl");

            if (siteUrlSession == null)
            {
                assetCheckFormService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
                SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            }
            else
            {
                assetCheckFormService.SetSiteUrl(siteUrlSession ?? ConfigResource.DefaultBOSiteUrl);
            }


            if (!string.IsNullOrEmpty(print))
            {
                const string RelativePath = "~/Views/ASSAssetCheckForm/Print.cshtml";
                var view = ViewEngines.Engines.FindView(ControllerContext, RelativePath, null);

                var fileName = data.hFormId.ToString() + "_AssetCheckForm.pdf";
                byte[] pdfBuf = null;
                string content;
                data.UrlImage = Request.Url.Scheme + "://" + Request.Url.Authority + Url.Content("~/img/logomca.png");
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
            if (!string.IsNullOrEmpty(calculate))
            {
                var viewModelDate = assetCheckFormService.GetPopulatedModelPrintDate(data.CreateDate);

                return View(viewModelDate);
            }

            var viewModel = assetCheckFormService.GetPopulatedModelPrint(ID);

            return View(viewModel);
        }
    }
}