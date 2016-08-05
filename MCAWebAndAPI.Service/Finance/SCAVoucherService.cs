using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using NLog;

namespace MCAWebAndAPI.Service.Finance
{
    public class SCAVoucherService:ISCAVoucherService
    {
        static Logger logger = LogManager.GetCurrentClassLogger();

        private const string LIST_NAME_SCAVOUCHER = "SCA Voucher";
        private const string LIST_NAME_SCAVOUCHER_ITEM = "SCA Voucher Item";
        private const string LIST_NAME_EVENT_BUDGET= "Event Budget";
        private const string FIELD_NAME_SCA_NO = "SCA No";
        private const string FIELD_NAME_DATE = "Date";
        private const string FIELD_NAME_SDOID = "SDO ID";
        private const string FIELD_NAME_EBUDGET_ID = "Event Budget ID";
        private const string FIELD_NAME_CURRENCY = "Currency";
        private const string FIELD_NAME_TOTAL_AMOUNT = "Total Amount";
        private const string FIELD_NAME_TA_WORDS = "Total Amount in Words";
        private const string FIELD_NAME_PURPOSE = "Purpose";
        private const string FIELD_NAME_PROJECT = "Project";
        private const string FIELD_NAME_ACTIVITY_ID = "Activity ID";
        private const string FIELD_NAME_SUB_ACTIVITY_ID = "Sub Activity ID";
        private const string FIELD_NAME_FUND = "Fund";
        private const string FIELD_NAME_REFFERENCE_NO = "Referrence No";
        private const string FIELD_NAME_REMARKS = "Remarks";
        private const string FIELD_NAME_WBS = "WBS";
        private const string FIELD_NAME_GL = "GL";
        private const string FIELD_NAME_TITLE = "Title";
        private const string FIELD_NAME_AMOUNT = "Amount";

        private string _siteUrl = string.Empty;

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = siteUrl;
        }

        public int GetActivityIDByEventBudgetID(int eventBudgetID)
        {
            int activityID = 0;
            if (eventBudgetID > 0)
            {
                var list = SPConnector.GetListItem(LIST_NAME_EVENT_BUDGET, eventBudgetID, _siteUrl);
                activityID = list["Activity_x0020_ID0"] == null ? 0 :
                        Convert.ToInt32((list["Activity_x0020_ID0"] as FieldLookupValue).LookupId);
            }

            return activityID;
        }

