using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Elmah;
using Kendo.Mvc.Extensions;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Converter;
using MCAWebAndAPI.Service.Finance;
using MCAWebAndAPI.Service.Finance.RequisitionNote;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;
using FinService = MCAWebAndAPI.Service.Finance;
using System.Globalization;

namespace MCAWebAndAPI.Web.Controllers
{
    /// <summary>
    /// Wireframe FIN04: Event Budget
    /// </summary>

    public class FINEventBudgetController : Controller
    {
        private const string IndexPage = "Index";
        private const string Error = "Error";

        private const string SuccessMsgFormatUpdated = "Event Budget number {0} has been successfully updated.";
        private const string FirstPageUrl = "{0}/Lists/Event%20Budget/AllItems.aspx";
        private const string PrintPageUrl = "~/Views/FINEventBudget/Print.cshtml";

        IEventBudgetService service;
        IRequisitionNoteService requisitionNoteService;

        public ActionResult Item(string siteUrl = null, int? id = null, string userEmail = "")
        {
            if (userEmail==string.Empty)
            {
                throw new InvalidOperationException("Invalid parameter: userEmail.");
            }

            siteUrl = siteUrl ?? ConfigResource.DefaultBOSiteUrl;
            SessionManager.Set(SharedController.Session_SiteUrl, siteUrl);

            service = new EventBudgetService(siteUrl);

            var viewModel = new EventBudgetVM();
            viewModel.UserEmail = userEmail;

            if (id.HasValue)
            {
                viewModel = service.Get(id);

                Tuple<int, string> rn = new RequisitionNoteService(siteUrl).GetIdAndNoByEventBudgetID(viewModel.ID.Value);

                viewModel.RequisitionNoteId = rn.Item1;
                viewModel.RequisitionNoteNo = rn.Item2;


                Tuple<int, string> sca = new SCAVoucherService(siteUrl).GetIdAndNoByEventBudgetID(viewModel.ID.Value);

                viewModel.SCAVoucherId = sca.Item1;
                viewModel.SCAVoucherNo = sca.Item2;


            }

            SetAdditionalSettingToViewModel(ref viewModel, (id.HasValue ? false : true));

            ViewBag.CancelUrl = string.Format(FirstPageUrl, siteUrl);
            return View(viewModel);
        }

        public JsonResult GetGLMaster()
        {
            var siteUrl = SessionManager.Get<string>(SharedController.Session_SiteUrl);
            service = new EventBudgetService(siteUrl);

            var glMasters = FinService.SharedService.GetGLMaster(siteUrl);

            return Json(glMasters.Select(e => new
            {
                Value = e.ID.HasValue ? Convert.ToString(e.ID) : string.Empty,
                Text = e.Title
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEventBudgetList()
        {

            service = new EventBudgetService(SessionManager.Get<string>(SharedController.Session_SiteUrl));

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
        public async Task<ActionResult> Save(string actionType, FormCollection form, EventBudgetVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>(SharedController.Session_SiteUrl) ?? ConfigResource.DefaultBOSiteUrl;
            service = new EventBudgetService(siteUrl);

            if (actionType != "Save")
            {
                return Redirect(string.Format(FirstPageUrl, siteUrl));
            }

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
                ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction("Index", "Error", new { errorMessage = e.Message });
            }

            List<Task> updateTaskRNOrSCA = new List<Task>();
            // Update related Requisition Note & SCA Voucher, can't do like this because in every metod there are id checking id = 0 which will trheo error.
            if (viewModel.RequisitionNoteId > 0)
            {
                updateTaskRNOrSCA.Add(service.UpdateRequisitionNoteAsync(siteUrl, viewModel.RequisitionNoteId));
            }

            if(viewModel.SCAVoucherId > 0)
            {
                updateTaskRNOrSCA.Add(service.UpdateSCAVoucherAsync(siteUrl, viewModel.SCAVoucherId));
            }

            if(updateTaskRNOrSCA.Count > 0)
            {
                Task allTasks2 = Task.WhenAll(updateTaskRNOrSCA.ToArray());
                await allTasks2;
            }

            return RedirectToAction("Index", "Success",
                new
                {
                    successMessage = string.Format(SuccessMsgFormatUpdated, viewModel.No),
                    previousUrl = string.Format(FirstPageUrl, siteUrl)
                });
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

        public ActionResult Print(FormCollection form, EventBudgetVM viewModel, string userEmail = "")
        {
            string RelativePath = PrintPageUrl;
            string domain = new SharedFinanceController().GetImageLogoPrint(Request.IsSecureConnection,Request.Url.Authority);

            var siteUrl = SessionManager.Get<string>(SharedController.Session_SiteUrl);
            service = new EventBudgetService(siteUrl);
            viewModel = service.Get(viewModel.ID);

            ViewData.Model = viewModel;
            var view = ViewEngines.Engines.FindView(ControllerContext, RelativePath, null);
            var fileName = viewModel.No + "_Application.pdf";
            byte[] pdfBuf = null;
            string content;

            string footer = string.Empty;

            //TODO: Resolve user name
            var allProfs = COMProfessionalController.GetAll();
            Model.HR.DataMaster.ProfessionalMaster user = allProfs.FirstOrDefault(x => x.OfficeEmail == userEmail);
            string userName = "xxxx";
            if (user != null)
                userName = user.Name;

            DateTime dt = DateTime.ParseExact(Request.Form[nameof(viewModel.ClientDateTime)].ToString().Substring(0, 24), "ddd MMM d yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            if (dt == DateTime.MinValue)
            {
                //server's time better than no time.
                dt = DateTime.Now;
            }
            footer = string.Format("This form was printed by {0}, {1:MM/dd/yyyy}, {2:HH:mm}", userName, dt, dt);

            using (var writer = new StringWriter())
            {
                var context = new ViewContext(ControllerContext, view.View, ViewData, TempData, writer);
                view.View.Render(context, writer);
                writer.Flush();
                content = writer.ToString();

                content = content.Replace("{XIMGPATHX}", domain);
                // Get PDF Bytes
                try
                {
                    pdfBuf = PDFConverter.Instance.ConvertFromHTML(fileName, content, footer);
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