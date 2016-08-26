using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.Utils;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.HR.DataMaster;
using MCAWebAndAPI.Service.HR.Common;
using Microsoft.SharePoint.Client;
using NLog;
using View = System.Web.UI.WebControls.View;

namespace MCAWebAndAPI.Service.HR.Timesheet
{
    public class TimesheetService : ITimesheetService
    {
        string _siteUrl;
        const string TYPE_PUB_HOLIDAY = "Public Holiday";
        const string TYPE_HOLIDAY = "Holiday";
        const string TYPE_DAYOFF = "Day-Off";
        const string TYPE_COMP_LEAVE = "Compensatory Leave";
        const string LIST_PUB_HOLIDAY = "Event Calendar";
        const string LIST_DAY_OFF = "Day-Off Request";
        const string LIST_DAY_OFF_DETAIL = "Day-Off Request Detail";
        const string LIST_COMPEN = "Compensatory Request";
        const string LIST_COMPEN_DETAIL = "Compensatory Request Detail";

        const string LIST_TIME = "Timesheet";
        const string LIST_TIME_DETAIL = "Timesheet Detail";

        const string LIST_WF = "Timesheet Workflow";
        const string LIST_WF_MAPPING = "Workflow Mapping Master";

        const string LIST_PROFESSIONAL= "Professional Master";


        //Compensatory Request
        private IDataMasterService _dataService;
        private IProfessionalService _professionalService;
        static Logger logger = LogManager.GetCurrentClassLogger();
        public TimesheetService()
        {
            _dataService = new DataMasterService();
            _professionalService = new ProfessionalService();
        }

        //public async Task GetTimesheetLoadUpdateAsync(int? id, string userlogin)
        //{
        //    GetTimesheetLoadUpdate(id, userlogin);
        //}

        public TimesheetVM GetFormType(int? id, string userlogin)
        {
            var viewModel = new TimesheetVM();
            var listItem = SPConnector.GetListItem(LIST_TIME, id, _siteUrl);
            viewModel.ProfessionalID = FormatUtil.ConvertLookupToID(listItem, "professional");
            viewModel.UserLogin = userlogin;
            viewModel.ProjectUnit = GetProjectUnitName(userlogin);

            return viewModel;
        }

        public TimesheetVM GetTimesheetLoadUpdate(int? id,string userlogin)
        {

            var viewModel = new TimesheetVM();

            var professionalDataEmail = GetProfessionalDataByEmail(userlogin);

            var listItem = SPConnector.GetListItem(LIST_TIME, id, _siteUrl);
            var professionalDataId = GetProfessionalDataById(FormatUtil.ConvertLookupToID(listItem, "professional"));

            //FieldUserValue userValue =(FieldUserValue)listItem["visibleto"];
            viewModel.ID = id;
            viewModel.ProfessionalID = FormatUtil.ConvertLookupToID(listItem, "professional");
            viewModel.UserLogin = professionalDataId.OfficeEmail;
            viewModel.Name= FormatUtil.ConvertLookupToValue(listItem, "professional");
            viewModel.Period = Convert.ToDateTime(listItem["DatePeriod"]);
            viewModel.ProjectUnit = professionalDataId.Project_Unit;
            viewModel.ProfessionalName.Value = viewModel.ProfessionalID;
            viewModel.ProfessionalName.Text = viewModel.Name;
            viewModel.ApprovalLevel = Convert.ToString(listItem["approvallevel"]);
            viewModel.ApproverPosition = Convert.ToString(listItem["approverposition"]);
            viewModel.Approver = Convert.ToString(listItem["approver"]);

            if (professionalDataEmail.Project_Unit == "Human Resources Unit")
            {
                viewModel.UserPermission = "HR";
            }
            else if (professionalDataEmail.OfficeEmail == viewModel.Approver)
            {
                viewModel.UserPermission = "Approver";
            }
            else if (viewModel.UserLogin.ToLower() == userlogin.ToLower())
            {
                viewModel.UserPermission = "Professional";
            }
            else
            {
                viewModel.UserPermission = "Not Authorized";
            }

            if (viewModel.UserPermission != "Not Authorized")
            {
                viewModel.TimesheetDetails = GetTimesheetDetailsLoadUpdate(id, Convert.ToDateTime(viewModel.Period), userlogin);
            }

           

            return viewModel;
        }

