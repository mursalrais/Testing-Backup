using MCAWebAndAPI.Model.ViewModel.Form.Finance;
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
        private const string SP_TAX_EXEMPTION_LIST_NAME = "Tax Exemption Income";
        private const string SP_TAX_EXEMPTION_DOCUMENTS_NAME = "Tax Exemption Income Documents";
        private const string SP_TYPE_OF_TAX_COLUMN_NAME = "TaxType";
        private const string SP_TYPE_OF_WITHHOLDING_TAX_COLUMN_NAME = "Type of Witholding Tax";
        private const string SP_TYPE_OF_WITHHOLDING_TAX_INTERNAL_COLUMN_NAME = "Type_x0020_of_x0020_Witholding_x";
        private const string SP_PERIOD_COLUMN_NAME = "Period";
        private const string SP_TOTAL_INCOME_RECIPIENTS_COLUMN_NAME = "Total Income Recipients";
        private const string SP_TOTAL_INCOME_RECIPIENTS_INTERNAL_COLUMN_NAME = "Total_x0020_Income_x0020_Recipie";
        private const string SP_GROSS_INCOME_COLUMN_NAME = "Gross Income";
        private readonly string SP_GROSS_INCOME_INTERNAL_COLUMN_NAME = XmlConvert.EncodeName(SP_GROSS_INCOME_COLUMN_NAME);
        private const string SP_TOTAL_INCOME_TAX_BORNE_COLUMN_NAME = "Total Income Tax Borne By Gov IDR";
        private const string SP_TOTAL_INCOME_TAX_BORNE_INTERNAL_COLUMN_NAME = "Total_x0020_Income_x0020_Tax_x00";
        private const string SP_REMARKS_COLUMN_NAME = "Remarks";


        private static Logger logger = LogManager.GetCurrentClassLogger();
        private string _siteUrl = string.Empty;

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
        }

        public TaxExemptionDataVM GetTaxExemptionData()
        {
            var viewModel = new TaxExemptionDataVM();
            viewModel.TaxPeriod = DateTime.Now;
            viewModel.TypeOfTax.Choices = SPConnector.GetChoiceFieldValues(SP_TAX_EXEMPTION_LIST_NAME, SP_TYPE_OF_TAX_COLUMN_NAME, _siteUrl);
            viewModel.TypeOfWithHoldingTax.Choices = SPConnector.GetChoiceFieldValues(SP_TAX_EXEMPTION_LIST_NAME, SP_TYPE_OF_WITHHOLDING_TAX_COLUMN_NAME, _siteUrl);
            return viewModel;
        }

        public TaxExemptionDataVM GetTaxExemptionData(int ID)
        {
            var listItem = SPConnector.GetListItem(SP_TAX_EXEMPTION_LIST_NAME, ID, _siteUrl);
            var viewModel = new TaxExemptionDataVM();

            viewModel.TypeOfTax.Choices = SPConnector.GetChoiceFieldValues(SP_TAX_EXEMPTION_LIST_NAME, SP_TYPE_OF_TAX_COLUMN_NAME, _siteUrl);
            viewModel.TypeOfWithHoldingTax.Choices = SPConnector.GetChoiceFieldValues(SP_TAX_EXEMPTION_LIST_NAME, SP_TYPE_OF_WITHHOLDING_TAX_COLUMN_NAME, _siteUrl);

            viewModel.TypeOfTax.Value = Convert.ToString(listItem[SP_TYPE_OF_TAX_COLUMN_NAME]);
            viewModel.TypeOfWithHoldingTax.Value = Convert.ToString(listItem[SP_TYPE_OF_WITHHOLDING_TAX_INTERNAL_COLUMN_NAME]);
            viewModel.TaxPeriod = Convert.ToDateTime(listItem[SP_PERIOD_COLUMN_NAME]);
            viewModel.TotalIncomeRecepients = Convert.ToInt32(listItem[SP_TOTAL_INCOME_RECIPIENTS_INTERNAL_COLUMN_NAME]);
            viewModel.GrossIncome = Convert.ToDecimal(listItem[SP_GROSS_INCOME_INTERNAL_COLUMN_NAME]);
            viewModel.TotalIncomeTaxBorneByGovernment = Convert.ToDecimal(listItem[SP_TOTAL_INCOME_TAX_BORNE_INTERNAL_COLUMN_NAME]);
            viewModel.Remarks = FormatUtil.ConvertMultipleLine(Convert.ToString(listItem[SP_REMARKS_COLUMN_NAME]));

            viewModel.ID = ID;

            return viewModel;
        }

        public int? CreateTaxExemptionData(TaxExemptionDataVM taxExemptionData)
        {
            int? result = null;
            var columnValues = new Dictionary<string, object>();
            //columnValues.Add("ID", taxExemptionData.ID);
            columnValues.Add(SP_TYPE_OF_TAX_COLUMN_NAME, taxExemptionData.TypeOfTax.Value);
            columnValues.Add(SP_TYPE_OF_WITHHOLDING_TAX_INTERNAL_COLUMN_NAME, taxExemptionData.TypeOfWithHoldingTax.Value);
            columnValues.Add(SP_PERIOD_COLUMN_NAME, taxExemptionData.TaxPeriod);
            columnValues.Add(SP_TOTAL_INCOME_RECIPIENTS_INTERNAL_COLUMN_NAME, taxExemptionData.TotalIncomeRecepients);
            columnValues.Add(SP_GROSS_INCOME_INTERNAL_COLUMN_NAME, taxExemptionData.GrossIncome);
            columnValues.Add(SP_TOTAL_INCOME_TAX_BORNE_INTERNAL_COLUMN_NAME, taxExemptionData.TotalIncomeTaxBorneByGovernment);
            columnValues.Add(SP_REMARKS_COLUMN_NAME, taxExemptionData.Remarks);

            try
            {
                SPConnector.AddListItem(SP_TAX_EXEMPTION_LIST_NAME, columnValues, _siteUrl);
                result = SPConnector.GetLatestListItemID(SP_TAX_EXEMPTION_LIST_NAME, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }
            var entity = new TaxExemptionDataVM();
            entity = taxExemptionData;
            return result;
        }

        public async Task CreateTaxExemptionDataAsync(int? ID, IEnumerable<HttpPostedFileBase> documents)
        {
            CreateTaxExemptionData(ID, documents);
        }

        private void CreateTaxExemptionData(int? ID, IEnumerable<HttpPostedFileBase> attachment)
        {
            if (ID != null)
            {
                foreach (var doc in attachment)
                {
                    var updateValue = new Dictionary<string, object>();
                    updateValue.Add(XmlConvert.EncodeName(SP_TAX_EXEMPTION_LIST_NAME), new FieldLookupValue { LookupId = Convert.ToInt32(ID) });
                    try
                    {
                        SPConnector.UploadDocument(SP_TAX_EXEMPTION_DOCUMENTS_NAME, updateValue, doc.FileName, doc.InputStream, _siteUrl);
                    }
                    catch (Exception e)
                    {
                        logger.Error(e.Message);
                        throw e;
                    }
                }
            }
        }

        public bool UpdateTaxExemptionData(TaxExemptionDataVM taxExemptionData)
        {
            var columnValues = new Dictionary<string, object>();

            columnValues.Add(SP_TYPE_OF_TAX_COLUMN_NAME, taxExemptionData.TypeOfTax.Value);
            columnValues.Add(SP_TYPE_OF_WITHHOLDING_TAX_INTERNAL_COLUMN_NAME, taxExemptionData.TypeOfWithHoldingTax.Value);
            columnValues.Add(SP_PERIOD_COLUMN_NAME, taxExemptionData.TaxPeriod);
            columnValues.Add(SP_TOTAL_INCOME_RECIPIENTS_INTERNAL_COLUMN_NAME, taxExemptionData.TotalIncomeRecepients);
            columnValues.Add(SP_GROSS_INCOME_INTERNAL_COLUMN_NAME, taxExemptionData.GrossIncome);
            columnValues.Add(SP_TOTAL_INCOME_TAX_BORNE_INTERNAL_COLUMN_NAME, taxExemptionData.TotalIncomeTaxBorneByGovernment);
            columnValues.Add(SP_REMARKS_COLUMN_NAME, taxExemptionData.Remarks);

            try
            {
                SPConnector.UpdateListItem(SP_TAX_EXEMPTION_LIST_NAME, taxExemptionData.ID, columnValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                return false;
            }
            var entity = new TaxExemptionDataVM();
            entity = taxExemptionData;
            return true;
        }
    }
}
