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
        public int CreateHeader(MonthlyFeeHeaderVM header)
        {
            var updatedValues = new Dictionary<string, object>();
            updatedValues.Add("ProfessionalId", header.ProfessionalID);
            updatedValues.Add("professional", header.ProfessionalNameEdit);
            updatedValues.Add("ProjectOrUnit", header.ProjectUnit);
            updatedValues.Add("position", header.Position);
            updatedValues.Add("maritalstatus", header.Status);
            updatedValues.Add("joindate", header.JoinDate);
            updatedValues.Add("dateofnewpsa", header.DateOfNewPsa);
            updatedValues.Add("psaexpirydate", header.EndOfContract);
            updatedValues.Add("DateOfNewFee", header.DateOfNewFee);
            updatedValues.Add("MonthlyFee", header.MonthlyFee);
            updatedValues.Add("MonthlyFeeCurrency", header.Currency.Value);
            try
            {
                SPConnector.AddListItem(SP_HEADER_LIST_NAME, updatedValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }

            return SPConnector.GetInsertedItemID(SP_HEADER_LIST_NAME, _siteUrl);
        }

        public MonthlyFeeHeaderVM GetHeader()
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
            int ID = header.Header.ID;
            //columnValues.Add("ProfessionalId", header.Header.ProfessionalID);
            //columnValues.Add("professional", header.Header.ProfessionalName);
            //columnValues.Add("ProjectOrUnit", header.Header.ProjectUnit);
            //columnValues.Add("position", header.Header.Position);
            //columnValues.Add("maritalstatus", header.Header.Status);
            //columnValues.Add("joindate", header.Header.JoinDate);
            //columnValues.Add("dateofnewpsa", header.Header.DateOfNewPsa);
            //columnValues.Add("psaexpirydate", header.Header.EndOfContract);
            columnValues.Add("DateOfNewFee", header.Header.DateOfNewFee);
            columnValues.Add("MonthlyFee", header.Header.MonthlyFee);
            //columnValues.Add("AnnualFee", header.Header.AnnualFee);
            columnValues.Add("MonthlyFeeCurrency", header.Header.Currency.Value);

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

            viewModel.Header.ProfessionalID = Convert.ToInt16(listItem["ProfessionalId"]);
            viewModel.Header.ProfessionalNameEdit = Convert.ToString(listItem["professional"]);
            viewModel.Header.ProjectUnit = Convert.ToString(listItem["ProjectOrUnit"]);
            viewModel.Header.Position = Convert.ToString(listItem["position"]);
            viewModel.Header.Status = Convert.ToString(listItem["maritalstatus"]);
            viewModel.Header.JoinDate = Convert.ToString(listItem["joindate"]);
            viewModel.Header.DateOfNewPsa = Convert.ToString(listItem["dateofnewpsa"]);
            viewModel.Header.EndOfContract = Convert.ToString(listItem["psaexpirydate"]);
            viewModel.Header.DateOfNewFee = Convert.ToDateTime(listItem["DateOfNewFee"]).ToLocalTime();
            viewModel.Header.MonthlyFee = Convert.ToInt32(listItem["MonthlyFee"]);
            viewModel.Header.AnnualFee = Convert.ToInt32(listItem["AnnualFee"]);
            viewModel.Header.Currency.Value = Convert.ToString(listItem["MonthlyFeeCurrency"]);
            viewModel.Header.ID = ID;

            return viewModel;
        }
    }
}
