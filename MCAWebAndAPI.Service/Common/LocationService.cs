using System;
using System.Collections.Generic;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;

namespace MCAWebAndAPI.Service.Common
{
    public class LocationService : ILocationService
    {
        string _siteUrl;
        const string SP_LOCATION_LIST_NAME = "Place Master";

        public IEnumerable<Location> GetCountries(int? parentId)
        {
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

        public IEnumerable<Location> GetProvinces(int? parentId)
        {
            var caml = @"
            <View>  
                <Query> 
                    <Where><Eq><FieldRef Name='Level' /><Value Type='Choice'>Province</Value></Eq></Where>
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


        public IEnumerable<Location> GetContinents(int? parentId)
        {
            var caml = @"
            <View>  
                <Query> 
                    <Where><Eq><FieldRef Name='Level' /><Value Type='Choice'>Continent</Value></Eq></Where>
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

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
        }
    }
}
