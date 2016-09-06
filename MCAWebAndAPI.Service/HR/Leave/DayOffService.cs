using MCAWebAndAPI.Model.HR.DataMaster;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.Utils;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using MCAWebAndAPI.Model.ViewModel.Control;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Service.Resources;

namespace MCAWebAndAPI.Service.HR.Leave
{
    public class DayOffService : IDayOffService
    {
        string _siteUrl;
        static Logger logger = LogManager.GetCurrentClassLogger();

        const string SP_DAYOFF_REQ_LIST_NAME = "Day-Off Request";
        const string SP_DAYOFF_BAL_LIST_NAME = "Day-Off Balance";
        const string SP_DAYOFF_REQ_DETAIL_LIST_NAME = "Day-Off Request Detail";
        const string SP_MAS_DAYOFF_TYPE_LIST_NAME = "Master Day Off Type";
        const string SP_PSA_LIST_NAME = "PSA Management";
        const string SP_PRO_MAS_LIST_NAME = "Professional Master";
        const string SP_POSITION_MAS_LIST_NAME = "Position Master";

        const string TYPE_UNPAID_DAYOFF = "Unpaid Day-Off";


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
            if (headerID == null)
                return 0;

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
            if (headerID == null)
                return false;

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

        public IEnumerable<DayOffRequest> GetDayOffRequests(IEnumerable<int> professionalIDs)
        {
            var dayOffRequests = new List<DayOffRequest>();
            foreach (var professionalID in professionalIDs)
            {
                var headerID = GetDayOffHeaderIDFromProfessional(professionalID);

                var caml = @"<View><Query><Where><Eq>
                    <FieldRef Name='dayoffrequest' LookupId='True'/><Value Type='Lookup'>" +
                        headerID + @"</Value></Eq></Where></Query></View>";

                foreach (var item in SPConnector.GetList(SP_DAYOFF_REQ_DETAIL_LIST_NAME, _siteUrl, caml))
                {
                    dayOffRequests.Add(ConvertToDayOffRequest(item, professionalID));
                }
            }
            return dayOffRequests;
        }

        private DayOffRequest ConvertToDayOffRequest(ListItem item, int professionalID)
        {
            return new DayOffRequest
            {
                ID = Convert.ToInt32(item["ID"]),
                DayOffType = FormatUtil.ConvertLookupToValue(item, "masterdayofftype"),
                StartDate = Convert.ToDateTime(item["requeststartdate"]),
                EndDate = Convert.ToDateTime(item["requestenddate"]),
                TotalDays = Convert.ToInt32(item["totaldays"]),
                ApprovalStatus = Convert.ToString(item["approvalstatus"]),
                ProfessionalID = professionalID
            };
        }

        public DayOffRequestVM GetRequestData(List<string> dayOffType, List<string> startDate, List<string> endDate, List<string> fullHalfDay, List<string> remarks, List<string> totalDays)
        {
            var model = new DayOffRequestVM();

            model.DayOffRequestDetailsDisplay = GetDayOffRequestsDetailsDisplay(dayOffType, startDate, endDate, fullHalfDay, remarks, totalDays);

            return model;
        }

        private IEnumerable<DayOffRequestDetailDisplayVM> GetDayOffRequestsDetailsDisplay(List<string> dayOffType, List<string> startDate, List<string> endDate, List<string> fullHalfDay, List<string> remarks, List<string> totalDays)
        {
            var dayOffRequestDetailDisplayData = new List<DayOffRequestDetailDisplayVM>();

            string[] dayOffTypeRequest = dayOffType[0].Split(',');
            string[] startDateRequest = startDate[0].Split(',');
            string[] endDateRequest = endDate[0].Split(',');
            string[] fullHalfRequest = fullHalfDay[0].Split(',');
            string[] remarksRequest = remarks[0].Split(',');
            string[] totalDaysRequest = totalDays[0].Split(',');

            var dayOffRequest = new List<string>();
            var startRequest = new List<string>();
            var endRequest = new List<string>();
            var fullOrHalfRequest = new List<string>();
            var remarkRequest = new List<string>();
            var totalRequest = new List<string>();

            foreach (string checkDayOff in dayOffTypeRequest)
            {
                dayOffRequest.Add(checkDayOff);
            }

            foreach(string checkStartDate in startDateRequest)
            {
                startRequest.Add(checkStartDate);
            }
            
            foreach(string checkEndDate in endDateRequest)
            {
                endRequest.Add(checkEndDate);
            }
            
            foreach(string checkFullOrHalf in fullHalfRequest)
            {
                fullOrHalfRequest.Add(checkFullOrHalf);
            }

            foreach(string checkRemarks in remarksRequest)
            {
                remarkRequest.Add(checkRemarks);
            }

            foreach (string checkTotalDays in totalDaysRequest)
            {
                totalRequest.Add(checkTotalDays);
            }

            int dataCount = dayOffRequest.Count;

            for (int i = 1; i <= dataCount; i++)
            {
                dayOffRequestDetailDisplayData.Add(ConvertToDayOffRequestDetailsDisplay(dayOffRequest[i-1], startRequest[i-1], endRequest[i-1], fullOrHalfRequest[i-1], remarkRequest[i-1], totalRequest[i-1]));
            }

            return dayOffRequestDetailDisplayData;
        }

