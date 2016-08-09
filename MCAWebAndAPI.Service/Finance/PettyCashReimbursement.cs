﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Utils;
using NLog;

namespace MCAWebAndAPI.Service.Finance
{
    public class PettyCashReimbursement : IPettyCashReimbursement
    {
        private const string ListName = "Petty Cash Reimbursement";

        private const string FieldName_Date = "Date";
        private const string FieldName_PaidTo = "Paid To";
        private const string FieldName_Professional = "Professional";
        private const string FieldName_Vendor = "Vendor";
        private const string FieldName_Driver = "Driver";

        private string siteUrl = string.Empty;
        static Logger logger = LogManager.GetCurrentClassLogger();

        public int? Create(PettyCashReimbursementVM viewModel)
        {
            int? result = null;
            var columnValues = new Dictionary<string, object>
           {
               {FieldName_Date, viewModel.Date},
               {FieldName_PaidTo, viewModel.PaidTo},
               {FieldName_Professional, viewModel.Professional},
               {FieldName_Vendor, viewModel.Vendor},
               {FieldName_Driver, viewModel.Driver}
            };

            try
            {
                SPConnector.AddListItem(ListName, columnValues, siteUrl);
                result = SPConnector.GetLatestListItemID(ListName, siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }

            return result;
        }

        public Task CreateAttachmentAsync(int? ID, IEnumerable<HttpPostedFileBase> attachment)
        {
            throw new NotImplementedException();
        }

        public void SetSiteUrl(string siteUrl)
        {
            this.siteUrl = siteUrl;
        }


    }
}
