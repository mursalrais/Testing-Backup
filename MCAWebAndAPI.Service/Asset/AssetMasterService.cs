using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using NLog;
using MCAWebAndAPI.Service.Utils;

namespace MCAWebAndAPI.Service.Asset
{
    public class AssetMasterService : IAssetMasterService
    {
        string _siteUrl = "https://eceos2.sharepoint.com/sites/mca-dev/dev/";
        static Logger logger = LogManager.GetCurrentClassLogger();
        const string SP_ASSMAS_LIST_NAME = "Asset Master";

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
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
            string _assetID = GenerateAssetID(assetMaster);
            columnValues.Add("AssetCategory", assetMaster.AssetCategory.Value);
            columnValues.Add("Title", assetMaster.AssetDesc);
            columnValues.Add("AssetLevel", assetMaster.AssetLevel.Value);
            columnValues.Add("AssetID", _assetID);
            columnValues.Add("AssetType", assetMaster.AssetType.Value);
            columnValues.Add("Condition", assetMaster.Condition.Value);
            columnValues.Add("ProjectUnit", assetMaster.ProjectUnit.Value);
            columnValues.Add("Remarks", assetMaster.Remarks);
            columnValues.Add("SerialNo", assetMaster.SerialNo);
            columnValues.Add("Spesifications", assetMaster.Spesifications);
            columnValues.Add("WarranyExpires", assetMaster.WarrantyExpires);

            try
            {
                SPConnector.AddListItem(SP_ASSMAS_LIST_NAME, columnValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
            }
            var entitiy = new AssetMasterVM();
            entitiy = assetMaster;
            return true;
        }

        public bool UpdateAssetMaster(AssetMasterVM assetMaster)
        {
            throw new NotImplementedException();
        }

        IEnumerable<AssetMasterVM> IAssetMasterService.GetAssetMasters()
        {
            var viewModels = new List<AssetMasterVM>();

            foreach(var item in SPConnector.GetList(SP_ASSMAS_LIST_NAME, _siteUrl))
            {
                viewModels.Add(new AssetMasterVM {
                    ID = Convert.ToInt32(item["ID"]), 
                    AssetDesc = Convert.ToString(item["Title"])
                });
            }

            return viewModels;
        }

        public AssetMasterVM GetAssetMaster_Dummy()
        {
            var viewModel = new AssetMasterVM();
            viewModel.AssetNoAssetDesc.Choices = GetChoiceFromList();
            viewModel.AssetLevel.Choices = SPConnector.GetChoiceFieldValues(SP_ASSMAS_LIST_NAME, "AssetLevel");
            viewModel.AssetCategory.Choices = SPConnector.GetChoiceFieldValues(SP_ASSMAS_LIST_NAME, "AssetCategory");
            viewModel.AssetType.Choices = SPConnector.GetChoiceFieldValues(SP_ASSMAS_LIST_NAME, "AssetType");
            viewModel.Condition.Choices = SPConnector.GetChoiceFieldValues(SP_ASSMAS_LIST_NAME, "Condition");
            viewModel.ProjectUnit.Choices = SPConnector.GetChoiceFieldValues(SP_ASSMAS_LIST_NAME, "ProjectUnit");

            return viewModel;
        }

        private string GenerateAssetID(AssetMasterVM assetMaster)
        {
            switch (assetMaster.AssetLevel.Value)
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
            var assetID = assetMaster.AssetNoAssetDesc.Value;
            var lastNumber = GetAssetIDLastNumber(assetID);
            assetID += "-" + FormatUtil.ConvertToDigitNumber(lastNumber, 2);
            return assetID;
        }

        private string GenerateAssetIDForMainAsset(AssetMasterVM assetMaster)
        {
            var assetID = GetAssetIDCode(assetMaster.AssetCategory.Value, assetMaster.ProjectUnit.Value, assetMaster.AssetType.Value);
            var lastNumber = GetAssetIDLastNumber(assetID);
            assetID += "-" + FormatUtil.ConvertToDigitNumber(lastNumber, 4);

            return assetID;
        }

        int GetAssetIDLastNumber(string assetID)
        {
            var caml = @"<View>  
                <Query> 
                    <Where><Contains><FieldRef Name='AssetID' /><Value Type='Text'>"
                + assetID
                + @"</Value></Contains></Where> 
                </Query> 
                <ViewFields><FieldRef Name='AssetID' /></ViewFields> 
            </View>";
            var listItem = SPConnector.GetList(SP_ASSMAS_LIST_NAME, _siteUrl, caml);
            if (listItem.Count == 0) // if not found
                return 1;

            var numbers = new List<int>();
            if (assetID.Length <= 14)  // if main asset
            {
                foreach (var item in listItem)
                {
                    var itemAssetID = Convert.ToString(item["AssetID"]);
                    if (itemAssetID.Length >= 15) // skip sub asset
                        continue;

                    var itemNumber = Convert.ToInt32(itemAssetID.Split('-')[3]);
                    numbers.Add(itemNumber);
                }
            }
            else //if sub asset
            {
                foreach (var item in listItem)
                {
                    var itemAssetID = Convert.ToString(item["AssetID"]);

                    var itemNumber = Convert.ToInt32(itemAssetID.Split('-')[4]);
                    numbers.Add(itemNumber);
                }
            }


            return numbers.Max() + 1;
        }

        private string GetAssetIDCode(string assetCategory, string projectUnit, string assetType)
        {
            var result = string.Compare(assetCategory, "Fixed Asset", StringComparison.OrdinalIgnoreCase) == 0 ?
                "FXA" : "SVA";
            return result += "-"+projectUnit+"-" + assetType;
        }

        private string[] GetChoiceFromList()
        {
            List<string> _choices = new List<string>();
            var listItems = SPConnector.GetList(SP_ASSMAS_LIST_NAME, _siteUrl);
            foreach (var item in listItems)
            {
                _choices.Add(item["AssetID"].ToString());
            }
            return _choices.ToArray();
        }

        public IEnumerable<AssetLocationVM> GetAssetLocations()
        {
            return new List<AssetLocationVM>
            {
                new AssetLocationVM
                {
                    ID = 1,
                    Name = "Jakarta"
                },
                new AssetLocationVM
                {
                    ID = 2,
                    Name = "Surabaya"
                }
            };
        }
    }
}
