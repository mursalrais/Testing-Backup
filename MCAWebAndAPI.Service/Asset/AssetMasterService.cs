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
    public class AssetMasterService : IAssetMasterService
    {
        string _siteUrl = null;
        static Logger logger = LogManager.GetCurrentClassLogger();
        const string SP_ASSMAS_LIST_NAME = "AssetMaster";

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = siteUrl;
        }

        public AssetMasterVM GetAssetMasters()
        {
            throw new NotImplementedException();
        }

        public bool CreateAssetMaster(AssetMasterVM assetMaster)
        {
            throw new NotImplementedException();
        }

        public bool CreateAssetMaster_dummy(AssetMasterVM assetMaster)
        {
            
            var columnValues = new Dictionary<string, object>();
            columnValues.Add("AssetCategory", assetMaster.AssetCategory.Value);
            columnValues.Add("Title", assetMaster.AssetDesc);
            columnValues.Add("AssetID", assetMaster.Id);
            columnValues.Add("AssetLevel", assetMaster.AssetLevel.Value);
            columnValues.Add("AssetNo", assetMaster.AssetNoAssetDesc.Value);
            columnValues.Add("AssetType",assetMaster.AssetType.Value);
            columnValues.Add("Condition",assetMaster.Condition.Value);
            columnValues.Add("ProjectUnit",assetMaster.ProjectUnit.Value);
            columnValues.Add("Remarks",assetMaster.Remarks);
            columnValues.Add("SerialNo",assetMaster.SerialNo);
            columnValues.Add("Spesifications", assetMaster.Spesifications);

            try
            {
                SPConnector.AddListItem(SP_ASSMAS_LIST_NAME, columnValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
            }
            return true;
        }        

        public bool UpdateAssetMaster(AssetMasterVM assetMaster)
        {
            throw new NotImplementedException();
        }

        IEnumerable<AssetMasterVM> IAssetMasterService.GetAssetMasters()
        {
            throw new NotImplementedException();
        }        

        public AssetMasterVM GetAssetMaster_Dummy()
        {
            var viewModel = new AssetMasterVM();           

            return viewModel;
        }

        private string GenerateAssetID(AssetMasterVM assetMaster)
        {
            switch (assetMaster.AssetLevel.DefaultValue)
            {
                case "Sub Asset":
                    return GenerateAssetIDForSubAsset(assetMaster);
                case "Main Asset":
                default:
                    return GenerateAssetIDForMainAsset(assetMaster);
            }
        }

        private string GenerateAssetIDForSubAsset(AssetMasterVM assetMaster)
        {
            throw new NotImplementedException();
        }

        private string GenerateAssetIDForMainAsset(AssetMasterVM assetMaster)
        {
            var assetID = GetAssetIDCode(assetMaster.AssetCategory.Value, assetMaster.ProjectUnit.Value, assetMaster.AssetType.Value);
            var lastNumber = GetAssetIDLastNumber(assetID);
            assetID += "-" + FormatUtil.ConvertToDigitNumber(lastNumber, 4);

            return assetID;
        }

        private int GetAssetIDLastNumber(string assetID)
        {
            var caml = @"<View>  
                <Query> 
                    <Where><Contains><FieldRef Name='AssetID' /><Value Type='Text'>AA" 
                + assetID
                + @"</Value></Contains></Where> 
                </Query> 
                <ViewFields><FieldRef Name='AssetID' /></ViewFields> 
            </View>";
            var listItem = SPConnector.GetList(SP_ASSMAS_LIST_NAME, _siteUrl, caml);

            var numbers = new List<int>();
            foreach(var item in listItem)
            {
                var itemAssetID = Convert.ToString(item["AssetID"]);
                if (itemAssetID.Length >= 14) // skip sub asset
                    continue;

                var itemNumber = Convert.ToInt32(itemAssetID.Split('-')[3]);
                numbers.Add(itemNumber);
            }

            return numbers.Max() + 1;
        }

        private string GetAssetIDCode(string assetCategory, string projectUnit, string assetType)
        {
            var result = string.Compare(assetCategory, "Fixed Asset", StringComparison.OrdinalIgnoreCase) == 0 ?
                "FXA" : "SVA";
            return result += projectUnit + assetType;
        }
    }
}
