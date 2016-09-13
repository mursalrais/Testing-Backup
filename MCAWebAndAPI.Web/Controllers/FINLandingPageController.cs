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

           
            result.BudgetVsActualDisbursement = EventBudgetService.GetLatestMonthBudgetActualDisbursement(DateTime.Now, ConfigResource.DefaultProgramSiteUrl);
            
            #endregion

            #region Outstanding Advances & SPHL
            var outstandingProfesional = OutstandingAdvanceService.GetChartDataOfProfessionalAndIndependentConsultant(siteUrl);
            result.OA_PIC_Number = outstandingProfesional.Count;
            result.OA_PIC_Amount = outstandingProfesional.Amount;

            var outstandingGrantee = OutstandingAdvanceService.GetChartDataOfGrantee(siteUrl);
            result.OA_G_Number = outstandingGrantee.Count;
            result.OA_G_Amount = outstandingGrantee.Amount;

            result.SPHL = SPHLService.GetAmount(siteUrl);
            #endregion

            #region Tax Exemption & Reimbursement

         
            result.IncomeTax_Exemp_YTD = TaxExemptionDataService.GetIncomeTax_Exemp_YTD(siteUrl);
            result.IncomeTax_Reimb_YTD = TaxReimbursementService.GetIncomeTax_Reimb_YTD(siteUrl);
            result.VAT_Exemp_YTD = TaxExemptionDataService.GetVAT_Exemp_YTD(siteUrl);
            result.VAT_Reimb_YTD = TaxReimbursementService.GetVAT_Reimb_YTD(siteUrl);
            result.OtherTax_Exemp_YTD = TaxExemptionDataService.GetOtherTax_Exemp_YTD(siteUrl);
            result.OtherTax_Reimb_YTD = TaxReimbursementService.GetOtherTax_Reimb_YTD(siteUrl);

            result.IncomeTax_Exemp_YTD_Min1 = TaxExemptionDataService.GetIncomeTax_Exemp_YTD_Min1(siteUrl);
            result.IncomeTax_Reimb_YTD_Min1 = TaxReimbursementService.GetIncomeTax_Reimb_YTD_Min1(siteUrl);
            result.VAT_Exemp_YTD_Min1 = TaxExemptionDataService.GetVAT_Exemp_YTD_Min1(siteUrl);
            result.VAT_Reimb_YTD_Min1 = TaxReimbursementService.GetVAT_Reimb_YTD_Min1(siteUrl);
            result.OtherTax_Exemp_YTD_Min1 = TaxExemptionDataService.GetOtherTax_Exemp_YTD_Min1(siteUrl);
            result.OtherTax_Reimb_YTD_Min1 = TaxReimbursementService.GetOtherTax_Reimb_YTD_Min1(siteUrl);

            result.IncomeTax_Exemp_YTD_Min2 = TaxExemptionDataService.GetIncomeTax_Exemp_YTD_Min2(siteUrl);
            result.IncomeTax_Reimb_YTD_Min2 = TaxReimbursementService.GetIncomeTax_Reimb_YTD_Min2(siteUrl);
            result.VAT_Exemp_YTD_Min2 = TaxExemptionDataService.GetVAT_Exemp_YTD_Min2(siteUrl);
            result.VAT_Reimb_YTD_Min2 = TaxReimbursementService.GetVAT_Reimb_YTD_Min2(siteUrl);
            result.OtherTax_Exemp_YTD_Min2 = TaxExemptionDataService.GetOtherTax_Exemp_YTD_Min2(siteUrl);
            result.OtherTax_Reimb_YTD_Min2 = TaxReimbursementService.GetOtherTax_Reimb_YTD_Min2(siteUrl);

            #endregion

            return result;
        }
    }
}