﻿using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Utils;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using static MCAWebAndAPI.Model.ViewModel.Form.Finance.Shared;
using Microsoft.SharePoint.Client;

namespace MCAWebAndAPI.Service.Finance
{
    /// <summary>
    /// Wireframe FIN11: Petty Cash Settlement
    /// 
    ///     Petty Cash Settlement is a transaction for settlement-reimbursement of petty cash where 
    ///     user has already asked for petty cash advance previously. 
    ///     
    ///     Through this feature, user will create the settlement-reimbursement of 
    ///     petty cash which results whether user needs to return the excess petty cash advance or 
    ///     receive the reimbursement in the case where the actual expense for 
    ///     petty cash exceeds the petty cash advance given. 
    ///     
    ///     It is created and maintained by finance. 																									
    ///
    /// </summary>

    public class PettyCashSettlementService : IPettyCashSettlementService
    {
        public const string ListName = "Petty Cash Settlement";

        private const string FieldName_ID = "ID";
        private const string FieldName_Date = "Settlement_x0020_Date";
        private const string FieldName_PettyCashVoucherId = "PettyCashVoucherId";
        private const string FieldName_PettyCashVoucherAdvanceReceivedDate = "PettyCashVoucherId_x003a_Advance";
        private const string FieldName_PettyCashVoucherNo = "PettyCashVoucherId_x003a_Voucher";
        private const string FieldName_AmountLiquidated = "Amount_x0020_Liquidated";
        private const string FieldName_AmountReimbursedOrReturned = "Amount_x0020_Reimbursed_x002f_Re";
        private const string FieldName_Remarks = "Remarks";

        private string siteUrl = string.Empty;
        static Logger logger = LogManager.GetCurrentClassLogger();

        public int? Save(PettyCashSettlementVM viewModel)
        {
            int? result = null;
            var columnValues = new Dictionary<string, object>
           {
                {FieldName_Date, viewModel.Date},
                {FieldName_PettyCashVoucherNo, viewModel.PettyCashVoucher},
                {FieldName_AmountLiquidated, viewModel.AmountLiquidated},
                {FieldName_AmountReimbursedOrReturned, viewModel.Amount},
                { FieldName_Remarks, viewModel.Remarks}
            };

            try
            {
                SPConnector.AddListItem(ListName, columnValues, siteUrl);
                result = SPConnector.GetLatestListItemID(ListName, siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }

            return result;
        }

        public Task CreateAttachmentAsync(int? ID, IEnumerable<HttpPostedFileBase> attachment)
        {
            throw new NotImplementedException();
        }

        public void SetSiteUrl(string siteUrl)
        {
            this.siteUrl = siteUrl;
        }

        public PettyCashSettlementVM Get(Operations op, int? id = default(int?))
        {
            if (op != Operations.c && id == null)
                throw new InvalidOperationException(ErrorDevInvalidState);

            var viewModel = new PettyCashSettlementVM();

            if (id != null)
            {
                var listItem = SPConnector.GetListItem(ListName, id, siteUrl);
                viewModel = ConvertToVM(siteUrl, listItem);
            }

            viewModel.Operation = op;

            return viewModel;
        }

        public delegate PettyCashTransactionItem ConvertToVMDelegate(string siteUrl, ListItem listItem);

        private static PettyCashSettlementVM ConvertToVM(string siteUrl, ListItem listItem)
        {
            PettyCashSettlementVM viewModel = new PettyCashSettlementVM();

            viewModel.ID = Convert.ToInt32(listItem[FieldName_ID]);
            viewModel.Date = Convert.ToDateTime(listItem[FieldName_Date]);

            if (viewModel.PettyCashVoucher != null)
            {
                viewModel.PettyCashVoucher.Value = (listItem[FieldName_PettyCashVoucherNo] as FieldLookupValue).LookupId;
                viewModel.PettyCashVoucher.Text = (listItem[FieldName_PettyCashVoucherNo] as FieldLookupValue).LookupValue;
            }

            viewModel.AmountLiquidated = Convert.ToDecimal(listItem[FieldName_AmountLiquidated]);
            viewModel.Amount= Convert.ToDecimal(listItem[FieldName_AmountReimbursedOrReturned]);

            viewModel.Remarks = Convert.ToString(listItem[FieldName_Remarks]);

            return viewModel;
        }

        public static IEnumerable<PettyCashTransactionItem> GetPettyCashTransaction(string siteUrl, DateTime dateFrom, DateTime dateTo)
        {
            return SharedService.GetPettyCashTransaction(siteUrl, dateFrom, dateTo, ListName, FieldName_Date,
                ConvertToVM);
        }
    }
}
