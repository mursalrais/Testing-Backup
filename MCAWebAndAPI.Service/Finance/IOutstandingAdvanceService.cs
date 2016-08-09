using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;

namespace MCAWebAndAPI.Service.Finance
{
    public interface IOutstandingAdvanceService
    {
        void SetSiteUrl(string siteUrl);

        int Create(OutstandingAdvanceVM viewModel);

        bool Update(OutstandingAdvanceVM viewModel);
        OutstandingAdvanceVM Get(int? ID);
    }
}
