using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using NLog;

namespace MCAWebAndAPI.Service.HR.Payroll
{
    public class HRPayrollServices : IHRPayrollServices
    {
        string _siteUrl;
        static Logger logger = LogManager.GetCurrentClassLogger();
        const string SP_HEADER_LIST_NAME = "Monthly Fee";
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
            columnValues.Add("DateOfNewFee", header.DateOfNewFee);
            columnValues.Add("MonthlyFee", header.MonthlyFee);
            columnValues.Add("AnnualFee", header.AnnualFee);
            columnValues.Add("MonthlyFeeCurrency", header.Currency.Value);
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
            columnValues.Add("DateOfNewFee", header.DateOfNewFee);
            columnValues.Add("MonthlyFee", header.MonthlyFee);
            columnValues.Add("AnnualFee", header.AnnualFee);
            columnValues.Add("MonthlyFeeCurrency", header.Currency.Value);

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

        public MonthlyFeeVM GetHeader(int ID)
        {
            var listItem = SPConnector.GetListItem(SP_HEADER_LIST_NAME, ID, _siteUrl);
            var viewModel = new MonthlyFeeVM();
            viewModel.ProfessionalID = listItem["professional_x003a_ID"] == null ? 0 :
               Convert.ToInt16((listItem["professional_x003a_ID"] as FieldLookupValue).LookupValue);
            viewModel.ProjectUnit = Convert.ToString(listItem["ProjectOrUnit"]);
            viewModel.Position = Convert.ToString(listItem["position"]);
            viewModel.Status = Convert.ToString(listItem["maritalstatus"]);
            viewModel.JoinDate = Convert.ToDateTime(listItem["joindate"]).ToLocalTime().ToShortDateString();
            viewModel.DateOfNewPsa = Convert.ToDateTime(listItem["dateofnewpsa"]).ToLocalTime().ToShortDateString();
            viewModel.EndOfContract = Convert.ToDateTime(listItem["psaexpirydate"]).ToLocalTime().ToShortDateString();
            viewModel.DateOfNewFee = Convert.ToDateTime(listItem["DateOfNewFee"]).ToLocalTime();
            viewModel.MonthlyFee = Convert.ToInt32(listItem["MonthlyFee"]);
            viewModel.AnnualFee = Convert.ToInt32(listItem["AnnualFee"]);
            viewModel.Currency.Value = Convert.ToString(listItem["MonthlyFeeCurrency"]);
            viewModel.ID = ID;

            return viewModel;
        }
    }
}
