using MCAWebAndAPI.Model.HR.DataMaster;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using System.Collections.Generic;
using System.Web;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Client;

namespace MCAWebAndAPI.Service.HR.Common
{
    public interface IHRDataMasterService
    {
        void SetSiteUrl(string siteUrl);

        IEnumerable<ProfessionalMaster> GetProfessionals();

        IEnumerable<PositionsMaster> GetPositions();
    }
}