        private DayOffRequestDetailDisplayVM ConvertToDayOffRequestDetailsDisplay(string dayOffType, string startDate, string endDate, string fullHalfDay, string remarks, string totalDays)
        {
            var dayOffRequestDetailDisplay = new DayOffRequestDetailDisplayVM();
            string dayOffTypeRequest = "";

            if (dayOffType == "AnnualDay-Off")
            {
                dayOffTypeRequest = "Annual Day-Off";
            }
            else if(dayOffType == "SickDay-Off")
            {
                dayOffTypeRequest = "Sick Day-Off";
            }
            else if(dayOffType == "SpecialDay-Off")
            {
                dayOffTypeRequest = "Special Day-Off";
            }
            else if (dayOffType == "UnpaidDay-Off")
            {
                dayOffTypeRequest = "Unpaid Day-Off";
            }
            else if (dayOffType == "CompensatoryTime")
            {
                dayOffTypeRequest = "Compensatory Time";
            }
            else if (dayOffType == "Maternity")
            {
                dayOffTypeRequest = "Maternity";
            }
            else if (dayOffType == "Miscarriage")
            {
                dayOffTypeRequest = "Miscarriage";
            }
            else if (dayOffType == "Paternity")
            {
                dayOffTypeRequest = "Paternity";
            }
            else if (dayOffType == "MarriageoftheProfessional")
            {
                dayOffTypeRequest = "Marriage of the Professional";
            }
            else if (dayOffType == "MarriageoftheProfessional'sChildren")
            {
                dayOffTypeRequest = "Marriage of the Professional's Children";
            }
            else if (dayOffType == "CircumcisionoftheProfessional'sSons")
            {
                dayOffTypeRequest = "Circumcision of the Professional's Sons";
            }
            else if (dayOffType == "BaptismoftheProfessional'sChildren")
            {
                dayOffTypeRequest = "Baptism of the Professional's Children";
            }
            else if (dayOffType == "DeathoftheProfessional’sdependent(i.e. spouse or children)orparentorparentin-laws")
            {
                dayOffTypeRequest = "Death of the Professional’s dependent (i.e. spouse or children) or parent or parent in-laws";
            }
            else if (dayOffType == "Deathofamemberofthe Professional’shouseholdotherthantheProfessional’sdependentorparent")
            {
                dayOffTypeRequest = "Death of a member of the Professional’s household other than the Professional’s dependent or parent";
            }
            else if (dayOffType == "Professional’sseparationdateisonorafter19ofthemonth")
            {
                dayOffTypeRequest = "Professional’s separation date is on or after 19 of the month";
            }
            else if (dayOffType == "UnscheduledclosingofMCA-Indonesiaoffice(s)")
            {
                dayOffTypeRequest = "Unscheduled closing of MCA-Indonesia office(s)";
            }
            else if (dayOffType == "VotingDay")
            {
                dayOffTypeRequest = "Voting Day";
            }
            else if (dayOffType == "Serviceasacourtwitness")
            {
                dayOffTypeRequest = "Service as  a court witness";
            }
            else if (dayOffType == "Other")
            {
                dayOffTypeRequest = "Other";
            }

            dayOffRequestDetailDisplay.DayOffType = Convert.ToString(dayOffTypeRequest);
            dayOffRequestDetailDisplay.RequestStartDate = Convert.ToString(startDate);
            dayOffRequestDetailDisplay.RequestEndDate = Convert.ToString(endDate);

            string subFullHalfDay1 = Convert.ToString(fullHalfDay).Substring(0,4);
            string subFullHalfDay2 = Convert.ToString(fullHalfDay).Substring(4,3);
            string concFullHalfDay = subFullHalfDay1 + " " + subFullHalfDay2;

            dayOffRequestDetailDisplay.FullHalf = concFullHalfDay;

            var s = remarks;
            s = string.Join(
                    string.Empty,
                    s.Select((x, i) => (
                         char.IsUpper(x) && i > 0 &&
                         (char.IsLower(s[i - 1]) || (i < s.Count() - 1 && char.IsLower(s[i + 1])))
                    ) ? " " + x : x.ToString()));

            dayOffRequestDetailDisplay.Remarks = Convert.ToString(s);
            dayOffRequestDetailDisplay.StrTotalDays = Convert.ToString(totalDays);

            return dayOffRequestDetailDisplay;
        }

        //Digunakan
        public DayOffRequestVM GetPopulatedModel(int? ID, string requestor = null)
        {
            string status = null;
            var camlProfessionalData = @"<View>  
            <Query> 
               <Where><Eq><FieldRef Name='officeemail' /><Value Type='Text'>" + requestor + @"</Value></Eq></Where> 
            </Query> 
      </View>";

            var model = new DayOffRequestVM();

            foreach (var item in SPConnector.GetList(SP_PRO_MAS_LIST_NAME, _siteUrl, camlProfessionalData))
            {
                model.Professional = Convert.ToString(item["Title"]);
                model.ProfessionalID = Convert.ToInt32(item ["ID"]);
                model.ProjectUnit = Convert.ToString(item["Project_x002f_Unit"]);
                model.PositionID = FormatUtil.ConvertLookupToID(item, "Position").Value;
                model.LastWorkingDate = Convert.ToDateTime(item["lastworkingdate"]).ToLocalTime();
                model.PSAExpiryDate = Convert.ToDateTime(item["PSAexpirydate"]).ToLocalTime();
                model.ProfessionalStatus = Convert.ToString(item["Professional_x0020_Status"]);

                string strLastWorkingDate = Convert.ToDateTime(item["lastworkingdate"]).ToLocalTime().ToShortDateString();
                string strPSAExpiryDate = Convert.ToDateTime(item["PSAexpirydate"]).ToLocalTime().ToShortDateString();

                if(strLastWorkingDate == "1/1/0001")
                {
                    model.LastWorkingDate = Convert.ToDateTime(item["PSAexpirydate"]).ToLocalTime();
                }

                break;
            }

            var positionData = SPConnector.GetListItem(SP_POSITION_MAS_LIST_NAME, model.PositionID, _siteUrl);
            model.PositionName = Convert.ToString(positionData["Title"]);

            model.ProfessionalFullName = model.Professional + " - " + model.PositionName;

            model.DayOffBalanceDetails = GetDayOffBalanceDetails(model.Professional);

            if(ID == null)
            {
                model.DayOffRequestDetails = GetDayOffRequestsDetails(ID, model.Professional);
                model.DayOffNextBalance = GetDayOffNextBalance(model.Professional);
            }

            //model.DayOffNextBalance = GetDayOffNextBalance(model.Professional);

            return model;
        }

