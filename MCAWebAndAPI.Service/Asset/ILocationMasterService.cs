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
        void SetSiteUrl(string siteUrl);

        LocationMasterVM GetLocationMaster();

        bool CreateLocationMaster(LocationMasterVM locationMaster);

        bool DeleteLocationMasterVM(LocationMasterVM locationMaster);

        bool UpdateLocationMasterVM(LocationMasterVM locationMaster);
    }
}