        //public async Task GetTimesheetDetailsLoadUpdateAsync(string userlogin, DateTime period)
        //{
        //    GetTimesheetDetailsLoadUpdate(userlogin, period);
        //}
        public IEnumerable<TimesheetDetailVM> GetTimesheetDetailsLoadUpdate(int? id, DateTime period, string userlogin)
        {

            var timesheetDetails = new List<TimesheetDetailVM>();

            string caml = @"<View><Query><Where>
                          <Eq><FieldRef Name='timesheet' />
                          <Value Type='Lookup'>" + id+"</Value></Eq>" +
                          "</Where></Query>" +
                         "</View>";
       
            foreach (var item in SPConnector.GetList(LIST_TIME_DETAIL, _siteUrl, caml))
            {
                timesheetDetails.Add(new TimesheetDetailVM
                {
                    Date = Convert.ToDateTime(item["timesheetdetaildate"]),
                    FullHalf = Convert.ToDouble(item["FullHalf"]),
                    Location = FormatUtil.ConvertLookupToValue(item, "location"),
                    LocationID = FormatUtil.ConvertLookupToID(item, "location_x003a_ID"),
                    Type = Convert.ToString(item["Title"])
                });
            }

             var startDate = period;
            var finishDate = period.GetLastPayrollDay();
            var dateRange = startDate.EachDay(finishDate);

            var listHoliday = getPublicHoliday( _siteUrl);
            var listDayOff = getUserDayOFF(_siteUrl,GetFullName(userlogin));
            var listCompen = getCompensatory(_siteUrl, GetFullName(userlogin));

            foreach (var item in dateRange)
            {
                if (IsDate(item, listHoliday))
                {
                    timesheetDetails.Add(new TimesheetDetailVM
                    {
                        Date = item,
                        FullHalf = 1,
                        //Status = TYPE_PUB_HOLIDAY,
                        Type = TYPE_PUB_HOLIDAY
                    });
                }
                else if (item.DayOfWeek == DayOfWeek.Saturday || item.DayOfWeek == DayOfWeek.Sunday)
                    timesheetDetails.Add(new TimesheetDetailVM
                    {
                        Date = item,
                        FullHalf = 1,
                        //Status = TYPE_HOLIDAY,
                        Type = TYPE_HOLIDAY
                    });
                else if (IsDate(item, listDayOff))
                {
                    timesheetDetails.Add(new TimesheetDetailVM
                    {
                        Date = item,
                        FullHalf = 1,
                        //Status = TYPE_DAYOFF,
                        Type = TYPE_DAYOFF
                    });
                }
                else if (IsDate(item, listCompen))
                {
                    timesheetDetails.Add(new TimesheetDetailVM
                    {
                        Date = item,
                        FullHalf = 1,
                        //Status = TYPE_COMP_LEAVE,
                        Type = TYPE_COMP_LEAVE
                    });
                }
            }


            return timesheetDetails;
        }

        public TimesheetVM GetTimesheet(string userlogin, DateTime period)
        {

            var viewModel = new TimesheetVM
            {
                ProfessionalID= GetProfessionalID(userlogin),
                UserLogin = userlogin,
                Name = GetFullName(userlogin),
                Period = period,
                ProjectUnit = GetProjectUnitName(userlogin),
                TimesheetDetails = GetTimesheetDetails(userlogin, period)
            };
            viewModel.ProfessionalName.Value = viewModel.ProfessionalID;
            viewModel.ProfessionalName.Text = viewModel.Name;
            viewModel.UserPermission  = viewModel.ProjectUnit == "Human Resources Unit" ? "HR" : "Professional";

            return viewModel;
        }


