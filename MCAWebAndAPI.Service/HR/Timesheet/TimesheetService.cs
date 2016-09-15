using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.Utils;
using MCAWebAndAPI.Model.Common;
//using MCAWebAndAPI.Model.HR.DataMaster;
using MCAWebAndAPI.Model.ViewModel.Form.Common;
using MCAWebAndAPI.Service.Common;
using MCAWebAndAPI.Service.HR.Common;
using MCAWebAndAPI.Service.Resources;
using Microsoft.SharePoint.Client;
using NLog;
using NLog.Targets.Wrappers;

//using View = System.Web.UI.WebControls.View;

namespace MCAWebAndAPI.Service.HR.Timesheet
{
    public class TimesheetService : ITimesheetService
    {
        string _siteUrl;
        const string TYPE_PUB_HOLIDAY = "Public Holiday";
        const string TYPE_HOLIDAY = "Holiday";
        const string TYPE_DAYOFF = "Day-Off";
        const string TYPE_COMP_LEAVE = "Compensatory Time Type";
        const string LIST_PUB_HOLIDAY = "Event Calendar";
        const string LIST_DAY_OFF = "Day-Off Request";
        const string LIST_DAY_OFF_DETAIL = "Day-Off Request Detail";
        const string LIST_COMPEN = "Compensatory Request";
        const string LIST_COMPEN_DETAIL = "Compensatory Request Detail";

        const string LIST_TIME = "Timesheet";
        const string LIST_TIME_DETAIL = "Timesheet Detail";

        const string LIST_WF = "Timesheet Workflow";
        const string LIST_WF_MAPPING = "Workflow Mapping Master";

        const string LIST_PROFESSIONAL = "Professional Master";
        //private List<DateTime> lstpublicHoliday;
        //private Dictionary<DateTime, double> lstCompensatory;
        //private Dictionary<DateTime, string> lstDayOff;

        private IDataMasterService _dataService;
       // private IProfessionalService _professionalService;
        static Logger logger = LogManager.GetCurrentClassLogger();
        public TimesheetService()
        {
            _dataService = new DataMasterService();
           // _professionalService = new ProfessionalService();

        }

        public async Task<TimesheetVM> GetTimesheetLoadUpdate(int? id, string userlogin, bool? bprint = null)
        {
            var viewModel = new TimesheetVM();
            try
            {


                var professionalDataEmail = GetListItemProfessionalMaster(userlogin);

                var listItem = SPConnector.GetListItem(LIST_TIME, id, _siteUrl);
                var professionalDataId = GetListItemProfessionalMaster(null, FormatUtil.ConvertLookupToID(listItem, "professional"));

                //FieldUserValue userValue =(FieldUserValue)listItem["visibleto"];
                viewModel.ID = id;
                viewModel.ProfessionalID = FormatUtil.ConvertLookupToID(listItem, "professional");
                viewModel.ProfesionalUserLogin = Convert.ToString(professionalDataId["officeemail"]);
                viewModel.Name = FormatUtil.ConvertLookupToValue(listItem, "professional");
                viewModel.Period = Convert.ToDateTime(listItem["DatePeriod"]);
                viewModel.ProjectUnit = Convert.ToString(professionalDataId["Project_x002f_Unit"]);
                viewModel.Position = FormatUtil.ConvertLookupToValue(professionalDataId, "Position");
                viewModel.ProfessionalName.Value = viewModel.ProfessionalID;
                viewModel.ProfessionalName.Text = viewModel.Name;
                viewModel.ApprovalLevel = Convert.ToString(listItem["approvallevel"]);
                viewModel.ApproverPosition = Convert.ToString(listItem["approverposition"]);
                viewModel.Approver = Convert.ToString(listItem["approver"]);
                viewModel.TimesheetStatus = Convert.ToString(listItem["timesheetstatus"]);

                viewModel.StartPeriod = Convert.ToDateTime(viewModel.Period).GetFirstPayrollDay(); ;
                viewModel.EndPeriod = Convert.ToDateTime(viewModel.Period).GetLastPayrollDay();

                if (Convert.ToString(professionalDataEmail["Project_x002f_Unit"]) == "Human Resources Unit")
                {
                    viewModel.UserPermission = "HR";
                }
                else if (Convert.ToString(professionalDataEmail["officeemail"]) == viewModel.Approver)
                {
                    viewModel.UserPermission = "Approver";
                }
                else if (viewModel.ProfesionalUserLogin.ToLower() == userlogin.ToLower())
                {
                    viewModel.UserPermission = "Professional";
                }
                else
                {
                    viewModel.UserPermission = "Not Authorized";
                }

                if (viewModel.UserPermission == "Not Authorized") return viewModel;

                //Get Workflow From Mapping Master

                var workflow = new WorkflowService();
                workflow.SetSiteUrl(_siteUrl);

                var timesheetWf = await workflow.CheckWorkflowTimesheet(Convert.ToInt32(id), LIST_WF, "timesheet");

                if (timesheetWf.Count() != 0)
                {
                    viewModel.WorkflowItems = timesheetWf;
                }
                else
                {
                    viewModel.WorkflowItems = await workflow.GetWorkflowDetailsTimeSheet(viewModel.ProfesionalUserLogin, "Timesheet");
                }


                viewModel.TimesheetDetails = await GetTimesheetDetailsLoadUpdate(id,
                    Convert.ToDateTime(viewModel.Period), viewModel.ProfesionalUserLogin, viewModel.Name);


                if (bprint != null)
                {
                    viewModel.dtDetails = GetTimesheetPrint(viewModel);
                    viewModel.dtLocation = GetTimesheetLocation(viewModel.dtDetails);
                }
               


            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }
            return viewModel;
        }

        //public  TimesheetVM GetTimesheetLoadUpdate(int? id,string userlogin)
        //{

        //    var viewModel = new TimesheetVM();

        //    var professionalDataEmail = GetListItemProfessionalMaster(userlogin);//GetProfessionalDataByEmail(userlogin);

        //    var listItem = SPConnector.GetListItem(LIST_TIME, id, _siteUrl);
        //    var professionalDataId = GetListItemProfessionalMaster(null,FormatUtil.ConvertLookupToID(listItem, "professional"));//GetProfessionalDataById(FormatUtil.ConvertLookupToID(listItem, "professional"));

        //    //FieldUserValue userValue =(FieldUserValue)listItem["visibleto"];
        //    viewModel.ID = id;
        //    viewModel.ProfessionalID = FormatUtil.ConvertLookupToID(listItem, "professional");
        //    viewModel.UserLogin = Convert.ToString(professionalDataId["officeemail"]);//professionalDataId.OfficeEmail;
        //    viewModel.Name= FormatUtil.ConvertLookupToValue(listItem, "professional");
        //    viewModel.Period = Convert.ToDateTime(listItem["DatePeriod"]);
        //    viewModel.ProjectUnit = Convert.ToString(professionalDataId["Project_x002f_Unit"]); //professionalDataId.Project_Unit;
        //    viewModel.ProfessionalName.Value = viewModel.ProfessionalID;
        //    viewModel.ProfessionalName.Text = viewModel.Name;
        //    viewModel.ApprovalLevel = Convert.ToString(listItem["approvallevel"]);
        //    viewModel.ApproverPosition = Convert.ToString(listItem["approverposition"]);
        //    viewModel.Approver = Convert.ToString(listItem["approver"]);
        //    viewModel.TimesheetStatus = Convert.ToString(listItem["timesheetstatus"]);

