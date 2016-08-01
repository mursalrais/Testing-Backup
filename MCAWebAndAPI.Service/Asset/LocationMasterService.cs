using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using NLog;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using MCAWebAndAPI.Service.Resources;

namespace MCAWebAndAPI.Service.Asset
{
    public class LocationMasterService : ILocationMasterService
    {
        string _siteUrl;
        static Logger logger = LogManager.GetCurrentClassLogger();
        const string SP_LOCATIONMASTER_LISTNAME = "Place Master";
        const string SP_PROVINCE_LISTNAME = "Province";

        public int CreateHeader(LocationMasterVM header, string province, string office, int floor, string room)
        {
            var caml = @"<View>  
            <Query> 
               <Where><And><And><And><Eq><FieldRef Name='Province' /><Value Type='Choice'>" + province + @"</Value></Eq><Eq><FieldRef Name='Title' /><Value Type='Text'>" + office + @"</Value></Eq></And><Eq><FieldRef Name='Floor' /><Value Type='Text'>" + floor + @"</Value></Eq></And><Eq><FieldRef Name='Room' /><Value Type='Text'>" + room + @"</Value></Eq></And></Where> 
            </Query> 
      </View>";
            int error = 0;
            var locationTemp = SPConnector.GetList(SP_LOCATIONMASTER_LISTNAME, _siteUrl, caml).Count();
            if (locationTemp != 0)
            {
                return error;
            }
            var columnValues = new Dictionary<string, object>();
            columnValues.Add("Province", header.Province.Value);
            columnValues.Add("Title", header.OfficeName);
            columnValues.Add("Floor", header.FloorName);
            columnValues.Add("Room", header.RoomName);
            columnValues.Add("Remarks", header.Remarks);
            try
            {
                SPConnector.AddListItem(SP_LOCATIONMASTER_LISTNAME, columnValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }

            return SPConnector.GetLatestListItemID(SP_LOCATIONMASTER_LISTNAME, _siteUrl);
        }

        public LocationMasterVM GetHeader(int? ID)
        {
            throw new NotImplementedException();
        }

        public LocationMasterVM GetPopulatedModel()
        {
            var model = new LocationMasterVM();
            return model;
        }

        public void SetSiteUrl(string siteUrl = null)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
        }

        public bool UpdateHeader(LocationMasterVM header)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LocationMasterVM> GetLocationMaster()
        {
            var models = new List<LocationMasterVM>();

            foreach (var item in SPConnector.GetList(SP_PROVINCE_LISTNAME, _siteUrl))
            {
                models.Add(ConvertToLocationMasterModel_Light(item));
            }

            return models;
        }

        private LocationMasterVM ConvertToLocationMasterModel_Light(ListItem item)
        {
            return new LocationMasterVM
            {
                ID = Convert.ToInt32(item["ID"]),
                Title = Convert.ToString(item["Province"]),
            };
        }

        private IEnumerable<LocationMasterVM> GetPlaceMasters()
        {
            var caml = @"<View>  
            <Query> 
               <Where><Eq><FieldRef Name='Level' /><Value Type='Choice'>Province</Value></Eq></Where> 
            </Query> 
      </View>";

            var LocationMaster = new List<LocationMasterVM>();

            var siteHR = _siteUrl.Replace("/bo", "/hr");

            foreach (var item in SPConnector.GetList(SP_LOCATIONMASTER_LISTNAME, _siteUrl, caml))
            {

                LocationMaster.Add(ConvertToProvinceVM(item));
            }

            return LocationMaster;
        }

        private LocationMasterVM ConvertToProvinceVM(ListItem item)
        {
            return new LocationMasterVM
            {
                ID = Convert.ToInt32(item["ID"]),
                LocationName = Convert.ToString(item["Title"])
            };
        }

        public LocationMasterVM UpdateProvince()
        {
            var viewModel = new LocationMasterVM();
            Int32 IDProvince = 0;
            foreach (var item in SPConnector.GetList(SP_PROVINCE_LISTNAME, _siteUrl))
            {
                IDProvince = Convert.ToInt32(item["ID"]);
                try
                {
                    SPConnector.DeleteListItem(SP_PROVINCE_LISTNAME, IDProvince, _siteUrl);
                }
                catch (Exception e)
                {
                    logger.Error(e);
                    throw e;
                }
            }

            viewModel.PlaceMasters = GetPlaceMasters();

            foreach (var model in viewModel.PlaceMasters)
            {
                var updatedValue = new Dictionary<string, object>();
                updatedValue.Add("Province", viewModel.LocationName);
                try
                {
                    SPConnector.AddListItem(SP_LOCATIONMASTER_LISTNAME, updatedValue, _siteUrl);
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw new Exception(ErrorResource.SPInsertError);
                }
            }
            return viewModel;
        }
    }
}
