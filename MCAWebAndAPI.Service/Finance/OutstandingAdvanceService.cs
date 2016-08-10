using System;
using System.Collections.Generic;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using NLog;

namespace MCAWebAndAPI.Service.Finance
{
    public class OutstandingAdvanceService : IOutstandingAdvanceService
    {
        private const string ListName = "Outstanding Advance";
        private const string FieldName_ID = "ID";
        private const string FieldName_DateOfUpload = "Date of Upload";
        private const string FieldName_StaffID = "Staff ID";
        private const string FieldName_Reference = "Reference";
        private const string FieldName_Remarks = "Remarks";
        private const string FieldName_DueDate = "Due Date";
        private const string FieldName_Currency = "Currency";
        private const string FieldName_Amount = "Amount";
        private const string FieldName_Project = "Project";

        string siteUrl = null;
        static Logger logger = LogManager.GetCurrentClassLogger();

        public int Create(OutstandingAdvanceVM viewModel)
        {

            var updatedValue = new Dictionary<string, object>();

            updatedValue.Add(FieldName_DateOfUpload, viewModel.DateOfUpload);
            updatedValue.Add(FieldName_StaffID, viewModel.StaffID.Value);
            updatedValue.Add(FieldName_Reference, viewModel.Reference);
            updatedValue.Add(FieldName_DueDate, viewModel.DueDate);
            updatedValue.Add(FieldName_Currency, viewModel.Currency.Value);
            updatedValue.Add(FieldName_Amount, viewModel.Amount);
            updatedValue.Add(FieldName_Project, viewModel.Project.Value);
            updatedValue.Add(FieldName_Remarks, viewModel.Remarks);

            try
            {
                SPConnector.AddListItem(ListName, updatedValue, siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                throw new Exception(ErrorResource.SPInsertError);
            }

            return SPConnector.GetLatestListItemID(ListName, siteUrl);
        }

        public void SetSiteUrl(string siteUrl)
        {
            this.siteUrl = siteUrl;
        }


        public bool Update(OutstandingAdvanceVM viewModel)
        {
            throw new NotImplementedException();
        }

        public OutstandingAdvanceVM Get(int? ID)
        {
            var viewModel = new OutstandingAdvanceVM();

            if (ID != null)
            {
                var listItem = SPConnector.GetListItem(ListName, ID, siteUrl);
                viewModel = ConvertToVM(listItem);
            }

            return viewModel;

        }


        private OutstandingAdvanceVM ConvertToVM(ListItem listItem)
        {
            OutstandingAdvanceVM viewModel = new OutstandingAdvanceVM();

            viewModel.ID = Convert.ToInt32(listItem[FieldName_ID]);
            viewModel.DateOfUpload = Convert.ToDateTime(listItem[FieldName_DateOfUpload]);
            viewModel.StaffID.Value = Convert.ToInt32(listItem[FieldName_StaffID]);
            viewModel.Reference = Convert.ToString(listItem[FieldName_Reference]);
            viewModel.DueDate = Convert.ToDateTime(listItem[FieldName_DueDate]);
            viewModel.Currency.Value = Convert.ToString(listItem[FieldName_Currency]);
            viewModel.Amount = Convert.ToDecimal(listItem[FieldName_Amount]);
            viewModel.Project.Value = Convert.ToString(listItem[FieldName_Project]);

            return viewModel;
        }


    }
}
