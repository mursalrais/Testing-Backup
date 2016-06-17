using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using NLog;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Service.Resources;

namespace MCAWebAndAPI.Service.HR.Payroll
{
    public class HRPayrollServices : IHRPayrollServices
    {
        string _siteUrl;
        static Logger logger = LogManager.GetCurrentClassLogger();
        const string SP_HEADER_LIST_NAME = "Monthly Fee";
        const string SP_DETAIL_LIST_NAME = "Monthly Fee Detail";
        public int CreateHeader(MonthlyFeeVM header)
        {
            var columnValues = new Dictionary<string, object>();
            columnValues.Add("professional", new FieldLookupValue { LookupId = Convert.ToInt32(header.ProfessionalName.Value) });
            columnValues.Add("ProjectOrUnit", header.ProjectUnit);
            columnValues.Add("position", header.Position);
            columnValues.Add("maritalstatus", header.Status);
            columnValues.Add("joindate", header.JoinDate);
            columnValues.Add("dateofnewpsa", header.DateOfNewPsa);
            columnValues.Add("psaexpirydate", header.EndOfContract);
            try
            {
                SPConnector.AddListItem(SP_HEADER_LIST_NAME, columnValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }

            return SPConnector.GetLatestListItemID(SP_HEADER_LIST_NAME, _siteUrl);
        }

        public MonthlyFeeVM GetHeader()
        {
            throw new NotImplementedException();
        }

        public MonthlyFeeVM GetPopulatedModel(int? id = null)
        {
            var model = new MonthlyFeeVM();
            return model;
        }

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
        }

        public bool UpdateHeader(MonthlyFeeVM header)
        {
            var columnValues = new Dictionary<string, object>();
            int? ID = header.ID;
            columnValues.Add("ProjectOrUnit", header.ProjectUnit);
            columnValues.Add("position", header.Position);
            columnValues.Add("maritalstatus", header.Status);
            columnValues.Add("joindate", header.JoinDate);
            columnValues.Add("dateofnewpsa", header.DateOfNewPsa);
            columnValues.Add("psaexpirydate", header.EndOfContract);

            try
            {
                SPConnector.UpdateListItem(SP_HEADER_LIST_NAME, ID, columnValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                return false;
            }
            var entitiy = new MonthlyFeeVM();
            entitiy = header;
            return true;
    }

        public MonthlyFeeVM GetHeader(int? ID)
        {
            var listItem = SPConnector.GetListItem(SP_HEADER_LIST_NAME, ID, _siteUrl);
            return ConvertToMonthlyFeeModel(listItem);
        }

        private MonthlyFeeVM ConvertToMonthlyFeeModel(ListItem listItem)
        {
            var viewModel = new MonthlyFeeVM();

            viewModel.ID = Convert.ToInt32(listItem["ID"]);
            viewModel.ProfessionalID = listItem["professional_x003a_ID"] == null ? 0 :
             Convert.ToInt16((listItem["professional_x003a_ID"] as FieldLookupValue).LookupValue);
            viewModel.ProjectUnit = Convert.ToString(listItem["ProjectOrUnit"]);
            viewModel.Position = Convert.ToString(listItem["position"]);
            viewModel.Status = Convert.ToString(listItem["maritalstatus"]);
            viewModel.JoinDate = Convert.ToDateTime(listItem["joindate"]).ToLocalTime().ToShortDateString();
            viewModel.DateOfNewPsa = Convert.ToDateTime(listItem["dateofnewpsa"]).ToLocalTime().ToShortDateString();
            viewModel.EndOfContract = Convert.ToDateTime(listItem["psaexpirydate"]).ToLocalTime().ToShortDateString();

            // Convert Details
            viewModel.MonthlyFeeDetails = GetMonthlyFeeDetails(viewModel.ID);

            return viewModel;
        }

        public void CreateMonthlyFeeDetails(int? headerID, IEnumerable<MonthlyFeeDetailVM> monthlyFeeDetails)
        {
            foreach (var viewModel in monthlyFeeDetails)
            {
                if (Item.CheckIfSkipped(viewModel))
                    continue;
                if (Item.CheckIfDeleted(viewModel))
                {
                    try
                    {
                        SPConnector.DeleteListItem(SP_DETAIL_LIST_NAME, viewModel.ID, _siteUrl);

                    }
                    catch (Exception e)
                    {
                        logger.Error(e);
                        throw e;
                    }
                    continue;
                }
                var updatedValue = new Dictionary<string, object>();
                updatedValue.Add("monthlyfeeid", new FieldLookupValue { LookupId = Convert.ToInt32(headerID) });
                updatedValue.Add("dateofnewfee", viewModel.DateOfNewFee);
                updatedValue.Add("monthlyfee", viewModel.MonthlyFee);
                updatedValue.Add("annualfee", viewModel.AnnualFee);
                updatedValue.Add("currency", viewModel.Currency.Text);
                try
                {
                    SPConnector.AddListItem(SP_DETAIL_LIST_NAME, updatedValue, _siteUrl);
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw new Exception(ErrorResource.SPInsertError);
                }
            }
        }
        private IEnumerable<MonthlyFeeDetailVM> GetMonthlyFeeDetails(int? ID)
        {

            var MonthlyFeeDetails = new List<MonthlyFeeDetailVM>();
            foreach (var item in SPConnector.GetList(SP_DETAIL_LIST_NAME, _siteUrl))
            {
                MonthlyFeeDetails.Add(ConvertToMonthlyFeeDetailVM(item));
            }

            return MonthlyFeeDetails;
        }

        private MonthlyFeeDetailVM ConvertToMonthlyFeeDetailVM(ListItem item)
        {
            return new MonthlyFeeDetailVM
            {
                ID = Convert.ToInt32(item["ID"]),
                DateOfNewFee = FormatUtil.ConvertDateStringToDateTime(item, "monthlyfee"),
                MonthlyFee = Convert.ToInt32(item["monthlyfee"]),
                AnnualFee = Convert.ToInt32(item["annualfee"]),
                Currency = MonthlyFeeDetailVM.GetCurrencyDefaultValue(
                    new Model.ViewModel.Control.InGridComboBoxVM
                    {
                        Text = Convert.ToString(item["currency"])
                    }),
            };
        }
    }
}
