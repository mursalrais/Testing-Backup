using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using static MCAWebAndAPI.Model.ViewModel.Form.Finance.Shared;
using MCAWebAndAPI.Model.HR.DataMaster;

namespace MCAWebAndAPI.Service.Finance
{
    public interface IPettyCashReimbursementService
    {
        /// <summary>
        ///     Wireframe FIN12: Petty Cash Reimbursement
        ///         Petty Cash Reimbursement is a transaction for the reimbursement of petty cash only when
        ///         user has not asked for any petty cash advance.
        ///
        ///         Through this feature, finance will create the reimbursement of petty cash which results in 
        ///         user needs to receive the reimbursement. 
        /// </summary>

        void SetSiteUrl(string siteUrl);

        int? Save(ref PettyCashReimbursementVM viewModel, IEnumerable<ProfessionalMaster> professionals);

        Task CreateAttachmentAsync(int? ID, IEnumerable<HttpPostedFileBase> attachment);

        PettyCashReimbursementVM Get(Operations op, int? id = default(int?));

        PettyCashReimbursementVM GetPettyCashReimbursement(int? ID);
    }
}
