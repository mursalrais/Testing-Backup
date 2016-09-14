using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MCAWebAndAPI.Model.ViewModel.Form.Finance
{
    /// <summary>
    /// FIN 01: Landing Page
    /// </summary>

    public class LandingPageVM
    {
        public IEnumerable<LPBudgetVsActualDisbursementVM> BudgetVsActualDisbursement { get; set; }

        public int OA_PIC_Number { get; set; }          // Outstanding Advance, Profesional & IC : Number of Advance

        public decimal OA_PIC_Amount { get; set; }      // Outstanding Advance, Profesional & IC : Ammount (IDR)

        public int OA_G_Number { get; set; }            // Outstanding Advance, Grantess : Number of Advance

        public decimal OA_G_Amount { get; set; }       // Outstanding Advance, Grantess : Ammount (IDR)

        public decimal SPHL { get; set; }               // SPHL

        #region Tax Exemption and Reimbursement YTD-2

        [DisplayFormat(DataFormatString = "{0:#,##0.##}")]
        public decimal VAT_Exemp_YTD_Min2 { get; set; }         // YTD-2 VAT : Exemption

        [DisplayFormat(DataFormatString = "{0:#,##0.##}")]
        public decimal VAT_Reimb_YTD_Min2 { get; set; }         // YTD-2 VAT : Reimbursement

        [DisplayFormat(DataFormatString = "{0:#,##0.##}")]
        public decimal IncomeTax_Exemp_YTD_Min2 { get; set; }   // YTD-2 IncomeTax : Exemption

        public decimal IncomeTax_Reimb_YTD_Min2 { get; set; }   // YTD-2 IncomeTax : Reimbursement

        public decimal OtherTax_Exemp_YTD_Min2 { get; set; }     // YTD-2 OtherTax : Exemption

        public decimal OtherTax_Reimb_YTD_Min2 { get; set; }     // YTD-2 OtherTax : Reimbursement

        #endregion

        #region Tax Exemption and Reimbursement YTD-1

        public decimal VAT_Exemp_YTD_Min1 { get; set; }         // YTD-1 VAT : Exemption

        public decimal VAT_Reimb_YTD_Min1 { get; set; }         // YTD-1 VAT : Reimbursement

        public decimal IncomeTax_Exemp_YTD_Min1 { get; set; }   // YTD-1 IncomeTax : Exemption

        public decimal IncomeTax_Reimb_YTD_Min1 { get; set; }   // YTD-1 IncomeTax : Reimbursement

        public decimal OtherTax_Exemp_YTD_Min1 { get; set; }     // YTD-1 OtherTax : Exemption

        public decimal OtherTax_Reimb_YTD_Min1 { get; set; }     // YTD-1 OtherTax : Reimbursement

        #endregion

        #region Tax Exemption and Reimbursement YTD

        public decimal VAT_Exemp_YTD { get; set; }         // YTD VAT : Exemption

        public decimal VAT_Reimb_YTD { get; set; }         // YTD VAT : Reimbursement

        public decimal IncomeTax_Exemp_YTD { get; set; }   // YTD IncomeTax : Exemption

        public decimal IncomeTax_Reimb_YTD { get; set; }   // YTD IncomeTax : Reimbursement

        public decimal OtherTax_Exemp_YTD { get; set; }     // YTD OtherTax : Exemption

        public decimal OtherTax_Reimb_YTD { get; set; }     // YTD OtherTax : Reimbursement

        #endregion

    }
}