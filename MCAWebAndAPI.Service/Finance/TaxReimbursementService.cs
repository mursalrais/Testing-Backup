﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using MCAWebAndAPI.Model.ViewModel.Control;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using NLog;
using static MCAWebAndAPI.Model.ViewModel.Form.Finance.Shared;

namespace MCAWebAndAPI.Service.Finance
{
    /// <summary>
    /// Wireframe FIN18: Tax Reimbursement
    /// </summary>
    public class TaxReimbursementService : ITaxReimbursementService
    {
        private const string ListName = "Tax Reimbursement";
        private const string ListName_Document = "Tax Reimbursement Documents";
        private const string ListName_Tax_Reimbursement = "Tax_x0020_Reimbursement";

        private const string FieldName_Id = "ID";
        private const string FieldName_TypeOfTax = "Type_x0020_of_x0020_Tax";
        private const string FieldName_LetterNo = "Title";
        private const string FieldName_LetterDate = "Letter_x0020_Date";
        private const string FieldName_Category = "Category";
        private const string FieldName_Contractor = "Contractor";
        private const string FieldName_VendorID = "VendorId";
        private const string FieldName_Object = "Object";
        private const string FieldName_TaxPeriod = "Tax_x0020_Period";
        private const string FieldName_AmountIDR = "Amount_x0020__x0028_IDR_x0029_";
        private const string FieldName_Remarks = "Remarks";
        private const string FieldName_DocumentNo = "Document_x0020_No_x002e_";
        private const string FieldName_PaymentReceivedDate = "Payment_x0020_Received_x0020_Dat";

        private const string TaxReimbursementDocument_URL = "{0}/Tax%20Reimbursement%20Documents/Forms/AllItems.aspx?FilterField1=Tax_x0020_Reimbursement&FilterValue1={1}";

        private enum TaxTypes { Income, VAT, Others };
        private enum Period { YTD, YTD_Min1, YTD_Min2 }


        string siteUrl = null;
        static Logger logger = LogManager.GetCurrentClassLogger();

        public void SetSiteUrl(string siteUrl)
        {
            this.siteUrl = siteUrl;
        }


        public TaxReimbursementVM Get(Operations op, int? id = default(int?))
        {
            if (op != Operations.c && id == null)
                throw new InvalidOperationException(ErrorDevInvalidState);

            var viewModel = new TaxReimbursementVM();

            if (id != null)
            {
                var listItem = SPConnector.GetListItem(ListName, id, siteUrl);
                viewModel = ConvertToRequisitionNoteVM(listItem);
                viewModel.DocumentUrl = GetDocumentUrl(viewModel.ID);
            }

            viewModel.Operation = op;

            return viewModel;
        }

