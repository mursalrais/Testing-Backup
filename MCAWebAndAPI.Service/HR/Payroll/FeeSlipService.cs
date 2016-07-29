﻿using System;
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
    public class FeeSlipService : IFeeSlipService
    {
        string _siteUrl;
        static Logger logger = LogManager.GetCurrentClassLogger();
        const string SP_HEADER_LIST_NAME = "Fee Slip";
        const string SP_DETAIL_LIST_NAME = "Fee Slip Detail";

        public void CreateFeeSlipDetails(int? headerID, IEnumerable<FeeSlipDetailVM> feeSlipDetails)
        {
            foreach (var viewModel in feeSlipDetails)
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
                updatedValue.Add("dateofnewfee", viewModel.Fee);
                updatedValue.Add("dateofnewfee", viewModel.Fee);
                updatedValue.Add("monthlyfee", viewModel.Deduction);
                updatedValue.Add("annualfee", viewModel.TotalIncome);
                updatedValue.Add("currency", viewModel.TotalDeduction);
                try
                {
                    if (Item.CheckIfUpdated(viewModel))
                        SPConnector.UpdateListItem(SP_DETAIL_LIST_NAME, viewModel.ID, updatedValue, _siteUrl);
                    else
                        SPConnector.AddListItem(SP_DETAIL_LIST_NAME, updatedValue, _siteUrl);
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw new Exception(ErrorResource.SPInsertError);
                }
            }
        }

        public int CreateHeader(FeeSlipVM header)
        {
            var columnValues = new Dictionary<string, object>();
            columnValues.Add("professional", new FieldLookupValue { LookupId = Convert.ToInt32(header.ProfessionalID) });
            columnValues.Add("ProjectOrUnit", header.Slip);
            columnValues.Add("position", header.Name);
            columnValues.Add("maritalstatus", header.JoiningDate);
            columnValues.Add("joindate", header.Designation);
            columnValues.Add("dateofnewpsa", header.PaymentMode);
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

        public FeeSlipVM GetHeader(int? ID)
        {
            var listItem = SPConnector.GetListItem(SP_HEADER_LIST_NAME, ID, _siteUrl);
            return ConvertToFeeSlipModel(listItem);
        }

        private FeeSlipVM ConvertToFeeSlipModel(ListItem listItem)
        {
            var viewModel = new FeeSlipVM();

            viewModel.ID = Convert.ToInt32(listItem["ID"]);
            viewModel.ProfessionalID = FormatUtil.ConvertLookupToID(listItem, "professional");
            viewModel.Slip = Convert.ToInt32(listItem["ProjectOrUnit"]);
            viewModel.Name = Convert.ToString(listItem["position"]);
            viewModel.JoiningDate = Convert.ToDateTime(listItem["joindate"]).ToLocalTime().ToShortDateString();
            viewModel.Designation = Convert.ToString(listItem["position"]);
            viewModel.PaymentMode = Convert.ToString(listItem["position"]);

            // Convert Details
            viewModel.FeeSlipDetails = GetFeeSlipDetails(viewModel.ID);

            return viewModel;
        }

        private IEnumerable<FeeSlipDetailVM> GetFeeSlipDetails(int? ID)
        {
            var caml = @"<View><Query><Where><Eq><FieldRef Name='monthlyfeeid' /><Value Type='Lookup'>" + ID.ToString() + "</Value></Eq></Where></Query></View>";

            var FeeSlipDetails = new List<FeeSlipDetailVM>();
            foreach (var item in SPConnector.GetList(SP_DETAIL_LIST_NAME, _siteUrl, caml))
            {
                FeeSlipDetails.Add(ConvertToFeeSlipDetailVM(item));
            }

            return FeeSlipDetails;
        }

        private FeeSlipDetailVM ConvertToFeeSlipDetailVM(ListItem item)
        {
            return new FeeSlipDetailVM
            {
                ID = Convert.ToInt32(item["ID"]),
                Fee = Convert.ToInt32(item["dateofnewfee"]),
                Deduction = Convert.ToInt32(item["monthlyfee"]),
                TotalIncome = Convert.ToInt32(item["annualfee"]),
                TotalDeduction = Convert.ToInt32(item["annualfee"]),
            };
        }

        public FeeSlipVM GetPopulatedModel(int? id = null)
        {
            var model = new FeeSlipVM();
            return model;
        }

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
        }

        public bool UpdateHeader(FeeSlipVM header)
        {
            var columnValues = new Dictionary<string, object>();
            int? ID = header.ID;
            columnValues.Add("professional", new FieldLookupValue { LookupId = Convert.ToInt32(header.ProfessionalID) });
            columnValues.Add("ProjectOrUnit", header.Slip);
            columnValues.Add("position", header.Name);
            columnValues.Add("maritalstatus", header.JoiningDate);
            columnValues.Add("joindate", header.Designation);
            columnValues.Add("dateofnewpsa", header.PaymentMode);

            try
            {
                SPConnector.UpdateListItem(SP_HEADER_LIST_NAME, ID, columnValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                return false;
            }
            var entitiy = new FeeSlipVM();
            entitiy = header;
            return true;
        }
    }
}
