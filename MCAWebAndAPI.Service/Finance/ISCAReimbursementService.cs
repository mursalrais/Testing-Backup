using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using static MCAWebAndAPI.Model.ViewModel.Form.Finance.Shared;

namespace MCAWebAndAPI.Service.Finance
{
    public interface ISCAReimbursementService
    {
        SCAReimbursementVM Get(Operations op, int? id = default(int?));

        int? Save(SCAReimbursementVM scaSettlement);

    }
}