        //    //if (professionalDataEmail.Project_Unit == "Human Resources Unit")
        //    if (Convert.ToString(professionalDataEmail["Project_x002f_Unit"]) == "Human Resources Unit")
        //    {
        //        viewModel.UserPermission = "HR";
        //    }
        //    else if (Convert.ToString(professionalDataEmail["officeemail"]) == viewModel.Approver)
        //    {
        //        viewModel.UserPermission = "Approver";
        //    }
        //    else if (viewModel.UserLogin.ToLower() == userlogin.ToLower())
        //    {
        //        viewModel.UserPermission = "Professional";
        //    }
        //    else
        //    {
        //        viewModel.UserPermission = "Not Authorized";
        //    }

        //    if (viewModel.UserPermission != "Not Authorized")
        //    {
        //        //viewModel.TimesheetDetails = GetTimesheetDetailsLoadUpdate(id, 
        //        //    Convert.ToDateTime(viewModel.Period), viewModel.UserLogin, viewModel.Name);
        //    }



        //    return viewModel;
        //}

        private string GetDayofWeekShort(string strDay)
        {
            var strResult = "";

            switch (strDay)
            {
                case "Sunday":
                    strResult = "Sun";
                    break;
                case "Monday":
                    strResult = "Mon";
                    break;
                case "Tuesday":
                    strResult = "Tue";
                    break;
                case "Wednesday":
                    strResult = "Wed";
                    break;
                case "Thursday":
                    strResult = "Thur";
                    break;
                case "Friday":
                    strResult = "Fri";
                    break;
                case "Saturday":
                    strResult = "Sat";
                    break;
            }

            return strResult;
        }

        private string GetDayOffName(string strType)
        {
            var strResult = "";

            switch (strType)
            {
                case "Sick Day-Off":
                    strResult = "Sick Leave";
                    break;
                case "Annual Day-Off":
                    strResult = "Eligible Days Off";
                    break;
                case "Unpaid Day-Off":
                    strResult = "Unpaid Days Off";
                    break;
                case "Compensatory Time":
                    strResult = "Compensatory Days Off";
                    break;
                case "Others":
                    strResult = "Others";
                    break;
            }

            return strResult;
        }
        public DataTable GetTimesheetPrint(TimesheetVM viewModel)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("No", typeof(string));
            dt.Columns.Add("DayName", typeof(string));
            dt.Columns.Add("DateName", typeof(string));
            dt.Columns.Add("HalfFullDay", typeof(double));
            dt.Columns.Add("Location", typeof(string));
            dt.Columns.Add("Type", typeof(string));
            dt.Columns.Add("SubType", typeof(string));

            DateTime startdate = Convert.ToDateTime(viewModel.StartPeriod);
            DateTime finishdate= Convert.ToDateTime(viewModel.EndPeriod);
            var dateRange = startdate.EachDay(finishdate);
            var i = 1;


            foreach (var itm in dateRange)
            {
                var dfull = 0;
                DataRow row = dt.NewRow();
                row["No"] = i.ToString();
                row["DayName"] = GetDayofWeekShort(Convert.ToString(itm.DayOfWeek));
                row["DateName"] = itm.Date.ToString("dd");

                var timesheetDetail = viewModel.TimesheetDetails.FirstOrDefault(x => x.Date != null && 
                Convert.ToDateTime(x.Date).ToString("yy-MM-dd")==itm.ToString("yy-MM-dd"));
                if (timesheetDetail != null)
                {

                    row["HalfFullDay"] = timesheetDetail.FullHalf;
                    if (!string.IsNullOrEmpty(timesheetDetail.Location)) row["Location"] = Convert.ToString(timesheetDetail.Location);

                    if (!string.IsNullOrEmpty(timesheetDetail.Type))
                    {
                        row["Type"] = Convert.ToString(timesheetDetail.Type);
                    }
                    else if (!string.IsNullOrEmpty(timesheetDetail.SubType))
                    {
                        row["SubType"] = Convert.ToString(timesheetDetail.SubType);
                    }
                }
                dt.Rows.Add(row);

                i++;

            }
            return dt;
        }

        public DataTable GetTimesheetLocation(DataTable dtDetails)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("Location", typeof(string));
            dt.Columns.Add("Type", typeof(string));
            dt.Columns.Add("SubType", typeof(string));
            dt.Columns.Add("DisplayName", typeof(string));
            dt.Columns.Add("Total", typeof(double));

         
            DataTable dtDistinct = dtDetails.DefaultView.ToTable(true, "Location");

            for (int i = 0; i <= dtDistinct.Rows.Count-1; i++)
            {
                object sumObject=null;
                var strLoc = Convert.ToString(dtDistinct.Rows[i]["Location"]);
                if (string.IsNullOrEmpty(strLoc))continue;
                if (dtDetails.DefaultView == null) continue;
                dtDetails.DefaultView.RowFilter = "Location='" + strLoc + "'";
                sumObject = dtDetails.Compute("Sum(HalfFullDay)", "Location='" + strLoc + "'");
                DataRow row = dt.NewRow();
                row["Location"] = strLoc;
                if (dtDetails.DefaultView != null && dtDetails.DefaultView.Count > 0)
                {
                    row["Total"] = Convert.ToDouble(sumObject);
                }
                dt.Rows.Add(row);
            }


           //var caml = string.Format(@"<View><Query>
           //                            <Where>
           //                               <Eq>
           //                                  <FieldRef Name='othercategory' />
           //                                  <Value Type='Boolean'>true</Value>
           //                               </Eq>
           //                            </Where>
           //                         </Query>
           //                         <ViewFields>
           //                            <FieldRef Name='Title' />
           //                            <FieldRef Name='othercategory' />
           //                         </ViewFields></View>");

           // var listMasterDayOff = SPConnector.GetList("Master Day Off Type", _siteUrl, caml);

           // foreach (var item in listMasterDayOff)
           // {

           //     object sumObject=null;
           //     var strType = Convert.ToString(Convert.ToString(item["Title"]));
           //     if (string.IsNullOrEmpty(strType)) continue;
           //     if (dtDetails.DefaultView == null) continue;
           //     dtDetails.DefaultView.RowFilter = "SubType='" + strType + "'";
               
