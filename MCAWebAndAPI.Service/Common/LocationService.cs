using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;

namespace MCAWebAndAPI.Service.Common
{
    public class LocationService : ILocationService
    {
        string _siteUrl;
        const string SP_LOCATION_LIST_NAME = "Location Master";

        public IEnumerable<Location> GetCities(int? parentId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Location> GetCountries(int? parentId)
        {
            //TODO: To set if parentID is not null
            var caml = @"
            <View>  
                <Query> 
                    <Where><Eq><FieldRef Name='Level' /><Value Type='Choice'>Country</Value></Eq></Where>
                    <OrderBy><FieldRef Name='Title' /></OrderBy> 
                </Query> 
                    <ViewFields><FieldRef Name='ID' /><FieldRef Name='Title' /></ViewFields> 
            </View>";

            var locations = new List<Location>();
            foreach (var item in SPConnector.GetList(SP_LOCATION_LIST_NAME, _siteUrl, caml))
            {
                locations.Add(ConvertToLocationModel(item));
            }

            return locations;
        }

        Location ConvertToLocationModel(ListItem item)
        {
            return new Location
            {
                ID = Convert.ToInt32(item["ID"]),
                Title = Convert.ToString(item["Title"])
            };
        }

        public IEnumerable<Location> GetProvinces(int? parentId)
        {
            throw new NotImplementedException();
        }

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
        }
    }
}
