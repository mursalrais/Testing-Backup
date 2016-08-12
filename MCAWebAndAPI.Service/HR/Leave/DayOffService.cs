using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Service.HR.Leave
{
    public class DayOffService : IDayOffService
    {
        string _siteUrl;
        static Logger logger = LogManager.GetCurrentClassLogger();

        const string SP_DAYOFF_REQ_LIST_NAME = "Day-Off Request";
        const string SP_DAYOFF_REQ_DETAIL_LIST_NAME = "Day-Off Request Detail";
        const string SP_DAYOFF_BAL_LIST_NAME = "Day-Off Balance";
        const string SP_MAS_DAYOFF_TYPE_LIST_NAME = "Master Day Off Type";
        const string SP_PSA_LIST_NAME = "PSA Management";


        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
        }

        private int CountDiffMont(DateTime DateAfter, DateTime DateBefore)
        {
            return (((DateAfter.Year - DateBefore.Year) * 12) + DateAfter.Month - DateBefore.Month) ;
        }

        public void PopulateBalance(int idPSA, PSAManagementVM viewModel, string action)
        {
            var updatedValues = new Dictionary<string, object>();
            double entitlement = 0;
            double dayofbrought = 0;
            double deduction = 0;
            string psatitle ;
            DateTime dateofnewpsa = viewModel.DateOfNewPSA.Value.ToLocalTime();
            DateTime endofcontract = viewModel.PSAExpiryDate.Value.ToLocalTime();
            DateTime lastworkingdate = viewModel.LastWorkingDate.Value.ToLocalTime();
            int statusdraft = 0;
            int statuspendingapproval = 0;
            int statusapproved = 0;
            int statusrejected = 0;
            psatitle = "PSA" + viewModel.RenewalNumber.ToString();
            int balanceBefore = 0;
            DateTime dateofnewpsaBefore = DateTime.Today.ToLocalTime();
            DateTime endofcontrackBefore = DateTime.Today.ToLocalTime();
            DateTime lastworkingdateBefore = DateTime.Today.ToLocalTime();

            if ((viewModel.PSARenewalNumber == 0))
            {
                entitlement = CountDiffMont(endofcontract, dateofnewpsa) * 1.25;                
            }
            else 
            {                   
                string caml = @"<View><Query><Where><And><Eq><FieldRef Name='professional' /><Value Type='Lookup'>"+viewModel.Professional.Value.Value.ToString()+"</Value></Eq><Eq><FieldRef Name='PSA_x003a_Renewal_x0020__x0023_' /><Value Type='Lookup'>"+viewModel.RenewalNumber--.ToString()+"</Value></Eq></And></Where></Query><ViewFields><FieldRef Name='ID' /><FieldRef Name='ContentType' /><FieldRef Name='Title' /><FieldRef Name='Modified' /><FieldRef Name='Created' /><FieldRef Name='Author' /><FieldRef Name='Editor' /><FieldRef Name='_UIVersionString' /><FieldRef Name='Attachments' /><FieldRef Name='Edit' /><FieldRef Name='LinkTitleNoMenu' /><FieldRef Name='LinkTitle' /><FieldRef Name='DocIcon' /><FieldRef Name='ItemChildCount' /><FieldRef Name='FolderChildCount' /><FieldRef Name='AppAuthor' /><FieldRef Name='AppEditor' /><FieldRef Name='entitlement' /><FieldRef Name='dayoffbrought' /><FieldRef Name='deduction' /><FieldRef Name='statusdraft' /><FieldRef Name='statuspendingapproval' /><FieldRef Name='statusapproved' /><FieldRef Name='statusrejected' /><FieldRef Name='entitlementtotal' /><FieldRef Name='balance' /><FieldRef Name='PSA' /><FieldRef Name='PSA_x003a_Renewal_x0020__x0023_' /><FieldRef Name='professional' /><FieldRef Name='dateofnewpsa' /><FieldRef Name='endofcontract' /><FieldRef Name='lastworkingdate' /></ViewFields><QueryOptions />";
                foreach (var item in SPConnector.GetList(SP_DAYOFF_BAL_LIST_NAME, _siteUrl, caml))
                {
                    balanceBefore = Convert.ToInt32(item["balance"]);
                    dateofnewpsaBefore = Convert.ToDateTime(item["dateofnewpsa"]).ToLocalTime();
                    endofcontrackBefore = Convert.ToDateTime(item["endofcontract"]).ToLocalTime();
                    lastworkingdateBefore = Convert.ToDateTime(item["lastworkingdate"]).ToLocalTime();                                        
                }
                if (dateofnewpsa > endofcontract)
                {
                    entitlement = CountDiffMont(endofcontract,dateofnewpsa) * 1.25;
                }
                else 
                {
                    entitlement = (CountDiffMont(lastworkingdate, dateofnewpsa) * 1.25) - (CountDiffMont(endofcontrackBefore , lastworkingdateBefore)*1.25);
                }
                if ((dateofnewpsa > lastworkingdate.AddDays(1)) || (balanceBefore <= 0) )
                {
                    dayofbrought = 0;
                }
                else if (balanceBefore > 5)
                {
                    dayofbrought = 5;
                }
                else
                {
                    dayofbrought = balanceBefore;
                }
                if (dateofnewpsa > lastworkingdateBefore.AddDays(1))
                {
                    deduction = 0;
                }
                else if (balanceBefore< 0)
                {
                    deduction = balanceBefore;
                }
                else
                {
                    deduction = 0;
                }
            }
            updatedValues.Add("entitlement", entitlement);
            updatedValues.Add("dayofbrought", dayofbrought);
            updatedValues.Add("deduction", deduction);
            updatedValues.Add("psatitle", psatitle); 
            updatedValues.Add("dateofnewpsa", dateofnewpsa);
            updatedValues.Add("endofcontract", endofcontract);
            updatedValues.Add("lastworkingdate", lastworkingdate);
            updatedValues.Add("statusdraft", statusdraft);
            updatedValues.Add("statuspendingapproval", statuspendingapproval);
            updatedValues.Add("statusrejected", statusrejected); 
            updatedValues.Add("statusapproved", statusapproved);
            updatedValues.Add("PSA", idPSA);
            updatedValues.Add("professional", viewModel.Professional.Value.Value);
            updatedValues.Add("type", "Annual");


            if (action == "Create")
            {
                try
                {
                    SPConnector.AddListItem(SP_DAYOFF_BAL_LIST_NAME, updatedValues, _siteUrl);
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw e;
                }
            }
        }

        private int? GetDayOffHeaderIDFromProfessional(int professionalID)
        {
            var caml = @"<View>  
            <Query> 
               <Where><Eq><FieldRef Name='professional' LookupId='True' /><Value Type='Lookup'>" + professionalID
               + @"</Value></Eq></Where> 
            </Query> 
                <ViewFields><FieldRef Name='professional' /><FieldRef Name='ID' /></ViewFields> 
            </View>";

            int? headerID = null;
            foreach (var item in SPConnector.GetList(SP_DAYOFF_REQ_LIST_NAME, _siteUrl, caml))
            {
                headerID = Convert.ToInt32(item["ID"]);
                break;
            }

            return headerID;
        }

        public int GetUnpaidDayOffTotalDays(int professionalID, IEnumerable<DateTime> dateRange)
        {
            var headerID = GetDayOffHeaderIDFromProfessional(professionalID);

            var caml = @"<View><Query><Where><And><Eq>
                    <FieldRef Name='masterdayofftype' />
                    <Value Type='Lookup'>Unpaid Day-Off</Value></Eq><Eq>
                    <FieldRef Name='dayoffrequest' LookupId='True'/><Value Type='Lookup'>" +
                    headerID + @"</Value></Eq></And></Where></Query></View>";

            var totalUnpaidDayOff = 0;
            foreach (var item in SPConnector.GetList(SP_DAYOFF_REQ_DETAIL_LIST_NAME, _siteUrl, caml))
            {
                totalUnpaidDayOff += Convert.ToInt32(item["totaldays"]);
            }

            return totalUnpaidDayOff;
        }

        public bool IsUnpaidDayOff(int professionalID, DateTime date, IEnumerable<DateTime> dateRange)
        {
            var headerID = GetDayOffHeaderIDFromProfessional(professionalID);

            var caml = @"<View><Query><Where><And><Eq>
                    <FieldRef Name='masterdayofftype' />
                    <Value Type='Lookup'>Unpaid Day-Off</Value></Eq><Eq>
                    <FieldRef Name='dayoffrequest' LookupId='True'/><Value Type='Lookup'>" +
                    headerID + @"</Value></Eq></And></Where></Query></View>";

            foreach (var item in SPConnector.GetList(SP_DAYOFF_REQ_DETAIL_LIST_NAME, _siteUrl, caml))
            {
                var startDate = Convert.ToDateTime(item["requeststartdate"]);
                var endDate = Convert.ToDateTime(item["requestenddate"]);

                if (startDate <= date && date <= endDate)
                    return true;
            }

            return false;
        }

        public DayOffRequestVM GetPopulatedModel(string requestor = null)
        {
            string status = null;
            var caml = @"<View>  
            <Query> 
               <Where><Eq><FieldRef Name='officeemail' /><Value Type='Text'>" + requestor + @"</Value></Eq></Where> 
            </Query> 
      </View>";

            var model = new DayOffRequestVM();

            foreach (var item in SPConnector.GetList(SP_DAYOFF_BAL_LIST_NAME, _siteUrl, caml))
            {
                    model.Professional = FormatUtil.ConvertLookupToValue(item, "professional");
                    model.ProfessionalID = FormatUtil.ConvertLookupToID(item, "ID");
                    model.ProjectUnit = Convert.ToString(item["projectunit"]);
            }

            model.DayOffBalanceDetails = GetDayOffBalanceDetails(model.ProfessionalID);

            return model;
        }

        private IEnumerable<DayOffBalanceVM> GetDayOffBalanceDetails(int? ID)
        {
            var caml = @"<View><Query><Where><Eq><FieldRef Name='professionalperformanceplan' /><Value Type='Lookup'>" + ID.ToString() + "</Value></Eq></Where></Query></View>";

            var DayOffBalanceDetail = new List<DayOffBalanceVM>();
            foreach (var item in SPConnector.GetList(SP_MAS_DAYOFF_TYPE_LIST_NAME, _siteUrl, caml))
            {
                DayOffBalanceDetail.Add(ConvertToDayOffBalanceDetail(item));
            }

            return DayOffBalanceDetail;
        }

        private DayOffBalanceVM ConvertToDayOffBalanceDetail(ListItem item)
        {
            return new DayOffBalanceVM
            {
                DayOffType = DayOffBalanceVM.GetDayOffTypeDefaultValue(
                    new InGridComboBoxVM
                    {
                        Text = Convert.ToString(item["Title"])
                    }),
                Quantity = Convert.ToInt32(item["quantity"]),

            };
        }

        public DayOffRequestVM GetHeader(int? ID)
        {
            throw new NotImplementedException();
        }

        public int CreateHeader(DayOffRequestVM header)
        {
            throw new NotImplementedException();
        }

        public bool UpdateHeader(DayOffRequestVM header)
        {
            throw new NotImplementedException();
        }

        public void CreateDayOffDetails(int? headerID, IEnumerable<DayOffRequestDetailVM> dayOffDetails)
        {
            foreach (var viewModel in dayOffDetails)
            {
                if (Item.CheckIfSkipped(viewModel))
                    continue;
                if (Item.CheckIfDeleted(viewModel))
                {
                    try
                    {
                        SPConnector.DeleteListItem(SP_DAYOFF_REQ_DETAIL_LIST_NAME, viewModel.ID, _siteUrl);
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
                updatedValue.Add("dateofnewfee", viewModel.DayOffType.Text);
                updatedValue.Add("monthlyfee", viewModel.FullHalf.Text);
                updatedValue.Add("annualfee", viewModel.RequestStartDate);
                updatedValue.Add("currency", viewModel.RequestEndDate);
                updatedValue.Add("currency", viewModel.Remarks);
                try
                {
                    if (Item.CheckIfUpdated(viewModel))
                        SPConnector.UpdateListItem(SP_DAYOFF_REQ_DETAIL_LIST_NAME, viewModel.ID, updatedValue, _siteUrl);
                    else
                        SPConnector.AddListItem(SP_DAYOFF_REQ_DETAIL_LIST_NAME, updatedValue, _siteUrl);
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw new Exception(ErrorResource.SPInsertError);
                }
            }
        }

        public async Task CreateDayOffDetailsAsync(int? headerID, IEnumerable<DayOffRequestDetailVM> dayOffDetails)
        {
            CreateDayOffDetails(headerID, dayOffDetails);
        }

        public void CreateDayOffBalanceDetails(int? headerID, IEnumerable<DayOffBalanceVM> dayOffBalanceDetails)
        {
            foreach (var viewModel in dayOffBalanceDetails)
            {
                if (Item.CheckIfSkipped(viewModel))
                    continue;
                if (Item.CheckIfDeleted(viewModel))
                {
                    try
                    {
                        SPConnector.DeleteListItem(SP_DAYOFF_BAL_LIST_NAME, viewModel.ID, _siteUrl);
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
                updatedValue.Add("Title", viewModel.DayOffType.Text);
                updatedValue.Add("quantity", viewModel.Balance);
                updatedValue.Add("annualfee", viewModel.DayOffBrought);
                updatedValue.Add("currency", viewModel.Unit.Text);
                try
                {
                    if (Item.CheckIfUpdated(viewModel))
                        SPConnector.UpdateListItem(SP_DAYOFF_BAL_LIST_NAME, viewModel.ID, updatedValue, _siteUrl);
                    else
                        SPConnector.AddListItem(SP_DAYOFF_BAL_LIST_NAME, updatedValue, _siteUrl);
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw new Exception(ErrorResource.SPInsertError);
                }
            }
        }

        public async Task CreateDayOffBalanceDetailsAsync(int? headerID, IEnumerable<DayOffBalanceVM> dayOffBalanceDetails)
        {
            CreateDayOffBalanceDetails(headerID, dayOffBalanceDetails);
        }
    }
}

