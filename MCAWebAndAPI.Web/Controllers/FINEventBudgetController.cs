using MCAWebAndAPI.Service.Finance;
using System.Web.Mvc;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;
using System.Linq;
using System;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using Elmah;
using FinService = MCAWebAndAPI.Service.Finance;

namespace MCAWebAndAPI.Web.Controllers
{
    public class FINEventBudgetController : FinSharedController
    {
        private const string IndexPage= "Index";
        private const string Error = "Error";

        private const string SESSION_SITE_URL = "SiteUrl";
        IEventBudgetService service;

        

        public FINEventBudgetController()
        {
            service = new EventBudgetService();
        }


        public ActionResult Create(string siteUrl = null, int? id = null)
        {
            siteUrl = siteUrl ?? ConfigResource.DefaultBOSiteUrl;

            service.SetSiteUrl(siteUrl);
            SessionManager.Set(SESSION_SITE_URL, siteUrl);

            var viewModel = service.Get(id);
            return View(viewModel);
        }

        public JsonResult GetGLMaster()
        {
            var siteUrl = SessionManager.Get<string>(SESSION_SITE_URL);
            service.SetSiteUrl(siteUrl);

            var glMasters = FinService.Shared.GetGLMaster(siteUrl);

            return Json(glMasters.Select(e => new
            {
                Value = e.ID.HasValue ? Convert.ToString(e.ID) : string.Empty,
                Text = e.Title
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEventBudgetList()
        {
            service.SetSiteUrl(SessionManager.Get<string>(SESSION_SITE_URL));

            var eventBudgets = service.GetEventBudgetList().ToList();

            //insert first empty data
            eventBudgets.Insert(0, new Model.ViewModel.Form.Finance.EventBudgetVM() { ID = 0, Title = "" });

            return Json(eventBudgets.Select(e => new
            {
                ID = e.ID,
                Title = (e.No +"-" + e.Title)
            }), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> CreateRequisitionNote(FormCollection form, EventBudgetVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>(SESSION_SITE_URL);
            service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            int? headerID = null;
            try
            {
                headerID = service.CreateEventBudget(viewModel);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction(IndexPage, Error, new { errorMessage = e.Message });
            }

            Task CreateDetailsTask = service.CreateItemsAsync(headerID, viewModel.ItemDetails);
            Task CreateDocumentsTask = service.CreateAttachmentsAsync(headerID, viewModel.Documents);

            Task allTasks = Task.WhenAll(CreateDetailsTask, CreateDocumentsTask);

            try
            {
                await allTasks;
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction(IndexPage, Error, new { errorMessage = e.Message });
            }

            return RedirectToAction(IndexPage,
                "Success",
                new
                {
                    errorMessage =
                string.Format(MessageResource.SuccessCreateApplicationData, headerID)
                });
        }
    }
}