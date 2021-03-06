﻿using System;
using System.Collections.Generic;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using NLog;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Service.HR.Common;
using System.Threading.Tasks;
using System.IO;

namespace MCAWebAndAPI.Service.HR.Payroll
{
    public class PayrollService : IPayrollService
    {
        string _siteUrl;
        static Logger logger = LogManager.GetCurrentClassLogger();
        const string SP_MON_FEE_LIST_NAME = "Monthly Fee";
        const string SP_MON_FEE_DETAIL_LIST_NAME = "Monthly Fee Detail";
        const string SP_PSA_LIST_NAME = "PSA";

        IDataMasterService _dataMasterService = new DataMasterService();

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
                SPConnector.AddListItem(SP_MON_FEE_LIST_NAME, columnValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }

            return SPConnector.GetLatestListItemID(SP_MON_FEE_LIST_NAME, _siteUrl);
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
            columnValues.Add("professional", new FieldLookupValue { LookupId = Convert.ToInt32(header.ProfessionalNameEdit.Value) });
            columnValues.Add("ProjectOrUnit", header.ProjectUnit);
            columnValues.Add("position", header.Position);
            columnValues.Add("maritalstatus", header.Status);
            columnValues.Add("joindate", header.JoinDate);
            columnValues.Add("dateofnewpsa", header.DateOfNewPsa);
            columnValues.Add("psaexpirydate", header.EndOfContract);

            try
            {
                SPConnector.UpdateListItem(SP_MON_FEE_LIST_NAME, ID, columnValues, _siteUrl);
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
            var listItem = SPConnector.GetListItem(SP_MON_FEE_LIST_NAME, ID, _siteUrl);
            return ConvertToMonthlyFeeModel(listItem);
        }

        private MonthlyFeeVM ConvertToMonthlyFeeModel(ListItem listItem)
        {
            var viewModel = new MonthlyFeeVM();

            viewModel.ID = Convert.ToInt32(listItem["ID"]);
            viewModel.ProfessionalNameEdit.Value = FormatUtil.ConvertLookupToID(listItem, "professional");
            viewModel.ProfessionalNameString = FormatUtil.ConvertLookupToValue(listItem, "professional");
            viewModel.ProfessionalID = FormatUtil.ConvertLookupToID(listItem, "professional_x003a_ID");
            viewModel.ProjectUnit = Convert.ToString(listItem["ProjectOrUnit"]);
            viewModel.Position = Convert.ToString(listItem["position"]);
            viewModel.Status = (listItem["professional_x003a_Tax_x0020_Sta"] as FieldLookupValue).LookupValue;
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
                        SPConnector.DeleteListItem(SP_MON_FEE_DETAIL_LIST_NAME, viewModel.ID, _siteUrl);
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
                updatedValue.Add("enddate", viewModel.EndDateFee);
                updatedValue.Add("monthlyfee", viewModel.MonthlyFee);
                updatedValue.Add("annualfee", viewModel.AnnualFee);
                updatedValue.Add("currency", viewModel.Currency.Text);
                try
                {
                    if (Item.CheckIfUpdated(viewModel))
                        SPConnector.UpdateListItem(SP_MON_FEE_DETAIL_LIST_NAME, viewModel.ID, updatedValue, _siteUrl);
                    else
                        SPConnector.AddListItem(SP_MON_FEE_DETAIL_LIST_NAME, updatedValue, _siteUrl);
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
            var caml = @"<View><Query><Where><Eq><FieldRef Name='monthlyfeeid' /><Value Type='Lookup'>" + ID.ToString() + "</Value></Eq></Where></Query></View>";

            var MonthlyFeeDetails = new List<MonthlyFeeDetailVM>();
            foreach (var item in SPConnector.GetList(SP_MON_FEE_DETAIL_LIST_NAME, _siteUrl, caml))
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
                DateOfNewFee = Convert.ToDateTime(item["dateofnewfee"]),
                EndDateFee = Convert.ToDateTime(item["enddate"]),
                MonthlyFee = Convert.ToInt32(item["monthlyfee"]),
                AnnualFee = Convert.ToInt32(item["annualfee"]),
                Currency = MonthlyFeeDetailVM.GetCurrencyDefaultValue(
                    new Model.ViewModel.Control.InGridComboBoxVM
                    {
                        Text = Convert.ToString(item["currency"])
                    }),
            };
        }

