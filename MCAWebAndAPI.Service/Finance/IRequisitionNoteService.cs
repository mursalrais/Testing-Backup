using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

using MCAWebAndAPI.Model.ViewModel.Form.Finance;

namespace MCAWebAndAPI.Service.Finance.RequisitionNote
{
    /// <summary>
    /// Wireframe FIN05: Requisition Note
    ///     i.e.: Purchase Requisition Note
    /// </summary>

    public interface IRequisitionNoteService
    {
        RequisitionNoteVM Get(int? ID);

        Task<RequisitionNoteVM> GetAsync(int? ID);

        IEnumerable<GLMasterVM> GetGLMasters();
        
        IEnumerable<WBSMasterVM> GetWBSMaster(string activiy);
       
        int CreateRequisitionNote(RequisitionNoteVM viewModel);
        bool UpdateRequisitionNote(RequisitionNoteVM viewModel);

        void CreateRequisitionNoteItems(int? headerID, IEnumerable<RequisitionNoteItemVM> noteItems);

        Task CreateRequisitionNoteItemsAsync(int? headerID, IEnumerable<RequisitionNoteItemVM> noteItems);

        Task CreateRequisitionNoteAttachmentsSync(int? headerID, IEnumerable<HttpPostedFileBase> documents);

        Task EditRequisitionNoteItemsAsync(int? headerID, IEnumerable<RequisitionNoteItemVM> noteItems);
        Task EditRequisitionNoteAttachmentsSync(int? headerID, IEnumerable<HttpPostedFileBase> documents);
        
        void DeleteDetail(int id);

        Tuple<int, string> GetIdAndNoByEventBudgetID(int eventBudgetId);

    }
}