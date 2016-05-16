using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using NLog;
using MCAWebAndAPI.Service.SPUtil;

namespace MCAWebAndAPI.Service.Asset
{
    public class LocationMasterService : ILocationMasterService
    {
        string _siteUrl = "https://eceos2.sharepoint.com/sites/mca-dev/dev";
        static Logger logger = LogManager.GetCurrentClassLogger();
        const string SP_LOCATIONMASTER_LISTNAME = "Location Master";

        public bool CreateLocationMaster(LocationMasterVM locationMaster)
        {
            var columnValues = new Dictionary<string, object>();
            columnValues.Add("Title", locationMaster.Header.Remarks);
            columnValues.Add("FloorName", locationMaster.Header.FloorName);
            columnValues.Add("RoomName", locationMaster.Header.RoomName);
            columnValues.Add("OfficeName", locationMaster.Header.OfficeName);
            try
            {
                SPConnector.AddListItem(SP_LOCATIONMASTER_LISTNAME, columnValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
            }
            var entitiy = new LocationMasterVM();
            entitiy = locationMaster;
            return true;
        }

        public LocationMasterVM GetLocationMaster()
        {
            var viewModel = new LocationMasterVM();

            return viewModel;
        }

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = siteUrl;
        }

        public bool DeleteLocationMasterVM(LocationMasterVM locationMaster)
        {
            throw new NotImplementedException();
        }

        public bool UpdateLocationMasterVM(LocationMasterVM locationMaster)
        {
            throw new NotImplementedException();
        }
    }
}
