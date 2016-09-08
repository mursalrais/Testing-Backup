using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;

namespace MCAWebAndAPI.Service.Finance
{
    public interface ICSVErrorLogService
    {
        void SetSiteUrl(string siteUrl);

        CSVErrorLogVM Get(int? ID);

        IEnumerable<CSVErrorLogVM> GetAll(string key);

        void Save(string key, IEnumerable<CSVErrorLogVM> viewModels);
    }
}
