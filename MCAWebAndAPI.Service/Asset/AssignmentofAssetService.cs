using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using NLog;

namespace MCAWebAndAPI.Service.Asset
{
    public class AssignmentofAssetService : IAssignmentofAssetService
    {
        string _siteUrl = "eceos2.sharepoint.com/sites/mca-dev/hr";
        static Logger logger = LogManager.GetCurrentClassLogger();

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = siteUrl;
        }

        public AssignmentofAssetVM GetAssignmentofAsset()
        {
            throw new NotImplementedException();
        }

        public bool CreateAssignmentofAsset(AssignmentofAssetVM assignmentofAsset)
        {
            throw new NotImplementedException();
        }

        public bool UpdateAssignmentofAsset(AssignmentofAssetVM assignmentofAsset)
        {
            throw new NotImplementedException();
        }

        IEnumerable<AssignmentofAssetVM> IAssignmentofAssetService.GetAssignmentofAsset()
        {
            throw new NotImplementedException();
        }

        public bool CreateAssignmentofAsset_Dummy(AssignmentofAssetItemVM assignmentofAsset)
        {
            var entity = new AssignmentofAssetItemVM();
            entity = assignmentofAsset;
            return true;
        }

        public bool UpdateAssignmentofAsset_Dummy(AssignmentofAssetItemVM assignmentofAsset)
        {
            throw new NotImplementedException();
        }

        public bool DestroyAssignmentofAsset_Dummy(AssignmentofAssetItemVM assignmentofAsset)
        {
            throw new NotImplementedException();
        }

        public AssignmentofAssetVM GetAssignmentofAssetItems_Dummy()
        {
            var viewModel = new AssignmentofAssetVM();

            var list = new List<AssignmentofAssetItemVM>();
            list.Add(new AssignmentofAssetItemVM()
            {
                NewAsset = "New",
                Item = "Chair",
                AssetDescription = "New Asset",
                Id = 1
            });
            viewModel.Header.TransactionType = "Cash";
            viewModel.Items = list;

            return viewModel;
        }
    }
}
