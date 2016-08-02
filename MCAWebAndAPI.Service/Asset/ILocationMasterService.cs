using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;

namespace MCAWebAndAPI.Service.Asset
{
    public interface ILocationMasterService
    {
        void SetSiteUrl(string siteUrl = null);

        LocationMasterVM GetPopulatedModel();

        LocationMasterVM GetHeader(int? ID);

        LocationMasterVM UpdateProvince();

        IEnumerable<LocationMasterVM> GetLocationMaster();

        int CreateHeader(LocationMasterVM header, string province, string office, int floor, string room);

        bool UpdateHeader(LocationMasterVM header);
    }
}
