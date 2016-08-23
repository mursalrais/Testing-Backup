using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Finance;
using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;
using FinService = MCAWebAndAPI.Service.Finance;
using System.IO;
using MCAWebAndAPI.Service.Converter;
using Elmah;

namespace MCAWebAndAPI.Web.Controllers
{
    /// <summary>
    /// Wireframe FIN04: Event Budget
    /// </summary>

    public class FINEventBudgetController : Controller
    {
        private const string IndexPage = "Index";
        private const string Error = "Error";

        private const string Session_SiteUrl = "SiteUrl";

        private const string PRINT_PAGE_URL = "~/Views/FINEventBudget/Print.cshtml";

        IEventBudgetService service;

        public FINEventBudgetController()
        {
            service = new EventBudgetService();
        }


        public ActionResult Item(string siteUrl = null, int? id = null)
        {
            siteUrl = siteUrl ?? ConfigResource.DefaultBOSiteUrl;

            service.SetSiteUrl(siteUrl);
            SessionManager.Set(Session_SiteUrl, siteUrl);

            var viewModel = new EventBudgetVM();
            if (id.HasValue)
            {
                viewModel = service.Get(id);
            }
            
            SetAdditionalSettingToViewModel(ref viewModel, (id.HasValue ? false : true));

            return View(viewModel);
        }


        public JsonResult GetGLMaster()
        {
            var siteUrl = SessionManager.Get<string>(Session_SiteUrl);
            service.SetSiteUrl(siteUrl);

            var glMasters = FinService.SharedService.GetGLMaster(siteUrl);

            return Json(glMasters.Select(e => new
            {
                Value = e.ID.HasValue ? Convert.ToString(e.ID) : string.Empty,
                Text = e.Title
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEventBudgetList()
        {
            service.SetSiteUrl(SessionManager.Get<string>(Session_SiteUrl));

            var eventBudgets = service.GetEventBudgetList().ToList();

            //insert first empty data
            eventBudgets.Insert(0, new EventBudgetVM() { ID = 0, Title = "" });

            return Json(eventBudgets.Select(e => new
            {
                ID = e.ID,
                Title = (e.No + "-" + e.Title),
                Project = e.Project.Text
            }), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> Save(FormCollection form, EventBudgetVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>(Session_SiteUrl) ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl);

            int? headerId = null;

            try
            {
                if (viewModel.ID.HasValue && viewModel.ID.Value > 0)
                {
                    service.Update(viewModel);
                    headerId = viewModel.ID.Value;
                }
                else
                {
                    headerId = service.Create(viewModel);
                }

                Task CreateDetailsTask = service.CreateItemsAsync(headerId, viewModel.ItemDetails);
                Task CreateDocumentsTask = service.CreateAttachmentsAsync(headerId, viewModel.Documents);

                Task allTasks = Task.WhenAll(CreateDetailsTask, CreateDocumentsTask);

                await allTasks;
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.FINEventBudget);
        }

        private void SetAdditionalSettingToViewModel(ref EventBudgetVM viewModel, bool isCreate)
        {
            viewModel.Activity.ControllerName = "ComboBox";
            viewModel.Activity.ActionName = "GetActivitiesByProject";
            viewModel.Activity.ValueField = "Value";
            viewModel.Activity.TextField = "Text";
            viewModel.Activity.Cascade = "Project_Value";
            viewModel.Activity.Filter = "filterProject";

        }

        [HttpPost]
        public ActionResult Print(FormCollection form, EventBudgetVM viewModel)
        {
            string RelativePath = PRINT_PAGE_URL;

            var siteUrl = SessionManager.Get<string>(Session_SiteUrl);
            service.SetSiteUrl(siteUrl);
            viewModel = service.Get(viewModel.ID);

            ViewData.Model = viewModel;
            var view = ViewEngines.Engines.FindView(ControllerContext, RelativePath, null);
            var fileName = viewModel.No + "_Application.pdf";
            byte[] pdfBuf = null;
            string content;

            using (var writer = new StringWriter())
            {
                var context = new ViewContext(ControllerContext, view.View, ViewData, TempData, writer);
                view.View.Render(context, writer);
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
                    return JsonHelper.GenerateJsonErrorResponse(e);
                }
            }
            if (pdfBuf == null)
                return HttpNotFound();
            return File(pdfBuf, "application/pdf");
        }

    }
}