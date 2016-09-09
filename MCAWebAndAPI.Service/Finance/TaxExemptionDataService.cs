using MCAWebAndAPI.Model.ViewModel.Control;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace MCAWebAndAPI.Service.Finance
{
    public class TaxExemptionDataService : ITaxExemptionDataService
    {
        private const string SP_TAX_EXEMPTION_INCOME_LIST_NAME = "Tax Exemption Income";
        private const string SP_TAX_EXEMPTION_INCOME_DOCUMENTS_NAME = "Tax Exemption Income Documents";

        private const string SP_TAX_EXEMPTION_VAT_LIST_NAME = "Tax Exemption VAT";
        private const string SP_TAX_EXEMPTION_VAT_DOCUMENTS_NAME = "Tax Exemption VAT Documents";

        private const string SP_TAX_EXEMPTION_OTHERS_LIST_NAME = "Tax Exemption Others";
        private const string SP_TAX_EXEMPTION_OTHERS_DOCUMENTS_NAME = "Tax Exemption Others Documents";

        private const string SP_TYPE_OF_TAX_COLUMN_NAME = "TaxType";
        private const string SP_PERIOD_COLUMN_NAME = "Period";
        private const string SP_REMARKS_COLUMN_NAME = "Remarks";

        private const string SP_INCOME_TYPE_OF_WITHHOLDING_TAX_COLUMN_NAME = "Type of Witholding Tax";
        private const string SP_INCOME_TYPE_OF_WITHHOLDING_TAX_INTERNAL_COLUMN_NAME = "Type_x0020_of_x0020_Witholding_x";
        private const string SP_INCOME_TOTAL_INCOME_RECIPIENTS_COLUMN_NAME = "Total Income Recipients";
        private const string SP_INCOME_TOTAL_INCOME_RECIPIENTS_INTERNAL_COLUMN_NAME = "Total_x0020_Income_x0020_Recipie";
        private const string SP_INCOME_GROSS_INCOME_COLUMN_NAME = "Gross Income";
        private readonly string SP_INCOME_GROSS_INCOME_INTERNAL_COLUMN_NAME = XmlConvert.EncodeName(SP_INCOME_GROSS_INCOME_COLUMN_NAME);
        private const string SP_INCOME_TOTAL_INCOME_TAX_BORNE_COLUMN_NAME = "Total Income Tax Borne By Gov IDR";
        private const string SP_INCOME_TOTAL_INCOME_TAX_BORNE_INTERNAL_COLUMN_NAME = "Total_x0020_Income_x0020_Tax_x00";

        private const string SP_VAT_TOTAL_TAX_BASED_IDR = "Total_x0020_Tax_x0020_Based_x002";
        private const string SP_VAT_TOTAL_VAT_NOT_COLLECTED = "Total_x0020_VAT_x0020_Not_x0020_";

        private const string SP_OTHER_GROSS_INCOME_IDR = "Gross_x0020_Income_x0020_IDR";
        private const string SP_OTHER_TOTAL_TAX_IDR = "Total_x0020_Tax_x0020_IDR";

        private const string TaxExemptionVATDocumentByID = "{0}/Tax%20Exemption%20VAT%20Documents/Forms/AllItems.aspx#InplviewHash5093bda1-84bf-4cad-8652-286653d6a83f=FilterField1%3Dpsa%255Fx003a%255FID-FilterValue1%3D{1}";
        private const string TaxExemptionOtherDocumentByID = "{0}/Tax%20Exemption%20Others%20Documents/Forms/AllItems.aspx#InplviewHash5093bda1-84bf-4cad-8652-286653d6a83f=FilterField1%3Dpsa%255Fx003a%255FID-FilterValue1%3D{1}";

        private static Logger logger = LogManager.GetCurrentClassLogger();
        private string _siteUrl = string.Empty;

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
        }

        public TaxExemptionIncomeVM GetTaxExemptionIncome()
        {
            var viewModel = new TaxExemptionIncomeVM();
            viewModel.TaxPeriod = DateTime.Now;
            viewModel.TypeOfTax.Choices = SPConnector.GetChoiceFieldValues(SP_TAX_EXEMPTION_INCOME_LIST_NAME, SP_TYPE_OF_TAX_COLUMN_NAME, _siteUrl);
            viewModel.TypeOfWithHoldingTax.Choices = SPConnector.GetChoiceFieldValues(SP_TAX_EXEMPTION_INCOME_LIST_NAME, SP_INCOME_TYPE_OF_WITHHOLDING_TAX_COLUMN_NAME, _siteUrl);
            return viewModel;
        }

        public TaxExemptionVATVM GetTaxExemptionVAT()
        {
            var viewModel = new TaxExemptionVATVM();
            viewModel.TaxPeriod = DateTime.Now;
            viewModel.TypeOfTax.Choices = SPConnector.GetChoiceFieldValues(SP_TAX_EXEMPTION_VAT_LIST_NAME, SP_TYPE_OF_TAX_COLUMN_NAME, _siteUrl);
            return viewModel;
        }

        public TaxExemptionOtherVM GetTaxExemptionOthers()
        {
            var viewModel = new TaxExemptionOtherVM();
            viewModel.TaxPeriod = DateTime.Now;
            viewModel.TypeOfTax.Choices = SPConnector.GetChoiceFieldValues(SP_TAX_EXEMPTION_OTHERS_LIST_NAME, SP_TYPE_OF_TAX_COLUMN_NAME, _siteUrl);
            return viewModel;
        }

        public TaxExemptionIncomeVM GetTaxExemptionIncome(int ID)
        {
            var listItem = SPConnector.GetListItem(SP_TAX_EXEMPTION_INCOME_LIST_NAME, ID, _siteUrl);
            var viewModel = new TaxExemptionIncomeVM();

            viewModel.TypeOfTax.Choices = SPConnector.GetChoiceFieldValues(SP_TAX_EXEMPTION_INCOME_LIST_NAME, SP_TYPE_OF_TAX_COLUMN_NAME, _siteUrl);
            viewModel.TypeOfWithHoldingTax.Choices = SPConnector.GetChoiceFieldValues(SP_TAX_EXEMPTION_INCOME_LIST_NAME, SP_INCOME_TYPE_OF_WITHHOLDING_TAX_COLUMN_NAME, _siteUrl);

            viewModel.TypeOfTax.Value = Convert.ToString(listItem[SP_TYPE_OF_TAX_COLUMN_NAME]);
            viewModel.TypeOfWithHoldingTax.Value = Convert.ToString(listItem[SP_INCOME_TYPE_OF_WITHHOLDING_TAX_INTERNAL_COLUMN_NAME]);
            viewModel.TaxPeriod = Convert.ToDateTime(listItem[SP_PERIOD_COLUMN_NAME]);
            viewModel.TotalIncomeRecepients = Convert.ToInt32(listItem[SP_INCOME_TOTAL_INCOME_RECIPIENTS_INTERNAL_COLUMN_NAME]);
            viewModel.GrossIncome = Convert.ToDecimal(listItem[SP_INCOME_GROSS_INCOME_INTERNAL_COLUMN_NAME]);
            viewModel.TotalIncomeTaxBorneByGovernment = Convert.ToDecimal(listItem[SP_INCOME_TOTAL_INCOME_TAX_BORNE_INTERNAL_COLUMN_NAME]);
            viewModel.Remarks = FormatUtil.ConvertMultipleLine(Convert.ToString(listItem[SP_REMARKS_COLUMN_NAME]));
            viewModel.DocumentUrl = GetIncomeDocumentUrl(viewModel.ID);
            viewModel.ID = ID;

            return viewModel;
        }

        private string GetIncomeDocumentUrl(int? iD)
        {
            return string.Format(UrlResource.TaxExemptionIncomeDocumentByID, _siteUrl, iD);
        }

        private string GetVATDocumentUrl(int? iD)
        {
            return string.Format(TaxExemptionVATDocumentByID, _siteUrl, iD);
        }

        private string GetOtherDocumentUrl(int? iD)
        {
            return string.Format(TaxExemptionOtherDocumentByID, _siteUrl, iD);
        }

        public int? CreateTaxExemptionData(TaxExemptionIncomeVM taxExemptionData)
        {
            int? result = null;
            var columnValues = new Dictionary<string, object>();
            columnValues.Add(SP_TYPE_OF_TAX_COLUMN_NAME, taxExemptionData.TypeOfTax.Value);
            columnValues.Add(SP_INCOME_TYPE_OF_WITHHOLDING_TAX_INTERNAL_COLUMN_NAME, taxExemptionData.TypeOfWithHoldingTax.Value);
            columnValues.Add(SP_PERIOD_COLUMN_NAME, taxExemptionData.TaxPeriod);
            columnValues.Add(SP_INCOME_TOTAL_INCOME_RECIPIENTS_INTERNAL_COLUMN_NAME, taxExemptionData.TotalIncomeRecepients);
            columnValues.Add(SP_INCOME_GROSS_INCOME_INTERNAL_COLUMN_NAME, taxExemptionData.GrossIncome);
            columnValues.Add(SP_INCOME_TOTAL_INCOME_TAX_BORNE_INTERNAL_COLUMN_NAME, taxExemptionData.TotalIncomeTaxBorneByGovernment);
            columnValues.Add(SP_REMARKS_COLUMN_NAME, taxExemptionData.Remarks);

            try
            {
                SPConnector.AddListItem(SP_TAX_EXEMPTION_INCOME_LIST_NAME, columnValues, _siteUrl);
                result = SPConnector.GetLatestListItemID(SP_TAX_EXEMPTION_INCOME_LIST_NAME, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }
            return result;
        }

        public int? CreateTaxExemptionData(TaxExemptionVATVM taxExemptionData)
        {
            int? result = null;
            var columnValues = new Dictionary<string, object>();
            columnValues.Add(SP_TYPE_OF_TAX_COLUMN_NAME, taxExemptionData.TypeOfTax.Value);
            columnValues.Add(SP_PERIOD_COLUMN_NAME, taxExemptionData.TaxPeriod);
            columnValues.Add(SP_REMARKS_COLUMN_NAME, taxExemptionData.Remarks);
            columnValues.Add(SP_VAT_TOTAL_TAX_BASED_IDR, taxExemptionData.TotalTaxBased);
            columnValues.Add(SP_VAT_TOTAL_VAT_NOT_COLLECTED, taxExemptionData.TotalVATNotCollected);

            try
            {
                SPConnector.AddListItem(SP_TAX_EXEMPTION_VAT_LIST_NAME, columnValues, _siteUrl);
                result = SPConnector.GetLatestListItemID(SP_TAX_EXEMPTION_VAT_LIST_NAME, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }
            return result;
        }

        public int? CreateTaxExemptionData(TaxExemptionOtherVM taxExemptionData)
        {
            int? result = null;
            var columnValues = new Dictionary<string, object>();
            columnValues.Add(SP_TYPE_OF_TAX_COLUMN_NAME, taxExemptionData.TypeOfTax.Value);
            columnValues.Add(SP_PERIOD_COLUMN_NAME, taxExemptionData.TaxPeriod);
            columnValues.Add(SP_REMARKS_COLUMN_NAME, taxExemptionData.Remarks);
            columnValues.Add(SP_OTHER_GROSS_INCOME_IDR, taxExemptionData.GrossIncome);
            columnValues.Add(SP_OTHER_TOTAL_TAX_IDR, taxExemptionData.TotalTax);

            try
            {
                SPConnector.AddListItem(SP_TAX_EXEMPTION_OTHERS_LIST_NAME, columnValues, _siteUrl);
                result = SPConnector.GetLatestListItemID(SP_TAX_EXEMPTION_OTHERS_LIST_NAME, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }
            return result;
        }

        public async Task CreateTaxExemptionDataAsync(int? ID, string taxType, IEnumerable<HttpPostedFileBase> documents)
        {
            CreateTaxExemptionData(ID, taxType, documents);
        }

        private void CreateTaxExemptionData(int? ID, string taxType, IEnumerable<HttpPostedFileBase> attachment)
        {
            if (ID != null)
            {
                string listName = string.Empty;
                string documentsListName = string.Empty;
                switch (taxType)
                {
                    case TaxTypeComboBoxVM.INCOME:
                        listName = SP_TAX_EXEMPTION_INCOME_LIST_NAME;
                        documentsListName = SP_TAX_EXEMPTION_INCOME_DOCUMENTS_NAME;
                        break;
                    case TaxTypeComboBoxVM.VAT:
                        listName = SP_TAX_EXEMPTION_VAT_LIST_NAME;
                        documentsListName = SP_TAX_EXEMPTION_VAT_DOCUMENTS_NAME;
                        break;
                    case TaxTypeComboBoxVM.OTHERS:
                        listName = SP_TAX_EXEMPTION_OTHERS_LIST_NAME;
                        documentsListName = SP_TAX_EXEMPTION_OTHERS_DOCUMENTS_NAME;
                        break;
                    default:
                        throw new NotImplementedException("Not implemented tax type: " + taxType);
                }

                foreach (var doc in attachment)
                {
                    var updateValue = new Dictionary<string, object>();
                    updateValue.Add(XmlConvert.EncodeName(listName), new FieldLookupValue { LookupId = Convert.ToInt32(ID) });
                    try
                    {
                        SPConnector.UploadDocument(documentsListName, updateValue, doc.FileName, doc.InputStream, _siteUrl);
                    }
                    catch (Exception e)
                    {
                        logger.Error(e.Message);
                        throw e;
                    }
                }
            }
        }

        public bool UpdateTaxExemption(TaxExemptionIncomeVM taxExemptionData)
        {
            var columnValues = new Dictionary<string, object>();

            columnValues.Add(SP_TYPE_OF_TAX_COLUMN_NAME, taxExemptionData.TypeOfTax.Value);
            columnValues.Add(SP_INCOME_TYPE_OF_WITHHOLDING_TAX_INTERNAL_COLUMN_NAME, taxExemptionData.TypeOfWithHoldingTax.Value);
            columnValues.Add(SP_PERIOD_COLUMN_NAME, taxExemptionData.TaxPeriod);
            columnValues.Add(SP_INCOME_TOTAL_INCOME_RECIPIENTS_INTERNAL_COLUMN_NAME, taxExemptionData.TotalIncomeRecepients);
            columnValues.Add(SP_INCOME_GROSS_INCOME_INTERNAL_COLUMN_NAME, taxExemptionData.GrossIncome);
            columnValues.Add(SP_INCOME_TOTAL_INCOME_TAX_BORNE_INTERNAL_COLUMN_NAME, taxExemptionData.TotalIncomeTaxBorneByGovernment);
            columnValues.Add(SP_REMARKS_COLUMN_NAME, taxExemptionData.Remarks);

            try
            {
                SPConnector.UpdateListItem(SP_TAX_EXEMPTION_INCOME_LIST_NAME, taxExemptionData.ID, columnValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                return false;
            }
            return true;
        }

        public TaxExemptionVATVM GetTaxExemptionVAT(int ID)
        {
            var listItem = SPConnector.GetListItem(SP_TAX_EXEMPTION_VAT_LIST_NAME, ID, _siteUrl);
            var viewModel = new TaxExemptionVATVM();

            viewModel.TypeOfTax.Choices = SPConnector.GetChoiceFieldValues(SP_TAX_EXEMPTION_VAT_LIST_NAME, SP_TYPE_OF_TAX_COLUMN_NAME, _siteUrl);
            viewModel.TypeOfTax.Value = Convert.ToString(listItem[SP_TYPE_OF_TAX_COLUMN_NAME]);
            viewModel.TaxPeriod = Convert.ToDateTime(listItem[SP_PERIOD_COLUMN_NAME]);
            viewModel.Remarks = FormatUtil.ConvertMultipleLine(Convert.ToString(listItem[SP_REMARKS_COLUMN_NAME]));

            viewModel.TotalTaxBased = Convert.ToDecimal(listItem[SP_VAT_TOTAL_TAX_BASED_IDR]);
            viewModel.TotalVATNotCollected = Convert.ToDecimal(listItem[SP_VAT_TOTAL_VAT_NOT_COLLECTED]);

            viewModel.DocumentUrl = GetVATDocumentUrl(viewModel.ID);
            viewModel.ID = ID;

            return viewModel;
        }

        public TaxExemptionOtherVM GetTaxExemptionOthers(int ID)
        {
            var listItem = SPConnector.GetListItem(SP_TAX_EXEMPTION_OTHERS_LIST_NAME, ID, _siteUrl);
            var viewModel = new TaxExemptionOtherVM();

            viewModel.TypeOfTax.Choices = SPConnector.GetChoiceFieldValues(SP_TAX_EXEMPTION_VAT_LIST_NAME, SP_TYPE_OF_TAX_COLUMN_NAME, _siteUrl);
            viewModel.TypeOfTax.Value = Convert.ToString(listItem[SP_TYPE_OF_TAX_COLUMN_NAME]);
            viewModel.TaxPeriod = Convert.ToDateTime(listItem[SP_PERIOD_COLUMN_NAME]);
            viewModel.Remarks = FormatUtil.ConvertMultipleLine(Convert.ToString(listItem[SP_REMARKS_COLUMN_NAME]));

            viewModel.GrossIncome = Convert.ToDecimal(listItem[SP_OTHER_GROSS_INCOME_IDR]);
            viewModel.TotalTax = Convert.ToDecimal(listItem[SP_OTHER_TOTAL_TAX_IDR]);

            viewModel.DocumentUrl = GetOtherDocumentUrl(viewModel.ID);
            viewModel.ID = ID;

            return viewModel;
        }

        public bool UpdateTaxExemption(TaxExemptionVATVM taxExemptionData)
        {
            var columnValues = new Dictionary<string, object>();
            columnValues.Add(SP_TYPE_OF_TAX_COLUMN_NAME, taxExemptionData.TypeOfTax.Value);
            columnValues.Add(SP_PERIOD_COLUMN_NAME, taxExemptionData.TaxPeriod);
            columnValues.Add(SP_REMARKS_COLUMN_NAME, taxExemptionData.Remarks);
            columnValues.Add(SP_VAT_TOTAL_TAX_BASED_IDR, taxExemptionData.TotalTaxBased);
            columnValues.Add(SP_VAT_TOTAL_VAT_NOT_COLLECTED, taxExemptionData.TotalVATNotCollected);

            try
            {
                SPConnector.UpdateListItem(SP_TAX_EXEMPTION_VAT_LIST_NAME, taxExemptionData.ID, columnValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                return false;
            }
            return true;
        }

        public bool UpdateTaxExemption(TaxExemptionOtherVM taxExemptionData)
        {
            var columnValues = new Dictionary<string, object>();
            columnValues.Add(SP_TYPE_OF_TAX_COLUMN_NAME, taxExemptionData.TypeOfTax.Value);
            columnValues.Add(SP_PERIOD_COLUMN_NAME, taxExemptionData.TaxPeriod);
            columnValues.Add(SP_REMARKS_COLUMN_NAME, taxExemptionData.Remarks);
            columnValues.Add(SP_OTHER_GROSS_INCOME_IDR, taxExemptionData.GrossIncome);
            columnValues.Add(SP_OTHER_TOTAL_TAX_IDR, taxExemptionData.TotalTax);

            try
            {
                SPConnector.UpdateListItem(SP_TAX_EXEMPTION_OTHERS_LIST_NAME, taxExemptionData.ID, columnValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                return false;
            }
            return true;
        }
    }
}