        //Digunakan
        private IEnumerable<DayOffBalanceVM> GetDayOffBalanceDetails(string professionalName)
        {
            var DayOffBalanceDetail = new List<DayOffBalanceVM>();

            foreach (var item in SPConnector.GetList(SP_MAS_DAYOFF_TYPE_LIST_NAME, _siteUrl, null))
            {
                DayOffBalanceDetail.Add(ConvertToDayOffBalanceDetail(item, professionalName));
            }

            return DayOffBalanceDetail;
        }

        //Digunakan
        private DayOffBalanceVM ConvertToDayOffBalanceDetail(ListItem item, string professionalName)
        {
            var dayOffBalanceDetail = new DayOffBalanceVM();


            if (Convert.ToString(item["Title"]) == "Annual Day-Off")
            {
                dayOffBalanceDetail = GetDayOffBalanceAnnualDayOff(Convert.ToString(item["Title"]), "Active", professionalName);
            }
            else if (Convert.ToString(item["Title"]) == "Special Day-Off")
            {
                dayOffBalanceDetail = GetDayOffBalanceSpecialDayOff(Convert.ToString(item["Title"]), professionalName);
            }
            else if (Convert.ToString(item["Title"]) == "Compensatory Time")
            {
                dayOffBalanceDetail = GetDayOffBalanceCompensatoryTime(Convert.ToString(item["Title"]), professionalName);
            }
            else if (Convert.ToString(item["Title"]) == "Paternity")
            {
                dayOffBalanceDetail = GetDayOffBalancePaternity(Convert.ToString(item["Title"]), professionalName);
            }
            else
            {
                dayOffBalanceDetail.DayOffType = DayOffBalanceVM.GetDayOffTypeDefaultValue(
                    new InGridComboBoxVM
                    {
                        Text = Convert.ToString(item["Title"])
                    });

                dayOffBalanceDetail.DayOffBrought = Convert.ToInt32(0);
                dayOffBalanceDetail.Unit = DayOffBalanceVM.GetUnitDefaultValue(
                        new InGridComboBoxVM
                        {
                            Text = Convert.ToString(item["uom"])
                        });
                dayOffBalanceDetail.Balance = Convert.ToDouble(item["quantity"]);
            }

            return dayOffBalanceDetail;
        }

        private IEnumerable<DayOffNextBalanceVM> GetDayOffNextBalance(string professionalName)
        {
            var DayOffNextBalance = new List<DayOffNextBalanceVM>();

            foreach (var item in SPConnector.GetList(SP_MAS_DAYOFF_TYPE_LIST_NAME, _siteUrl, null))
            {
                DayOffNextBalance.Add(ConvertToDayOffNextBalance(item, professionalName));
            }

            return DayOffNextBalance;
        }

        private DayOffNextBalanceVM ConvertToDayOffNextBalance(ListItem item, string professionalName)
        {
            var dayOffNextBalance = new DayOffNextBalanceVM();


            if (Convert.ToString(item["Title"]) == "Annual Day-Off")
            {
                dayOffNextBalance = GetDayOffNextBalanceAnnualDayOff(Convert.ToString(item["Title"]), "Active", professionalName);
            }
            else if (Convert.ToString(item["Title"]) == "Special Day-Off")
            {
                dayOffNextBalance = GetDayOffNextBalanceSpecialDayOff(Convert.ToString(item["Title"]), professionalName);
            }
            else if (Convert.ToString(item["Title"]) == "Compensatory Time")
            {
                dayOffNextBalance = GetDayOffNextBalanceCompensatoryTime(Convert.ToString(item["Title"]), professionalName);
            }
            else if (Convert.ToString(item["Title"]) == "Paternity")
            {
                dayOffNextBalance = GetDayOffNextBalancePaternity(Convert.ToString(item["Title"]), professionalName);
            }
            else
            {
                dayOffNextBalance.DayOffType = DayOffBalanceVM.GetDayOffTypeDefaultValue(
                    new InGridComboBoxVM
                    {
                        Text = Convert.ToString(item["Title"])
                    });

                dayOffNextBalance.DayOffBrought = Convert.ToInt32(0);
                dayOffNextBalance.Unit = DayOffBalanceVM.GetUnitDefaultValue(
                        new InGridComboBoxVM
                        {
                            Text = Convert.ToString(item["uom"])
                        });
                dayOffNextBalance.Balance = Convert.ToDouble(item["quantity"]);
            }

            return dayOffNextBalance;
        }

        //Digunakan
        private DayOffBalanceVM GetDayOffBalanceAnnualDayOff(string dayOffName, string psaStatus, string professionalName)
        {
            var dayOffBalanceData = new DayOffBalanceVM();

            var camlDayOffBalanceData = @"<View>  
            <Query> 
               <Where><And><And><Eq><FieldRef Name='dayoffname' /><Value Type='Choice'>" + dayOffName + @"</Value></Eq><Eq><FieldRef Name='professional' /><Value Type='Lookup'>" + professionalName + @"</Value></Eq></And><Eq><FieldRef Name='psastatus' /><Value Type='Choice'>" + psaStatus + @"</Value></Eq></And></Where> 
            </Query> 
      </View>";

            foreach (var dayOffBalanceRecord in SPConnector.GetList(SP_DAYOFF_BAL_LIST_NAME, _siteUrl, camlDayOffBalanceData))
            {
                dayOffBalanceData.DayOffType = DayOffBalanceVM.GetDayOffTypeDefaultValue(
                    new InGridComboBoxVM
                    {
                        Text = Convert.ToString(dayOffBalanceRecord["dayoffname"])
                    });

                dayOffBalanceData.DayOffBrought = Convert.ToInt32(dayOffBalanceRecord["dayoffbrought"]);
                dayOffBalanceData.Unit = DayOffBalanceVM.GetUnitDefaultValue(
                        new InGridComboBoxVM
                        {
                            Text = Convert.ToString("Days")
                        });
                dayOffBalanceData.Balance = Convert.ToInt32(dayOffBalanceRecord["finalbalance"]);
            }

            return dayOffBalanceData;
        }

