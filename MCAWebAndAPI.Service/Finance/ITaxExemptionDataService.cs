using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MCAWebAndAPI.Service.Finance
{
    public interface ITaxExemptionDataService
    {
        void SetSiteUrl(string siteUrl);

        TaxExemptionIncomeVM GetTaxExemptionIncome();
        TaxExemptionVATVM GetTaxExemptionVAT();
        TaxExemptionOtherVM GetTaxExemptionOthers();

        int? CreateTaxExemptionData(TaxExemptionIncomeVM taxExemptionData);
        int? CreateTaxExemptionData(TaxExemptionVATVM taxExemptionData);
        int? CreateTaxExemptionData(TaxExemptionOtherVM taxExemptionData);

        Task CreateTaxExemptionDataAsync(int? ID, string taxType, IEnumerable<HttpPostedFileBase> documents);

        TaxExemptionIncomeVM GetTaxExemptionIncome(int ID);
        TaxExemptionVATVM GetTaxExemptionVAT(int ID);
        TaxExemptionOtherVM GetTaxExemptionOthers(int ID);
        bool UpdateTaxExemption(TaxExemptionIncomeVM taxExemptionData);
        bool UpdateTaxExemption(TaxExemptionVATVM taxExemptionData);
        bool UpdateTaxExemption(TaxExemptionOtherVM taxExemptionData);

    }
}