           //     sumObject = dtDetails.Compute("Sum(HalfFullDay)", "SubType='" + strType + "'");
           //     DataRow row = dt.NewRow();
           //     row["SubType"] = strType;
           //     row["DisplayName"] = GetDayOffName(strType);
           //     if (dtDetails.DefaultView != null && dtDetails.DefaultView.Count > 0)
           //     {
           //         row["Total"] = Convert.ToDouble(sumObject);
           //     }
           //     else
           //     {
           //         continue;
           //     }

               
           //     dt.Rows.Add(row);
           // }


            //if (dtDetails.DefaultView != null)
            //{
            //    object sumObject = null;
            //    dtDetails.DefaultView.RowFilter = "Type='Compensatory Time'";
            //    sumObject = dtDetails.Compute("Sum(HalfFullDay)", "Type='Compensatory Time'");
            //    DataRow row = dt.NewRow();
            //    row["Type"] = "Compensatory Time";
            //    if (dtDetails.DefaultView != null && dtDetails.DefaultView.Count > 0)
            //    {
            //        row["Total"] = Convert.ToDouble(sumObject);
            //        dt.Rows.Add(row);
            //    }

            //}


            //if (dtDetails.DefaultView != null)
            //{
            //    object sumObject = null;
            //    dtDetails.DefaultView.RowFilter = "Type='Public Holiday'";
            //    sumObject = dtDetails.Compute("Sum(HalfFullDay)", "Type='Public Holiday'");
            //    DataRow row = dt.NewRow();
            //    row["Type"] = "Public Holiday";
            //    if (dtDetails.DefaultView != null && dtDetails.DefaultView.Count > 0)
            //    {
            //        row["Total"] = Convert.ToDouble(sumObject);
            //        dt.Rows.Add(row);
            //    }
               
            //}