        private DayOffNextBalanceVM GetDayOffNextBalanceAnnualDayOff(string dayOffName, string psaStatus, string professionalName)
        {
            var dayOffBalanceData = new DayOffNextBalanceVM();

            var camlDayOffBalanceData = @"<View>  
            <Query> 
               <Where><And><And><Eq><FieldRef Name='dayoffname' /><Value Type='Choice'>" + dayOffName + @"</Value></Eq><Eq><FieldRef Name='professional' /><Value Type='Lookup'>" + professionalName + @"</Value></Eq></And><Eq><FieldRef Name='psastatus' /><Value Type='Choice'>" + psaStatus + @"</Value></Eq></And></Where> 
            </Query> 
      </View>";

            foreach (var dayOffBalanceRecord in SPConnector.GetList(SP_DAYOFF_BAL_LIST_NAME, _siteUrl, camlDayOffBalanceData))
            {
                dayOffBalanceData.DayOffType = DayOffNextBalanceVM.GetDayOffTypeDefaultValue(
                    new InGridComboBoxVM
                    {
                        Text = Convert.ToString(dayOffBalanceRecord["dayoffname"])
                    });

                dayOffBalanceData.DayOffBrought = Convert.ToInt32(dayOffBalanceRecord["dayoffbrought"]);
                dayOffBalanceData.Unit = DayOffNextBalanceVM.GetUnitDefaultValue(
                        new InGridComboBoxVM
                        {
                            Text = Convert.ToString("Days")
                        });
                dayOffBalanceData.Balance = Convert.ToInt32(dayOffBalanceRecord["finalbalance"]);
            }

            return dayOffBalanceData;
        }

        //Digunakan
        private DayOffBalanceVM GetDayOffBalanceSpecialDayOff(string dayOffName, string professionalName)
        {
            var dayOffBalanceData = new DayOffBalanceVM();

            var camlDayOffBalanceData = @"<View>  
            <Query> 
               <Where><And><Eq><FieldRef Name='dayoffname' /><Value Type='Choice'>" + dayOffName + @"</Value></Eq><Eq><FieldRef Name='professional' /><Value Type='Lookup'>" + professionalName + @"</Value></Eq></And></Where> 
            </Query> 
      </View>";

            foreach (var dayOffBalanceRecord in SPConnector.GetList(SP_DAYOFF_BAL_LIST_NAME, _siteUrl, camlDayOffBalanceData))
            {
                dayOffBalanceData.DayOffType = DayOffBalanceVM.GetDayOffTypeDefaultValue(
                    new InGridComboBoxVM
                    {
                        Text = Convert.ToString(dayOffBalanceRecord["dayoffname"])
                    });

                dayOffBalanceData.DayOffBrought = Convert.ToInt32(dayOffBalanceRecord["dayoffbrought"]);
                dayOffBalanceData.Unit = DayOffBalanceVM.GetUnitDefaultValue(
                        new InGridComboBoxVM
                        {
                            Text = Convert.ToString("Days")
                        });
                dayOffBalanceData.Balance = Convert.ToInt32(dayOffBalanceRecord["finalbalance"]);
            }

            return dayOffBalanceData;
        }

        private DayOffNextBalanceVM GetDayOffNextBalanceSpecialDayOff(string dayOffName, string professionalName)
        {
            var dayOffBalanceData = new DayOffNextBalanceVM();

            var camlDayOffBalanceData = @"<View>  
            <Query> 
               <Where><And><Eq><FieldRef Name='dayoffname' /><Value Type='Choice'>" + dayOffName + @"</Value></Eq><Eq><FieldRef Name='professional' /><Value Type='Lookup'>" + professionalName + @"</Value></Eq></And></Where> 
            </Query> 
      </View>";

            foreach (var dayOffBalanceRecord in SPConnector.GetList(SP_DAYOFF_BAL_LIST_NAME, _siteUrl, camlDayOffBalanceData))
            {
                dayOffBalanceData.DayOffType = DayOffNextBalanceVM.GetDayOffTypeDefaultValue(
                    new InGridComboBoxVM
                    {
                        Text = Convert.ToString(dayOffBalanceRecord["dayoffname"])
                    });

                dayOffBalanceData.DayOffBrought = Convert.ToInt32(dayOffBalanceRecord["dayoffbrought"]);
                dayOffBalanceData.Unit = DayOffNextBalanceVM.GetUnitDefaultValue(
                        new InGridComboBoxVM
                        {
                            Text = Convert.ToString("Days")
                        });
                dayOffBalanceData.Balance = Convert.ToInt32(dayOffBalanceRecord["finalbalance"]);
            }

            return dayOffBalanceData;
        }

        //Digunakan
        private DayOffBalanceVM GetDayOffBalanceCompensatoryTime(string dayOffName, string professionalName)
        {
            var dayOffBalanceData = new DayOffBalanceVM();

            var camlDayOffBalanceData = @"<View>  
            <Query> 
               <Where><And><Eq><FieldRef Name='dayoffname' /><Value Type='Choice'>" + dayOffName + @"</Value></Eq><Eq><FieldRef Name='professional' /><Value Type='Lookup'>" + professionalName + @"</Value></Eq></And></Where> 
            </Query> 
      </View>";

            foreach (var dayOffBalanceRecord in SPConnector.GetList(SP_DAYOFF_BAL_LIST_NAME, _siteUrl, camlDayOffBalanceData))
            {
                dayOffBalanceData.DayOffType = DayOffBalanceVM.GetDayOffTypeDefaultValue(
                    new InGridComboBoxVM
                    {
                        Text = Convert.ToString(dayOffBalanceRecord["dayoffname"])
                    });

                dayOffBalanceData.DayOffBrought = Convert.ToInt32(dayOffBalanceRecord["dayoffbrought"]);
                dayOffBalanceData.Unit = DayOffBalanceVM.GetUnitDefaultValue(
                        new InGridComboBoxVM
                        {
                            Text = Convert.ToString("Days")
                        });
                dayOffBalanceData.Balance = Convert.ToInt32(dayOffBalanceRecord["finalbalance"]);
            }

            return dayOffBalanceData;
        }

