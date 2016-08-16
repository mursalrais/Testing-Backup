using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using System.Data;

namespace MCAWebAndAPI.Service.Asset
{
    public interface ILocationMasterService
    {
        void SetSiteUrl(string siteUrl = null);

        LocationMasterVM GetPopulatedModel(string SiteUrl);

        LocationMasterVM GetPopulatedModel(int ID, string SiteUrl);

        LocationMasterVM GetHeader(int? ID, string SiteUrl);

        LocationMasterVM UpdateProvince();

        IEnumerable<LocationMasterVM> GetLocationMaster();

        int CreateHeader(LocationMasterVM header, string province, string office, int floor, string room);

        bool UpdateHeader(LocationMasterVM header, string province, string office, int floor, string room);

        int? MassUpload(string ListName, DataTable CSVDataTable, string SiteUrl = null);
    }
}