        private ProfessionalMaster GetProfessionalDataByEmail(string userlogin)
        {
            var professionalData = _dataService.GetProfessionals().FirstOrDefault(e => e.OfficeEmail == userlogin);


            return professionalData;
        }

        private ProfessionalMaster GetProfessionalDataById(int? id)
        {
            var professionalData = _dataService.GetProfessionals().FirstOrDefault(e => e.ID == id);


            return professionalData;
        }

        private string GetProjectUnitName(string userlogin)
        {
            var professionalData = _dataService.GetProfessionals().FirstOrDefault(e => e.OfficeEmail == userlogin);
            return professionalData.Project_Unit;
        }

        private int? GetProfessionalID(string userlogin)
        {
            var professionalData = _dataService.GetProfessionals().FirstOrDefault(e => e.OfficeEmail == userlogin);
            return professionalData.ID;
        }

        private string GetFullName(string userlogin)
        {
            var professionalData = _dataService.GetProfessionals().FirstOrDefault(e => e.OfficeEmail == userlogin);
            return professionalData.Name;
        }

        private static List<DateTime> getPublicHoliday(string strUrl)
        {
            var listItem = SPConnector.GetList(LIST_PUB_HOLIDAY, strUrl);
            List<DateTime> lstpublicHoliday = new List<DateTime>();
            foreach (var item in listItem)
            {
                lstpublicHoliday.Add(Convert.ToDateTime(item["EventDate"]).ToLocalTime());
            }

            return lstpublicHoliday;
 
        }

        private static List<DateTime> getCompensatory(string strUrl, string strName)
        {
            List<DateTime> lstCompensatory = new List<DateTime>();
            var caml = @"<View><Query><Where><And><Eq><FieldRef Name='professional' />
                        <Value Type='Lookup'>" + strName +"</Value></Eq>" +
                       "<Eq><FieldRef Name='crstatus' /><Value Type='Text'>Approved</Value></Eq>" +
                       "</And></Where></Query></View>";
            var listMaster = SPConnector.GetList(LIST_COMPEN, strUrl, caml);
           
            foreach (var item in listMaster)
            {
                caml = @"<View><Query><Where><Eq><FieldRef Name='compensatoryrequest' />
                        <Value Type='Lookup'>" + Convert.ToString(item["ID"]) +
                        "</Value></Eq></Where></Query></View>";
                var listDetail = SPConnector.GetList(LIST_COMPEN_DETAIL, strUrl, caml);

                foreach (var itemDetail in listDetail)
                {
                    var startdate = Convert.ToDateTime(itemDetail["compensatorystarttime"]).ToLocalTime();
                    var finishdate = Convert.ToDateTime(itemDetail["compensatoryendtime"]).ToLocalTime();
                    var dateRange = startdate.EachDay(finishdate);
                    lstCompensatory.AddRange(dateRange);
                }

            }
            return lstCompensatory;
        }

        private static List<DateTime> getUserDayOFF(string strUrl,string strName)
        {
            var caml = @"<View><Query><Where><Eq><FieldRef Name='professional' /><Value Type='Lookup'>" + strName +
               "</Value></Eq></Where></Query></View>";
            var listMaster = SPConnector.GetList(LIST_DAY_OFF, strUrl, caml);
            List<DateTime> lstDayOff = new List<DateTime>();
            foreach (var item in listMaster)
            {
                caml = @"<View><Query><Where><And><Eq><FieldRef Name='dayoffrequest' />
                        <Value Type='Lookup'>" + Convert.ToString(item["ID"] ) +
                        "</Value></Eq>" +
                        "<Eq><FieldRef Name='approvalstatus' />" +
                        "<Value Type='Text'>Approved</Value></Eq></And></Where></Query></View>";
                var listDetail = SPConnector.GetList(LIST_DAY_OFF_DETAIL, strUrl, caml);

                foreach (var itemDetail in listDetail)
                {
                    var startdate = Convert.ToDateTime(itemDetail["requeststartdate"]).ToLocalTime();
                    var finishdate = Convert.ToDateTime(itemDetail["requestenddate"]).ToLocalTime();
                    var dateRange = startdate.EachDay(finishdate);
                    lstDayOff.AddRange(dateRange);
                }
             
            }
            return lstDayOff;
        }