        public int? CreateSCAVoucher(SCAVoucherVM scaVoucher)
        {
            int? result = null;
            var columnValues = new Dictionary<string, object>
            {
                {FIELD_NAME_SCA_NO,scaVoucher.SCAVoucherNo },
                {FIELD_NAME_DATE,scaVoucher.SCAVoucherDate},
                {FIELD_NAME_SDOID,new FieldLookupValue { LookupId = Convert.ToInt32(scaVoucher.SDO.Value) }},
                {FIELD_NAME_EBUDGET_ID, new FieldLookupValue { LookupId = Convert.ToInt32(scaVoucher.EventBudgetNo.Value) }},
                {FIELD_NAME_CURRENCY,scaVoucher.Currency},
                {FIELD_NAME_TOTAL_AMOUNT,scaVoucher.TotalAmount},
                {FIELD_NAME_TA_WORDS,scaVoucher.TotalAmountInWord},
                {FIELD_NAME_PURPOSE,scaVoucher.Purpose},
                {FIELD_NAME_PROJECT,scaVoucher.Project},
                {FIELD_NAME_ACTIVITY_ID, new FieldLookupValue { LookupId = Convert.ToInt32(scaVoucher.Activity.Value) } },
                {FIELD_NAME_SUB_ACTIVITY_ID,new FieldLookupValue { LookupId = Convert.ToInt32(scaVoucher.SubActivity.Value) }},
                {FIELD_NAME_FUND,scaVoucher.Fund},
                {FIELD_NAME_REFFERENCE_NO,scaVoucher.RefferenceNo},
                {FIELD_NAME_REMARKS,scaVoucher.Remarks}
            };

            try
            {
                SPConnector.AddListItem(LIST_NAME_SCAVOUCHER, columnValues, _siteUrl);
                result = SPConnector.GetLatestListItemID(LIST_NAME_SCAVOUCHER, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }

            return result;
        }

        public async Task CreateSCAVoucherItem(int? scaVoucherID, IEnumerable<SCAVoucherItemsVM> viewModels)
        {
            foreach (var viewModel in viewModels)
            {
                var columnValues = new Dictionary<string, object>
                {
                    {FIELD_NAME_TITLE,scaVoucherID},
                    {FIELD_NAME_WBS,new FieldLookupValue { LookupId = Convert.ToInt32(viewModel.WBSID) }},
                    {FIELD_NAME_GL, new FieldLookupValue { LookupId = Convert.ToInt32(viewModel.GLID) }},
                    {FIELD_NAME_AMOUNT,viewModel.Amount}
                };
                try
                {
                    SPConnector.AddListItem(LIST_NAME_SCAVOUCHER_ITEM, columnValues, _siteUrl);
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw e;
                }
            }
        }

        public SCAVoucherVM GetSCAVoucherVMData(int? ID)
        {
            var viewModel = new SCAVoucherVM();
            var viewModels = new List<SCAVoucherItemsVM>();
            var caml = @"<View><Query><Where><Eq><FieldRef Name='SCAVoucher' /><Value Type='Lookup'>" + ID.ToString() + "</Value></Eq></Where></Query></View>";

            if (ID != null)
            {
                var list = SPConnector.GetListItem(LIST_NAME_SCAVOUCHER,ID, _siteUrl);
                viewModel = ConvertToSCAVoucherVM(list);

                foreach (var item in SPConnector.GetList(LIST_NAME_SCAVOUCHER_ITEM, _siteUrl, caml))
                {
                    viewModels.Add(
                        new SCAVoucherItemsVM
                        {
                            GLID = Convert.ToInt32(item[FIELD_NAME_WBS]),
                            WBSID = Convert.ToInt32(item[FIELD_NAME_WBS]),
                            Amount = Convert.ToInt32(item[FIELD_NAME_AMOUNT])
                        }
                        );
                }
            }

            return viewModel;
        }

        private SCAVoucherVM ConvertToSCAVoucherVM(ListItem ListItem)
        {
            return new SCAVoucherVM()
            {
                SCAVoucherNo = ListItem[FIELD_NAME_SCA_NO].ToString(),
                SCAVoucherDate= Convert.ToDateTime(ListItem[FIELD_NAME_DATE]),
                SDO = new Model.ViewModel.Control.AjaxComboBoxVM { Value = Convert.ToInt32(ListItem[FIELD_NAME_SDOID])},
                EventBudgetNo = new Model.ViewModel.Control.AjaxComboBoxVM{Value = Convert.ToInt32(ListItem[FIELD_NAME_EBUDGET_ID])},
                Currency = ListItem[FIELD_NAME_CURRENCY].ToString(),
                TotalAmount = Convert.ToDecimal(ListItem[FIELD_NAME_TOTAL_AMOUNT]),
                TotalAmountInWord = ListItem[FIELD_NAME_TA_WORDS].ToString(),
                Purpose = ListItem[FIELD_NAME_PURPOSE].ToString(),
                Project = ListItem[FIELD_NAME_PROJECT].ToString(),
                Activity = new Model.ViewModel.Control.AjaxComboBoxVM{ Value = Convert.ToInt32(ListItem[FIELD_NAME_ACTIVITY_ID])},
                SubActivity= new Model.ViewModel.Control.AjaxCascadeComboBoxVM { Value = Convert.ToInt32(ListItem[FIELD_NAME_SUB_ACTIVITY_ID])},
                Fund = Convert.ToDecimal(ListItem[FIELD_NAME_FUND]),
                RefferenceNo = ListItem[FIELD_NAME_REFFERENCE_NO].ToString(),
                Remarks = ListItem[FIELD_NAME_REMARKS].ToString()
            };
        }
        
    }
}
