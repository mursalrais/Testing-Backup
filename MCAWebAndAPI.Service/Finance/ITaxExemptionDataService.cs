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
        TaxExemptionDataVM GetTaxExemptionData();
        TaxExemptionDataVM GetTaxExemptionData(int ID);
        int? CreateTaxExemptionData(TaxExemptionDataVM taxExemptionData);
        bool UpdateTaxExemptionData(TaxExemptionDataVM taxExemptionData);
        Task CreateTaxExemptionDataAsync(int? ID, IEnumerable<HttpPostedFileBase> documents);
    }
}
