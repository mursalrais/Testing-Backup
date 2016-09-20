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

        const string SP_AJUDATA_LIST_NAME = "Adjustment";
        const string SP_PROMAS_LIST_NAME = "Professional Master";

        public void SetSiteUrl(string siteUrl = null)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
        }

        public bool CheckRequest(AdjustmentDataVM header)
        {
            var viewModel = new CompensatoryVM();

            var columnValues = new Dictionary<string, object>();

            var adjustmentlist = new List<AdjustmentDetailsVM>();

            foreach (var detailitem in SPConnector.GetList(SP_AJUDATA_LIST_NAME, _siteUrl, ""))
            {
                adjustmentlist.Add(ConvertToAdjusDetailVM(detailitem));
            }

            foreach (var cekadjust in header.AdjustmentDetails)
            {
                if (cekadjust.EditMode != 0)
                {
                    foreach (var getadjust in adjustmentlist)
                    {
                        if (cekadjust.ddlProfessional.Value == getadjust.ddlProfessional.Value && cekadjust.ajusmentType.Text == getadjust.ajusmentType.Text)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public AdjustmentDataVM GetPeriod(int? id)
        {
            var viewModel = new AdjustmentDataVM();

            if (id == null)
                return viewModel;

            var caml = @"<View>  
                            <Query>
                               <Where>
                                  <Eq>
                                     <FieldRef Name='ID' />
                                     <Value Type='Lookup'>" + id + @"</Value>
                                  </Eq>
                               </Where>
                            </Query>
                            <ViewFields>
                               <FieldRef Name='ID' />
                               <FieldRef Name='ContentType' />
                               <FieldRef Name='adjustmentperiod' />
                               <FieldRef Name='professional' />
                               <FieldRef Name='adjustmenttype' />
                               <FieldRef Name='adjustmentamount' />
                               <FieldRef Name='debitorcredit' />
                               <FieldRef Name='remarks' />
                            </ViewFields>
                            <QueryOptions />
                           </View>";

            foreach (var item in SPConnector.GetList(SP_AJUDATA_LIST_NAME, _siteUrl, caml))
            {
                var getperiod = Convert.ToDateTime(item["adjustmentperiod"]);
                var perioddate = FormatUtil.ConvertToDateString(getperiod);
                getperiod = Convert.ToDateTime(perioddate);

                viewModel.periodDate = getperiod;
                viewModel.periodval = Convert.ToDateTime(item["adjustmentperiod"]);
            }

            return viewModel;
        }

        public AdjustmentDataVM GetAjusmentData(string period)
        {
            var viewModel = new AdjustmentDataVM();

            if (period == null)
                return viewModel;

            viewModel.AdjustmentDetails = GetAjusmentDetails(period);

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
            var adjustmentDetails = new List<AdjustmentDetailsVM>();

            foreach (var item in SPConnector.GetList(SP_AJUDATA_LIST_NAME, _siteUrl, ""))
            {
                adjustmentDetails.Add(ConvertToAdjusDetailVM(item));
            }

            var getperiod = Convert.ToDateTime(period);
            var perioddate = FormatUtil.ConvertToDateString(getperiod);
            getperiod = Convert.ToDateTime(perioddate);

            var adjustmentList = from a in adjustmentDetails where a.period == getperiod select a;

            adjustmentDetails = adjustmentList.ToList();

            return adjustmentDetails;
        }

        private AdjustmentDetailsVM ConvertToAdjusDetailVM(ListItem item)
        {
            int? idprof = Convert.ToInt32(FormatUtil.ConvertLookupToID(item, "professional_x003a_ID") + string.Empty);
            var proj_unit = "";
            var position = "";
            var models = GetProfessionalsActives(idprof);

            foreach (var prof_item in models)
            {
               proj_unit = prof_item.Project_Unit;
               position = prof_item.Position;
            }

            return new AdjustmentDetailsVM
            {
                ajusmentType = AdjustmentDetailsVM.getAjusmentDefaultValue(
                    new Model.ViewModel.Control.AjaxComboBoxVM
                    {
                        Text = Convert.ToString(item["adjustmenttype"]),
                    }),

                payType = AdjustmentDetailsVM.getpayTypeDefaultValue(
                    new Model.ViewModel.Control.AjaxComboBoxVM
                    {
                        Text = Convert.ToString(item["debitorcredit"]),
                    }),

                ddlProfessional = AdjustmentDetailsVM.getprofDefaultValue(FormatUtil.ConvertToInGridAjaxComboBox(item, "professional")),
                projUnit = proj_unit,
                position = position,
                period = Convert.ToDateTime(item["adjustmentperiod"]),
                amount = Convert.ToString(item["adjustmentamount"]),
                remark = Convert.ToString(item["remarks"]),
                Title = Convert.ToString(item["Title"]),
                ID = Convert.ToInt32(item["ID"]),
            };
        }

        private int CheckAdjustment (int? profID)
        {
            var caml = @"<View>  
                            <Query>
                               <Where>
                                  <Eq>
                                     <FieldRef Name='professional_x003a_ID' />
                                     <Value Type='Lookup'>" + profID + @"</Value>
                                  </Eq>
                               </Where>
                            </Query>
                            <ViewFields>
                               <FieldRef Name='ID' />
                               <FieldRef Name='ContentType' />
                               <FieldRef Name='adjustmentperiod' />
                               <FieldRef Name='professional' />
                               <FieldRef Name='adjustmenttype' />
                               <FieldRef Name='adjustmentamount' />
                               <FieldRef Name='debitorcredit' />
                               <FieldRef Name='remarks' />
                            </ViewFields>
                            <QueryOptions />
                           </View>";

            var count = SPConnector.GetList(SP_AJUDATA_LIST_NAME, _siteUrl, caml).Count();
     
            return count;
        }

        public void CreateAdjustmentData(string period, AdjustmentDataVM viewModels)
        {
            var getperiod = Convert.ToDateTime(period);
            var perioddate = FormatUtil.ConvertToDateString(getperiod);
            getperiod = Convert.ToDateTime(perioddate);

            foreach (var viewModel in viewModels.AdjustmentDetails)
            {
                if (viewModel.ID == null)
                {
                    var cratedValueDetail = new Dictionary<string, object>();
                    cratedValueDetail.Add("Title", Convert.ToString(getperiod));
                    cratedValueDetail.Add("adjustmentperiod", getperiod);
                    cratedValueDetail.Add("professional", new FieldLookupValue { LookupId = (int)viewModel.ddlProfessional.Value });
                    cratedValueDetail.Add("adjustmenttype", viewModel.ajusmentType.Text);
                    cratedValueDetail.Add("adjustmentamount", viewModel.amount);
                    cratedValueDetail.Add("debitorcredit", viewModel.payType.Text);
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
                    if (Item.CheckIfUpdated(viewModel))
                    {
                        var updatedValue = new Dictionary<string, object>();

                        updatedValue.Add("Title", Convert.ToString(getperiod));
                        updatedValue.Add("adjustmentperiod", getperiod);
                        updatedValue.Add("professional", new FieldLookupValue { LookupId = (int)viewModel.ddlProfessional.Value });
                        updatedValue.Add("adjustmenttype", viewModel.ajusmentType.Text);
                        updatedValue.Add("adjustmentamount", viewModel.amount);
                        updatedValue.Add("debitorcredit", viewModel.payType.Text);
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

        private IEnumerable<ProfessionalMaster> GetProfessionalsActives(int? id)
        {
            var caml = @"<View><Query><Where><Eq><FieldRef Name='ID' /><Value Type='Number'>" + id + @"</Value></Eq></Where> </Query></View>";

            var models = new List<ProfessionalMaster>();
            foreach (var item in SPConnector.GetList(SP_PROMAS_LIST_NAME, _siteUrl, caml))
            {
                models.Add(ConvertToProfessionalModel_Light(item));
            }

            return models;
        }
        private ProfessionalMaster ConvertToProfessionalModel_Light(ListItem item)
        {
            return new ProfessionalMaster
            {
                ID = Convert.ToInt32(item["ID"]),
                Name = Convert.ToString(item["Title"]) + " " + Convert.ToString(item["lastname"]),
                Position = item["Position"] == null ? string.Empty :
                        Convert.ToString((item["Position"] as FieldLookupValue).LookupValue),
                PositionId = item["Position"] == null ? 0 :
                        Convert.ToInt32((item["Position"] as FieldLookupValue).LookupId),
                Project_Unit = Convert.ToString(item["Project_x002f_Unit"]),
                OfficeEmail = Convert.ToString(item["officeemail"]),

            };
        }
    }
}