        private DayOffNextBalanceVM GetDayOffNextBalanceCompensatoryTime(string dayOffName, string professionalName)
        {
            var dayOffBalanceData = new DayOffNextBalanceVM();

            var camlDayOffBalanceData = @"<View>  
            <Query> 
               <Where><And><Eq><FieldRef Name='dayoffname' /><Value Type='Choice'>" + dayOffName + @"</Value></Eq><Eq><FieldRef Name='professional' /><Value Type='Lookup'>" + professionalName + @"</Value></Eq></And></Where> 
            </Query> 
      </View>";

            foreach (var dayOffBalanceRecord in SPConnector.GetList(SP_DAYOFF_BAL_LIST_NAME, _siteUrl, camlDayOffBalanceData))
            {
                dayOffBalanceData.DayOffType = DayOffNextBalanceVM.GetDayOffTypeDefaultValue(
                    new InGridComboBoxVM
                    {
                        Text = Convert.ToString(dayOffBalanceRecord["dayoffname"])
                    });

                dayOffBalanceData.DayOffBrought = Convert.ToInt32(dayOffBalanceRecord["dayoffbrought"]);
                dayOffBalanceData.Unit = DayOffNextBalanceVM.GetUnitDefaultValue(
                        new InGridComboBoxVM
                        {
                            Text = Convert.ToString("Days")
                        });
                dayOffBalanceData.Balance = Convert.ToInt32(dayOffBalanceRecord["finalbalance"]);
            }

            return dayOffBalanceData;
        }

        //Digunakan
        private DayOffBalanceVM GetDayOffBalancePaternity(string dayOffName, string professionalName)
        {
            var dayOffBalanceData = new DayOffBalanceVM();

            var camlDayOffBalanceData = @"<View>  
            <Query> 
               <Where><And><Eq><FieldRef Name='dayoffname' /><Value Type='Choice'>" + dayOffName + @"</Value></Eq><Eq><FieldRef Name='professional' /><Value Type='Lookup'>" + professionalName + @"</Value></Eq></And></Where> 
            </Query> 
      </View>";

            var paternityRecord = new List<int>();
            int paternityID;

            foreach(var paternityData in SPConnector.GetList(SP_DAYOFF_BAL_LIST_NAME, _siteUrl, camlDayOffBalanceData))
            {
                paternityID = Convert.ToInt32(paternityData["ID"]);

                paternityRecord.Add(paternityID);
            }

            int latestPaternityID = paternityRecord.LastOrDefault();

            var paternityBalanceValid = SPConnector.GetListItem(SP_DAYOFF_BAL_LIST_NAME, latestPaternityID, _siteUrl);

            dayOffBalanceData.DayOffType = DayOffBalanceVM.GetDayOffTypeDefaultValue(
                    new InGridComboBoxVM
                    {
                        Text = Convert.ToString(paternityBalanceValid["dayoffname"])
                    });

            dayOffBalanceData.DayOffBrought = Convert.ToInt32(paternityBalanceValid["dayoffbrought"]);
            dayOffBalanceData.Unit = DayOffBalanceVM.GetUnitDefaultValue(
                    new InGridComboBoxVM
                    {
                        Text = Convert.ToString("Days")
                    });
            dayOffBalanceData.Balance = Convert.ToInt32(paternityBalanceValid["finalbalance"]);

            return dayOffBalanceData;
        }

        private DayOffNextBalanceVM GetDayOffNextBalancePaternity(string dayOffName, string professionalName)
        {
            var dayOffBalanceData = new DayOffNextBalanceVM();

            var camlDayOffBalanceData = @"<View>  
            <Query> 
               <Where><And><Eq><FieldRef Name='dayoffname' /><Value Type='Choice'>" + dayOffName + @"</Value></Eq><Eq><FieldRef Name='professional' /><Value Type='Lookup'>" + professionalName + @"</Value></Eq></And></Where> 
            </Query> 
      </View>";

            var paternityRecord = new List<int>();
            int paternityID;

            foreach (var paternityData in SPConnector.GetList(SP_DAYOFF_BAL_LIST_NAME, _siteUrl, camlDayOffBalanceData))
            {
                paternityID = Convert.ToInt32(paternityData["ID"]);

                paternityRecord.Add(paternityID);
            }

            int latestPaternityID = paternityRecord.LastOrDefault();

            var paternityBalanceValid = SPConnector.GetListItem(SP_DAYOFF_BAL_LIST_NAME, latestPaternityID, _siteUrl);

            dayOffBalanceData.DayOffType = DayOffNextBalanceVM.GetDayOffTypeDefaultValue(
                    new InGridComboBoxVM
                    {
                        Text = Convert.ToString(paternityBalanceValid["dayoffname"])
                    });

            dayOffBalanceData.DayOffBrought = Convert.ToInt32(paternityBalanceValid["dayoffbrought"]);
            dayOffBalanceData.Unit = DayOffNextBalanceVM.GetUnitDefaultValue(
                    new InGridComboBoxVM
                    {
                        Text = Convert.ToString("Days")
                    });
            dayOffBalanceData.Balance = Convert.ToInt32(paternityBalanceValid["finalbalance"]);

            return dayOffBalanceData;
        }

        //Digunakan
        private IEnumerable<DayOffRequestDetailVM> GetDayOffRequestsDetails(int? dayOffRequestHeaderID, string professionalName)
        {
            var DayOffRequestDetail = new List<DayOffRequestDetailVM>();

            if (dayOffRequestHeaderID != null)
            {
                var camlDayOffRequestDetail = @"<View>  
            <Query> 
               <Where><Eq><FieldRef Name='dayoffrequest' /><Value Type='Lookup'>" + dayOffRequestHeaderID + @"</Value></Eq></Where> 
            </Query></View>";
                
                foreach (var requestDetail in SPConnector.GetList(SP_DAYOFF_REQ_DETAIL_LIST_NAME, _siteUrl, camlDayOffRequestDetail))
                {
                    DayOffRequestDetail.Add(ConvertToDayOffRequestDetail(requestDetail));
                }
            }

            return DayOffRequestDetail;
        }

