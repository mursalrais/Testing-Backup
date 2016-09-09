using System;
using System.Collections.Generic;
using System.Linq;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using Microsoft.SharePoint.Client;
using static MCAWebAndAPI.Model.ViewModel.Form.Finance.PettyCashTransactionItem;
using System.Threading.Tasks;
using System.Collections.Concurrent;

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

            var pettyCashStatements = new List<PettyCashTransactionItem>();
            var list1 = new List<PettyCashTransactionItem>();
            var list2 = new List<PettyCashTransactionItem>();
            var list3 = new List<PettyCashTransactionItem>();
            var list4 = new List<PettyCashTransactionItem>();

            //We probably could use ConcurrentBag (thread safe list), but ConcurrentBag does not have AddRange method so we will have to loop to add to the ConcurrentBag.
            //That will be on separate thread, so perhaps the loop is not so bad.
            //But still not which one is faster: using ConcurrentBag with loop or 4 separate list as below.
            Task cashPaymentVoucherService = Task.Run(() => { list1.AddRange(PettyCashPaymentVoucherService.GetPettyCashTransaction(siteUrl, dateFrom, dateTo, Post.CR)); });
            Task cashSettlementService = cashSettlementService = Task.Run(() => { list2.AddRange(PettyCashSettlementService.GetPettyCashTransaction(siteUrl, dateFrom, dateTo, Post.CR)); });
            Task cashReimbursementService = Task.Run(() => { list3.AddRange(PettyCashReimbursementService.GetPettyCashTransaction(siteUrl, dateFrom, dateTo, Post.CR)); });
            Task cashReplenishmentService = Task.Run(() => { list4.AddRange(PettyCashReplenishmentService.GetPettyCashTransaction(siteUrl, dateFrom, dateTo, Post.DR)); });
            Task.WaitAll(cashPaymentVoucherService, cashSettlementService, cashReimbursementService, cashReplenishmentService);

            pettyCashStatements.AddRange(list1);
            pettyCashStatements.AddRange(list2);
            pettyCashStatements.AddRange(list3);
            pettyCashStatements.AddRange(list4);

            decimal runningTotal = 0;
            List<PettyCashTransactionItem> ordered = pettyCashStatements.OrderBy(o => o.Date)
                .Select(i => 
                    {
                        decimal currentAmount = 0;
                        if (i.Amount.HasValue)
                        {
                            currentAmount = i.Amount.Value;
                            runningTotal += currentAmount;
                        }
                        return new PettyCashTransactionItem()
                        {
                            ID = i.ID,
                            Title = i.Title,
                            EditMode = i.EditMode,
                            Date = i.Date,
                            TransactionType = i.TransactionType,
                            TransactionNo = i.TransactionNo,
                            Currency = i.Currency,
                            Amount = i.Amount,
                            Balance = runningTotal
                        };
                   }).ToList();

            return ordered;

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