        public bool Update(TaxReimbursementVM viewModel)
        {
            var updatedValue = new Dictionary<string, object>();

            updatedValue.Add(FieldName_Id, viewModel.ID);
            updatedValue.Add(FieldName_TypeOfTax, viewModel.Type.Value);
            updatedValue.Add(FieldName_LetterNo, viewModel.LetterNo);

            updatedValue.Add(FieldName_LetterDate, viewModel.LetterDate);
            updatedValue.Add(FieldName_Category, viewModel.Category.Value);
            updatedValue.Add(FieldName_VendorID, viewModel.Vendor.Value);
            updatedValue.Add(FieldName_Contractor, viewModel.Contractor);
            updatedValue.Add(FieldName_Object, viewModel.Object);
            updatedValue.Add(FieldName_TaxPeriod, viewModel.Period);
            updatedValue.Add(FieldName_AmountIDR, viewModel.AmountIDR);
            updatedValue.Add(FieldName_DocumentNo, viewModel.DocumentNo);

            try
            {
                SPConnector.UpdateListItem(ListName, viewModel.ID, updatedValue, siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                throw new Exception(ErrorResource.SPInsertError);
            }

            return true;
        }

        public int Create(TaxReimbursementVM viewModel)
        {
            var willCreate = viewModel.ID == null;
            var updatedValue = new Dictionary<string, object>();

            updatedValue.Add(FieldName_TypeOfTax, viewModel.Type.Value);
            updatedValue.Add(FieldName_LetterNo, viewModel.LetterNo);
            updatedValue.Add(FieldName_LetterDate, viewModel.LetterDate);
            updatedValue.Add(FieldName_Category, viewModel.Category.Value);
            updatedValue.Add(FieldName_Contractor, viewModel.Contractor);
            updatedValue.Add(FieldName_VendorID, viewModel.Vendor.Value);
            updatedValue.Add(FieldName_Object, viewModel.Object);
            updatedValue.Add(FieldName_TaxPeriod, viewModel.Period);
            updatedValue.Add(FieldName_AmountIDR, viewModel.AmountIDR);
            updatedValue.Add(FieldName_Remarks, viewModel.Remarks);
            updatedValue.Add(FieldName_PaymentReceivedDate, viewModel.PaymentReceivedDate);
            updatedValue.Add(FieldName_DocumentNo, viewModel.DocumentNo);

            try
            {
                if (viewModel.Operation == Operations.c)
                    SPConnector.AddListItem(ListName, updatedValue, siteUrl);
                else
                    SPConnector.UpdateListItem(ListName, viewModel.ID, updatedValue, siteUrl);

            }
            catch (ServerException e)
            {
                var errMsg = e.Message + Environment.NewLine + e.ServerErrorValue;
                logger.Error(errMsg);

#if DEBUG
                throw new Exception(errMsg);
#else
                 throw new Exception(ErrorResource.SPInsertError);
#endif
            }
            catch (Exception e)
            {
                logger.Error(e.Message);

#if DEBUG
                throw new Exception(e.Message);
#else
                 throw new Exception(ErrorResource.SPInsertError);
#endif
            }


            return SPConnector.GetLatestListItemID(ListName, siteUrl);
        }

        public async Task CreateAttachmentAsync(int? ID, IEnumerable<HttpPostedFileBase> documents)
        {
            CreateAttachment(ID, documents);
        }


        #region Supply data to Landing Page

        public static decimal GetIncomeTax_Reimb_YTD(string siteUrl)
        {
            return GetTaxAmount(siteUrl, TaxTypes.Income, Period.YTD);
        }

        public static decimal GetIncomeTax_Reimb_YTD_Min2(string siteUrl)
        {
            return GetTaxAmount(siteUrl, TaxTypes.Income, Period.YTD_Min2);
        }

        public static decimal GetIncomeTax_Reimb_YTD_Min1(string siteUrl)
        {
            return GetTaxAmount(siteUrl, TaxTypes.Income, Period.YTD_Min1);
        }

        public static decimal GetVAT_Reimb_YTD(string siteUrl)
        {
           return GetTaxAmount(siteUrl, TaxTypes.VAT, Period.YTD);
        }

        public static decimal GetVAT_Reimb_YTD_Min2(string siteUrl)
        {
            return GetTaxAmount(siteUrl, TaxTypes.VAT, Period.YTD_Min2);
        }

        public static decimal GetVAT_Reimb_YTD_Min1(string siteUrl)
        {
            return GetTaxAmount(siteUrl, TaxTypes.VAT, Period.YTD_Min1);
        }

        public static decimal GetOtherTax_Reimb_YTD(string siteUrl)
        {
            return GetTaxAmount(siteUrl, TaxTypes.Others, Period.YTD);
        }

        public static decimal GetOtherTax_Reimb_YTD_Min2(string siteUrl)
        {
            return GetTaxAmount(siteUrl, TaxTypes.Others, Period.YTD_Min2);
        }

        public static decimal GetOtherTax_Reimb_YTD_Min1(string siteUrl)
        {
            return GetTaxAmount(siteUrl, TaxTypes.Others, Period.YTD_Min1);
        }

        private static decimal GetTaxAmount(string siteUrl, TaxTypes taxType, Period period)
        {
            decimal result = 0;
            int year = DateTime.Now.Year;
            string selectedTaxType = string.Empty;

            if (period == Period.YTD_Min1)
            {
                year--;
            }
            else if (period == Period.YTD_Min2)
            {
                year -= 2;
            }

            switch (taxType)
            {
                case TaxTypes.Income:
                    selectedTaxType = TaxTypeComboBoxVM.INCOME;
                    break;

                case TaxTypes.VAT:
                    selectedTaxType = TaxTypeComboBoxVM.VAT;
                    break;

                case TaxTypes.Others:
                    selectedTaxType = TaxTypeComboBoxVM.OTHERS;
                    break;
            }

            var dateFrom = String.Format("{0}-{1}-{2}", year, "01", "01");
            var dateTo = String.Format("{0}-{1}-{2}", year, 12, 31);

            var caml = @"<View><Query><Where>" +
                               "<And><Eq><FieldRef Name='" + FieldName_TypeOfTax + "' /><Value Type='Choice'>" + selectedTaxType + "</Value></Eq>" +
                               "<And><Geq><FieldRef Name='" + FieldName_LetterDate + "' /><Value IncludeTimeValue = 'False' Type='DateTime'>" + dateFrom + "</Value></Geq>" +
                               "<Leq><FieldRef Name='" + FieldName_LetterDate + "' /><Value IncludeTimeValue = 'False' Type='DateTime'>" + dateTo + "</Value></Leq></And></And>" +
                               "</Where></Query>" + 
                               "<ViewFields> <FieldRef Name='"+ FieldName_AmountIDR + "' />  </ViewFields>" +
                               "</View>";

            foreach (var listItem in SPConnector.GetList(ListName, siteUrl, caml))
            {
                result += Convert.ToDecimal(listItem[FieldName_AmountIDR]);
            }

            return result;

        }
        #endregion



        private void CreateAttachment(int? ID, IEnumerable<HttpPostedFileBase> attachment)
        {
            if (ID != null)
            {
                foreach (var doc in attachment)
                {
                    if (doc != null)
                    {
                        var updateValue = new Dictionary<string, object>();
                        updateValue.Add(ListName_Tax_Reimbursement, new FieldLookupValue { LookupId = Convert.ToInt32(ID) });
                        try
                        {
                            SPConnector.UploadDocument(ListName_Document, updateValue, doc.FileName, doc.InputStream, siteUrl);
                        }
                        catch (Exception e)
                        {
                            logger.Error(e.Message);
                            throw e;
                        }
                    }
                }
            }
        }

        private TaxReimbursementVM ConvertToRequisitionNoteVM(ListItem listItem)
        {
            TaxReimbursementVM viewModel = new TaxReimbursementVM();

            viewModel.ID = Convert.ToInt32(listItem[FieldName_Id]);
            viewModel.Type.Value = Convert.ToString(listItem[FieldName_TypeOfTax]);
            viewModel.LetterNo = Convert.ToString(listItem[FieldName_LetterNo]);
            viewModel.LetterDate = Convert.ToDateTime(listItem[FieldName_LetterDate]);
            viewModel.Category.Value = Convert.ToString(listItem[FieldName_Category]);
            viewModel.Contractor = Convert.ToString(listItem[FieldName_Contractor]);
            viewModel.Vendor.Value = Convert.ToInt32(listItem[FieldName_VendorID] == null ? 0 :(listItem[FieldName_VendorID] as FieldLookupValue).LookupId);
            viewModel.Object = Convert.ToString(listItem[FieldName_Object]);
            viewModel.Period = Convert.ToDateTime(listItem[FieldName_TaxPeriod]);
            viewModel.AmountIDR = Convert.ToDecimal(listItem[FieldName_AmountIDR]);
            viewModel.Remarks = FormatUtil.ConvertMultipleLine(Convert.ToString(listItem[FieldName_Remarks]));
            viewModel.PaymentReceivedDate = Convert.ToDateTime(listItem[FieldName_PaymentReceivedDate]);
            viewModel.DocumentNo = Convert.ToString(listItem[FieldName_DocumentNo]);

            return viewModel;
        }

        private string GetDocumentUrl(int? ID)
        {
            return string.Format(TaxReimbursementDocument_URL, siteUrl, ID);
        }
    }
}
