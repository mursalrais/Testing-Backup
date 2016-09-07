using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using NLog;
using static MCAWebAndAPI.Model.ViewModel.Form.Finance.PettyCashTransactionItem;
using static MCAWebAndAPI.Model.ViewModel.Form.Finance.Shared;

namespace MCAWebAndAPI.Service.Finance
{
    /// <summary>
    /// Wireframe FIN13: Petty Cash Journal
    /// </summary>

    public class PettyCashJournalService : IPettyCashJournalService
    {
        private const string ListName = "Petty Cash Journal";
        private const string ListName_Item = "Petty Cash Journal Item";
        private const string ListName_Settlement = "Petty Cash Settlement";
        private const string ListNam_reimbursement = "Petty Cash Reimbursement";

        #region settlement
        private const string FieldName_Settlement_Date = "Settlement_x0020_Date";
        private const string FieldName_DocumentNo = "Title";
        private const string FieldName_PaidTo = "Paid_x0020_To";
        private const string FieldName_Reason = "Reason_x0020_of_x0020_Payment";
        private const string FieldName_WBS = "WBS";
        private const string FieldName_GL = "GL";
        private const string FieldName_AmountLiquidated = "Amount_x0020_Liquidated";
        #endregion

        #region reimbursement
        private const string FieldName_DocNo = "Title";
        private const string FieldName_Reimbursement_Date = "Reimbursement_x0020_Date";
        private const string FieldName_Reimbursement_PaidTo = "Paid_x0020_To";
        private const string FieldName_Reimbursement_Reason = "Reason_x0020_of_x0020_Payment";
        private const string FieldName_WBSID = "WBS_x0020_ID_x003a_WBS_x0020_ID";
        private const string FieldName_WBSDesc = "WBS_x0020_ID_x003a_WBS_x0020_Des";
        private const string FieldName_GLNo = "GL_x0020_ID_x003a_GL_x0020_No";
        private const string FieldName_GLDesc = "GL_x0020_ID_x003a_GL_x0020_Descr";
        private const string FieldName_Reimbursement_AmountLiquidated = "Amount_x0020_Liquidated";
        #endregion

        #region Journal
        private const string FieldName_ID = "ID";
        private const string FieldName_DateFrom = "DateFrom";
        private const string FieldName_DateTo = "DateTo";
        private const string FieldName_TotalAmountToBeReplenished = "TotalAmountToBeReplenished";
        private const string FieldName_Additional1 = "Additional1";
        private const string FieldName_Additional2 = "Additional2";
        private const string FieldName_Additional3 = "Additional3";
        private const string FieldName_CashOnHand = "CashOnHand";
        private const string FieldName_TotalPettyCashOnHand = "TotalPettyCashOnHand";
        #endregion

        #region Journal Item
        private const string FieldName_Item_Title = "Title";
        private const string FieldName_Item_Date = "Date";
        private const string FieldName_Item_Payee = "Payee";
        private const string FieldName_Item_DescOfExpenses = "DescOfExpenses";
        private const string FieldName_Item_WBS = "WBS";
        private const string FieldName_Item_GL = "GL";
        private const string FieldName_Item_PettyCashJournalID = "PettyCashJournalID";
        private const string FieldName_Item_Amount = "Amount";
        #endregion

        private const string FieldName_Fund = "Fund";

        private string siteUrl = string.Empty;
        static Logger logger = LogManager.GetCurrentClassLogger();

        public void SetSiteUrl(string siteUrl)
        {
            this.siteUrl = siteUrl;
        }

