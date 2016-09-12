using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using static MCAWebAndAPI.Model.ViewModel.Form.Finance.Shared;

namespace MCAWebAndAPI.Service.Finance
{
    /// <summary>
    /// Wirefram FIN07: SCA Settlement
    /// </summary>

    public interface ISCASettlementService
    {
        SCASettlementVM Get(Operations op, int? id = default(int?));

        int? Save(SCASettlementVM scaSettlement);

        decimal GetAllSCAVoucherAmount(int scaVoucherId, int scaSettlementID);
    }
}
