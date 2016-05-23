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
            //updatedValues.Add("MonthlyFee", header.ID);
            //updatedValues.Add("MonthlyFee", header.ProfessionalName);
            //updatedValues.Add("MonthlyFee", header.ProjectUnit);
            //updatedValues.Add("MonthlyFee", header.Position);
            //updatedValues.Add("MonthlyFee", header.Status);
            //updatedValues.Add("MonthlyFee", header.JoinDate);
            //updatedValues.Add("MonthlyFee", header.DateOfNewPsa);
            updatedValues.Add("MonthlyFee", header.EndOfContract);
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
            columnValues.Add("DateOfNewFee", header.Header.DateOfNewFee);
            columnValues.Add("MonthlyFee", header.Header.MonthlyFee);
            columnValues.Add("AnnualFee", header.Header.AnnualFee);
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

            viewModel.Header.DateOfNewFee = Convert.ToDateTime(listItem["DateOfNewFee"]);
            viewModel.Header.MonthlyFee = Convert.ToInt32(listItem["MonthlyFee"]);
            viewModel.Header.AnnualFee = Convert.ToInt32(listItem["AnnualFee"]);
            viewModel.Header.Currency.Value = Convert.ToString(listItem["MonthlyFeeCurrency"]);
            viewModel.Header.ProfessionalName.Value = Convert.ToInt32(listItem["ProfessionalId"]);
            viewModel.Header.ID = ID;

            return viewModel;
        }
    }
}
