using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Utils;
using NLog;
using static MCAWebAndAPI.Model.ViewModel.Form.Finance.Shared;

namespace MCAWebAndAPI.Service.Finance
{
    public class SCAReimbursementService : ISCAReimbursementService
    {
        private string siteUrl = string.Empty;
        static Logger logger = LogManager.GetCurrentClassLogger();

        public void SetSiteUrl(string siteUrl)
        {
            this.siteUrl = siteUrl;
        }

        public SCAReimbursementVM Get(Operations op, int? id = default(int?))
        {
            if (op != Operations.c && id == null)
                throw new InvalidOperationException(ErrorDevInvalidState);

            var viewModel = new SCAReimbursementVM();

            if (id != null)
            {
               
                //var listItem = SPConnector.GetListItem(ListName, id, siteUrl);
                //viewModel = ConvertToSCASettlementVM(listItem);

                //viewModel.ItemDetails = GetSCASettlementItemDetails(id.Value);
            }

            viewModel.Operation = op;

            return viewModel;
        }

        public int? Save(SCAReimbursementVM scaSettlement)
        {
            int? result = null;

            return result;
        }

     
    }
}
