using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Service.Asset
{
    public interface IReportFixedAssetService
    {
        void SetSiteUrl(string siteUrl);
        IEnumerable<ReportFixedAssetVM> GetReport(String SiteUrl);
        void Inserting(ReportFixedAssetVM model, string SiteUrl);
    }
}
