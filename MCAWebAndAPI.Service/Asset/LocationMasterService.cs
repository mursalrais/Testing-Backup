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
        const string SP_PLACE_MAS_LISTNAME = "Place Master";
        const string SP_PROVINCE_LISTNAME = "Province";
        const string SP_LOCATION_MAS_LISTNAME = "Location Master";

        public int CreateHeader(LocationMasterVM header, string province, string office, int floor, string room)
        {
            var caml = @"<View>  
            <Query> 
               <Where><And><And><And><Eq><FieldRef Name='Province' /><Value Type='Choice'>" + province + @"</Value></Eq><Eq><FieldRef Name='Title' /><Value Type='Text'>" + office + @"</Value></Eq></And><Eq><FieldRef Name='Floor' /><Value Type='Text'>" + floor + @"</Value></Eq></And><Eq><FieldRef Name='Room' /><Value Type='Text'>" + room + @"</Value></Eq></And></Where> 
            </Query> 
      </View>";
            int error = 0;
            var locationTemp = SPConnector.GetList(SP_LOCATION_MAS_LISTNAME, _siteUrl, caml).Count();
            if (locationTemp != 0)
            {
                return error;
            }
            var columnValues = new Dictionary<string, object>();
            var camlProvinceInfo = @"<View><Query>
                               <Where>
                                  <Eq>
                                     <FieldRef Name='Province' />
                                     <Value Type='Text'>"+header.Province.Value+@"</Value>
                                  </Eq>
                               </Where>
                            </Query>
                            <ViewFields />
                            <QueryOptions /></View>";
            var ProvinceInfo = SPConnector.GetList("Province", _siteUrl, camlProvinceInfo);
            foreach(var prop in ProvinceInfo)
            {
                columnValues.Add("Province", Convert.ToInt32(prop["ID"]));
            }
            
            columnValues.Add("Title", header.OfficeName);
            columnValues.Add("Floor", header.FloorName);
            columnValues.Add("Room", header.RoomName);
            columnValues.Add("Remarks", header.Remarks);
            try
            {
                SPConnector.AddListItem(SP_LOCATION_MAS_LISTNAME, columnValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }

            return SPConnector.GetLatestListItemID(SP_LOCATION_MAS_LISTNAME, _siteUrl);
        }

        public LocationMasterVM GetHeader(int? ID)
        {
            throw new NotImplementedException();
        }

        public LocationMasterVM GetPopulatedModel(string SiteUrl)
        {
            var model = new LocationMasterVM();
            var siteHr = SiteUrl.Replace("/bo", "/hr");
            model.Province.Choices = GetChoicesFromListHR("Place Master", "Title", siteHr);
            return model;
        }

        private IEnumerable<string> GetChoicesFromListHR(string listname, string field1, string siteHr, string field2 = null)
        {
            var caml = @"<View>  
            <Query> 
               <Where><Eq><FieldRef Name='Level' /><Value Type='Choice'>Province</Value></Eq></Where> 
            </Query>
      </View>";
            List<string> _choices = new List<string>();
            var listItems = SPConnector.GetList(listname, siteHr, caml);
            foreach (var item in listItems)
            {
                if (field2 == null)
                {

                    _choices.Add(item[field1].ToString());
                }
                else
                {
                    _choices.Add(item[field1].ToString());
                }
            }
            return _choices.ToArray();
        }

        public void SetSiteUrl(string siteUrl = null)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
        }

        public bool UpdateHeader(LocationMasterVM header, string province, string office, int floor, string room)
        {
            var caml = @"<View>  
            <Query> 
               <Where><And><And><And><Eq><FieldRef Name='Province' /><Value Type='Choice'>" + province + @"</Value></Eq><Eq><FieldRef Name='Title' /><Value Type='Text'>" + office + @"</Value></Eq></And><Eq><FieldRef Name='Floor' /><Value Type='Text'>" + floor + @"</Value></Eq></And><Eq><FieldRef Name='Room' /><Value Type='Text'>" + room + @"</Value></Eq></And></Where> 
            </Query> 
      </View>";
            bool error = false;
            var locationTemp = SPConnector.GetList(SP_LOCATION_MAS_LISTNAME, _siteUrl, caml).Count();
            if (locationTemp != 0)
            {
                return error;
            }
            var ID = header.ID;
            var columnValues = new Dictionary<string, object>();
            var camlProvinceInfo = @"<View><Query>
                               <Where>
                                  <Eq>
                                     <FieldRef Name='Province' />
                                     <Value Type='Text'>" + header.Province.Value + @"</Value>
                                  </Eq>
                               </Where>
                            </Query>
                            <ViewFields />
                            <QueryOptions /></View>";
            var ProvinceInfo = SPConnector.GetList("Province", _siteUrl, camlProvinceInfo);
            foreach (var prop in ProvinceInfo)
            {
                columnValues.Add("Province", Convert.ToInt32(prop["ID"]));
            }
            columnValues.Add("Title", header.OfficeName);
            columnValues.Add("Floor", header.FloorName);
            columnValues.Add("Room", header.RoomName);
            columnValues.Add("Remarks", header.Remarks);
            try
            {
                SPConnector.UpdateListItem(SP_LOCATION_MAS_LISTNAME, ID, columnValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }

            return true;
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
            var site = _siteUrl;
            var siteHR = site.Replace("/bo", "/hr");

            foreach (var item in SPConnector.GetList(SP_PLACE_MAS_LISTNAME, siteHR, caml))
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

            var caml = @"<View>  
            <Query> 
               <Where><Eq><FieldRef Name='Level' /><Value Type='Choice'>Province</Value></Eq></Where> 
            </Query> 
      </View>";

            var site = _siteUrl;
            var siteHR = site.Replace("/bo", "/hr");

            viewModel.PlaceMasters = GetPlaceMasters();

            var collectionProvince = new List<string>();
            var collectionLocation = new List<string>();
            var collectionIDLocation = new List<int>();

            foreach (var item in SPConnector.GetList(SP_PROVINCE_LISTNAME, _siteUrl))
            {
                collectionProvince.Add(Convert.ToString(item["Province"]));
            }

            foreach (var model in viewModel.PlaceMasters)
            {
                var updatedValue = new Dictionary<string, object>();

                if (!(collectionProvince.Any(e => e == model.LocationName)))
                {
                    updatedValue.Add("Province", model.LocationName);

                    try
                    {
                        SPConnector.AddListItem(SP_PROVINCE_LISTNAME, updatedValue, _siteUrl);
                    }
                    catch (Exception e)
                    {
                        logger.Error(e.Message);
                        throw new Exception(ErrorResource.SPInsertError);
                    }
                }
            }

            foreach (var item in SPConnector.GetList(SP_PLACE_MAS_LISTNAME, siteHR, caml))
            {
                collectionIDLocation.Add(Convert.ToInt32(item["ID"]));
                collectionLocation.Add(Convert.ToString(item["Title"]));
            }

            foreach (var item in SPConnector.GetList(SP_PROVINCE_LISTNAME, _siteUrl))
            {
                var updatedValue = new Dictionary<string, object>();

                string province = null;
                int id = 0;
                province = Convert.ToString(item["Province"]);
                id = Convert.ToInt32(item["ID"]);
                if (!(collectionLocation.Any(e => e == province)))
                {
                    try
                    {
                        SPConnector.DeleteListItem(SP_PROVINCE_LISTNAME, id, _siteUrl);
                    }
                    catch (Exception e)
                    {
                        logger.Error(e.Message);
                        throw new Exception(ErrorResource.SPInsertError);
                    }
                }
            }

            return viewModel;
        }

        public LocationMasterVM GetPopulatedModel(int ID, string SiteUrl)
        {
            throw new NotImplementedException();
        }
    }
}
