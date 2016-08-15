using MCAWebAndAPI.Model.ViewModel.Control;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Finance;
using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MCAWebAndAPI.Web.Controllers
{
    /// <summary>
    /// Wireframe FIN11: Petty Cash Settlement
    /// 
    ///     Petty Cash Settlement is a transaction for settlement-reimbursement of petty cash where 
    ///     user has already asked for petty cash advance previously. 
    ///     
    ///     Through this feature, user will create the settlement-reimbursement of 
    ///     petty cash which results whether user needs to return the excess petty cash advance or 
    ///     receive the reimbursement in the case where the actual expense for 
    ///     petty cash exceeds the petty cash advance given. 
    ///     
    ///     It is created and maintained by finance. 																									
    ///
    /// </summary>

    public class FINPettyCashSettlementController : Controller
    {
        IPettyCashSettlement service;

        public FINPettyCashSettlementController()
        {
            service = new PettyCashSettlement();
        }

        public ActionResult Create(string siteUrl = null)
        {
            siteUrl = siteUrl ?? ConfigResource.DefaultBOSiteUrl;

            service.SetSiteUrl(siteUrl);
            SessionManager.Set("SiteUrl", siteUrl);

            var viewModel = new PettyCashSettlementVM();

            viewModel.PettyCasVoucher = new AjaxCascadeComboBoxVM
            {
                ControllerName = "FINPettyCashSettlement",
                ActionName = "GetPettyCashVoucher",
                ValueField = "ID",
                TextField = "Title"
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(FormCollection form, PettyCashSettlementVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl") ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            int? ID = null;
            ID = service.Create(viewModel);
            // Task createApplicationDocumentTask = service.CreateAttachmentAsync(ID, viewModel.Documents);
            // Task allTasks = Task.WhenAll(createApplicationDocumentTask);

            //try
            //{
            //    await allTasks;
            //}
            //catch (Exception e)
            //{
            //    ErrorSignal.FromCurrentContext().Raise(e);
            //    return RedirectToAction("Index", "Error", new { errorMessage = e.Message });
            //}

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.FINSPHL);
        }

        public JsonResult GetPettyCashVoucher(int? id, string title)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl") ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl);


            var PettyCashSettlements = new List<PettyCashSettlementVM>();

            //TODO: move to service class
            //PettyCashSettlements.Add(new PettyCashSettlementVM() { ID = -1, Title = string.Empty });

            //foreach (var item in SPConnector.GetList(PettyCashSettlement.ListName, siteUrl, null))
            //{
            //    PettyCashSettlements.Add(ConvertToPettyCashSettlementModel(item));
            //}

            return Json(PettyCashSettlements.Select(e => new
            {
                e.ID,
                e.Title
            }), JsonRequestBehavior.AllowGet);
        }


        //TODO: move to service class
        //private static PettyCashSettlementVM ConvertToPettyCashSettlementModel(SPClient.ListItem item)
        //{
        //    return new PettyCashSettlementVM
        //    {
        //        //ID = Convert.ToInt32(item[FIELD_ID]),
        //        //Title = Convert.ToString(item[FIELD_TITLE]),


        //    };
        //}

    }
}