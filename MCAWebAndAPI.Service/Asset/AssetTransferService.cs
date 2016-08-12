using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using NLog;
using MCAWebAndAPI.Service.Utils;
using MCAWebAndAPI.Model.ViewModel.Form.Shared;

namespace MCAWebAndAPI.Service.Asset
{
    public class AssetTransferService : IAssetTransferService
    {
        string _siteUrl = "https://eceos2.sharepoint.com/sites/mca-dev/bo";
        static Logger logger = LogManager.GetCurrentClassLogger();       
        const string SP_ASSACQ_LIST_NAME = "Asset Acquisition";
        const string SP_ASSACQDetails_LIST_NAME = "Asset Acquisition Details";
        const string SP_ASSET_HOLDER_FROM_LIST_NAME = "Professional Master";
     

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = siteUrl;
        }

        public AssetTransactionVM GetAssetTransfers()
        {
            throw new NotImplementedException();
        }

        public bool CreateAssetTransfer(AssetTransactionVM assetTransfer)
        {
            throw new NotImplementedException();
        }

        public bool UpdateAssetTransfer(AssetTransactionVM assetTransfer)
        {
            throw new NotImplementedException();
        }

        IEnumerable<AssetTransactionVM> IAssetTransferService.GetAssetTransfers()
        {
            throw new NotImplementedException();
        }

        public int CreateHeader(AssetTransferVM header)
        {
            throw new NotImplementedException();
        }

        public AssetTransferVM GetPopulatedModel(int? id = default(int?))
        {
            var model = new AssetTransferVM();
            model.TransactionType = Convert.ToString("Asset Transfer");
            model.AssetHolderFrom.Choices = GetChoicesFromList(SP_ASSET_HOLDER_FROM_LIST_NAME, "ID", "Title","Position");
            model.AssetHolderTo.Choices = GetChoicesFromList(SP_ASSET_HOLDER_FROM_LIST_NAME, "ID", "Title","Position");
            return model;
        }

        //private string GetContactFromAssetHolderList(int? ID)
        //{
        //    var caml = @"<View><Query><Where><Eq><FieldRef Name='ContactNo' /><Value Type='Text'>" + ID.ToString() + "</Value></Eq></Where></Query><ViewFields /><QueryOptions /></View>";

        //    var ContactNo = new List<AssetTransferHeaderVM>();
        //    foreach (var item in SPConnector.GetList(SP_ASSET_HOLDER_FROM_LIST_NAME, _siteUrl, caml))
        //    {
        //        Add(item[v1].ToString());
        //    }

        //    return ContactNo;
        //}

        private IEnumerable<string> GetChoicesFromList(string listname, string v1, string v2 = null,string v3=null)
        {
            List<string> _choices = new List<string>();
            var listItems = SPConnector.GetList(listname, _siteUrl);
            foreach (var item in listItems)
            {
                if (v3 != null)
                {
                    _choices.Add(item[v1] + "-" +item[v2].ToString() + "-" + item[v3].ToString());
                }
                else
                {
                    _choices.Add(item[v2].ToString());
                }
            }
            return _choices.ToArray();
        }

        public ProfessionalVM GetAssetHolderFromInfo(int? ID, string SiteUrl)
        {
            var list = SPConnector.GetListItem(SP_ASSET_HOLDER_FROM_LIST_NAME, ID, SiteUrl);
            var viewmodel = new ProfessionalVM();
            viewmodel.ID = Convert.ToInt32(ID);
            viewmodel.ProfesionalName = Convert.ToString(list["Title"]);
            viewmodel.ContactNo = Convert.ToString(list["ProjectUnit"]);
            viewmodel.ProjectName = Convert.ToString(list["ContactNo"]);


            //viewmodel.VendorID = Convert.ToString(list["VendorID"]);
            //viewmodel.VendorName = Convert.ToString(list["Vendor"]);
            //viewmodel.PoNo = Convert.ToString(list["PoNo"]);

            return viewmodel;
        }

        
    }
}
