using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.HR.DataMaster;

namespace MCAWebAndAPI.Service.HR.Common
{
    public interface IPSAMasterService
    {
        void SetSiteUrl(string siteUrl);

        IEnumerable<PSAMaster> GetPSAs();
    }
}
