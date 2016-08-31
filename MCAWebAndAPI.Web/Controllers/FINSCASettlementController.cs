using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Finance;
using MCAWebAndAPI.Service.Utils;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;

using static MCAWebAndAPI.Model.ViewModel.Form.Finance.Shared;


namespace MCAWebAndAPI.Web.Controllers
{
    /// <summary>
    /// Wirefram FIN07: SCA Settlement
    /// </summary>

    public class FINSCASettlementController : Controller
    {
        ISCASettlementService service;
        ISCAVoucherService serviceSCAVoucher;

        public FINSCASettlementController()
        {
            service = new SCASettlementService();
            serviceSCAVoucher = new SCAVoucherService();
        }

        public ActionResult Item(string siteUrl = null, string op = null, int? id = null)
        {
            siteUrl = siteUrl ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl);
            SessionManager.Set(SharedFinanceController.Session_SiteUrl, siteUrl);

            var viewModel = service.Get(GetOperation(op), id);

            SetAdditionalSettingToViewModel(ref viewModel);

            return View(viewModel);
        }

        private void SetAdditionalSettingToViewModel(ref SCASettlementVM viewModel)
        {
            viewModel.SCACouvher.ControllerName = "FINCombobox";
            viewModel.SCACouvher.ActionName = "GetSCAVouchers";
            viewModel.SCACouvher.ValueField = "Value";
            viewModel.SCACouvher.TextField = "Text";
            viewModel.SCACouvher.OnSelectEventName = "OnSelectSCAVoucher";

        }

        public ActionResult GetSCAVouchers(string ID)
        {
            List<SCAVoucherVM> r = new List<SCAVoucherVM>();
            List<SCAVoucherItemsVM> d = new List<SCAVoucherItemsVM>();
            decimal? total = 0;
            string totalInWord = string.Empty;

            serviceSCAVoucher.SetSiteUrl(ConfigResource.DefaultBOSiteUrl);
            service.SetSiteUrl(ConfigResource.DefaultBOSiteUrl);

            var header = serviceSCAVoucher.Get(Convert.ToInt32(ID));
            var detail = serviceSCAVoucher.GetSCAVoucherItems(Convert.ToInt32(ID)).ToList();

            foreach (var item in detail)
            {
                total += item.Amount;
            }

            totalInWord = FormatUtil.ConvertToEnglishWords(Convert.ToInt32(total), header.Currency);
            r.Add(header);

            return Json(r.Select(m =>
                new
                {
                    TotalAmount = total,
                    TotalAmountInWord = totalInWord,
                    Purpose = m.Purpose,
                    Project = m.Project,
                    ActivityID = m.ActivityID,
                    ActivityName = m.ActivityName,
                    Fund = 3000,
                    Currency = m.Currency
                }),
                JsonRequestBehavior.AllowGet);
        }

    }
}