        public IEnumerable<TimesheetDetailVM> GetTimesheetDetails(string userlogin, DateTime period)
        {
            var startDate = period;
            var finishDate = period.GetLastPayrollDay();
            var dateRange = startDate.EachDay(finishDate);

            var listHoliday = getPublicHoliday( _siteUrl);
            var listDayOff = getUserDayOFF(_siteUrl,GetFullName(userlogin));
            var listCompen = getCompensatory(_siteUrl, GetFullName(userlogin));

            var timesheetDetails = new List<TimesheetDetailVM>();

            foreach (var item in dateRange)
            {
                if (IsDate(item, listHoliday))
                {
                    timesheetDetails.Add(new TimesheetDetailVM
                    {
                        Date = item,
                        FullHalf = 1,
                        //Status = TYPE_PUB_HOLIDAY,
                        Type = TYPE_PUB_HOLIDAY
                    });
                }
                else if (item.DayOfWeek == DayOfWeek.Saturday || item.DayOfWeek == DayOfWeek.Sunday)
                    timesheetDetails.Add(new TimesheetDetailVM
                    {
                        Date = item,
                        FullHalf = 1,
                        //Status = TYPE_HOLIDAY,
                        Type = TYPE_HOLIDAY
                    });
                else if (IsDate(item, listDayOff))
                {
                    timesheetDetails.Add(new TimesheetDetailVM
                    {
                        Date = item,
                        FullHalf = 1,
                        //Status = TYPE_DAYOFF,
                        Type = TYPE_DAYOFF
                    });
                }else if (IsDate(item, listCompen))
                {
                    timesheetDetails.Add(new TimesheetDetailVM
                    {
                        Date = item,
                        FullHalf = 1,
                        //Status = TYPE_COMP_LEAVE,
                        Type = TYPE_COMP_LEAVE
                    });
                }
            }

            return timesheetDetails;
        }

       

        //private bool IsCompLeave(DateTime item)
        //{
        //    //TODO: To get from SP list
        //    return (item.Day % 17 == 0);
        //}

        //private bool IsDayOff(DateTime date, List<DateTime> lstRange)
        //{
        //    var bcek = false;
        //    if (lstRange == null) return false;
        //    else
        //    {
        //        if (lstRange.Any(item => date.ToString("yy-MM-dd") == item.ToString("yy-MM-dd")))
        //        {
        //            return true;
        //        }
        //    }

        //    return bcek;
        //}

        private bool IsDate(DateTime date,List<DateTime> lstRange )
        {
            var bcek = false;
            if (lstRange == null) return false;
            else
            {
                if (lstRange.Any(item => date.ToString("yy-MM-dd") == item.ToString("yy-MM-dd")))
                {
                    return true;
                }
            }

            return bcek;
        }

        public void SetSiteUrl(string siteUrl = null)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
            _dataService.SetSiteUrl(_siteUrl);
            _professionalService.SetSiteUrl(_siteUrl);
        }