        //Digunakan
        private DayOffRequestDetailVM ConvertToDayOffRequestDetail(ListItem item)
        {
            var dayOffRequestDetail = new DayOffRequestDetailVM();
            
            dayOffRequestDetail.MasterDayOffType = FormatUtil.ConvertToInGridAjaxComboBox(item, "masterdayofftype");
            dayOffRequestDetail.FullHalf = DayOffRequestDetailVM.GetFullHalfDefaultValue(
                    new InGridComboBoxVM
                    {
                        Text = Convert.ToString(item["fullhalfday"])
                    });
            dayOffRequestDetail.RequestStartDate = Convert.ToDateTime(item["requeststartdate"]);
            dayOffRequestDetail.RequestEndDate = Convert.ToDateTime(item["requestenddate"]);
            dayOffRequestDetail.Remarks = Convert.ToString(item["remarks"]);
            return dayOffRequestDetail;
        }

        public DayOffRequestVM GetHeader(int? ID, string requstor)
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

        public DayOffBalanceVM GetCalculateBalance(int? ID, string siteUrl, string requestor, string listName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DayOffTypeMaster> GetDayOffType()
        {
            var dayOffTypeMaster = new List<DayOffTypeMaster>();
            
            foreach(var dayOffTypeMasterData in SPConnector.GetList(SP_MAS_DAYOFF_TYPE_LIST_NAME, _siteUrl, null))
            {
                dayOffTypeMaster.Add(ConvertToDayOffTypeMaster(dayOffTypeMasterData));
            }

            return dayOffTypeMaster;
        }

        public DayOffTypeMaster ConvertToDayOffTypeMaster(ListItem item)
        {
            var dayOffTypeMaster = new DayOffTypeMaster();

            dayOffTypeMaster.DayOffTypeID = Convert.ToInt32(item["ID"]);
            dayOffTypeMaster.DayOffTypeName = Convert.ToString(item["Title"]);

            return dayOffTypeMaster;
        }

        public int CreateDayOffRequestHeader(DayOffRequestVM dayOffRequest)
        {
            var updatedValues = new Dictionary<string, object>();
            var statusExitProcedure = "Pending Approval";

            updatedValues.Add("professional", new FieldLookupValue { LookupId = (int)dayOffRequest.ProfessionalID});
            updatedValues.Add("projectunit", dayOffRequest.ProjectUnit);
            updatedValues.Add("requestdate", dayOffRequest.RequestDate);
            updatedValues.Add("dayoffrequeststatus", dayOffRequest.StatusForm);

            if (dayOffRequest.StatusForm == "Draft")
            {
                statusExitProcedure = "Draft";

                var professionalData = SPConnector.GetListItem(SP_PRO_MAS_LIST_NAME, dayOffRequest.ProfessionalID, _siteUrl);
                string professionalOfficeMail = Convert.ToString(professionalData["officeemail"]);

                updatedValues.Add("visibleto", SPConnector.GetUser(professionalOfficeMail, _siteUrl, "hr"));
            }
            if (dayOffRequest.StatusForm == "Pending Approval")
            {
                statusExitProcedure = "Pending Approval";

                var professionalData = SPConnector.GetListItem(SP_PRO_MAS_LIST_NAME, dayOffRequest.ProfessionalID, _siteUrl);
                string professionalOfficeMail = Convert.ToString(professionalData["officeemail"]);

                updatedValues.Add("visibleto", SPConnector.GetUser(professionalOfficeMail, _siteUrl, "hr"));
            }
            //if (exitProcedure.StatusForm == "Saved by HR")
            //{
            //    statusExitProcedure = "Draft";

            //    var professionalData = SPConnector.GetListItem(SP_PROMAS_LIST_NAME, exitProcedure.ProfessionalID, _siteUrl);
            //    string professionalOfficeMail = Convert.ToString(professionalData["officeemail"]);

            //    updatedValues.Add("visibleto", SPConnector.GetUser(professionalOfficeMail, _siteUrl, "hr"));
            //}
            //if (exitProcedure.StatusForm == "Approved by HR")
            //{
            //    statusExitProcedure = "Approved";

            //    var professionalData = SPConnector.GetListItem(SP_PROMAS_LIST_NAME, exitProcedure.ProfessionalID, _siteUrl);
            //    string professionalOfficeMail = Convert.ToString(professionalData["officeemail"]);

            //    updatedValues.Add("visibleto", SPConnector.GetUser(professionalOfficeMail, _siteUrl, "hr"));
            //}

            //updatedValues.Add("status", statusExitProcedure);

            try
            {
                SPConnector.AddListItem(SP_DAYOFF_REQ_LIST_NAME, updatedValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                throw e;
            }

            return SPConnector.GetLatestListItemID(SP_DAYOFF_REQ_LIST_NAME, _siteUrl);
        }

        public int GetPositionID(string requestorposition, string requestorunit, int positionID, int number)
        {
            var caml = @"<View>  
            <Query> 
               <Where><And><Eq><FieldRef Name='Title' /><Value Type='Text'>" + requestorposition + @"</Value></Eq><Eq><FieldRef Name='projectunit' /><Value Type='Choice'>" + requestorunit + @"</Value></Eq></And></Where> 
            </Query> 
      </View>";

            //int positionID = 0;
            //int number = 0;

            foreach (var item in SPConnector.GetList(SP_POSITION_MAS_LIST_NAME, _siteUrl, caml))
            {
                if (item["ID"] != null)
                {
                    positionID = Convert.ToInt32(item["ID"]);
                    number = 1;
                    break;
                }
                else
                {
                    number = 0;
                }
            }

            if (number == 1)
            {
                return positionID;
            }
            else
            {
                return 0;
            }

        }

        public async Task CreateDayOffRequestDetailAsync(DayOffRequestVM dayOffRequest, int? dayOffRequestHeaderID, IEnumerable<DayOffRequestDetailVM> dayOffRequestDetail, string requestorposition, string requestorunit, int? positionID)
        {
            CreateDayOffRequestDetail(dayOffRequest, dayOffRequestHeaderID, dayOffRequestDetail, requestorposition, requestorunit, positionID);
        }

        public void CreateDayOffRequestDetail(DayOffRequestVM dayOffRequest, int? dayOffRequestHeaderID, IEnumerable<DayOffRequestDetailVM> dayOffRequestDetail, string requestorposition, string requestorunit, int? positionID)
        {
            double totalDays;
            DateTime returnToWork;

            foreach (var viewModel in dayOffRequestDetail)
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

                updatedValue.Add("masterdayofftype", new FieldLookupValue { LookupId = Convert.ToInt32(viewModel.MasterDayOffType.Value) });
                updatedValue.Add("requeststartdate", viewModel.RequestStartDate);
                updatedValue.Add("requestenddate", viewModel.RequestEndDate);
                updatedValue.Add("fullhalfday", viewModel.FullHalf.Text);
                updatedValue.Add("remarks", viewModel.Remarks);
                updatedValue.Add("dayoffrequest", new FieldLookupValue { LookupId = Convert.ToInt32(dayOffRequestHeaderID) });

                totalDays = CalculcateTotalDays(Convert.ToDateTime(viewModel.RequestStartDate), Convert.ToDateTime(viewModel.RequestEndDate), Convert.ToString(viewModel.FullHalf.Text));

                updatedValue.Add("totaldays", Convert.ToDouble(totalDays));

                returnToWork = GetDateReturnToWork(Convert.ToDateTime(viewModel.RequestStartDate), Convert.ToDateTime(viewModel.RequestEndDate), totalDays);

                updatedValue.Add("returntowork", returnToWork);
                
                try
                {
                    if (Item.CheckIfUpdated(viewModel))
                        SPConnector.UpdateListItem(SP_DAYOFF_REQ_DETAIL_LIST_NAME, viewModel.ID, updatedValue, _siteUrl);
                    else
                    {
                        SPConnector.AddListItem(SP_DAYOFF_REQ_DETAIL_LIST_NAME, updatedValue, _siteUrl);
                    }

                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw new Exception(ErrorResource.SPInsertError);
                }

                ////i++;
            }
        }

        private double CalculcateTotalDays(DateTime requestStartDate, DateTime requestEndDate, string fullHalfDay)
        {
            double numberDays = 0;

            string strRequestStartDate = requestStartDate.ToShortDateString();
            string strRequestEndDate = requestEndDate.ToShortDateString();

            if(strRequestEndDate == strRequestStartDate)
            {
                if (fullHalfDay == "Full Day")
                {
                    numberDays = Convert.ToInt32((requestEndDate - requestStartDate).TotalDays) + 1;
                }
                else
                {
                    numberDays = 0.5;
                }
            }
            else
            {
                if(requestStartDate < requestEndDate)
                {
                    TimeSpan span = requestEndDate - requestStartDate;

                    numberDays = (span.Days) + 1;
                }
            }
            
            return numberDays;
        }

        private DateTime GetDateReturnToWork(DateTime requestStartDate, DateTime requestEndDate, double totalDays)
        {
            DateTime returnToWork = DateTime.Now;
            bool publicHoliday = true;

            if (totalDays == 0.5)
            {
                returnToWork = requestEndDate.ToLocalTime();
            }
            else
            {
                returnToWork = requestEndDate.AddDays(1);

                if (returnToWork.DayOfWeek == DayOfWeek.Saturday)
                {
                    returnToWork = returnToWork.AddDays(2);

                    if ((returnToWork.DayOfWeek == DayOfWeek.Monday)|| (returnToWork.DayOfWeek == DayOfWeek.Tuesday) || (returnToWork.DayOfWeek == DayOfWeek.Wednesday) || (returnToWork.DayOfWeek == DayOfWeek.Thursday) || (returnToWork.DayOfWeek == DayOfWeek.Friday))
                    {
                        publicHoliday = checkPublicHoliday(returnToWork);

                        if(publicHoliday == false)
                        {
                            return returnToWork;
                        }
                        else
                        {
                            returnToWork = returnToWork.AddDays(1);
                            return returnToWork;
                        }
                    }
                }
                else if(returnToWork.DayOfWeek == DayOfWeek.Sunday)
                {
                    returnToWork = returnToWork.AddDays(1);

                    if ((returnToWork.DayOfWeek == DayOfWeek.Monday) || (returnToWork.DayOfWeek == DayOfWeek.Tuesday) || (returnToWork.DayOfWeek == DayOfWeek.Wednesday) || (returnToWork.DayOfWeek == DayOfWeek.Thursday) || (returnToWork.DayOfWeek == DayOfWeek.Friday))
                    {
                        publicHoliday = checkPublicHoliday(returnToWork);

                        if (publicHoliday == false)
                        {
                            return returnToWork;
                        }
                        else
                        {
                            returnToWork = returnToWork.AddDays(1);
                            return returnToWork;
                        }
                    }
                }
                else
                {
                    publicHoliday = checkPublicHoliday(returnToWork);

                    if(publicHoliday == true)
                    {
                        returnToWork = returnToWork.AddDays(1);
                        return returnToWork;
                    }
                    else
                    {
                        return returnToWork;
                    }
                }
            }

            return returnToWork;
        }

        private bool checkPublicHoliday(DateTime returnToWork)
        {
            int year = 2016;
            string strReturnToWork = returnToWork.ToShortDateString();

            DateTime tahunBaru = new DateTime(year, 1, 1);
            string strTahunBaru = tahunBaru.ToShortDateString();

            DateTime imlek = new DateTime(year, 2, 8);
            string strImlek = imlek.ToShortDateString();

            DateTime nyepi = new DateTime(year, 3, 9);
            string strNyepi = nyepi.ToShortDateString();

            DateTime wafatIsaAlMasih = new DateTime(year, 3, 25);
            string strWafatIsaAlmasih = wafatIsaAlMasih.ToShortDateString();

            DateTime hariBuruh = new DateTime(year, 5, 1);
            string strHariBuruh = hariBuruh.ToShortDateString();

            DateTime kenaikanYesus = new DateTime(year, 5, 5);
            string strKenaikanYesus = kenaikanYesus.ToShortDateString();

            DateTime israMiraj = new DateTime(year, 5, 6);
            string strIsraMiraj = israMiraj.ToShortDateString();

            DateTime waisak = new DateTime(year, 5, 22);
            string strWaisak = waisak.ToShortDateString();

            DateTime idulfitri = new DateTime(year, 7, 7);
            string strIdulFitri = idulfitri.ToShortDateString();

            DateTime hutRI = new DateTime(year, 8, 17);
            string strHutRI = hutRI.ToShortDateString();

            DateTime idulAdha = new DateTime(year, 9, 12);
            string strIdulAdha = idulAdha.ToShortDateString();

            DateTime tahunBaruIslam = new DateTime(year, 10, 2);
            string strTahunBaruIslam = tahunBaruIslam.ToShortDateString();

            DateTime maulidNabi = new DateTime(year, 12, 12);
            string strMaulidNabi = maulidNabi.ToShortDateString();

            DateTime natal = new DateTime(year, 12, 25);
            string strNatal = natal.ToShortDateString();

            if(strReturnToWork == strTahunBaru)
            {
                return true;
            }
            else if(strReturnToWork == strImlek)
            {
                return true;
            }
            else if(strReturnToWork == strNyepi)
            {
                return true;
            }
            else if(strReturnToWork == strWafatIsaAlmasih)
            {
                return true;
            }
            else if(strReturnToWork == strHariBuruh)
            {
                return true;
            }
            else if(strReturnToWork == strKenaikanYesus)
            {
                return true;
            }
            else if(strReturnToWork == strIsraMiraj)
            {
                return true;
            }
            else if(strReturnToWork == strWaisak)
            {
                return true;
            }
            else if(strReturnToWork == strIdulFitri)
            {
                return true;
            }
            else if(strReturnToWork == strHutRI)
            {
                return true;
            }
            else if(strReturnToWork == strIdulAdha)
            {
                return true;
            }
            else if(strReturnToWork == strTahunBaruIslam)
            {
                return true;
            }
            else if(strReturnToWork == strMaulidNabi)
            {
                return true;
            }
            else if(strReturnToWork == strNatal)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //public int CreateExitProcedure(ExitProcedureVM exitProcedure)
        //{
        //    var updatedValues = new Dictionary<string, object>();
        //    var statusExitProcedure = "Pending Approval";

        //    updatedValues.Add("Title", exitProcedure.FullName);
        //    updatedValues.Add("requestdate", exitProcedure.RequestDate);
        //    updatedValues.Add("professional", new FieldLookupValue { LookupId = (int)exitProcedure.ProfessionalID });
        //    updatedValues.Add("projectunit", exitProcedure.ProjectUnit);
        //    updatedValues.Add("position", exitProcedure.Position);
        //    updatedValues.Add("joindate", exitProcedure.ProfessionalJoinDate);
        //    updatedValues.Add("mobilenumber", exitProcedure.PhoneNumber);
        //    updatedValues.Add("officeemail", exitProcedure.ProfessionalPersonalMail);
        //    updatedValues.Add("currentaddress", exitProcedure.CurrentAddress);
        //    updatedValues.Add("lastworkingdate", exitProcedure.LastWorkingDate);
        //    updatedValues.Add("exitreason", exitProcedure.ExitReason.Value);
        //    updatedValues.Add("reasondescription", exitProcedure.ReasonDesc);
        //    updatedValues.Add("psanumber", exitProcedure.PSANumber);

        //    if (exitProcedure.StatusForm == "Draft")
        //    {
        //        statusExitProcedure = "Draft";

        //        var professionalData = SPConnector.GetListItem(SP_PROMAS_LIST_NAME, exitProcedure.ProfessionalID, _siteUrl);
        //        string professionalOfficeMail = Convert.ToString(professionalData["officeemail"]);

        //        updatedValues.Add("visibleto", SPConnector.GetUser(professionalOfficeMail, _siteUrl, "hr"));
        //    }
        //    if (exitProcedure.StatusForm == "Pending Approval")
        //    {
        //        statusExitProcedure = "Pending Approval";

        //        var professionalData = SPConnector.GetListItem(SP_PROMAS_LIST_NAME, exitProcedure.ProfessionalID, _siteUrl);
        //        string professionalOfficeMail = Convert.ToString(professionalData["officeemail"]);

        //        updatedValues.Add("visibleto", SPConnector.GetUser(professionalOfficeMail, _siteUrl, "hr"));
        //    }
        //    if (exitProcedure.StatusForm == "Saved by HR")
        //    {
        //        statusExitProcedure = "Draft";

        //        var professionalData = SPConnector.GetListItem(SP_PROMAS_LIST_NAME, exitProcedure.ProfessionalID, _siteUrl);
        //        string professionalOfficeMail = Convert.ToString(professionalData["officeemail"]);

        //        updatedValues.Add("visibleto", SPConnector.GetUser(professionalOfficeMail, _siteUrl, "hr"));
        //    }
        //    if (exitProcedure.StatusForm == "Approved by HR")
        //    {
        //        statusExitProcedure = "Approved";

        //        var professionalData = SPConnector.GetListItem(SP_PROMAS_LIST_NAME, exitProcedure.ProfessionalID, _siteUrl);
        //        string professionalOfficeMail = Convert.ToString(professionalData["officeemail"]);

        //        updatedValues.Add("visibleto", SPConnector.GetUser(professionalOfficeMail, _siteUrl, "hr"));
        //    }

        //    updatedValues.Add("status", statusExitProcedure);

        //    try
        //    {
        //        SPConnector.AddListItem(SP_EXP_LIST_NAME, updatedValues, _siteUrl);
        //    }
        //    catch (Exception e)
        //    {
        //        logger.Error(e.Message);
        //        throw e;
        //    }

        //    return SPConnector.GetLatestListItemID(SP_EXP_LIST_NAME, _siteUrl);
        //}
    }
}

