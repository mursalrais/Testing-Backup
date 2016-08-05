using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;

namespace MCAWebAndAPI.Service.Asset
{
    public interface IAssignmentofAssetService
    {
        void SetSiteUrl(string siteUrl);

        IEnumerable<AssignmentofAssetVM> GetAssignmentofAsset();

        AssignmentofAssetVM GetPopulatedModel(int? id = null);

        AssignmentofAssetVM GetHeader(int? ID);

        int CreateHeader(AssignmentofAssetVM header);

        bool UpdateHeader(AssignmentofAssetVM header);

        bool CreateAssignmentofAsset(AssignmentofAssetVM assignmentofAsset);

        bool UpdateAssignmentofAsset(AssignmentofAssetVM assignmentofAsset);

        void CreateMonthlyFeeDetails(int? headerID, IEnumerable<AssignmentofAssetVM> monthlyFeeDetails);

        bool CreateAssignmentofAsset_Dummy(AssignmentofAssetItemVM assignmentofAsset);

        bool UpdateAssignmentofAsset_Dummy(AssignmentofAssetItemVM assignmentofAsset);

        bool DestroyAssignmentofAsset_Dummy(AssignmentofAssetItemVM assignmentofAsset);

        AssignmentofAssetVM GetAssignmentofAssetItems_Dummy();
    }
}
