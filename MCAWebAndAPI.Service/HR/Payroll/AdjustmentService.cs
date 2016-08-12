using System;
using System.Collections.Generic;
using System.Web;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.Utils;
using NLog;
using Microsoft.SharePoint.Client;
using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.HR.DataMaster;
using System.Threading.Tasks;
using System.Linq;

namespace MCAWebAndAPI.Service.HR.Payroll
{
    public class AdjustmentService : IAdjustmentService
    {
        string _siteUrl;
        static Logger logger = LogManager.GetCurrentClassLogger();

        const string SP_AJUDATA_LIST_NAME = "Ajustment";
        const string SP_PROMAS_LIST_NAME = "Professional Master";

        public void SetSiteUrl(string siteUrl = null)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
        }

        public AdjustmentDataVM GetAjusmentData(string period)
        {
            var viewModel = new AdjustmentDataVM();

            if (period == null)
                return viewModel;

            viewModel.ajustmentDetails = GetAjusmentDetails(period);

            return viewModel;
        }

        //<ViewFields>
        //   <FieldRef Name = 'Title' />
        //   < FieldRef Name='applications' />
        //   <FieldRef Name = 'university' />
        //   < FieldRef Name='yearofgraduation' />
        //   <FieldRef Name = 'remarks' />
        //</ ViewFields >
        private IEnumerable<AdjustmentDetailsVM> GetAjusmentDetails(string period)
        {
            var caml = @"<View>  
                              <Query> 
                            <Where>
                              <Eq>
                                 <FieldRef Name='adjustmentperiod' />
                                 <Value IncludeTimeValue='TRUE' Type='DateTime'>" + period + @"</Value>
                              </Eq>
                            </Where>
                                </Query> 
                               <Query />
                            <ViewFields>
                                <FieldRef Name='ID' />
                                <FieldRef Name='ContentType' />
                                <FieldRef Name='Title' />
                                <FieldRef Name='adjustmentperiod' />
                                <FieldRef Name='professional' />
                                <FieldRef Name='professional_x003a_ID' />
                                <FieldRef Name='adjustmenttype' />
                                <FieldRef Name='adjustmentamount' />
                                <FieldRef Name='adjustmentcurrency' />
                                <FieldRef Name='debitorcredit' />
                                <FieldRef Name='remarks' />
                            </ViewFields>
                           </View>";

            var adjustmentDetails = new List<AdjustmentDetailsVM>();

            foreach (var item in SPConnector.GetList(SP_AJUDATA_LIST_NAME, _siteUrl, caml))
            {
                adjustmentDetails.Add(ConvertToAdjusDetailVM(item));
            }

            return adjustmentDetails;
        }

        private AdjustmentDetailsVM ConvertToAdjusDetailVM(ListItem item)
        {
            return new AdjustmentDetailsVM
            {
                ajusmentType = AdjustmentDetailsVM.getAjusmentDefaultValue(
                    new Model.ViewModel.Control.AjaxComboBoxVM
                    {
                        Text = Convert.ToString(item["adjustmenttype"]),
                    }),

                currency = AdjustmentDetailsVM.getCurrencyDefaultValue(
                    new Model.ViewModel.Control.AjaxComboBoxVM
                    {
                        Text = Convert.ToString(item["adjustmentcurrency"]),
                    }),

                payType = AdjustmentDetailsVM.getpayTypeDefaultValue(
                    new Model.ViewModel.Control.AjaxComboBoxVM
                    {
                        Text = Convert.ToString(item["debitorcredit"]),
                    }),

                ddlProfessional = AdjustmentDetailsVM.getpayTypeDefaultValue(
                    new Model.ViewModel.Control.AjaxComboBoxVM
                    {
                        ValueField = Convert.ToString(item["professional_x003a_ID"]),
                    }),

                remark = Convert.ToString(item["remarks"]),
                Title = Convert.ToString(item["Title"]),
                ID = Convert.ToInt32(item["ID"]),
            };
        }

        public void CreateAdjustmentData(string period, AdjustmentDataVM viewModels)
        {
            foreach (var viewModel in viewModels.ajustmentDetails)
            {
                if (viewModel.ID == null)
                {
                    var cratedValueDetail = new Dictionary<string, object>();

                    cratedValueDetail.Add("Title", viewModel.Title);
                    cratedValueDetail.Add("adjustmentperiod", period);
                    cratedValueDetail.Add("professional_x003a_ID", viewModel.profId);
                    cratedValueDetail.Add("adjustmenttype", viewModel.ajusmentType);
                    cratedValueDetail.Add("adjustmentamount", viewModel.amount);
                    cratedValueDetail.Add("adjustmentcurrency", viewModel.currency);
                    cratedValueDetail.Add("debitorcredit", viewModel.payType);
                    cratedValueDetail.Add("remarks", viewModel.remark);

                    try
                    {
                        SPConnector.AddListItem(SP_AJUDATA_LIST_NAME, cratedValueDetail, _siteUrl);
                    }
                    catch (Exception e)
                    {
                        logger.Error(e.Message);
                        throw e;
                    }
                }
                else
                {
                    var updatedValue = new Dictionary<string, object>();

                    updatedValue.Add("Title", viewModel.Title);
                    updatedValue.Add("adjustmentperiod", period);
                    updatedValue.Add("professional_x003a_ID", viewModel.profId);
                    updatedValue.Add("adjustmenttype", viewModel.ajusmentType);
                    updatedValue.Add("adjustmentamount", viewModel.amount);
                    updatedValue.Add("adjustmentcurrency", viewModel.currency);
                    updatedValue.Add("debitorcredit", viewModel.payType);
                    updatedValue.Add("remarks", viewModel.remark);

                    try
                    {
                        SPConnector.UpdateListItem(SP_AJUDATA_LIST_NAME, viewModel.ID, updatedValue, _siteUrl);

                    }
                    catch (Exception e)
                    {
                        logger.Error(e.Message);
                        throw e;
                    }
                }
                if (Item.CheckIfDeleted(viewModel))
                {
                    try
                    {
                        SPConnector.DeleteListItem(SP_AJUDATA_LIST_NAME, viewModel.ID, _siteUrl);
                    }
                    catch (Exception e)
                    {
                        logger.Error(e);
                        throw e;
                    }
                    continue;
                }
            }
        }
    }
}