        public IEnumerable<PayrollDetailVM> GetPayrollDetails(DateTime period)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// To produce table of payroll run result
        /// </summary>
        /// <param name="periodParam"></param>
        /// <param name="isSummary">Identify whether to display date per row or professional per row </param>
        /// <returns></returns>
        public async Task<IEnumerable<PayrollWorksheetDetailVM>> GetPayrollWorksheetDetailsAsync(DateTime? periodParam, bool isSummary = false)
        {
            var worksheet = new List<PayrollWorksheetDetailVM>();

            if (periodParam == null)
                return worksheet;

            var period = (DateTime)periodParam;
            var startDate = period.GetFirstPayrollDay().ToLocalTime();
            var finishDate = period.GetLastPayrollDay().ToLocalTime();
            var dateRange = startDate.EachDay(finishDate);

            // Set Site URL
            worksheet.SetSiteUrl(_siteUrl);

            // Retrive required data to cut network round trip time
            var populateProfessionalTask = worksheet.PopulateAllProfessionals();
            var populateValidPSATask = worksheet.PopulateAllValidPSAs(startDate);
            var populateHolidaysAndPublicHolidays = worksheet.PopulateAllHolidaysAndPublicHolidays(dateRange);

            // Get professionals whose PSA are still valid
            var professionalIDs = worksheet.GetValidProfessionalIDs(startDate);

            // Retrive required data to cut network round trip time
            var populateProfessionalMonthlyFeeTask = worksheet.PopulateAllProfessionalMonthlyFee(professionalIDs);

            // Retrive required data to cut network round trip time
            var populateAdjustmentTask = worksheet.PopulateALLAdjustment(professionalIDs);

            // Retrive required data to cut network round trip time
            var populateProfessionalDayOffRequestTask = worksheet.PopulateAllProfessionalDayOffRequests(professionalIDs);

            // Retrive required data to cut network round trip time
            var populateProfessionalDayOffBalanceTask = worksheet.populateProfessionalDayOffBalances(professionalIDs);

            // Populate rows
            worksheet.PopulateRows(dateRange, professionalIDs);

            // Make sure all tasks to retrive requied data have been completed
            try
            {
                await Task.WhenAll(populateProfessionalTask, populateValidPSATask, 
                    populateProfessionalMonthlyFeeTask, populateProfessionalDayOffRequestTask, 
                    populateHolidaysAndPublicHolidays, populateAdjustmentTask, populateProfessionalDayOffBalanceTask);
            }
            catch (Exception e)
            {
                logger.Error(e);
            }

            // Populate columns
            var populateColumnTask = worksheet.PopulateColumns();

            // If worksheet mode
            if (!isSummary)
                return await populateColumnTask;

            // If summary mode / HR-15 mode is required, then the data should be agggregated
            await populateColumnTask;
            worksheet.SummarizeData();
            return worksheet;
        }

        /// <summary>
        /// Run payroll in Background for given period. The result is stored in the given filePath
        /// </summary>
        /// <param name="period"></param>
        /// <param name="filePath"></param>
        public void SavePayrollWorksheetDetailInBackground(DateTime period, string filePath)
        {
            var viewModel = GetPayrollWorksheetDetailsAsync(period, false);
            var fileContents = FileUtil.ConvertObjectToByteArray(viewModel);

            try
            {
                if (fileContents.Length > 0)
                {
                    var path = filePath;
                    System.IO.File.WriteAllBytes(path, fileContents);
                }
                else
                {
                    logger.Error(new FileNotFoundException());
                }
            }
            catch (Exception e)
            {
                logger.Error(e);
            }

            logger.Info(string.Format("Payroll Worksheet draft has been stored at {0}", filePath));
        }
    }
}
