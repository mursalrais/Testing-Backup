using System;
using System.Collections.Generic;
using System.Linq;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using Microsoft.SharePoint.Client;
using static MCAWebAndAPI.Model.ViewModel.Form.Finance.PettyCashTransactionItem;

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

            //TODO: pls convert to async
            pettyCashStatements.AddRange(PettyCashPaymentVoucherService.GetPettyCashTransaction(siteUrl, dateFrom, dateTo, Post.CR));
            pettyCashStatements.AddRange(PettyCashSettlementService.GetPettyCashTransaction(siteUrl, dateFrom, dateTo, Post.CR));
            pettyCashStatements.AddRange(PettyCashReimbursementService.GetPettyCashTransaction(siteUrl, dateFrom, dateTo, Post.CR));
            pettyCashStatements.AddRange(PettyCashReplenishmentService.GetPettyCashTransaction(siteUrl, dateFrom, dateTo, Post.DR));

            List<PettyCashTransactionItem> ordered = pettyCashStatements.OrderBy(o => o.Date).ToList();

            //pettyCashStatements.Sort((x, y) => x.Date.CompareTo(y.Date));

            pettyCashStatements.Sort((x, y) => DateTime.Compare(x.Date, y.Date));

            return pettyCashStatements;

            //return ordered;
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