        public IEnumerable<TimesheetDetailVM> AppendWorkingDays(IEnumerable<TimesheetDetailVM> currentDays, DateTime from,
            DateTime to, bool isFullDay, string location = null, int? locationid = null)
        {
            var dateRange = from.EachDay(to);
            var existingDays = currentDays.Select(e => (DateTime)e.Date).ToList();
            var allDays = currentDays.ToList();

            foreach (var workingDay in dateRange)
            {
                if (!existingDays.ContainsSameDay(workingDay))
                {
                    if (workingDay.DayOfWeek != DayOfWeek.Saturday && workingDay.DayOfWeek != DayOfWeek.Sunday)
                    {
                        allDays.Add(new TimesheetDetailVM
                        {
                            Date = workingDay,
                            FullHalf = isFullDay ? 1.0d : 0.5d,
                            Location = location ?? string.Empty,
                            LocationID = locationid ?? null,
                            Type = "Working Days"
                        });
                    }

                }
            }

            return allDays;
        }


        public async Task CreateTimesheetDetailsAsync(int? headerID, IEnumerable<TimesheetDetailVM> timesheetDetails)
        {
            CreateTimesheetDetails(headerID, timesheetDetails);
        }

        public void CreateTimesheetDetails(int? headerId, 
            IEnumerable<TimesheetDetailVM> timesheetDetails)
        {
            var mastervalue = new Dictionary<string, Dictionary<string, object>>();
            var i = 1;
            foreach (var viewModel in timesheetDetails)
            {
                if (viewModel.Type != "Working Days") continue;
                var updatedValue = new Dictionary<string, object>
                {
                    {"timesheet", new FieldLookupValue {LookupId = Convert.ToInt32(headerId)}},
                   {"timesheetdetaildate", viewModel.Date},
                    {"FullHalf", viewModel.FullHalf}
                };

                if (!string.IsNullOrEmpty(viewModel.Location))
                {
                    updatedValue.Add("location", new FieldLookupValue { LookupId = Convert.ToInt32(viewModel.LocationID) });
                    updatedValue.Add("Title", "Working Days");
                }
                else
                {
                    updatedValue.Add("Title", viewModel.Type);
                }

               
                    mastervalue.Add(i.ToString(), updatedValue);
                    i++;
               
            }

            try
            {
                SPConnector.AddListItemAsync(LIST_TIME_DETAIL, mastervalue, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                //throw new Exception(ErrorResource.SPInsertError);
                throw new Exception(e.Message);
            }
        }

        public async Task CreateWorkflowTimesheetAsync(int? headerId, TimesheetVM header)
        {
            if (header.TimesheetStatus == "Draft") return;
            CreateWorkflowTimesheet(headerId, header);
        }

        private DataTable Getworkflowmapping(string strProjectUnit)
        {
          

            var dtView = new DataTable();
            dtView.Columns.Add("ApproverUnit", typeof(string));
            dtView.Columns.Add("ApproverPosition", typeof(string));
            dtView.Columns.Add("Level", typeof(int));
            dtView.Columns.Add("IsDefault", typeof(string));


            var caml = @"<View><Query>
                        <Where>
                        <And>
                        <Eq>
                            <FieldRef Name='transactiontype' />
                            <Value Type='Choice'>Timesheet</Value>
                        </Eq>
                        <Eq>
                            <FieldRef Name='requestorunit' />
                             <Value Type='Choice'>" + strProjectUnit + "</Value>" +
                       "</Eq>" +
                       "</And></Where></Query></View>";

            var listcoll = SPConnector.GetList(LIST_WF_MAPPING, _siteUrl, caml);

            if (listcoll == null || listcoll.Count == 0) return null;

            foreach (var item in listcoll)
            {
                DataRow row = dtView.NewRow();
                row["ApproverUnit"] = Convert.ToString(item["approverunit"]);
                row["ApproverPosition"] = FormatUtil.ConvertLookupToValue(item, "approverposition");
                row["Level"] = Convert.ToString(item["approverlevel"]);
                row["IsDefault"] = Convert.ToString(item["isdefault"]);
                dtView.Rows.Add(row);
            }
            dtView.DefaultView.Sort = "Level ASC";

            dtView.DefaultView.RowFilter = "IsDefault='Yes'";

            if (dtView.DefaultView == null || dtView.DefaultView.Count == 0) return null;

            return dtView.DefaultView.ToTable();
        }

        private void UpdateHeaderApprover(int? headerId,string strLevel,string strPosition, string strEmail)
        {
            var columnValues = new Dictionary<string, object>
                {
                    {"approverposition", strPosition},
                    {"approvallevel", strLevel},
                     {"approver", strEmail},
                };
            SPConnector.UpdateListItem(LIST_TIME, headerId, columnValues, _siteUrl);
        }
        public void CreateWorkflowTimesheet(int? headerId, TimesheetVM header)
        {
            var dtView = Getworkflowmapping( header.ProjectUnit);
            var iCount = dtView.DefaultView.Count ;
            var profMasterPosition = _dataService.GetProfessionals();
            for (int i = 1; i <= iCount; i++)
            {
                var strApproverPosition = "";
                var strApproverEmail = "";
                var columnValues = new Dictionary<string, object>
                {
                    {"Title", header.UserLogin},
                    {"timesheet", new FieldLookupValue {LookupId = Convert.ToInt32(headerId)}},
                    {"status", "Pending Approval"},
                    {"approverlevel", i.ToString()}
                };

                switch (i)
                {
                    case 1:
                        dtView.DefaultView.RowFilter = "Level = 1 And IsDefault='Yes'";
                        if (dtView.DefaultView != null && dtView.DefaultView.Count > 0)
                        {
                            strApproverPosition = Convert.ToString(dtView.DefaultView[0]["ApproverPosition"]);
                            strApproverEmail = profMasterPosition.FirstOrDefault(e => e.Position == strApproverPosition).OfficeEmail;
                        }
                        else
                        {
                            goto case 2;
                        }
                        UpdateHeaderApprover(headerId, "1", strApproverPosition, strApproverEmail);
                        columnValues.Add("currentstate", "Yes");
                        columnValues.Add("approverposition", strApproverPosition);
                        columnValues.Add("approver0", strApproverEmail);
                        break;
                    case 2:
                        dtView.DefaultView.RowFilter = "Level = 2 And IsDefault='Yes'";
                        if (dtView.DefaultView != null && dtView.DefaultView.Count > 0)
                        {
                            strApproverPosition = Convert.ToString(dtView.DefaultView[0]["ApproverPosition"]);
                            strApproverEmail = profMasterPosition.FirstOrDefault(e => e.Position == strApproverPosition).OfficeEmail;
                        }
                        else
                        {
                            goto case 3;
                        }
                        if (i==1) UpdateHeaderApprover(headerId, "1", strApproverPosition, strApproverEmail);
                        columnValues.Add("currentstate", i == 2 ? "No" : "Yes");
                        columnValues.Add("approverposition", strApproverPosition);
                        columnValues.Add("approver0", strApproverEmail);
                        break;
                    case 3:
                        dtView.DefaultView.RowFilter = "Level = 3 And IsDefault='Yes'";
                        if (dtView.DefaultView != null && dtView.DefaultView.Count > 0)
                        {
                            strApproverPosition = Convert.ToString(dtView.DefaultView[0]["ApproverPosition"]);
                            strApproverEmail = profMasterPosition.FirstOrDefault(e => e.Position == strApproverPosition).OfficeEmail;
                            if (i == 1) UpdateHeaderApprover(headerId, "1", strApproverPosition, strApproverEmail);
                            columnValues.Add("currentstate", i == 3 ? "No" : "Yes");
                            columnValues.Add("approverposition", strApproverPosition);
                            columnValues.Add("approver0", strApproverEmail);
                        }
                        break;
                }

               if (!string.IsNullOrEmpty(strApproverPosition)) SPConnector.AddListItem(LIST_WF, columnValues, _siteUrl);
            }
        }


        //private string getLevelNext(string level)
        //{
        //    var str = "";

        //    if 

        //    return str;
        //}
        public void UpdateApproval( TimesheetVM header)
        {
           var caml = string.Format(@"<View><Query>
                        <Where>
                        <Eq>
                            <FieldRef Name='timesheet' />
                            <Value Type='Lookup'>{0}</Value>
                        </Eq>
                       </Where></Query></View>", header.ID);

            var listcoll = SPConnector.GetList(LIST_WF, _siteUrl, caml);

            var strNextLevel = header.ApprovalLevel == "3" ? header.ApprovalLevel : (Convert.ToInt32(header.ApprovalLevel) + 1).ToString();
            var listitemNext= listcoll.FirstOrDefault(e =>Convert.ToString(e["approverlevel"]) == strNextLevel);

            var strStatus = "";
            if (header.TimesheetStatus == "Rejected")
            {
                strStatus = header.TimesheetStatus;
            }
            else if (header.TimesheetStatus == "Approved" && header.ApprovalLevel != "3")
            {
                strStatus = "Pending Approval";
            }
            else if (header.TimesheetStatus == "Approved" && header.ApprovalLevel == "3")
            {
                strStatus = "Approved";
            }
            var columnValues = new Dictionary<string, object>
                {
                    {"timesheetstatus", strStatus},
                    {"approvallevel", strNextLevel}
                };

            if (header.TimesheetStatus != "Rejected")
            {

                if (listitemNext != null)
                {
                    columnValues.Add("approverposition", Convert.ToString(listitemNext["approverposition"]));
                    columnValues.Add("approver", Convert.ToString(listitemNext["approver0"]));
                }

            }

            SPConnector.UpdateListItem(LIST_TIME, header.ID, columnValues, _siteUrl);


            if (listitemNext == null) return;
            var idNext = Convert.ToInt32(listitemNext["ID"]);



            var columnWfValues = new Dictionary<string, object> ();

            if (header.TimesheetStatus != "Rejected") columnWfValues.Add("currentstate", "Yes");
            if (header.ApprovalLevel=="3") columnWfValues.Add("status", header.TimesheetStatus);


            SPConnector.UpdateListItem(LIST_WF, idNext, columnWfValues, _siteUrl);

            columnWfValues = new Dictionary<string, object>();
            columnWfValues.Add("currentstate", "No");
            if (header.ApprovalLevel != "3") columnWfValues.Add("status", header.TimesheetStatus);
            SPConnector.UpdateListItem(LIST_WF, (idNext - 1), columnWfValues, _siteUrl);

        }


        public int CreateHeader(TimesheetVM header)
        {
            var dtView = Getworkflowmapping(header.ProjectUnit);

            if (dtView == null || dtView.Rows.Count == 0) throw new Exception("Please check Workflow Mapping Master");

            int ID = 0;
            var columnValues = new Dictionary<string, object>
           {
               {"timesheetstatus", header.TimesheetStatus}
           };

            var strPeriod = Convert.ToDateTime(header.Period).ToString("MM") + "-" + Convert.ToDateTime(header.Period).ToString("yyyy");

            columnValues.Add("Title", strPeriod);
            columnValues.Add("DatePeriod", header.Period);

            if (header.ProfessionalName.Value != null)
            {
                columnValues.Add("professional",
                    new FieldLookupValue { LookupId = Convert.ToInt32(header.ProfessionalName.Value) });
                header.ProfessionalID = Convert.ToInt32(header.ProfessionalName.Value);
            }
            else
            {
                columnValues.Add("professional", header.ProfessionalID);
            }

            columnValues.Add("visibleto", SPConnector.GetUser(header.UserLogin, _siteUrl, "hr"));

            try
            {
                SPConnector.AddListItem(LIST_TIME, columnValues, _siteUrl);
                ID = SPConnector.GetLatestListItemID(LIST_TIME, _siteUrl);
              

            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }

            return ID;
        }


    }
}
