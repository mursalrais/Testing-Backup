using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

using MCAWebAndAPI.Model.ViewModel.Form.Finance;

namespace MCAWebAndAPI.Service.Finance.RequisitionNote
{
    public interface IRequisitionNote
    {
        void SetSiteUrl(string siteUrl);
        RequisitionNoteVM GetRequisitionNote(int? ID);

        IEnumerable<GLMasterVM> GetGLMaster();
        
        IEnumerable<WBSMasterVM> GetWBSMaster(string activiy);
       
        int CreateRequisitionNote(RequisitionNoteVM viewModel);
        bool UpdateRequisitionNote(RequisitionNoteVM viewModel);

        Task CreateRequisitionNoteItemsAsync(int? headerID, IEnumerable<RequisitionNoteItemVM> noteItems);

        Task CreateRequisitionNoteAttachmentsSync(int? headerID, IEnumerable<HttpPostedFileBase> documents);

        Task EditRequisitionNoteItemsAsync(int? headerID, IEnumerable<RequisitionNoteItemVM> noteItems);
        Task EditRequisitionNoteAttachmentsSync(int? headerID, IEnumerable<HttpPostedFileBase> documents);

    }
}