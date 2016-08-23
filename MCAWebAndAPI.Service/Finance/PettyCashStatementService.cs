using System;
using System.Collections.Generic;
using MCAWebAndAPI.Model.Form.Finance;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using Microsoft.SharePoint.Client;

namespace MCAWebAndAPI.Service.Finance
{
    public class PettyCashStatementService : IPettyCashStatementService
    {
        /// <summary>
        /// FIN15: Petty Cash Statement
        /// </summary>

        string siteUrl = string.Empty;

        public IEnumerable<PettyCashTransactionItem> GetPettyCashStatements(DateTime dateFrom, DateTime dateTo)
        {
            List<PettyCashTransactionItem> pettyCashStatements = new List<PettyCashTransactionItem>();

            pettyCashStatements.AddRange(PettyCashPaymentVoucherService.GetPettyCashTransaction(siteUrl, dateFrom, dateTo));
            pettyCashStatements.AddRange(PettyCashSettlementService.GetPettyCashTransaction(siteUrl, dateFrom, dateTo));
            pettyCashStatements.AddRange(PettyCashReimbursementService.GetPettyCashTransaction(siteUrl, dateFrom, dateTo));
            pettyCashStatements.AddRange(PettyCashReplenishmentService.GetPettyCashTransaction(siteUrl, dateFrom, dateTo));

            pettyCashStatements.Sort((x, y) => x.Date.CompareTo(y.Date));

            return pettyCashStatements;
        }

        public void SetSiteUrl(string siteUrl)
        {
            this.siteUrl = siteUrl;
        }

        private PettyCashStatementVM ConvertToVM(ListItem listItem)
        {
            PettyCashStatementVM viewModel = new PettyCashStatementVM();
        
            return viewModel;
        }

    }
}
