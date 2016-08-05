using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using NLog;
using MCAWebAndAPI.Service.Utils;
using System.Text.RegularExpressions;

namespace MCAWebAndAPI.Service.Asset
{
    public class AssetMasterService : IAssetMasterService
    {
        string _siteUrl;
        static Logger logger = LogManager.GetCurrentClassLogger();
        const string SP_ASSMAS_LIST_NAME = "Asset Master";

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
        }

        public AssetMasterVM GetAssetMaster()
        {
            var viewModel = new AssetMasterVM();
            viewModel.AssetNoAssetDesc.Choices = GetChoiceFromList("AssetID");
            viewModel.AssetLevel.Choices = SPConnector.GetChoiceFieldValues(SP_ASSMAS_LIST_NAME, "AssetLevel", _siteUrl);
            viewModel.AssetCategory.Choices = SPConnector.GetChoiceFieldValues(SP_ASSMAS_LIST_NAME, "AssetCategory", _siteUrl);
            viewModel.AssetType.Choices = SPConnector.GetChoiceFieldValues(SP_ASSMAS_LIST_NAME, "AssetType", _siteUrl);
            viewModel.Condition.Choices = SPConnector.GetChoiceFieldValues(SP_ASSMAS_LIST_NAME, "Condition", _siteUrl);
            viewModel.ProjectUnit.Choices = SPConnector.GetChoiceFieldValues(SP_ASSMAS_LIST_NAME, "ProjectUnit", _siteUrl);

            return viewModel;
        }

        public AssetMasterVM GetAssetMaster(int ID)
        {
            var listItem = SPConnector.GetListItem(SP_ASSMAS_LIST_NAME, ID, _siteUrl);
            var viewModel = new AssetMasterVM();

            viewModel.AssetNoAssetDesc.Choices = GetChoiceFromList("AssetID");
            viewModel.AssetLevel.Choices = SPConnector.GetChoiceFieldValues(SP_ASSMAS_LIST_NAME, "AssetLevel", _siteUrl);
            viewModel.AssetCategory.Choices = SPConnector.GetChoiceFieldValues(SP_ASSMAS_LIST_NAME, "AssetCategory", _siteUrl);
            viewModel.AssetType.Choices = SPConnector.GetChoiceFieldValues(SP_ASSMAS_LIST_NAME, "AssetType", _siteUrl);
            viewModel.Condition.Choices = SPConnector.GetChoiceFieldValues(SP_ASSMAS_LIST_NAME, "Condition", _siteUrl);
            viewModel.ProjectUnit.Choices = SPConnector.GetChoiceFieldValues(SP_ASSMAS_LIST_NAME, "ProjectUnit", _siteUrl);

            viewModel.ProjectUnit.Value = Convert.ToString(listItem["ProjectUnit"]);
            viewModel.Remarks = Convert.ToString(listItem["Remarks"]);
            viewModel.SerialNo = Convert.ToString(listItem["SerialNo"]);

           
            viewModel.Spesifications = Regex.Replace(listItem["Spesifications"].ToString(), "<.*?>", string.Empty); 
            viewModel.WarrantyExpires = Convert.ToDateTime(listItem["WarranyExpires"]);
            viewModel.AssetCategory.Value = Convert.ToString(listItem["AssetCategory"]);
            viewModel.AssetDesc = Convert.ToString(listItem["Title"]);
            viewModel.AssetLevel.Value = Convert.ToString(listItem["AssetLevel"]);
            viewModel.AssetType.Value = Convert.ToString(listItem["AssetType"]);
            viewModel.Condition.Value = Convert.ToString(listItem["Condition"]);
            viewModel.AssetNoAssetDesc.Value = Convert.ToString(listItem["AssetID"]);
            viewModel.ID = ID;

            return viewModel;
        }

