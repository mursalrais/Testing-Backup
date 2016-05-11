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
        string _siteUrl = "https://eceos2.sharepoint.com/sites/mca-dev/dev/";
        static Logger logger = LogManager.GetCurrentClassLogger();
        const string SP_ASSETMASTER_LISTNAME = "Asset Master";



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
            //columnValues.Add("AssetID", assetMaster.Id);
            columnValues.Add("AssetLevel", assetMaster.AssetLevel.Value);
            columnValues.Add("AssetID", assetMaster.AssetNoAssetDesc.Value);
            columnValues.Add("AssetType", assetMaster.AssetType.Value);
            columnValues.Add("Condition", assetMaster.Condition.Value);
            columnValues.Add("ProjectUnit", assetMaster.ProjectUnit.Value);
            columnValues.Add("Remarks", assetMaster.Remarks);
            columnValues.Add("SerialNo", assetMaster.SerialNo);
            columnValues.Add("Spesifications", assetMaster.Spesifications);
            columnValues.Add("WarranyExpires", assetMaster.WarrantyExpires);

            try
            {
                SPConnector.AddListItem(SP_ASSETMASTER_LISTNAME, columnValues, _siteUrl);
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
            throw new NotImplementedException();
        }

        public AssetMasterVM GetAssetMaster_Dummy()
        {
            var viewModel = new AssetMasterVM();
            viewModel.AssetNoAssetDesc.Choices = GetChoiceFromList();
            return viewModel;
        }

        public void SetSiteUrl(string siteUrl)
        {
            throw new NotImplementedException();
        }

        private string[] GetChoiceFromList()
        {
            List<string> _choices = new List<string>();
            var listItems = SPConnector.GetList(SP_ASSETMASTER_LISTNAME, _siteUrl);
            foreach (var item in listItems)
            {
                _choices.Add(item["AssetLevel"].ToString());
            }
            return _choices.ToArray();
        }
    }
    
}
