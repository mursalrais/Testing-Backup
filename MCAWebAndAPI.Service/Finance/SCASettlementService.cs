using System;
using System.Collections.Generic;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using NLog;
using static MCAWebAndAPI.Model.ViewModel.Form.Finance.Shared;

namespace MCAWebAndAPI.Service.Finance
{
    public class SCASettlementService : ISCASettlementService
    {
        /// <summary>
        /// Wirefram FIN07: SCA Settlement
        /// </summary>

        public const string ListName = "SCA Settlement";
        private const string ListName_SCASettlementItem = "SCA Settlement Item";

        private const string FIELD_FORMAT_DOC = "SSCA/{0}-{1}/";
        private const int DIGIT_DOCUMENTNO = 5;

        private const string FieldNameId = "ID";
        private const string FieldNameSCADocumentNo = "Title";
        private const string FieldNameSCAVoucherId = "SCAVoucher";
        private const string FieldNameSCAPurpose = "SCAVoucher_x003a_Purpose";
        private const string FieldNameSCAFund = "SCAVoucher_x003a_Fund";
        private const string FieldNameSCACurrency = "SCAVoucher_x003a_Currency";
        private const string FieldNameSCAVoucherNo = "SCAVoucher_x003a_SCA_x0020_No";
        private const string FieldNameSCAAmount = "SCAVoucher_x003a_Total_x0020_Amo";
        private const string FieldNameTotalExpense = "TotalExpense";
        private const string FieldNameReceivedFromTo = "ReceivedFromTo";
        private const string FieldNameDetailReceiptDate = "ReceiptDate";
        private const string FieldNameDetailReceiptNo = "ReceiptNo";
        private const string FieldNameDetailPayee = "Payee";
        private const string FieldNameDetailDescription = "DescriptionOfExpense";

        private const string FieldNameDetailWBSId = "WBSID";
        private const string FieldNameDetailWBSDescription = "WBSDescription";

        private const string FieldNameDetailGL = "GL";
        private const string FieldNameDetailAmount = "AmountPerItem";
        private const string FieldNameDetailSCAHeaderID = "SCA_x0020_Settlement";
        private const string FieldNameDetailGLNo = "GL_x003a_GL_x0020_No";
        private const string FieldNameDetailGLDesc = "GL_x003a_GL_x0020_Description";
        private const string FieldNameTypeOfSettlement = "Type_x0020_of_x0020_Settlement";

        private string siteUrl = string.Empty;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public SCASettlementService(string siteUrl)
        {
            this.siteUrl = siteUrl;
        }

        public SCASettlementVM Get(Operations op, int? id = default(int?))
        {
            if (op != Operations.c && id == null)
                throw new InvalidOperationException(ErrorDevInvalidState);

            var viewModel = new SCASettlementVM();

            if (id != null)
            {
                var listItem = SPConnector.GetListItem(ListName, id, siteUrl);
                viewModel = ConvertToSCASettlementVM(listItem);

                viewModel.ItemDetails = GetSCASettlementItemDetails(id.Value);
            }

            viewModel.Operation = op;

            return viewModel;
        }

        public int? Save(SCASettlementVM scaSettlement)
        {
            int? result = null;
            var columnValues = new Dictionary<string, object>();
            var documentNoFormat = string.Format(FIELD_FORMAT_DOC, DateTimeExtensions.GetMonthInRoman(DateTime.Now), DateTime.Now.ToString("yy")) + "{0}";

            columnValues.Add(FieldNameSCAVoucherId, new FieldLookupValue { LookupId = Convert.ToInt32(scaSettlement.SCAVoucher.Value) });
            columnValues.Add(FieldNameTotalExpense, scaSettlement.TotalExpense);
            columnValues.Add(FieldNameReceivedFromTo, scaSettlement.ReceivedFromTo);
            columnValues.Add(FieldNameTypeOfSettlement, scaSettlement.TypeOfSettlement.Value);

            try
            {
                if (scaSettlement.Operation == Operations.c)
                {
                    scaSettlement.DocNo = DocumentNumbering.Create(siteUrl, documentNoFormat, DIGIT_DOCUMENTNO);
                    columnValues.Add(FieldNameSCADocumentNo, scaSettlement.DocNo);

                    SPConnector.AddListItem(ListName, columnValues, siteUrl);

                    result = SPConnector.GetLatestListItemID(ListName, siteUrl);

                    scaSettlement.ID = result;
                }
                else if (scaSettlement.Operation == Operations.e)
                {
                    SPConnector.UpdateListItem(ListName, scaSettlement.ID, columnValues, siteUrl);
                    result = scaSettlement.ID;
                }

                SaveSCASettlementDetailItems(scaSettlement.ID, scaSettlement.ItemDetails);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                throw e;
            }

            return result;
        }

        public decimal GetAllSCAVoucherAmount(int scaVoucherId, int scaSettlementID)
        {
            var caml = @"<View><Query><Where><Eq><FieldRef Name='" + FieldNameSCAVoucherId + "' /><Value Type='Lookup'>" + scaVoucherId.ToString() + "</Value></Eq></Where></Query>" +
                         "<ViewFields> <FieldRef Name='" + FieldNameTotalExpense + "' />  </ViewFields></View>";

            decimal result = 0;
            foreach (var listItem in SPConnector.GetList(ListName, siteUrl, caml))
            {
                if (!Convert.ToString(listItem[FieldNameId]).Equals(scaSettlementID.ToString()))
                {
                    result += Convert.ToDecimal(listItem[FieldNameTotalExpense]);
                }
            }

            return result;
        }

