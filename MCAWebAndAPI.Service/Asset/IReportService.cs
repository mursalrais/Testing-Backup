using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Service.Asset
{
    public interface IReportService
    {
        void SetSiteUrl(string siteUrl);
        IEnumerable<AssetReportVM> GetReport(string SiteUrl, string mode);
        DataTable getTable(string mode, string isempty = null);
    }
}