            return dt;
        }

        public async Task<IEnumerable<TimesheetDetailVM>> GetTimesheetDetailsLoadUpdate(int? id, DateTime period,
            string userlogin, string strName)
        {

            var timesheetDetails = new List<TimesheetDetailVM>();

            string caml = @"<View><Query><Where>
                          <Eq><FieldRef Name='timesheet' />
                          <Value Type='Lookup'>" + id + "</Value></Eq>" +
                          "</Where></Query>" +
                         "</View>";

            foreach (var item in SPConnector.GetList(LIST_TIME_DETAIL, _siteUrl, caml))
            {
                timesheetDetails.Add(new TimesheetDetailVM
                {
                    ID = Convert.ToInt32(item["ID"]),
                    Date = Convert.ToDateTime(item["timesheetdetaildate"]),
                    FullHalf = Convert.ToDouble(item["FullHalf"]),
                    Location = FormatUtil.ConvertLookupToValue(item, "location"),
                    LocationID = FormatUtil.ConvertLookupToID(item, "location_x003a_ID"),
                    Type = Convert.ToString(item["Title"])
                });
            }

            var startDate = period.GetFirstPayrollDay();
            var finishDate = period.GetLastPayrollDay();
            var dateRange = startDate.EachDay(finishDate);

            var listHoliday = await GetPublicHoliday(_siteUrl);
            var listDayOff = await GetUserDayOff(_siteUrl, strName);
            var listCompen = await GetCompensatory(_siteUrl, strName);

            foreach (var item in dateRange)
            {
                if (IsDate(item, listHoliday))
                {
                    timesheetDetails.Add(new TimesheetDetailVM
                    {
                        Date = item,
                        FullHalf = 1,
                        Type = TYPE_PUB_HOLIDAY
                    });
                }
                else if (item.DayOfWeek == DayOfWeek.Saturday || item.DayOfWeek == DayOfWeek.Sunday)
                    timesheetDetails.Add(new TimesheetDetailVM
                    {
                        Date = item,
                        FullHalf = 1,
                        Type = TYPE_HOLIDAY
                    });
                else if (IsDayOff(item, listDayOff))
                {
                    var arrKey = listDayOff[item].Split(Convert.ToChar(";"));
                    var typeDayoff = arrKey[0];
                    var fullhalf = arrKey[1];
                    var otherCat = arrKey[2];
                    var timesheetDetail = new TimesheetDetailVM
                    {
                        Date = item,
                        Type = TYPE_DAYOFF,
                        FullHalf = fullhalf == "Full Day" ? 1 : 0.5,
                        SubType = otherCat == "False" ? typeDayoff : "Others"
                    };
                    timesheetDetails.Add(timesheetDetail);

                }
                else if (IsCompenTime(item, listCompen))
                {
                    timesheetDetails.Add(new TimesheetDetailVM
                    {
                        Date = item,
                        FullHalf = listCompen[item],
                        Type = TYPE_COMP_LEAVE
                    });
                }
            }


            return timesheetDetails;
        }

        //public TimesheetVM GetTimesheet(string userlogin, DateTime period)
        //{

        //    var professionalData = GetListItemProfessionalMaster(userlogin);
        //    var startPeriod = period.GetFirstPayrollDay();
        //    var viewModel = new TimesheetVM
        //    {
        //        ProfessionalID = Convert.ToInt32(professionalData["ID"]),
        //        UserLogin = userlogin,
        //        Name = Convert.ToString(professionalData["Title"]),
        //        Period = period,
        //        ProjectUnit = Convert.ToString(professionalData["Project_x002f_Unit"])

        //    };
        //    viewModel.StartPeriod = startPeriod;
        //    viewModel.EndPeriod = period.GetLastPayrollDay();
        //    viewModel.TimesheetDetails = GetTimesheetDetails(userlogin, startPeriod, viewModel.Name);
        //    viewModel.ProfessionalName.Value = viewModel.ProfessionalID;
        //    viewModel.ProfessionalName.Text = viewModel.Name;
        //    viewModel.UserPermission = viewModel.ProjectUnit == "Human Resources Unit" ? "HR" : "Professional";




        //    return viewModel;
        //}

        public async Task<TimesheetVM> GetTimesheetAsync(string userlogin, DateTime period)
        {
            var professionalData = GetListItemProfessionalMaster(userlogin);
            var startPeriod = period.GetFirstPayrollDay();
            var viewModel = new TimesheetVM
            {
                ProfessionalID = Convert.ToInt32(professionalData["ID"]),
                ProfesionalUserLogin = userlogin,
                Name = Convert.ToString(professionalData["Title"]),
                Period = period,
                ProjectUnit = Convert.ToString(professionalData["Project_x002f_Unit"])

            };
            viewModel.StartPeriod = startPeriod;
            viewModel.EndPeriod = period.GetLastPayrollDay();

            viewModel.ProfessionalName.Value = viewModel.ProfessionalID;
            viewModel.ProfessionalName.Text = viewModel.Name;
            viewModel.UserPermission = viewModel.ProjectUnit == "Human Resources Unit" ? "HR" : "Professional";

            //Get Workflow From Mapping Master
            var workflow = new WorkflowService();
            workflow.SetSiteUrl(_siteUrl);


            viewModel.WorkflowItems = await workflow.GetWorkflowDetailsTimeSheet(userlogin, "Timesheet");

            viewModel.TimesheetDetails = await GetTimesheetDetailsAsync(userlogin, startPeriod, viewModel.Name);



            return viewModel;
        }


        //private ProfessionalMaster GetProfessionalDataByEmail(string userlogin)
        //{
        //    var professionalData = _dataService.GetProfessionals().FirstOrDefault(e => e.OfficeEmail == userlogin);


        //    return professionalData;
        //}

        //private ProfessionalMaster GetProfessionalDataById(int? id)
        //{
        //    var professionalData = _dataService.GetProfessionals().FirstOrDefault(e => e.ID == id);


        //    return professionalData;
        //}

        private ListItem GetListItemProfessionalMaster(string userlogin = null, int? id = null)
        {
            string caml = @"<View><Query><Where>
                          <Eq><FieldRef Name='officeemail' />
                          <Value Type='Text'>" + userlogin + "</Value></Eq>" +
                          "</Where></Query>" +
                         "</View>";
            if (id != null)
            {
                caml = @"<View><Query><Where>
                          <Eq><FieldRef Name='ID' />
                          <Value Type='Number'>" + id + "</Value></Eq>" +
                          "</Where></Query>" +
                         "</View>";
            }

            var item = SPConnector.GetList(LIST_PROFESSIONAL, _siteUrl, caml);

            return item.FirstOrDefault();

        }

        //private string GetFullNamez(string userlogin)
        //{
        //    var professionalData = _dataService.GetProfessionals().FirstOrDefault(e => e.OfficeEmail == userlogin);
        //    return professionalData.Name;
        //}

        private async Task<List<DateTime>> GetPublicHoliday(string strUrl)
        {
            var listItem = SPConnector.GetList(LIST_PUB_HOLIDAY, strUrl);
            List<DateTime> lstpublicHoliday = new List<DateTime>();
            lstpublicHoliday = new List<DateTime>();
            foreach (var item in listItem)
            {
                lstpublicHoliday.Add(Convert.ToDateTime(item["EventDate"]).ToLocalTime());
            }

            return lstpublicHoliday;

        }

        private async Task<Dictionary<DateTime, double>> GetCompensatory(string strUrl, string strName)
        {
            Dictionary<DateTime, double> lstCompensatory = new Dictionary<DateTime, double>();
            lstCompensatory = new Dictionary<DateTime, double>();
            var caml = @"<View><Query><Where><And><Eq><FieldRef Name='professional' />
                        <Value Type='Lookup'>" + strName + "</Value></Eq>" +
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
                    var totaldays = Convert.ToDouble(itemDetail["totaldays"]);
                    var dateRange = startdate.EachDay(finishdate);

                    foreach (var itm in dateRange)
                    {
                        lstCompensatory.Add(itm, totaldays);
                    }
                }

            }
            return lstCompensatory;
        }

        private async Task<Dictionary<DateTime, string>> GetUserDayOff(string strUrl, string strName)
        {
            var caml = string.Format(@"<View><Query><Where>
            <Eq><FieldRef Name='professional'/><Value Type='Lookup'>{0}</Value></Eq></Where></Query></View>", strName);
            var listMaster = SPConnector.GetList(LIST_DAY_OFF, strUrl, caml);

            caml = string.Format(@"<View><Query><Where>
            <Gt><FieldRef Name='ID'/><Value Type='Number'>{0}</Value></Gt></Where></Query></View>", 0);

            var listMasterDayOff = SPConnector.GetList("Master Day Off Type", strUrl, caml);

            // Dictionary<DateTime, string> lstDayOff = new Dictionary<DateTime, string>();
            Dictionary<DateTime, string> lstDayOff = new Dictionary<DateTime, string>();
            foreach (var item in listMaster)
            {
                caml = @"<View><Query><Where><And><Eq><FieldRef Name='dayoffrequest' />
                        <Value Type='Lookup'>" + Convert.ToString(item["ID"]) +
                        "</Value></Eq>" +
                        "<Eq><FieldRef Name='approvalstatus' />" +
                        "<Value Type='Text'>Approved</Value></Eq></And></Where></Query></View>";
                var listDetail = SPConnector.GetList(LIST_DAY_OFF_DETAIL, strUrl, caml);

                foreach (var itemDetail in listDetail)
                {
                    var startdate = Convert.ToDateTime(itemDetail["requeststartdate"]).ToLocalTime();
                    var finishdate = Convert.ToDateTime(itemDetail["requestenddate"]).ToLocalTime();
                    var typeDayoff = FormatUtil.ConvertLookupToValue(itemDetail, "masterdayofftype");
                    var idTypeDayoff = FormatUtil.ConvertLookupToID(itemDetail, "masterdayofftype");
                    var fullhalf = Convert.ToString(itemDetail["fullhalfday"]);
                    var query = listMasterDayOff.FirstOrDefault(t => Convert.ToString(t.Id) == Convert.ToString(idTypeDayoff));
                    var otherCat = query["othercategory"].ToString();
                    var dateRange = startdate.EachDay(finishdate);
                    foreach (var itm in dateRange)
                    {
                        if (!lstDayOff.ContainsKey(itm))
                        {
                            lstDayOff.Add(itm, typeDayoff + ";" + fullhalf + ";" + otherCat);
                        }
                      
                    }

                }

            }
            return lstDayOff;
        }

        private async Task<IEnumerable<TimesheetDetailVM>> GetTimesheetDetailsAsync(string userlogin, DateTime period, string strName)
        {
            var startDate = period;
            var finishDate = period.GetLastPayrollDay();
            var dateRange = startDate.EachDay(finishDate);

            //var listHoliday = GetPublicHoliday(_siteUrl);
            //var listDayOff = GetUserDayOff(_siteUrl, strName);
            //var listCompen = GetCompensatory(_siteUrl, strName);

          

            Task<List<DateTime>> getPublicHolidayTask = GetPublicHoliday(_siteUrl);
            Task<Dictionary<DateTime, double>> getCompensatoryTask = GetCompensatory(_siteUrl, strName);
            Task<Dictionary<DateTime, string>> getUserDayOffTask = GetUserDayOff(_siteUrl, strName);

            var lstpublicHoliday = await getPublicHolidayTask;
            var lstCompensatory = await getCompensatoryTask;
            var lstDayOff = await getUserDayOffTask;

            //List <DateTime> lstpublicHoliday = new List<DateTime>();
            //Dictionary<DateTime, double> lstCompensatory = new Dictionary<DateTime, double>();
            //Dictionary<DateTime, string> lstDayOff = new Dictionary<DateTime, string>();
            //Task allTask = Task.WhenAll(listHoliday, listDayOff, listCompen);

            //await allTask;

            var timesheetDetails = new List<TimesheetDetailVM>();

            foreach (var item in dateRange)
            {

                if (IsDate(item, lstpublicHoliday))
                {
                    timesheetDetails.Add(new TimesheetDetailVM
                    {
                        Date = item,
                        FullHalf = 1,
                        Type = TYPE_PUB_HOLIDAY
                    });
                }
                else if (item.DayOfWeek == DayOfWeek.Saturday || item.DayOfWeek == DayOfWeek.Sunday)
                    timesheetDetails.Add(new TimesheetDetailVM
                    {
                        Date = item,
                        FullHalf = 1,
                        Type = TYPE_HOLIDAY
                    });
                else if (IsDayOff(item, lstDayOff))
                {
                    var arrKey = lstDayOff[item].Split(Convert.ToChar(";"));
                    var typeDayoff = arrKey[0];
                    var fullhalf = arrKey[1];
                    var otherCat = arrKey[2];
                    var timesheetDetail = new TimesheetDetailVM
                    {
                        Date = item,
                        Type = TYPE_DAYOFF,
                        FullHalf = fullhalf == "Full Day" ? 1 : 0.5,
                        SubType = otherCat == "False" ? typeDayoff : "Others"
                    };
                    timesheetDetails.Add(timesheetDetail);

                }
                else if (IsCompenTime(item, lstCompensatory))
                {
                    timesheetDetails.Add(new TimesheetDetailVM
                    {
                        Date = item,
                        FullHalf = lstCompensatory[item],
                        Type = TYPE_COMP_LEAVE
                    });
                }
            }

            return timesheetDetails;
        }
        public IEnumerable<TimesheetDetailVM> GetTimesheetDetails(string userlogin, DateTime period, string strName)
        {
            var startDate = period;
            var finishDate = period.GetLastPayrollDay();
            var dateRange = startDate.EachDay(finishDate);

            //var listHoliday = GetPublicHoliday( _siteUrl);
            //var listDayOff = getUserDayOFF(_siteUrl, strName);
            //var listCompen = getCompensatory(_siteUrl, strName);

            var timesheetDetails = new List<TimesheetDetailVM>();

            //foreach (var item in dateRange)
            //{

            //    if (IsDate(item, listHoliday))
            //    {
            //        timesheetDetails.Add(new TimesheetDetailVM
            //        {
            //            Date = item,
            //            FullHalf = 1,
            //            Type = TYPE_PUB_HOLIDAY
            //        });
            //    }
            //    else if (item.DayOfWeek == DayOfWeek.Saturday || item.DayOfWeek == DayOfWeek.Sunday)
            //        timesheetDetails.Add(new TimesheetDetailVM
            //        {
            //            Date = item,
            //            FullHalf = 1,
            //            Type = TYPE_HOLIDAY
            //        });
            //    else if (IsDayOff(item, listDayOff))
            //    {
            //        var arrKey= listDayOff[item].Split(Convert.ToChar(";"));
            //        var typeDayoff = arrKey[0];
            //        var fullhalf = arrKey[1];
            //        var otherCat = arrKey[2];
            //        var timesheetDetail = new TimesheetDetailVM
            //        {
            //            Date = item,
            //            Type = TYPE_DAYOFF,
            //            FullHalf = fullhalf == "Full Day" ? 1 : 0.5,
            //            SubType = otherCat == "False" ? typeDayoff : "Others"
            //        };
            //        timesheetDetails.Add(timesheetDetail);

            //    }
            //    else if (IsCompenTime(item, listCompen))
            //    {
            //        timesheetDetails.Add(new TimesheetDetailVM
            //        {
            //            Date = item,
            //            FullHalf = listCompen[item],
            //            Type = TYPE_COMP_LEAVE
            //        });
            //    }
            //}

            return timesheetDetails;
        }

        private bool IsCompenTime(DateTime date, Dictionary<DateTime, double> lstRange)
        {
            var bcek = false;
            if (lstRange == null) return false;
            else
            {
                if (lstRange.Any(item => date.ToString("yy-MM-dd") == item.Key.ToString("yy-MM-dd")))
                {
                    return true;
                }
            }

            return bcek;
        }

        private bool IsDayOff(DateTime date, Dictionary<DateTime, string> lstRange)
        {
            var bcek = false;
            if (lstRange == null) return false;
            else
            {
                if (lstRange.Any(item => date.ToString("yy-MM-dd") == item.Key.ToString("yy-MM-dd")))
                {
                    return true;
                }
            }

            return bcek;
        }

        private bool IsDate(DateTime date, List<DateTime> lstRange)
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
          //  _professionalService.SetSiteUrl(_siteUrl);
        }

        public IEnumerable<TimesheetDetailVM> AppendWorkingDays(int? id, IEnumerable<TimesheetDetailVM> currentDays, DateTime from,
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
                else
                {
                    var query = allDays.FirstOrDefault(d => d.Date == workingDay);
                    if (query.Type != "Working Days") continue;
                    query.FullHalf = isFullDay ? 1.0d : 0.5d;
                    query.Location = location ?? string.Empty;
                    query.LocationID = locationid ?? null;
                    query.Type = "Working Days";
                    if (id != null) query.EditMode = 1;
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
            var mastervalueInsert = new Dictionary<string, Dictionary<string, object>>();
            var mastervalueUpdate = new Dictionary<string, Dictionary<string, object>>();
            var itemsDelete = new List<string>();
            var i = 1;
            foreach (var viewModel in timesheetDetails)
            {
                if (Item.CheckIfSkipped(viewModel)) continue;
                if (Item.CheckIfDeleted(viewModel))
                {
                    itemsDelete.Add(viewModel.ID.ToString());
                    continue;
                }

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

                if (Item.CheckIfUpdated(viewModel)) mastervalueUpdate.Add(viewModel.ID + ";Edit", updatedValue);
                else mastervalueInsert.Add(i + ";Add", updatedValue);


                i++;

            }

            try
            {
                if (itemsDelete.Count > 0) SPConnector.DeleteMultipleListItemAsync(LIST_TIME_DETAIL, itemsDelete, _siteUrl);
                if (mastervalueInsert.Count > 0) SPConnector.AddListItemAsync(LIST_TIME_DETAIL, mastervalueInsert, _siteUrl);
                if (mastervalueUpdate.Count > 0) SPConnector.UpdateMultipleListItemAsync(LIST_TIME_DETAIL, mastervalueUpdate, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                //throw new Exception(ErrorResource.SPInsertError);
                throw new Exception(e.Message);
            }
        }

        //private DataTable Getworkflowmapping(string strProjectUnit)
        //{


        //    var dtView = new DataTable();

        //    dtView.Columns.Add("ApproverUnit", typeof(string));
        //    dtView.Columns.Add("ApproverPosition", typeof(string));
        //    dtView.Columns.Add("Level", typeof(int));
        //    dtView.Columns.Add("IsDefault", typeof(string));


        //    var caml = @"<View><Query>
        //                <Where>
        //                <And>
        //                <Eq>
        //                    <FieldRef Name='transactiontype' />
        //                    <Value Type='Choice'>Timesheet</Value>
        //                </Eq>
        //                <Eq>
        //                    <FieldRef Name='requestorunit' />
        //                     <Value Type='Choice'>" + strProjectUnit + "</Value>" +
        //               "</Eq>" +
        //               "</And></Where></Query></View>";

        //    var listcoll = SPConnector.GetList(LIST_WF_MAPPING, _siteUrl, caml);

        //    if (listcoll == null || listcoll.Count == 0) return null;

        //    foreach (var item in listcoll)
        //    {
        //        DataRow row = dtView.NewRow();
        //        row["ApproverUnit"] = Convert.ToString(item["approverunit"]);
        //        row["ApproverPosition"] = FormatUtil.ConvertLookupToValue(item, "approverposition");
        //        row["Level"] = Convert.ToString(item["approverlevel"]);
        //        row["IsDefault"] = Convert.ToString(item["isdefault"]);
        //        dtView.Rows.Add(row);
        //    }
        //    dtView.DefaultView.Sort = "Level ASC";

        //    dtView.DefaultView.RowFilter = "IsDefault='Yes'";

        //    if (dtView.DefaultView == null || dtView.DefaultView.Count == 0) return null;

        //    return dtView.DefaultView.ToTable();
        //}

        private void UpdateHeaderApprover(int? headerId, string strLevel, string strPosition,
            string strEmail, string strPermission = null)
        {
            var columnValues = new Dictionary<string, object>
                {
                    {"approverposition", strPosition},
                    {"approvallevel", strLevel},
                    {"approver", strEmail},
                };

            if (strPermission != null) columnValues.Add("timesheetstatus", "Approved");

            SPConnector.UpdateSingleListItemAsync(LIST_TIME, headerId, columnValues, _siteUrl);
        }

        public async Task CreateWorkflowTimesheetAsync(int? headerId, IEnumerable<WorkflowItemVM> workflowItems, string strStatus)
        {
            //if (strStatus == "Draft") return;
            CreateWorkflowTimesheet(headerId, workflowItems);
        }

        public void CreateWorkflowTimesheet(int? headerId, IEnumerable<WorkflowItemVM> workflowItems)
        {
            var mastervalueInsert = new Dictionary<string, Dictionary<string, object>>();
            var mastervalueUpdate = new Dictionary<string, Dictionary<string, object>>();
            var itemsDelete = new List<string>();
            var i = 1;
            foreach (var item in workflowItems)
            {
                if (Item.CheckIfSkipped(item)) continue;
                if (Item.CheckIfDeleted(item))
                {
                    itemsDelete.Add(item.ID.ToString());
                    continue;
                }


                var updatedValue = new Dictionary<string, object>();
                updatedValue.Add("timesheet", new FieldLookupValue { LookupId = Convert.ToInt32(headerId) });
                updatedValue.Add("approverlevel", item.Level);
                updatedValue.Add("position", item.ApproverPositionId);
                updatedValue.Add("projectunit", item.ApproverUnitText);
                updatedValue.Add("approvername", item.ApproverName.Value);
                updatedValue.Add("status", item.Status);

                if (i == 1) updatedValue.Add("currentstate", "Yes");
                // mastervalueInsert.Add(i + ";Add", updatedValue);requestor

                if (Item.CheckIfUpdated(item)) mastervalueUpdate.Add(item.ID + ";Edit", updatedValue);
                else mastervalueInsert.Add(i + ";Add", updatedValue);

                if (i == 1)
                {
                    UpdateHeaderApprover(headerId, item.Level, item.ApproverPositionText,
                        item.ApproverEmail);
                }

                i++;

            }

            if (itemsDelete.Count > 0) SPConnector.DeleteMultipleListItemAsync(LIST_WF, itemsDelete, _siteUrl);
            if (mastervalueInsert.Count > 0) SPConnector.AddListItemAsync(LIST_WF, mastervalueInsert, _siteUrl);
            if (mastervalueUpdate.Count > 0) SPConnector.UpdateMultipleListItemAsync(LIST_WF, mastervalueUpdate, _siteUrl);

            // if (mastervalueInsert.Count > 0) SPConnector.AddListItemAsync(LIST_WF, mastervalueInsert, _siteUrl);

        }

        //public  void CreateWorkflowTimesheet(int? headerId, TimesheetVM header)
        //{

        //    var caml = string.Format(@"<View><Query>
        //                <Where>
        //                <Eq>
        //                    <FieldRef Name='timesheet' />
        //                    <Value Type='Lookup'>{0}</Value>
        //                </Eq>
        //                </Where></Query></View>", headerId);

        //    var listWF = SPConnector.GetList(LIST_WF, _siteUrl, caml);
        //    if (listWF.Count > 0) return;

        //    var dtView = Getworkflowmapping(header.ProjectUnit);
        //    var iCount = dtView.DefaultView.Count;
        //    IEnumerable<ProfessionalMaster> profMasterPosition = _dataService.GetProfessionals();


        //    var mastervalue = new Dictionary<string, Dictionary<string, object>>();
        //    for (int i = 1; i <= iCount; i++)
        //    {
        //        var strApproverPosition = "";
        //        var strApproverEmail = "";
        //        var columnValues = new Dictionary<string, object>
        //        {
        //            {"Title", header.UserLogin},
        //            {"timesheet", new FieldLookupValue {LookupId = Convert.ToInt32(headerId)}},
        //            {"approverlevel", i.ToString()}
        //        };

        //        columnValues.Add("status", header.UserPermission == "HR" ? "Approved" : "Pending Approval");


        //        switch (i)
        //        {
        //            case 1:
        //                dtView.DefaultView.RowFilter = "Level = 1 And IsDefault='Yes'";
        //                if (dtView.DefaultView != null && dtView.DefaultView.Count > 0)
        //                {
        //                    strApproverPosition = Convert.ToString(dtView.DefaultView[0]["ApproverPosition"]);
        //                    strApproverEmail = profMasterPosition.FirstOrDefault(e => e.Position == strApproverPosition).OfficeEmail;
        //                }
        //                else
        //                {
        //                    goto case 2;
        //                }
        //                if (header.UserPermission != "HR")
        //                {
        //                    UpdateHeaderApprover(headerId, "1", strApproverPosition, strApproverEmail);
        //                }
        //                else UpdateHeaderApprover(headerId, "1", strApproverPosition, strApproverEmail, "HR");
        //                columnValues.Add("currentstate", header.UserPermission == "HR" ? "No" : "Yes");
        //                columnValues.Add("approverposition", strApproverPosition);
        //                columnValues.Add("approver0", strApproverEmail);
        //                break;
        //            case 2:
        //                dtView.DefaultView.RowFilter = "Level = 2 And IsDefault='Yes'";
        //                if (dtView.DefaultView != null && dtView.DefaultView.Count > 0)
        //                {
        //                    strApproverPosition = Convert.ToString(dtView.DefaultView[0]["ApproverPosition"]);
        //                    strApproverEmail = profMasterPosition.FirstOrDefault(e => e.Position == strApproverPosition).OfficeEmail;
        //                }
        //                else
        //                {
        //                    goto case 3;
        //                }
        //                if (i == 1)
        //                {
        //                    if (header.UserPermission != "HR")
        //                    {
        //                        UpdateHeaderApprover(headerId, "1", strApproverPosition, strApproverEmail);
        //                    }
        //                    else UpdateHeaderApprover(headerId, "1", strApproverPosition, strApproverEmail,"HR");
        //                    columnValues.Add("currentstate", "Yes");
        //                }
        //                else if (i == 2)
        //                {
        //                    columnValues.Add("currentstate", "No" );
        //                }

        //                columnValues.Add("approverposition", strApproverPosition);
        //                columnValues.Add("approver0", strApproverEmail);
        //                break;
        //            case 3:
        //                dtView.DefaultView.RowFilter = "Level = 3 And IsDefault='Yes'";
        //                if (dtView.DefaultView != null && dtView.DefaultView.Count > 0)
        //                {
        //                    strApproverPosition = Convert.ToString(dtView.DefaultView[0]["ApproverPosition"]);
        //                    strApproverEmail = profMasterPosition.FirstOrDefault(e => e.Position == strApproverPosition).OfficeEmail;

        //                    if (i == 1)
        //                    {
        //                        if (header.UserPermission != "HR")
        //                        {
        //                            UpdateHeaderApprover(headerId, "1", strApproverPosition, strApproverEmail);
        //                        }
        //                        else
        //                        {
        //                            UpdateHeaderApprover(headerId, "1", strApproverPosition, strApproverEmail,"HR");
        //                        }
        //                        columnValues.Add("currentstate", "Yes");
        //                    }
        //                    else if (i == 2)
        //                    {
        //                        columnValues.Add("currentstate", "No");
        //                        if (header.UserPermission == "HR") UpdateHeaderApprover(headerId, "2", strApproverPosition, strApproverEmail,"HR");
        //                    }
        //                    else if (i == 3)
        //                    {
        //                        if (header.UserPermission == "HR") UpdateHeaderApprover(headerId, "3", strApproverPosition, strApproverEmail,"HR");
        //                        columnValues.Add("currentstate", header.UserPermission != "HR" ? "No" : "Yes");
        //                    }

        //                    columnValues.Add("approverposition", strApproverPosition);
        //                    columnValues.Add("approver0", strApproverEmail);
        //                }
        //                break;
        //        }

        //       if (!string.IsNullOrEmpty(strApproverPosition)) mastervalue.Add(i + ";Add", columnValues); 
        //    }
        //    SPConnector.AddListItemAsync(LIST_WF, mastervalue, _siteUrl);

        //}

        public void UpdateApproval(TimesheetVM header)
        {
            try
            {
                //var caml = string.Format(@"<View><Query>
                //             <Where>
                //             <Eq>
                //                 <FieldRef Name='timesheet' />
                //                 <Value Type='Lookup'>{0}</Value>
                //             </Eq>
                //            </Where></Query></View>", header.ID);

                // var listcoll = SPConnector.GetList(LIST_WF, _siteUrl, caml);

                if (!header.WorkflowItems.Any()) return;

                var wf = header.WorkflowItems;

                var strNextLevel = "";
                if (header.ApprovalLevel == "3" && header.TimesheetStatus != "Rejected")
                {
                    strNextLevel = header.ApprovalLevel;
                }
                else if (header.ApprovalLevel != "3" && header.TimesheetStatus != "Rejected")
                {
                    strNextLevel = (Convert.ToInt32(header.ApprovalLevel) + 1).ToString();
                }
                else if (header.TimesheetStatus == "Rejected")
                {
                    strNextLevel = header.ApprovalLevel;
                }

                var listitemNext = wf.FirstOrDefault(e => e.Level == strNextLevel); //listcoll.FirstOrDefault(e => Convert.ToString(e["approverlevel"]) == strNextLevel);

                var strStatus = "";
                if (header.TimesheetStatus == "Rejected")
                {
                    strStatus = header.TimesheetStatus;
                }
                else if (header.TimesheetStatus == "Approved" && header.ApprovalLevel == "1")
                {
                    strStatus = "Pending Approval 2 of 3";
                }
                else if (header.TimesheetStatus == "Approved" && header.ApprovalLevel == "2")
                {
                    strStatus = "Pending Approval 3 of 3";
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
                        //columnValues.Add("approverposition", FormatUtil.ConvertLookupToValue(listitemNext,"position"));
                        //columnValues.Add("approver", FormatUtil.ConvertLookupToValue(listitemNext, "approvername_x003a_Office_x0020_"));

                        columnValues.Add("approverposition", listitemNext.ApproverPositionText);
                        columnValues.Add("approver", listitemNext.ApproverEmail);

                    }

                }

                SPConnector.UpdateSingleListItemAsync(LIST_TIME, header.ID, columnValues, _siteUrl);


                if (listitemNext == null) return;
                var columnWfValues = new Dictionary<string, object>();
                //var idNext = Convert.ToInt32(listitemNext["ID"]);
                var idNext = listitemNext.ID;
                if (header.TimesheetStatus != "Rejected")
                {

                    if (header.TimesheetStatus != "Rejected") columnWfValues.Add("currentstate", "Yes");
                    if (header.ApprovalLevel == "3") columnWfValues.Add("status", header.TimesheetStatus);


                    SPConnector.UpdateSingleListItemAsync(LIST_WF, idNext, columnWfValues, _siteUrl);

                    columnWfValues = new Dictionary<string, object>();
                    if (header.TimesheetStatus != "Rejected") columnWfValues.Add("currentstate", "No");
                    if (header.ApprovalLevel != "3") columnWfValues.Add("status", header.TimesheetStatus);
                    SPConnector.UpdateSingleListItemAsync(LIST_WF, (idNext - 1), columnWfValues, _siteUrl);
                }
                else
                {
                    columnWfValues.Add("status", header.TimesheetStatus);
                    SPConnector.UpdateSingleListItemAsync(LIST_WF, (idNext), columnWfValues, _siteUrl);
                }

                var strApproverName = wf.FirstOrDefault().ApproverNameText;
                SendEmailApproveReject(strApproverName, header.Name,
                header.TimesheetStatus, header.ProfesionalUserLogin, header.ID, header.StartPeriod,
                header.EndPeriod);

                if (header.ApprovalLevel != "3" && header.TimesheetStatus == "Approved")
                {
                    SendEmailSubmitForApproval(listitemNext.ApproverNameText, header.Name, listitemNext.ApproverEmail,
                        header.ID, header.StartPeriod, header.EndPeriod);
                }
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }




        }

        public IEnumerable<TimesheetDetailVM> DeleteSelectedWorkingDays(int? headerId, IEnumerable<TimesheetDetailVM> currentDays,
            DateTime from, DateTime to)
        {
            var dateRange = from.EachDay(to);
            var existingDays = currentDays.Select(e => (DateTime)e.Date).ToList();
            var allDays = currentDays.ToList();

            foreach (var workingDay in dateRange)
            {
                if (!existingDays.ContainsSameDay(workingDay)) continue;
                var query = allDays.FirstOrDefault(d => d.Date == workingDay);
                if (query.Type != "Working Days") continue;
                if (headerId == null)
                {
                    allDays.Remove(query);
                }
                else
                {
                    query.EditMode = -1;
                }
            }

            return allDays;
        }

        public DataTable GetTimesheetProfessionalDataTable()
        {
            var dtDetail = new DataTable();

            dtDetail.Columns.Add("Date", typeof(string));
            dtDetail.Columns.Add("Location 1:", typeof(string));
            dtDetail.Columns.Add("Location 2:", typeof(string));
            dtDetail.Columns.Add("Location 3:", typeof(string));
            dtDetail.Columns.Add("Location 4:", typeof(string));
            dtDetail.Columns.Add("Location 5:", typeof(string));
            dtDetail.Columns.Add("Location 6:", typeof(string));
            dtDetail.Columns.Add("Sick Leave", typeof(string));
            dtDetail.Columns.Add("Eligible Day-Off", typeof(string));
            dtDetail.Columns.Add("Others*", typeof(string));
            dtDetail.Columns.Add("Unpaid Day-Off", typeof(string));
            dtDetail.Columns.Add("Compensatory Day-Off", typeof(string));
            dtDetail.Columns.Add("Total", typeof(string));
            dtDetail.Columns.Add("Public Holiday", typeof(string));
            dtDetail.Columns.Add("Compensatory Time", typeof(string));
            dtDetail.Columns.Add("Approval Status", typeof(string));

            return dtDetail;
        }

        public TimesheetVM GetTimesheetProfessional(string userlogin)
        {
            var viewModel = new TimesheetVM
            {
                URL = _siteUrl,
                ProfesionalUserLogin = userlogin,
                dtDetails = GetTimesheetProfessionalDataTable()
            };
            return viewModel;
        }

        private void SendEmailSubmitForApproval(string strNameApprover,
            string strRequestorName,
         string strApproverEmail = null, int? id = null,
         DateTime? startPeriod = null, DateTime? endPeriod = null)
        {
            if (string.IsNullOrEmpty(strApproverEmail)) return;

            var strBody = "";

            strBody += "Dear Mr/Ms. " + strNameApprover + ",<br/><br/>";
            strBody += @"You are authorized as an approver for Timesheet Period "
            + Convert.ToDateTime(startPeriod).ToString("MMM-dd-yyyy") + " to "
            + Convert.ToDateTime(endPeriod).ToString("MMM-dd-yyyy") + " <br/>";
            strBody += "The Timesheet is requested by " + strRequestorName;
            strBody += "<br/>Please complete the approval process immediately.";
            strBody += "<br/><br/>To view the detail of the Timesheet, please click following link:<br/>";
            strBody += _siteUrl + UrlResource.Timesheet + "/EditForm.aspx?ID=" + id;
            strBody += "<br/><br/>Thank you for your attention.";

            //if (!string.IsNullOrEmpty(strOfficeEmail))
            //{
            //   
            //}
            List<string> lstEmail = new List<string> { strApproverEmail };
            EmailUtil.SendMultiple(lstEmail, "Request for Approval of Timesheet ", strBody);


        }

        private void SendEmailApproveReject(string strNameApprover,
        string strRequestorName, string strStatus,
        string strRequestorEmail = null, int? id = null,
        DateTime? startPeriod = null, DateTime? endPeriod = null)
        {
            if (string.IsNullOrEmpty(strRequestorEmail)) return;
            var strSubject = "";
            var strStatusBy = "";
            if (strStatus == "Approved")
            {
                strSubject = "Approval Notification of Timesheet";
                strStatusBy = " has been approved by ";
            }
            else if (strStatus == "Rejected")
            {
                strSubject = "Rejection Notification of Timesheet";
                strStatusBy = " has been rejected by ";
            }
            var strBody = "";

            strBody += "Dear Mr/Ms. " + strRequestorName + ",<br/><br/>";
            strBody += @"This is to notify you that the Timesheet Period  "
            + Convert.ToDateTime(startPeriod).ToString("MMM-dd-yyyy") + " to "
            + Convert.ToDateTime(endPeriod).ToString("MMM-dd-yyyy") + strStatusBy + strNameApprover + " <br/>";
            strBody += "<br/>Please kindly find attached links :<br/>";
            strBody += _siteUrl + UrlResource.InsuranceClaim + "/EditForm.aspx?ID=" + id;
            strBody += "<br/><br/>Thank you for your attention.";

            List<string> lstEmail = new List<string> { strRequestorEmail };

            EmailUtil.SendMultiple(lstEmail, strSubject, strBody);


        }

        public int CreateHeader(TimesheetVM header)
        {

            int ID = 0;

            var strGuid = Guid.NewGuid().ToString();
            var columnValues = new Dictionary<string, object>
           {
               {"timesheetstatus", header.TimesheetStatus},
                {"TimesheetGUID", strGuid}
           };

            var strPeriod = Convert.ToDateTime(header.Period).ToString("MMMM") + " " + Convert.ToDateTime(header.Period).ToString("yyyy");

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

            columnValues.Add("visibleto", SPConnector.GetUser(header.ProfesionalUserLogin, _siteUrl, "hr"));

            try
            {
                SPConnector.AddListItem(LIST_TIME, columnValues, _siteUrl);
                ID = SPConnector.GetLatestListItemIdbyGuid(LIST_TIME, _siteUrl, strGuid);
                if (header.TimesheetStatus == "Pending Approval 1 of 3")
                {
                    if (!header.WorkflowItems.Any()) return ID;
                    var wf = header.WorkflowItems;
                    var strEmail = wf.FirstOrDefault().ApproverEmail;
                    var strName = wf.FirstOrDefault().ApproverNameText;
                    SendEmailSubmitForApproval(strName, header.Name, strEmail, ID, header.StartPeriod,
                        header.EndPeriod);
                }

            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }

            return ID;
        }

        public void UpdateHeader(TimesheetVM header)
        {
            var columnValues = new Dictionary<string, object>
           {
               {"timesheetstatus", header.TimesheetStatus}
           };


            try
            {
                SPConnector.UpdateSingleListItemAsync(LIST_TIME, header.ID, columnValues, _siteUrl);
                if (header.TimesheetStatus == "Pending Approval 1 of 3")
                {
                    if (!header.WorkflowItems.Any()) return;
                    var wf = header.WorkflowItems;
                    var strEmail = wf.FirstOrDefault().ApproverEmail;
                    var strName = wf.FirstOrDefault().ApproverNameText;
                    SendEmailSubmitForApproval(strName, header.ProfessionalName.Text, strEmail, header.ID, header.StartPeriod,
                        header.EndPeriod);
                }
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }
        }
    }
}