        public bool CreateAssetMaster(AssetMasterVM assetMaster)
        {
            var columnValues = new Dictionary<string, object>();
            string _assetID = GenerateAssetID(assetMaster);

            if (assetMaster.AssetLevel.Value == "Sub Asset")
            {
                var qCaml = @"<View><Query>
                           <Where>
                              <Eq>
                                 <FieldRef Name='AssetID' />
                                 <Value Type='Text'>"+ _assetID + @"</Value>
                              </Eq>
                           </Where>
                        </Query></View>";
                var listItem = SPConnector.GetList(SP_ASSMAS_LIST_NAME, _siteUrl, qCaml);

                foreach(var item in listItem)
                {
                    columnValues.Add("AssetCategory", Convert.ToString(item["AssetCategory"]));
                    columnValues.Add("AssetType", Convert.ToString(item["AssetType"]));
                    columnValues.Add("ProjectUnit", Convert.ToString(item["ProjectUnit"]));
                }
            }
            else
            {
                columnValues.Add("AssetCategory", assetMaster.AssetCategory.Value);
                columnValues.Add("AssetType", assetMaster.AssetType.Value);
                columnValues.Add("ProjectUnit", assetMaster.ProjectUnit.Value);
            }

            columnValues.Add("Title", assetMaster.AssetDesc);
            columnValues.Add("AssetLevel", assetMaster.AssetLevel.Value);
            columnValues.Add("AssetID", _assetID);
            columnValues.Add("Condition", assetMaster.Condition.Value);
            columnValues.Add("Remarks", assetMaster.Remarks);
            columnValues.Add("SerialNo", assetMaster.SerialNo);
            columnValues.Add("Spesifications", assetMaster.Spesifications);
            if (assetMaster.WarrantyExpires.HasValue)
            {
                columnValues.Add("WarranyExpires", assetMaster.WarrantyExpires.Value.Date);
            }
            else
            {
                columnValues.Add("WarranyExpires", null);
            }
            
            try
            {
                SPConnector.AddListItem(SP_ASSMAS_LIST_NAME, columnValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                return false;
            }
            var entitiy = new AssetMasterVM();
            entitiy = assetMaster;
            return true;
        }

        public bool UpdateAssetMaster(AssetMasterVM assetMaster)
        {
            var columnValues = new Dictionary<string, object>();
            int ID = assetMaster.ID.Value;
            string _assetID = GenerateAssetID(assetMaster);

            if (assetMaster.AssetLevel.Value == "Sub Asset")
            {
                //make sure
                if(_assetID.Length > 14)
                {
                    string[] splitID = _assetID.Split('-');
                    if(splitID[0] == "FXA")
                    {
                        columnValues.Add("AssetCategory", "Fixed Asset");
                    }
                    else
                    {
                        columnValues.Add("AssetCategory", "Small Value Asset");
                    }
                    
                    columnValues.Add("ProjectUnit", Convert.ToString(splitID[1]));
                    columnValues.Add("AssetType", Convert.ToString(splitID[2]));
                }
            }
            else
            {
                columnValues.Add("AssetCategory", assetMaster.AssetCategory.Value);
                columnValues.Add("AssetType", assetMaster.AssetType.Value);
                columnValues.Add("ProjectUnit", assetMaster.ProjectUnit.Value);
            }

            columnValues.Add("Title", assetMaster.AssetDesc);
            columnValues.Add("AssetLevel", assetMaster.AssetLevel.Value);
            columnValues.Add("AssetID", _assetID);
            columnValues.Add("Condition", assetMaster.Condition.Value);
            columnValues.Add("Remarks", assetMaster.Remarks);
            columnValues.Add("SerialNo", assetMaster.SerialNo);
            columnValues.Add("Spesifications", assetMaster.Spesifications);
            columnValues.Add("WarranyExpires", assetMaster.WarrantyExpires.Value);

            try
            {
                SPConnector.UpdateListItem(SP_ASSMAS_LIST_NAME, ID, columnValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                return false;
            }
            var entitiy = new AssetMasterVM();
            entitiy = assetMaster;
            return true;
        }

        IEnumerable<AssetMasterVM> IAssetMasterService.GetAssetMasters()
        {
            var viewModels = new List<AssetMasterVM>();

            foreach (var item in SPConnector.GetList(SP_ASSMAS_LIST_NAME, _siteUrl))
            {
                viewModels.Add(new AssetMasterVM
                {
                    ID = Convert.ToInt32(item["ID"]),
                    AssetDesc = Convert.ToString(item["Title"])
                });
            }

            return viewModels;
        }

        public AssetMasterVM GetAssetMaster_Dummy()
        {
            var viewModel = new AssetMasterVM();
            string listName = "AssetID";
            viewModel.AssetNoAssetDesc.Choices = GetChoiceFromList(listName);
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
            var lastNumber = GetAssetIDLastNumberForSubAsset(assetID);
            assetID += "-" + FormatUtil.ConvertToDigitNumber(Convert.ToInt32(lastNumber), 3);
            return assetID;
        }

        private object GetAssetIDLastNumberForSubAsset(string assetID, string from = null)
        {
            if(_siteUrl != "https://eceos2.sharepoint.com/sites/mca-dev/bo")
            {
                _siteUrl = "https://eceos2.sharepoint.com/sites/mca-dev/bo";
            }
            var caml = @"<View>  
                <Query> 
                    <Where><Contains><FieldRef Name='AssetID' /><Value Type='Text'>"
                + assetID
                + @"</Value></Contains></Where> 
                </Query> 
                <ViewFields><FieldRef Name='AssetID' /></ViewFields> 
            </View>";
            var listItem = SPConnector.GetList(SP_ASSMAS_LIST_NAME, _siteUrl, caml);
            if(from == "sub asset" && listItem.Count == 0)
            {
                return 0;
            }

            var numbers = new List<int>();
            var aID = new List<string>();
            foreach (var item in listItem)
            {
                aID.Add(Convert.ToString(item["AssetID"]));
            }

            if (aID.Count > 1)
            {
                for (int x = 0; x < aID.Count; x++)
                {
                    if (aID[x].Length <= 14)
                    {
                        aID.RemoveAt(x);
                        x = -1;
                    }
                    else
                    {
                        var itemNumber = Convert.ToInt32(aID.Max().Split('-')[4]);
                        numbers.Add(itemNumber);
                    }
                }
            }
            else
            {
                return 1;
            }

            return numbers.Max() + 1;
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
            if (_siteUrl != "https://eceos2.sharepoint.com/sites/mca-dev/bo")
            {
                _siteUrl = "https://eceos2.sharepoint.com/sites/mca-dev/bo";
            }
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
            return result += "-" + projectUnit + "-" + assetType;
        }

        private string[] GetChoiceFromList(string listName)
        {
            List<string> _choices = new List<string>();
            var listItems = SPConnector.GetList(SP_ASSMAS_LIST_NAME, _siteUrl);
            foreach (var item in listItems)
            {
                if (Convert.ToString(item[listName]).Length == 14)
                {
                    _choices.Add(item[listName].ToString());
                }
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

        public string GetAssetIDForMainAsset(string category, string projectunit, string type)
        {
            if (category == "Fixed Asset")
            {
                category = "FX";
            }
            else
            {
                category = "SVA";
            }
            var assetID = GetAssetIDCode(category, projectunit, type);
            var lastNumber = GetAssetIDLastNumber(assetID);
            assetID += "-" + FormatUtil.ConvertToDigitNumber(lastNumber, 4);
            
            return assetID;
        }

        public string GetAssetIDForSubAsset(string category, string projectunit, string type, int number)
        {
            if (category == "Fixed Asset")
            {
                category = "FX";
            }
            else
            {
                category = "SVA";
            }
            var assetID = GetAssetIDCode(category, projectunit, type);
            var lastNumber = GetAssetIDLastNumber(assetID);
            assetID += "-" + FormatUtil.ConvertToDigitNumber(lastNumber, 4);

            return assetID;
        }

        public string GetAssetIDForSubAsset(string assetID)
        {
            List<string> similarAssetIDs = new List<string>();
            var lastNumber = GetAssetIDLastNumberForSubAsset(assetID, "sub asset");
            if(Convert.ToInt32(lastNumber) == 0)
            {
                assetID = "";
            }
            else
            {
                assetID += "-" + FormatUtil.ConvertToDigitNumber(Convert.ToInt32(lastNumber), 3);
            }            

            return assetID;
        }
    }
}
