using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Elmah;
using MCAWebAndAPI.Model.ViewModel.Form.Finance.SPHL;
using MCAWebAndAPI.Service.Finance;
using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;

namespace MCAWebAndAPI.Web.Controllers
{
    public class FINSPHLController : Controller
    {
        ISPHLService service;
        private const string _siteUrl = "SiteUrl";

        public FINSPHLController()
        {
            service = new SPHLService();
        }

        public ActionResult CreateSPHL(string siteUrl = null)
        {
            siteUrl = siteUrl ?? ConfigResource.DefaultBOSiteUrl;

            service.SetSiteUrl(siteUrl);
            SessionManager.Set(_siteUrl, siteUrl);

            var viewModel = new SPHLVM();

            return View(viewModel);
        }

        public ActionResult EditSPHL(string siteUrl = null, int? ID=null)
        {
            siteUrl = siteUrl ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl);
            SessionManager.Set(_siteUrl, siteUrl);
            
            var viewModel = new SPHLVM();
            if (ID != null)
            {
                viewModel = service.GetDataSPHL(ID);
            }
            
            return View(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> CreateSPHL(FormCollection form, SPHLVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>(_siteUrl) ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl);

            try
            {
                if (!service.CheckExistingSPHLNo(viewModel.No))
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }

                int? ID = service.CreateSPHL(viewModel);
                Task createApplicationDocumentTask = service.CreateSPHLAttachmentAsync(ID, viewModel.No, viewModel.Documents);
                Task allTasks = Task.WhenAll(createApplicationDocumentTask);

                await allTasks;
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.FINSPHL);
        }

        [HttpPost]
        public async Task<ActionResult> EditSPHL(FormCollection form, SPHLVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>(_siteUrl) ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl);

            try
            {
                if (service.UpdateSPHL(viewModel))
                {
                    Task createApplicationDocumentTask = service.CreateSPHLAttachmentAsync(viewModel.ID, viewModel.No, viewModel.Documents);
                    Task allTasks = Task.WhenAll(createApplicationDocumentTask);

                    await allTasks;
                }
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.FINSPHL);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult CheckExistingSPHLNo(string no)
        {
            var siteUrl = SessionManager.Get<string>(_siteUrl) ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl);

            bool ifEmailExist = false;
            try
            {
                ifEmailExist = service.CheckExistingSPHLNo(no);
                return Json(ifEmailExist, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
    }
}