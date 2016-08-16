using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Elmah;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Converter;
using MCAWebAndAPI.Service.Finance;
using MCAWebAndAPI.Service.Finance.RequisitionNote;
using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;

namespace MCAWebAndAPI.Web.Controllers
{
    /// <summary>
    /// Wireframe FIN09: Outstanding Advance
    /// </summary>

    [Filters.HandleError]
    public class FINOutstandingAdvanceController : Controller
    {
        private const string SessionSiteUrl = "SiteUrl";

        readonly IOutstandingAdvanceService service;

        public FINOutstandingAdvanceController()
        {
            service = new OutstandingAdvanceService();
        }

        public ActionResult Item(string siteUrl = null, int? id = null)
        {
            siteUrl = siteUrl ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl);
            SessionManager.Set(SessionSiteUrl, siteUrl);

            var viewModel = service.Get(id);

            return View(viewModel);
        }


        [HttpPost]
        public async Task<ActionResult> Save(FormCollection form, OutstandingAdvanceVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>(SessionSiteUrl) ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl);

            try
            {
                int? id = service.Save(viewModel);
                Task createApplicationDocumentTask = service.SaveAttachmentAsync(id, viewModel.Reference, viewModel.Documents);
                Task allTasks = Task.WhenAll(createApplicationDocumentTask);

                await allTasks;
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.FINOutstandingAdvance);
        }

    }
}