        public int? Create(PettyCashJournalVM viewModel)
        {
            int? result = null;
            var columnValues = new Dictionary<string, object>
            {
                {FieldName_DateFrom, viewModel.DateFrom},
                {FieldName_DateTo, viewModel.DateTo},
                {FieldName_TotalAmountToBeReplenished, viewModel.TotalAmount},
                {FieldName_Additional1, viewModel.AdvancesForOperationalCar1},
                {FieldName_Additional2, viewModel.AdvancesForOperationalCar2},
                {FieldName_Additional3, viewModel.AdvancesForOperationalCar3},
                {FieldName_CashOnHand, viewModel.CashOnhand}
            };

            try
            {
                if (viewModel.Operation == Operations.c)
                {
                    SPConnector.AddListItem(ListName, columnValues, siteUrl);
                    result = SPConnector.GetLatestListItemID(ListName, siteUrl);
                }
                else if (viewModel.Operation == Operations.e)
                {
                    SPConnector.UpdateListItem(ListName, viewModel.ID, columnValues, siteUrl);
                    result = viewModel.ID;
                }
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }

            return result;
        }

        public async Task CreateItems(int? ID, PettyCashJournalVM viewModel)
        {
            try
            {
                if (ID != null)
                {
                    //delete old data
                    if (viewModel.ItemEdited)
                    {
                        var listItemID = GetIDItemDetails(siteUrl, (int)ID);
                        SPConnector.DeleteMultipleListItemAsync(ListName_Item, listItemID, siteUrl);
                    }

                    foreach (var item in viewModel.ItemDetails)
                    {
                        var columnValues = new Dictionary<string, object>
                        {
                            {FieldName_Item_Title, item.PCVNo},
                            {FieldName_Item_Date, item.Date},
                            {FieldName_Item_Payee, item.Payee},
                            {FieldName_Item_DescOfExpenses, item.DescOfExpenses},
                            {FieldName_Item_WBS, item.WBS},
                            {FieldName_Item_GL, item.GL},
                            {FieldName_Item_PettyCashJournalID, new FieldLookupValue { LookupId = Convert.ToInt32(ID) } },
                            {FieldName_Item_Amount, item.Amount},
                        };
                        if (viewModel.Operation == Operations.c || viewModel.ItemEdited)
                            SPConnector.AddListItem(ListName_Item, columnValues, siteUrl);
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }
        }

        public PettyCashJournalVM Get(Operations op, int? id = default(int?))
        {
            var viewModel = new PettyCashJournalVM();

            switch (op)
            {
                case Operations.c:
                    // When creating a new PC Journal, take data from PC Settlement & Reimbursement
                    IEnumerable<PettyCashTransactionItem> transactionItems = GetTransactionItems(viewModel.DateFrom, viewModel.DateTo);

                    var items = new List<PettyCashJournalItemVM>();

                    foreach (var item in transactionItems)
                    {
                        items.Add(ConvertToItemVM(item));
                    }

                    viewModel.ItemDetails = items;


                    break;

                case Operations.e:
                case Operations.v:
                    //When viewing or editing, take data from PettyCashJournalItem

                    if (id == null)
                        throw new InvalidOperationException(ErrorDevInvalidState);

                    var listItem = SPConnector.GetListItem(ListName, id, siteUrl);
                    viewModel = ConvertToVM(listItem);

                    viewModel.ItemDetails = GetItemDetails(this.siteUrl, (int)id);

                    break;
            }

            viewModel.Operation = op;

            return viewModel;
        }

        public IEnumerable<PettyCashJournalItemVM> Get(int? id = default(int?))
        {
            var viewModel = new List<PettyCashJournalItemVM>();

            var caml = CamlQueryUtil.Generate(FieldName_Item_PettyCashJournalID, "Lookup", id.ToString());

            foreach (var list in SPConnector.GetList(ListName_Item, siteUrl, caml))
            {
                try
                {
                    viewModel.Add(ConvertToItemVM(list));
                }
                catch (ServerException e)
                {
                    throw e;
                }
                catch (Exception e)
                {

                    throw e;
                }
            }
            return viewModel;
        }

        public IEnumerable<PettyCashJournalItemVM> GetPettyCashTransactions(DateTime dateFrom, DateTime dateTo)
        {
            var listView = new List<PettyCashJournalItemVM>();
            IEnumerable<PettyCashTransactionItem> transactionItems = GetTransactionItems(dateFrom, dateTo);
            foreach (var item in transactionItems)
            {
                listView.Add(ConvertToItemVM(item));
            }

            return listView;
        }

        private void GetPettyCashSettlement(DateTime dateFrom, DateTime dateTo, ref List<PettyCashJournalItemVM> listView)
        {
            var from = String.Format("{0}-{1}-{2}", dateFrom.Year, dateFrom.Month, dateFrom.Day);
            var to = String.Format("{0}-{1}-{2}", dateTo.Year, dateTo.Month, dateTo.Day);

            var caml = @"<View><Query><Where><And><Geq><FieldRef Name='" + FieldName_Settlement_Date + "' /><Value Type='DateTime'>" + from + "</Value></Geq><Leq><FieldRef Name='" + FieldName_Settlement_Date + "' /><Value Type='DateTime'>" + to + "</Value></Leq></And></Where></Query></View>";

            foreach (var listItem in SPConnector.GetList(ListName_Settlement, siteUrl, caml))
            {
                var item = new PettyCashJournalItemVM();

                try
                {
                    item.Date = Convert.ToDateTime(listItem[FieldName_Settlement_Date]);
                    item.PCVNo = Convert.ToString(listItem[FieldName_DocumentNo]);
                    item.Payee = Convert.ToString(listItem[FieldName_PaidTo]);
                    item.DescOfExpenses = Convert.ToString(listItem[FieldName_Reason]);
                    item.WBS = Convert.ToString(listItem[FieldName_WBS]);
                    item.GL = Convert.ToString(listItem[FieldName_GL]);
                    item.Amount = Convert.ToDecimal(listItem[FieldName_AmountLiquidated]);

                    listView.Add(item);
                }
                catch (ServerException e)
                {
                    throw e;
                }
                catch (Exception e)
                {

                    throw e;
                }
            }
        }

        private void GetPettyCashReiumbursement(DateTime dateFrom, DateTime dateTo, ref List<PettyCashJournalItemVM> listView)
        {
            var from = String.Format("{0}-{1}-{2}", dateFrom.Year, dateFrom.Month, dateFrom.Day);
            var to = String.Format("{0}-{1}-{2}", dateTo.Year, dateTo.Month, dateTo.Day);
            var caml = @"<View><Query><Where><And><Geq><FieldRef Name='" + FieldName_Reimbursement_Date + "' /><Value Type='DateTime'>" + from + "</Value></Geq><Leq><FieldRef Name='" + FieldName_Reimbursement_Date + "' /><Value Type='DateTime'>" + to + "</Value></Leq></And></Where></Query></View>";

            foreach (var listItem in SPConnector.GetList(ListNam_reimbursement, siteUrl, caml))
            {
                var item = new PettyCashJournalItemVM();

                try
                {
                    item.Date = Convert.ToDateTime(listItem[FieldName_Reimbursement_Date]);
                    item.PCVNo = Convert.ToString(listItem[FieldName_DocumentNo]);
                    item.Payee = Convert.ToString(listItem[FieldName_Reimbursement_PaidTo]);
                    item.DescOfExpenses = Convert.ToString(listItem[FieldName_Reimbursement_Reason]);
                    item.WBS = "";//string.Format("{0} - {1}", (listItem[FieldName_WBSID] as FieldLookupValue).LookupValue.ToString(), (listItem[FieldName_WBSDesc] as FieldLookupValue).LookupValue.ToString());
                    item.GL = ""; //string.Format("{0} - {1}", (listItem[FieldName_GLNo] as FieldLookupValue).LookupValue.ToString(), (listItem[FieldName_GLDesc] as FieldLookupValue).LookupValue.ToString());
                    item.Amount = Convert.ToDecimal(listItem[FieldName_Reimbursement_AmountLiquidated]);

                    listView.Add(item);
                }
                catch (ServerException e)
                {
                    throw e;
                }
                catch (Exception e)
                {

                    throw e;
                }
            }
        }

        private PettyCashJournalVM ConvertToVM(ListItem listItem)
        {
            return new PettyCashJournalVM
            {
                ID = Convert.ToInt32(listItem[FieldName_ID]),
                DateFrom = Convert.ToDateTime(listItem[FieldName_DateFrom]),
                DateTo = Convert.ToDateTime(listItem[FieldName_DateTo]),
                TotalAmount = Convert.ToDecimal(listItem[FieldName_TotalAmountToBeReplenished]),
                AdvancesForOperationalCar1 = Convert.ToDecimal(listItem[FieldName_Additional1]),
                AdvancesForOperationalCar2 = Convert.ToDecimal(listItem[FieldName_Additional2]),
                AdvancesForOperationalCar3 = Convert.ToDecimal(listItem[FieldName_Additional3]),
                CashOnhand = Convert.ToDecimal(listItem[FieldName_CashOnHand])
            };
        }

        private static PettyCashJournalItemVM ConvertToItemVM(ListItem listItem)
        {
            return new PettyCashJournalItemVM
            {
                ID = Convert.ToInt32(listItem[FieldName_ID]),
                PCVNo = Convert.ToString(listItem[FieldName_Item_Title]),
                Date = Convert.ToDateTime(listItem[FieldName_Item_Date]),
                Payee = Convert.ToString(listItem[FieldName_Item_Payee]),
                DescOfExpenses = Convert.ToString(listItem[FieldName_Item_DescOfExpenses]),
                WBS = Convert.ToString(listItem[FieldName_Item_WBS]),
                GL = Convert.ToString(listItem[FieldName_Item_GL]),
                PettyCashJournalID = Convert.ToInt32((listItem[FieldName_Item_PettyCashJournalID] as FieldLookupValue).LookupId),
                Amount = Convert.ToDecimal(listItem[FieldName_Item_Amount]),
            };
        }

        private static PettyCashJournalItemVM ConvertToItemVM(PettyCashTransactionItem item)
        {
            return new PettyCashJournalItemVM
            {
                PCVNo = item.TransactionNo,
                Date = item.Date,
                Payee = item.Title,
                Amount = (decimal)item.Amount
            };
        }

        private IEnumerable<PettyCashTransactionItem> GetTransactionItems(DateTime dateFrom, DateTime dateTo)
        {
            List<PettyCashTransactionItem> pettyCashTransactions = new List<PettyCashTransactionItem>();

            //TODO: review DR & CR posts
            pettyCashTransactions.AddRange(PettyCashSettlementService.GetPettyCashTransaction(siteUrl, dateFrom, dateTo, Post.DR));
            pettyCashTransactions.AddRange(PettyCashReimbursementService.GetPettyCashTransaction(siteUrl, dateFrom, dateTo, Post.DR));

            return pettyCashTransactions;
        }

        private static IEnumerable<PettyCashJournalItemVM> GetItemDetails(string siteUrl, int headerID)
        {
            List<PettyCashJournalItemVM> details = null;

            if (headerID > 0)
            {
                var caml = @"<View><Query><Where><Eq><FieldRef Name='" + FieldName_Item_PettyCashJournalID + "' /><Value Type='Lookup'>" + headerID.ToString() + "</Value></Eq></Where></Query></View>";

                details = new List<PettyCashJournalItemVM>();

                foreach (var item in SPConnector.GetList(ListName_Item, siteUrl, caml))
                {
                    details.Add(ConvertToItemVM(item));
                }
            }

            return details;
        }

        private static List<string> GetIDItemDetails(string siteUrl, int headerID)
        {
            List<string> details = new List<string>();

            if (headerID > 0)
            {
                var caml = @"<View><Query><Where><Eq><FieldRef Name='" + FieldName_Item_PettyCashJournalID + "' /><Value Type='Lookup'>" + headerID.ToString() + "</Value></Eq></Where></Query></View>";

                foreach (var item in SPConnector.GetList(ListName_Item, siteUrl, caml))
                {
                    details.Add(item[FieldName_ID].ToString());
                }
            }

            return details;
        }
    }
}