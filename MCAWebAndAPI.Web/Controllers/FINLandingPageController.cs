using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Finance;
using MCAWebAndAPI.Service.Finance.RequisitionNote;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;

namespace MCAWebAndAPI.Web.Controllers
{
    public class FINLandingPageController : Controller
    {
        // GET: FINLandingPage
        public ActionResult Index(string siteUrl = null)
        {
            siteUrl = siteUrl ?? ConfigResource.DefaultBOSiteUrl;
            SessionManager.Set(SharedController.Session_SiteUrl, siteUrl);

            return View(Get(siteUrl));
        }

        public LandingPageVM Get(string siteUrl)
        {
            // TODO: perlu dibuat async supaya lebih responsive

            LandingPageVM result;
    
            result = new LandingPageVM();


            #region Budget vs Actual

            //TODO: budget and actual needs to be filled in at result.BudgetVsActualDisbursement
            //          next is to bind IEnumerable<LPBudgetVsActualDisbursementVM> BudgetVsActualDisbursement
            //          to Kendo Grid
            //      Source List: https://eceos2.sharepoint.com/sites/mca-dev/Lists/Budget%20VS%20Actual%20Disbursement%20MCDR/AllItems.aspx <== ini di Compact Program, bukan BO/HR!
            //      Kompleksitas permasalahan:
            //      - Data di List itu adalah data WBS, sementara yang ingin ditampilkan adalah data Activity ==> perlu join
            //      - Joinnya lewat List WBSMapping yang ada di Compact Program juga
            //      - Implementasi List "Budget VS Actual Disbursement MCDR" di Finance menggunakan cara yagn sama dengan WBS (pakai Common)
            //          + tiru persis aja cara WBS
            result.BudgetVsActualDisbursement = EventBudgetService.GetLatestMonthBudgetActualDisbursement(new DateTime(2016,1,1), ConfigResource.DefaultProgramSiteUrl);

            #endregion

            #region Outstanding Advances & SPHL

            result.OA_PIC_Number = OutstandingAdvanceService.GetCountForProfessionalAndIndependentConsultant();
            result.OA_PIC_Amount = OutstandingAdvanceService.GetAmountForProfessionalAndIndependentConsultant();

            result.OA_G_Number = OutstandingAdvanceService.GetCountForGrantee();
            result.OA_G_Amount = OutstandingAdvanceService.GetAmountForGranteet();

            result.SPHL = SPHLService.GetAmount();

            #endregion

            #region Tax Exemption & Reimbursement

            //TODO: pass siteUrl to all the rest
            //TODO: baru ada contoh implementasi di IncomeTax, perlu dilanjutkan dengan cara yang sama di VAT dan Others

            result.IncomeTax_Exemp_YTD = TaxExemptionDataService.GetIncomeTax_Exemp_YTD(siteUrl);
            result.IncomeTax_Reimb_YTD = TaxReimbursementService.GetIncomeTax_Reimb_YTD();
            result.VAT_Exemp_YTD = TaxExemptionDataService.GetVAT_Exemp_YTD(siteUrl);
            result.VAT_Reimb_YTD = TaxReimbursementService.GetVAT_Reimb_YTD();
            result.OtherTax_Exemp_YTD = TaxExemptionDataService.GetOtherTax_Exemp_YTD(siteUrl);
            result.OtherTax_Reimb_YTD = TaxReimbursementService.GetOtherTax_Reimb_YTD();

            result.IncomeTax_Exemp_YTD_Min1 = TaxExemptionDataService.GetIncomeTax_Exemp_YTD_Min1(siteUrl);
            result.IncomeTax_Reimb_YTD_Min1 = TaxReimbursementService.GetIncomeTax_Reimb_YTD_Min1();
            result.VAT_Exemp_YTD_Min1 = TaxExemptionDataService.GetVAT_Exemp_YTD_Min1(siteUrl);
            result.VAT_Reimb_YTD_Min1 = TaxReimbursementService.GetVAT_Reimb_YTD_Min1();
            result.OtherTax_Exemp_YTD_Min1 = TaxExemptionDataService.GetOtherTax_Exemp_YTD_Min1(siteUrl);
            result.OtherTax_Reimb_YTD_Min1 = TaxReimbursementService.GetOtherTax_Reimb_YTD_Min1();

            result.IncomeTax_Exemp_YTD_Min2 = TaxExemptionDataService.GetIncomeTax_Exemp_YTD_Min2(siteUrl);
            result.IncomeTax_Reimb_YTD_Min2 = TaxReimbursementService.GetIncomeTax_Reimb_YTD_Min2();
            result.VAT_Exemp_YTD_Min2 = TaxExemptionDataService.GetVAT_Exemp_YTD_Min2(siteUrl);
            result.VAT_Reimb_YTD_Min2 = TaxReimbursementService.GetVAT_Reimb_YTD_Min2();
            result.OtherTax_Exemp_YTD_Min2 = TaxExemptionDataService.GetOtherTax_Exemp_YTD_Min2(siteUrl);
            result.OtherTax_Reimb_YTD_Min2 = TaxReimbursementService.GetOtherTax_Reimb_YTD_Min2();

            #endregion

            return result;
        }
    }
}