        private void SaveSCASettlementDetailItems(int? headerID, IEnumerable<SCASettlementItemVM> viewModels)
        {
            foreach (var viewModel in viewModels)
            {
                if (Item.CheckIfSkipped(viewModel))
                    continue;

                if (Item.CheckIfDeleted(viewModel))
                {
                    try
                    {
                        SPConnector.DeleteListItem(ListName_SCASettlementItem, viewModel.ID, siteUrl);
                    }
                    catch (Exception e)
                    {
                        logger.Error(e);
                        throw e;
                    }
                    continue;
                }

                var updatedValue = new Dictionary<string, object>();

                updatedValue.Add(FieldNameDetailSCAHeaderID, new FieldLookupValue { LookupId = Convert.ToInt32(headerID) });

                var wbsId = Convert.ToInt32(viewModel.WBS.Value);
                var wbs = Common.WBSService.Get(siteUrl, wbsId);
                updatedValue.Add(FieldNameDetailWBSId, wbsId);
                updatedValue.Add(FieldNameDetailWBSDescription, wbs.WBSIDDescription);

                updatedValue.Add(FieldNameDetailGL, new FieldLookupValue { LookupId = Convert.ToInt32(viewModel.GL.Value) });
                updatedValue.Add(FieldNameDetailReceiptDate, viewModel.ReceiptDate);
                updatedValue.Add(FieldNameDetailReceiptNo, viewModel.ReceiptNo);
                updatedValue.Add(FieldNameDetailPayee, viewModel.Payee);
                updatedValue.Add(FieldNameDetailDescription, viewModel.DescriptionOfExpense);
                updatedValue.Add(FieldNameDetailAmount, viewModel.Amount);

                try
                {
                    if (Item.CheckIfCreated(viewModel))
                    {
                        SPConnector.AddListItem(ListName_SCASettlementItem, updatedValue, siteUrl);
                    }
                    else
                    {
                        SPConnector.UpdateListItem(ListName_SCASettlementItem, viewModel.ID, updatedValue, siteUrl);
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw e;
                }
            }
        }

        private IEnumerable<SCASettlementItemVM> GetSCASettlementItemDetails(int HeaderID)
        {
            List<SCASettlementItemVM> details = null;

            if (HeaderID > 0)
            {
                var caml = @"<View><Query><Where><Eq><FieldRef Name='" + FieldNameDetailSCAHeaderID + "' /><Value Type='Lookup'>" + HeaderID.ToString() + "</Value></Eq></Where></Query></View>";

                details = new List<SCASettlementItemVM>();

                foreach (var item in SPConnector.GetList(ListName_SCASettlementItem, siteUrl, caml))
                {
                    details.Add(ConvertToSCASettlementItemVM(item));
                }
            }

            return details;
        }

        private SCASettlementItemVM ConvertToSCASettlementItemVM(ListItem listItem)
        {
            SCASettlementItemVM viewModel = new SCASettlementItemVM();

            viewModel.ID = Convert.ToInt32(listItem[FieldNameId]);
            viewModel.ReceiptDate = Convert.ToDateTime(listItem[FieldNameDetailReceiptDate]);
            viewModel.ReceiptNo = Convert.ToString(listItem[FieldNameDetailReceiptNo]);
            viewModel.Payee = Convert.ToString(listItem[FieldNameDetailPayee]);
            viewModel.DescriptionOfExpense = Convert.ToString(listItem[FieldNameDetailDescription]);

            var wbsId = (listItem[FieldNameDetailGLNo] as FieldLookupValue).LookupId;
            var wbs = Common.WBSService.Get(siteUrl, wbsId);
            viewModel.WBS.Value = wbsId;
            viewModel.WBS.Text = wbs.WBSIDDescription;

            viewModel.GL.Value = (listItem[FieldNameDetailGL] as FieldLookupValue).LookupId;
            viewModel.GL.Text = string.Format("{0}-{1}", (listItem[FieldNameDetailGLNo] as FieldLookupValue).LookupValue, (listItem[FieldNameDetailGLDesc] as FieldLookupValue).LookupValue);

            viewModel.Amount = Convert.ToDecimal(listItem[FieldNameDetailAmount]);

            viewModel.EditMode = (int)Item.Mode.UPDATED;
            return viewModel;
        }

        private SCASettlementVM ConvertToSCASettlementVM(ListItem listItem)
        {
            SCASettlementVM viewModel = new SCASettlementVM();

            viewModel.ID = Convert.ToInt32(listItem[FieldNameId]);
            viewModel.DocNo = Convert.ToString(listItem[FieldNameSCADocumentNo]);

            if (listItem[FieldNameSCAVoucherId] != null)
            {
                viewModel.SCAVoucher.Value = (listItem[FieldNameSCAVoucherId] as FieldLookupValue).LookupId;
                viewModel.SCAVoucher.Text = (listItem[FieldNameSCAVoucherId] as FieldLookupValue).LookupValue;
            }

            viewModel.Description = (listItem[FieldNameSCAPurpose] as FieldLookupValue).LookupValue;
            viewModel.Fund = (listItem[FieldNameSCAFund] as FieldLookupValue).LookupValue;
            viewModel.Currency.Value = (listItem[FieldNameSCACurrency] as FieldLookupValue).LookupValue;
            viewModel.SpecialCashAdvanceAmount = Convert.ToDecimal((listItem[FieldNameSCAAmount] as FieldLookupValue).LookupValue);
            viewModel.EditMode = (int)Item.Mode.UPDATED;
            viewModel.TotalExpense = Convert.ToDecimal(listItem[FieldNameTotalExpense]);
            viewModel.ReceivedFromTo = Convert.ToDecimal(listItem[FieldNameReceivedFromTo]);
            viewModel.TypeOfSettlement.Value = Convert.ToString(listItem[FieldNameTypeOfSettlement]);

            return viewModel;
        }
    }
}