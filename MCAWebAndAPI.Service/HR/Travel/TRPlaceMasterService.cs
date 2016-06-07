using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using NLog;

namespace MCAWebAndAPI.Service.HR.Travel
{
    public class TRPlaceMasterService : ITRPlaceMasterService
    {
        string _siteUrl;
        static Logger logger = LogManager.GetCurrentClassLogger();
        const string SP_HEADER_LIST_NAME = "Place Master";

        public int CreateHeader(PlaceMasterVM header)
        {
            var columnValues = new Dictionary<string, object>();
            columnValues.Add("Title", header.LocationName);
            columnValues.Add("Level", header.LevelOfPlace.Value);
            if (header.LevelOfPlace.Value != "Continent")
            {
                columnValues.Add("parentlocation", new FieldLookupValue { LookupId = Convert.ToInt32(header.ParentLocation.Value) });
            }
            try
            {
                SPConnector.AddListItem(SP_HEADER_LIST_NAME, columnValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }

            return SPConnector.GetInsertedItemID(SP_HEADER_LIST_NAME, _siteUrl);
        }

        public PlaceMasterVM GetHeader(int ID)
        {
            var listItem = SPConnector.GetListItem(SP_HEADER_LIST_NAME, ID, _siteUrl);
            var viewModel = new PlaceMasterVM();

            viewModel.LocationName = Convert.ToString(listItem["Title"]);
            viewModel.LevelOfPlace.Value = Convert.ToString(listItem["Level"]);
            viewModel.ID = ID;

            return viewModel;
        }

        public PlaceMasterVM GetPopulatedModel(int? id = default(int?))
        {
            var model = new PlaceMasterVM();
            return model;
        }

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
        }

        public bool UpdateHeader(PlaceMasterVM header)
        {
            var columnValues = new Dictionary<string, object>();
            int? ID = header.ID;
            columnValues.Add("Title", header.LocationName);
            columnValues.Add("Level", header.LevelOfPlace.Value);
            if (header.LevelOfPlace.Value != "Continent")
            {
                columnValues.Add("parentlocation", new FieldLookupValue { LookupId = Convert.ToInt32(header.ParentLocation.Value) });
            }
            if (header.LevelOfPlace.Value == "Continent")
            {
                columnValues.Add("parentlocation", "");
            }
            try
            {
                SPConnector.UpdateListItem(SP_HEADER_LIST_NAME, ID, columnValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                return false;
            }
            var entitiy = new PlaceMasterVM();
            entitiy = header;
            return true;
        }
    }
}
