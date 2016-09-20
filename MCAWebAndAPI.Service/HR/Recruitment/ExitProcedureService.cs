using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Model.HR.DataMaster;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using NLog;
using System.Web;
using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Model.ViewModel.Control;
using MCAWebAndAPI.Model.Common;


namespace MCAWebAndAPI.Service.HR.Recruitment
{
    public class ExitProcedureService : IExitProcedureService
    {
        string _siteUrl;
        static Logger logger = LogManager.GetCurrentClassLogger();

        const string SP_EXP_LIST_NAME = "Exit Procedure";
        const string SP_EXP_CHECK_LIST_NAME = "Exit Procedure Checklist";
        const string SP_EXP_DOC_LIST_NAME = "Exit Procedure Documents";
        const string SP_EXP_WF_LIST_NAME = "Exit Procedure Workflow";
        const string SP_WORKFLOW_LISTNAME = "Workflow Mapping Master";
        const string SP_PROMAS_LIST_NAME = "Professional Master";
        const string SP_POSMAS_LIST_NAME = "Position Master";
        const string SP_PSA_LIST_NAME = "PSA";


        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
        }

        //Display Exit Procedure Data based on ID

        public int CreateExitProcedure(ExitProcedureVM exitProcedure)
        {
            var updatedValues = new Dictionary<string, object>();
            var statusExitProcedure = "Pending Approval";
            
            updatedValues.Add("Title", exitProcedure.FullName);
            updatedValues.Add("requestdate", exitProcedure.RequestDate);
            updatedValues.Add("professional", new FieldLookupValue { LookupId = (int)exitProcedure.ProfessionalID });
            updatedValues.Add("projectunit", exitProcedure.ProjectUnit);
            updatedValues.Add("position", exitProcedure.Position);
            updatedValues.Add("joindate", exitProcedure.ProfessionalJoinDate);
            updatedValues.Add("mobilenumber", exitProcedure.PhoneNumber);
            updatedValues.Add("officeemail", exitProcedure.ProfessionalPersonalMail);
            updatedValues.Add("currentaddress", exitProcedure.CurrentAddress);
            updatedValues.Add("lastworkingdate", exitProcedure.LastWorkingDate);
            updatedValues.Add("exitreason", exitProcedure.ExitReason.Value);
            updatedValues.Add("reasondescription", exitProcedure.ReasonDesc);

            if(exitProcedure.PSANumber == null)
            {
                string psaNumber = GetPSANumber(exitProcedure.FullName, exitProcedure.Position, exitProcedure.ProjectUnit, "Active");
                updatedValues.Add("psanumber", psaNumber);
            }
            else
            {
                updatedValues.Add("psanumber", exitProcedure.PSANumber);
            }
            
            if (exitProcedure.StatusForm == "Draft")
            {
                statusExitProcedure = "Draft";

                var professionalData = SPConnector.GetListItem(SP_PROMAS_LIST_NAME, exitProcedure.ProfessionalID, _siteUrl);
                string professionalOfficeMail = Convert.ToString(professionalData["officeemail"]);

                updatedValues.Add("visibleto", SPConnector.GetUser(professionalOfficeMail, _siteUrl));
            }
            if (exitProcedure.StatusForm == "Pending Approval")
            {
                statusExitProcedure = "Pending Approval";

                var professionalData = SPConnector.GetListItem(SP_PROMAS_LIST_NAME, exitProcedure.ProfessionalID, _siteUrl);
                string professionalOfficeMail = Convert.ToString(professionalData["officeemail"]);

                updatedValues.Add("visibleto", SPConnector.GetUser(professionalOfficeMail, _siteUrl));
            }
            if (exitProcedure.StatusForm == "Saved by HR")
            {
                statusExitProcedure = "Draft";

                var professionalData = SPConnector.GetListItem(SP_PROMAS_LIST_NAME, exitProcedure.ProfessionalID, _siteUrl);
                string professionalOfficeMail = Convert.ToString(professionalData["officeemail"]);

                updatedValues.Add("visibleto", SPConnector.GetUser(professionalOfficeMail, _siteUrl));
            }
            if (exitProcedure.StatusForm == "Approved by HR")
            {
                statusExitProcedure = "Approved";

                var professionalData = SPConnector.GetListItem(SP_PROMAS_LIST_NAME, exitProcedure.ProfessionalID, _siteUrl);
                string professionalOfficeMail = Convert.ToString(professionalData["officeemail"]);

                updatedValues.Add("visibleto", SPConnector.GetUser(professionalOfficeMail, _siteUrl));
            }

            updatedValues.Add("status", statusExitProcedure);
            
            try
            {
                SPConnector.AddListItem(SP_EXP_LIST_NAME, updatedValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                throw e;
            }

            return SPConnector.GetLatestListItemID(SP_EXP_LIST_NAME, _siteUrl);
        }

        public ExitProcedureVM GetExitProcedure(int? ID, string siteUrl, string requestor, string listName, string user)
        {
            var viewModel = new ExitProcedureVM();

            if (ID == null)
            {
                viewModel = GetWorkflowExitProcedure(listName, requestor, user);

                return viewModel;
            }
            
            var listItem = SPConnector.GetListItem(SP_EXP_LIST_NAME, ID, _siteUrl);
            viewModel = ConvertToExitProcedureVM(listItem);
            viewModel = GetExitProcedureDetails(viewModel);

            return viewModel;
        }

        public ExitProcedureVM GetExitProcedureHR(int? ID, string siteUrl)
        {
            var viewModel = new ExitProcedureVM();

            //return viewModel;

            if (ID == null)
            {
                //viewModel = GetWorkflowExitProcedure(listName, requestor, user);
                return viewModel;
            }

            var listItem = SPConnector.GetListItem(SP_EXP_LIST_NAME, ID, _siteUrl);
            viewModel = ConvertToExitProcedureVM(listItem);
            //viewModel = GetExitProcedureDetails(viewModel);

            return viewModel;
        }

        public ExitProcedureVM GetExitChecklistForHR(int? ID, string siteUrl, string professionalMail, string listName)
        {
            var viewModel = new ExitProcedureVM();

            if (ID == null)
            {
             
                viewModel = GetWorkflowExitProcedureHR(listName, professionalMail);

                return viewModel;
            }

            //var listItem = SPConnector.GetListItem(SP_EXP_LIST_NAME, ID, _siteUrl);
            //viewModel = ConvertToExitProcedureVM(listItem);
            //viewModel = GetExitProcedureDetails(viewModel);

            return viewModel;
        }

        public ExitProcedureForApproverVM GetExitProcedureApprover(int? ID, string siteUrl, string requestor, int? level)
        {
            var viewModel = new ExitProcedureForApproverVM();

            var caml = @" <View><ViewFields>
      <FieldRef Name='approver0' />
      <FieldRef Name='approverlevel' />
      <FieldRef Name='exitprocedure' />
      <FieldRef Name='requestor0' />
      <FieldRef Name='Title' />
        <FieldRef Name='exitchecklist' />
   </ViewFields>
    <Query>
   <Where>
      <And>
         <And>
            <Eq>
               <FieldRef Name='approver0' />
               <Value Type='Text'>" + requestor + @"</Value>
            </Eq>
            <Eq>
               <FieldRef Name='approverlevel' />
               <Value Type='Choice'>"+ level +@"</Value>
            </Eq>
         </And>
         <Eq>
            <FieldRef Name='ID' />
            <Value Type='Counter'>"+ ID + @"</Value>
         </Eq>
      </And>
   </Where></Query></View>";


            foreach (var item in SPConnector.GetList(SP_EXP_WF_LIST_NAME, _siteUrl, caml))
            {
                viewModel.ApproverLevel = Convert.ToInt32(item["approverlevel"]);
                viewModel.ApproverMail = Convert.ToString(item["approver"]);
                viewModel.ExitProcedureID = Convert.ToInt32(item["exitprocedure"]);
                viewModel.ItemExitProcedure = Convert.ToString(item["Title"]);
                viewModel.RequestorMail = Convert.ToString(item["requestor"]);
                viewModel.ExitCheckListID = Convert.ToInt32(item["exitchecklist"]);
            }

            return viewModel;
        }

        public ExitProcedureVM GetWorkflowExitProcedure(string listName, string requestor, string user)
        {
            var viewModel = new ExitProcedureVM();
            viewModel.ListName = listName;

            // Get Position in Professional Master
            var caml = @"<View><Query><Where><Eq>
                <FieldRef Name='officeemail' /><Value Type='Text'>" + requestor +
                @"</Value></Eq></Where></Query></View>";

            int? positionID = 0;

            foreach (var item in SPConnector.GetList(SP_PROMAS_LIST_NAME, _siteUrl, caml))
            {
                viewModel.Position = FormatUtil.ConvertLookupToValue(item, "Position");
                viewModel.RequestorPosition = FormatUtil.ConvertLookupToValue(item, "Position");
                viewModel.FullName = Convert.ToString(item["Title"]);
                viewModel.ProjectUnit = Convert.ToString(item["Project_x002f_Unit"]);
                positionID = FormatUtil.ConvertLookupToID(item, "Position");
                viewModel.ProfessionalID = Convert.ToInt32(item["ID"]);
                    
                viewModel.ProfessionalJoinDate = GetJoinDate(viewModel.FullName, viewModel.Position, viewModel.ProjectUnit, "Active");

                //viewModel.ProfessionalJoinDate = Convert.ToDateTime(item["Join_x0020_Date"]).ToLocalTime();
                viewModel.RequestorMailAddress = Convert.ToString(item["officeemail"]);
                viewModel.ProfessionalPersonalMail = Convert.ToString(item["personalemail"]);

                string psaNumber = GetPSANumber(viewModel.FullName, viewModel.Position, viewModel.ProjectUnit, "Active");
                viewModel.PSANumber = psaNumber;
                break;
            }

            // Get Unit in Position Master
            var position = SPConnector.GetListItem(SP_POSMAS_LIST_NAME, positionID, _siteUrl);
            viewModel.RequestorUnit = Convert.ToString(position["projectunit"]);
            viewModel.ProjectUnit = Convert.ToString(position["projectunit"]);

            // Get List of Workflow Items based on List name, Requestor Position, and Requestor Unit
            caml = @"<View>
            <Query> 
               <Where><And><And><Eq><FieldRef Name='requestorposition' /><Value Type='Lookup'>" + viewModel.RequestorPosition +
               @"</Value></Eq><Eq><FieldRef Name='requestorunit' /><Value Type='Choice'>" + viewModel.RequestorUnit + @"</Value></Eq></And><Eq>
               <FieldRef Name='transactiontype' /><Value Type='Choice'>" + listName + @"</Value></Eq></And></Where> 
            <OrderBy><FieldRef Name='approverlevel' /></OrderBy>
            </Query>
                
            </View>";

            var exitProcedureCheckList = new List<ExitProcedureChecklistVM>();
            foreach (var item in SPConnector.GetList(SP_WORKFLOW_LISTNAME, _siteUrl, caml))
            {
                if (string.Compare(Convert.ToString(item["isdefault"]), "No",
                    StringComparison.OrdinalIgnoreCase) == 0
                    &&
                    string.Compare(Convert.ToString(item["workflowtype"]), "Sequential",
                    StringComparison.OrdinalIgnoreCase) == 0)
                    continue;

                var vm = ConvertToExitProcedureChecklistVM(item);
                exitProcedureCheckList.Add(vm);
            }

            
            viewModel.ExitProcedureChecklist = exitProcedureCheckList;

            return viewModel;
        }

        private DateTime GetJoinDate (string professionalName, string position, string projectUnit, string psaStatus)
        {
            DateTime professionalJoinDate = DateTime.Now;

            var camlPSAData = @"<View>  
            <Query> 
               <Where><And><And><And><Eq><FieldRef Name='ProjectOrUnit' /><Value Type='Choice'>" + projectUnit + @"</Value></Eq><Eq><FieldRef Name='position' /><Value Type='Lookup'>" + position + @"</Value></Eq></And><Eq><FieldRef Name='professionalfullname' /><Value Type='Text'>" + professionalName + @"</Value></Eq></And><Eq><FieldRef Name='psastatus' /><Value Type='Text'>" + psaStatus + @"</Value></Eq></And></Where> 
            </Query> 
      </View>";

            foreach(var psaData in SPConnector.GetList(SP_PSA_LIST_NAME, _siteUrl, camlPSAData))
            {
                professionalJoinDate = Convert.ToDateTime(psaData["joindate"]).ToLocalTime();
            }

            return professionalJoinDate;
        }

        private string GetPSANumber(string professionalName, string positionName, string projectUnit, string psaStatus)
        {
            string psaNumber = "";

            var camlPSA = @"<View>  
            <Query> 
               <Where><And><And><And><Eq><FieldRef Name='professional' /><Value Type='Lookup'>" + professionalName + @"</Value></Eq><Eq><FieldRef Name='ProjectOrUnit' /><Value Type='Choice'>" + projectUnit + @"</Value></Eq></And><Eq><FieldRef Name='position' /><Value Type='Lookup'>" + positionName + @"</Value></Eq></And><Eq><FieldRef Name='psastatus' /><Value Type='Text'>" + psaStatus +@"</Value></Eq></And></Where> 
            </Query> 
      </View>";

            foreach(var psaData in SPConnector.GetList(SP_PSA_LIST_NAME, _siteUrl, camlPSA))
            {
                psaNumber = Convert.ToString(psaData["Title"]);
                break;
            }

            return psaNumber;
        }

        public ExitProcedureVM GetWorkflowExitProcedureHR(string listName, string professionalMail)
        {
            var viewModel = new ExitProcedureVM();
            viewModel.ListName = listName;

            // Get Position in Professional Master
            var caml = @"<View><Query><Where><Eq>
                <FieldRef Name='officeemail' /><Value Type='Text'>" + professionalMail +
                @"</Value></Eq></Where></Query></View>";

            int? positionID = 0;

            foreach (var item in SPConnector.GetList(SP_PROMAS_LIST_NAME, _siteUrl, caml))
            {
                viewModel.RequestorPosition = FormatUtil.ConvertLookupToValue(item, "Position");
                viewModel.FullName = Convert.ToString(item["Title"]) + " " + Convert.ToString(item["lastname"]);
                viewModel.ProjectUnit = Convert.ToString(item["Project_x002f_Unit"]);
                positionID = FormatUtil.ConvertLookupToID(item, "Position");
                viewModel.ProfessionalID = Convert.ToInt32(item["ID"]);
                viewModel.ProfessionalJoinDate = Convert.ToDateTime(item["Join_x0020_Date"]).ToLocalTime();
                viewModel.RequestorMailAddress = Convert.ToString(item["officeemail"]);
                viewModel.ProfessionalPersonalMail = Convert.ToString(item["personalemail"]);

                string psaNumber = GetPSANumber(viewModel.FullName, viewModel.Position, viewModel.ProjectUnit, "Active");
                viewModel.PSANumber = psaNumber;
                
                break;
            }

            // Get Unit in Position Master
            var position = SPConnector.GetListItem(SP_POSMAS_LIST_NAME, positionID, _siteUrl);
            viewModel.RequestorUnit = Convert.ToString(position["projectunit"]);


            // Get List of Workflow Items based on List name, Requestor Position, and Requestor Unit
            caml = @"<View>
            <Query> 
               <Where><And><And><Eq><FieldRef Name='requestorposition' /><Value Type='Lookup'>" + viewModel.RequestorPosition +
               @"</Value></Eq><Eq><FieldRef Name='requestorunit' /><Value Type='Choice'>" + viewModel.RequestorUnit + @"</Value></Eq></And><Eq>
               <FieldRef Name='transactiontype' /><Value Type='Choice'>" + listName + @"</Value></Eq></And></Where> 
            <OrderBy><FieldRef Name='approverlevel' /></OrderBy>
            </Query>
                
            </View>";

            var exitProcedureCheckList = new List<ExitProcedureChecklistVM>();
            foreach (var item in SPConnector.GetList(SP_WORKFLOW_LISTNAME, _siteUrl, caml))
            {
                if (string.Compare(Convert.ToString(item["isdefault"]), "No",
                    StringComparison.OrdinalIgnoreCase) == 0
                    &&
                    string.Compare(Convert.ToString(item["workflowtype"]), "Sequential",
                    StringComparison.OrdinalIgnoreCase) == 0)
                    continue;

                var vm = ConvertToExitProcedureChecklistVM(item);
                exitProcedureCheckList.Add(vm);
            }


            viewModel.ExitProcedureChecklist = exitProcedureCheckList;

            return viewModel;
        }

        private ExitProcedureChecklistVM ConvertToExitProcedureChecklistVM(ListItem item)
        {

            var viewModel = new ExitProcedureChecklistVM();
            viewModel.ApproverPosition = FormatUtil.ConvertToInGridAjaxComboBox(item, "approverposition");

            viewModel.ProfessionalPosition = FormatUtil.ConvertToInGridAjaxComboBox(item, "requestorposition");

            viewModel.IsDefault = Convert.ToString(item["isdefault"]);

            viewModel.IsDefaultTemp =
                ExitProcedureChecklistVM.GetIsDefaultTempValue(new InGridComboBoxVM
                {
                    Text = Convert.ToString(item["approverunit"])
                });

            viewModel.ApproverUnit =
                ExitProcedureChecklistVM.GetUnitDefaultValue(new InGridComboBoxVM
                {
                    Text = Convert.ToString(item["approverunit"])
                });

            viewModel.ProfessionalUnit =
                ExitProcedureChecklistVM.GetProfessionalUnitDefaultValue(new InGridComboBoxVM
                {
                    Text = Convert.ToString(item["requestorunit"])
                });

            var approvernames = GetApproverUserName(viewModel.ApproverPosition.Text, viewModel.ApproverUnit.Text);

            viewModel.Level = Convert.ToString(item["approverlevel"]);

            viewModel.WorkflowType = Convert.ToString(item["workflowtype"]);

            if (viewModel.Level == "1")
            {
                viewModel.ItemExitProcedure = "Close-Out/Handover Report";
                viewModel.Remarks = "";
            }
            else if (viewModel.Level == "2")
            {
                viewModel.ItemExitProcedure = "MCA Indonesia Propietary Information";
                viewModel.Remarks = "";
            }
            else if (viewModel.Level == "3")
            {
                viewModel.ItemExitProcedure = "Laptop/Desktop";
                viewModel.Remarks = "";
            }
            else if (viewModel.Level == "4")
            {
                viewModel.ItemExitProcedure = "SAP Password, Computer Password";
                viewModel.Remarks = "";
            }
            else if (viewModel.Level == "5")
            {
                viewModel.ItemExitProcedure = "IT Tools";
                viewModel.Remarks = "";
            }
            else if (viewModel.Level == "6")
            {
                viewModel.ItemExitProcedure = "Keys (Drawers,desk,etc)";
                viewModel.Remarks = "";
            }
            else if (viewModel.Level == "7")
            {
                viewModel.ItemExitProcedure = "Car";
                viewModel.Remarks = "";
            }
            else if (viewModel.Level == "8")
            {
                viewModel.ItemExitProcedure = "Advance Statement";
                viewModel.Remarks = "Rp 5.000.000";

            }
            else if (viewModel.Level == "9")
            {
                viewModel.ItemExitProcedure = "Travel Statement";
                viewModel.Remarks = "Rp 2.000.000";
            }
            else if (viewModel.Level == "10")
            {
                viewModel.ItemExitProcedure = "Resignation/Separation Letter";
                viewModel.Remarks = "";
            }
            else if (viewModel.Level == "11")
            {
                viewModel.ItemExitProcedure = "Timesheet/Leave Form";
                viewModel.Remarks = "";
            }
            else if (viewModel.Level == "12")
            {
                viewModel.ItemExitProcedure = "Exit Interview/NDA";
                viewModel.Remarks = "";
            }
            else if (viewModel.Level == "13")
            {
                viewModel.ItemExitProcedure = "Insurance Card";
                viewModel.Remarks = "";
            }
            else if (viewModel.Level == "14")
            {
                viewModel.ItemExitProcedure = "ID Card & Access Card";
                viewModel.Remarks = "";
            }


            viewModel.CheckListItemApproval =
                ExitProcedureChecklistVM.GetCheckListItemApprovalDefaultValue();

            
            if (viewModel.CheckListItemApproval.Text == "Pending Approval")
            {
                viewModel.ApprovalIndicator = "red";
            }
            if(viewModel.CheckListItemApproval.Text == "Approved")
            {
                viewModel.ApprovalIndicator = "green";
            }
            
            var userName = approvernames.FirstOrDefault();
            viewModel.ApproverUserName = AjaxComboBoxVM.GetDefaultValue(new AjaxComboBoxVM
            {
                Text = userName.Name,
                Value = userName.ID
            });

            viewModel.ApprovalMail = userName.UserLogin;

            return viewModel;
        }

        

        public async Task CreateExitProcedureChecklistAsync(ExitProcedureVM exitProcedure, int? exitProcID, IEnumerable<ExitProcedureChecklistVM> exitProcedureChecklist, string requestorposition, string requestorunit, int? positionID)
        {
            CreateExitProcedureChecklist(exitProcedure, exitProcID, exitProcedureChecklist, requestorposition, requestorunit, positionID);
        }
        
        public void CreateExitProcedureChecklist(ExitProcedureVM exitProcedure, int? exitProcID, IEnumerable<ExitProcedureChecklistVM> ExitProcedureChecklist, string requestorposition, string requestorunit, int? positionID)
        {

            //int i = 1;

            foreach (var viewModel in ExitProcedureChecklist)
            {
                if (Item.CheckIfSkipped(viewModel))
                    continue;
                if (Item.CheckIfDeleted(viewModel))
                {
                    try
                    {
                        SPConnector.DeleteListItem(SP_EXP_CHECK_LIST_NAME, viewModel.ID, _siteUrl);
                    }
                    catch (Exception e)
                    {
                        logger.Error(e);
                        throw e;
                    }
                    continue;
                }
                var updatedValue = new Dictionary<string, object>();

                updatedValue.Add("Title", viewModel.ItemExitProcedure);
                updatedValue.Add("approverposition", new FieldLookupValue { LookupId = Convert.ToInt32(viewModel.ApproverPosition.Value) });
                updatedValue.Add("approverusername", new FieldLookupValue { LookupId = Convert.ToInt32(viewModel.ApproverUserName.Value) });
                updatedValue.Add("checklistitemapproval", viewModel.CheckListItemApproval.Text);
                updatedValue.Add("dateofapproval", viewModel.DateOfApproval);
                updatedValue.Add("remarks", viewModel.Remarks);
                updatedValue.Add("requestorunit", requestorunit);
                updatedValue.Add("requestorposition", new FieldLookupValue { LookupId = Convert.ToInt32(positionID) });
                updatedValue.Add("approverlevel", viewModel.Level);
                updatedValue.Add("approverunit", viewModel.ApproverUnit.Text);
                updatedValue.Add("isdefault", viewModel.IsDefault);

                var item = SPConnector.GetListItem(SP_PROMAS_LIST_NAME, viewModel.ApproverUserName.Value, _siteUrl);
                updatedValue.Add("approvalmail", Convert.ToString(item["officeemail"]));

                //var updatedHeaderValue = new Dictionary<string, object>();
                
                //if (i == 1)
                //{
                //    updatedHeaderValue.Add("visibletoapprover1", Convert.ToString(item["officeemail"]));

                //    try
                //    {
                //        SPConnector.UpdateListItem(SP_EXP_LIST_NAME, exitProcedure.ID, updatedHeaderValue, _siteUrl);
                //    }
                //    catch (Exception e)
                //    {
                //        logger.Error(e.Message);
                //        throw new Exception(ErrorResource.SPInsertError);
                //    }
                //}
                //if(i == 2)
                //{
                //    updatedHeaderValue.Add("visibletoapprover2", Convert.ToString(item["officeemail"]));

                //    try
                //    {
                //        SPConnector.UpdateListItem(SP_EXP_LIST_NAME, exitProcedure.ID, updatedHeaderValue, _siteUrl);
                //    }
                //    catch (Exception e)
                //    {
                //        logger.Error(e.Message);
                //        throw new Exception(ErrorResource.SPInsertError);
                //    }
                //}
                //if(i == 3)
                //{
                //    updatedHeaderValue.Add("visibletoapprover3", Convert.ToString(item["officeemail"]));

                //    try
                //    {
                //        SPConnector.UpdateListItem(SP_EXP_LIST_NAME, exitProcedure.ID, updatedHeaderValue, _siteUrl);
                //    }
                //    catch (Exception e)
                //    {
                //        logger.Error(e.Message);
                //        throw new Exception(ErrorResource.SPInsertError);
                //    }
                //}
                //if(i == 4)
                //{
                //    updatedHeaderValue.Add("visibletoapprover4", Convert.ToString(item["officeemail"]));

                //    try
                //    {
                //        SPConnector.UpdateListItem(SP_EXP_LIST_NAME, exitProcedure.ID, updatedHeaderValue, _siteUrl);
                //    }
                //    catch (Exception e)
                //    {
                //        logger.Error(e.Message);
                //        throw new Exception(ErrorResource.SPInsertError);
                //    }
                //}
                //if(i == 5)
                //{
                //    updatedHeaderValue.Add("visibletoapprover5", Convert.ToString(item["officeemail"]));

                //    try
                //    {
                //        SPConnector.UpdateListItem(SP_EXP_LIST_NAME, exitProcedure.ID, updatedHeaderValue, _siteUrl);
                //    }
                //    catch (Exception e)
                //    {
                //        logger.Error(e.Message);
                //        throw new Exception(ErrorResource.SPInsertError);
                //    }
                //}

                //if((viewModel.ApprovalMail != null) || (viewModel.ApprovalMail == null))
                //{
                //    var item = SPConnector.GetListItem(SP_PROMAS_LIST_NAME, viewModel.ApproverUserName.Value, _siteUrl);

                //    updatedValue.Add("approvalmail", Convert.ToString(item["officeemail"]));
                //}

                if (exitProcedure.StatusForm == "Pending Approval")
                {
                    var startDateApproval = exitProcedure.StartDateApproval.ToLocalTime().ToShortDateString();
                    updatedValue.Add("startdateapproval", startDateApproval);
                }
                
                updatedValue.Add("exitprocedure", new FieldLookupValue { LookupId = Convert.ToInt32(exitProcID) });

                try
                {
                    if (Item.CheckIfUpdated(viewModel))
                        SPConnector.UpdateListItem(SP_EXP_CHECK_LIST_NAME, viewModel.ID, updatedValue, _siteUrl);
                    else
                    {
                        SPConnector.AddListItem(SP_EXP_CHECK_LIST_NAME, updatedValue, _siteUrl);
                    }
                        
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw new Exception(ErrorResource.SPInsertError);
                }

                //i++;
            }
        }

        public ExitProcedureVM GetExitProcedureForApprove(int? ID, string siteUrl, string approver)
        {
            var viewModel = new ExitProcedureVM();

            var listItem = SPConnector.GetListItem(SP_EXP_LIST_NAME, ID, _siteUrl);
            viewModel = ConvertToExitProcedureVM(listItem);
            viewModel = GetExitProcedureDetailsForApprove(viewModel, approver, "Pending Approval");

            return viewModel;
        }

        private ExitProcedureVM ConvertToExitProcedureVM(ListItem listItem)
        {
            var viewModel = new ExitProcedureVM();

            viewModel.ID = Convert.ToInt32(listItem["ID"]);
            viewModel.RequestDate = Convert.ToDateTime(listItem["requestdate"]).ToLocalTime();
            viewModel.FullName = Convert.ToString(listItem["Title"]);
            viewModel.Professional.Value = FormatUtil.ConvertLookupToID(listItem, "professional");
            viewModel.ProfessionalID = Convert.ToInt32(viewModel.Professional.Value);
            viewModel.ProjectUnit = Convert.ToString(listItem["projectunit"]);
            viewModel.RequestorUnit = Convert.ToString(listItem["projectunit"]);
            //viewModel.Position = FormatUtil.ConvertLookupToValue(listItem, "position");
            viewModel.RequestorPosition = Convert.ToString(listItem["position"]);
            viewModel.Position = Convert.ToString(listItem["position"]);
            viewModel.PhoneNumber = Convert.ToString(listItem["mobilenumber"]);
            viewModel.ProfessionalPersonalMail = Convert.ToString(listItem["officeemail"]);
            viewModel.CurrentAddress = Convert.ToString(listItem["currentaddress"]);
            viewModel.JoinDate = Convert.ToDateTime(listItem["joindate"]).ToLocalTime();
            viewModel.ProfessionalJoinDate = Convert.ToDateTime(listItem["joindate"]).ToLocalTime();
            viewModel.LastWorkingDate = Convert.ToDateTime(listItem["lastworkingdate"]).ToLocalTime();
            viewModel.ExitReason.Value = Convert.ToString(listItem["exitreason"]);
            viewModel.ReasonDesc = Convert.ToString(listItem["reasondescription"]);
            viewModel.PSANumber = Convert.ToString(listItem["psanumber"]);
            viewModel.Requestor = Convert.ToString(listItem["Title"]);
            viewModel.StatusForm = Convert.ToString(listItem["status"]);

            viewModel.ExitProcedureChecklist = GetExitProcedureChecklist(viewModel.ID);
            viewModel.DocumentUrl = GetDocumentUrl(viewModel.ID);

            return viewModel;
        }


        
        public ExitProcedureVM GetExitProcedure(int? ID)
        {
            var viewModel = new ExitProcedureVM();

            if (ID == null)
            {
                return viewModel;
            }

            var listItem = SPConnector.GetListItem(SP_EXP_LIST_NAME, ID, _siteUrl);
            viewModel = ConvertToExitProcedureVM(listItem);
            viewModel = GetExitProcedureDetails(viewModel);

            return viewModel;
        }
              

        private ExitProcedureVM GetExitProcedureDetails(ExitProcedureVM viewModel)
        {
            viewModel.ExitProcedureChecklist = GetExitProcedureChecklist(viewModel.ID);

            return viewModel;
        }
        private ExitProcedureVM GetExitProcedureDetailsForApprove(ExitProcedureVM viewModel, string approver, string approvalStatus)
        {
            viewModel.ExitProcedureChecklist = GetExitProcedureChecklistForApprove(viewModel.ID, approver, approvalStatus);

            return viewModel;
        }

        private IEnumerable<ExitProcedureChecklistVM> GetExitProcedureChecklistForApprove(int? iD, string approver, string approvalStatus)
        {
            var camlapprover = @"<View>  
            <Query> 
               <Where><And><And><Eq><FieldRef Name='exitprocedure' /><Value Type='Lookup'>" + iD + @"</Value></Eq><Eq><FieldRef Name='approvalmail' /><Value Type='Text'>" + approver + @"</Value></Eq></And><Eq><FieldRef Name='checklistitemapproval' /><Value Type='Choice'>" + approvalStatus + @"</Value></Eq></And></Where> 
            </Query> 
      </View>";

            var exitProcedureChecklist = new List<ExitProcedureChecklistVM>();

            foreach (var item in SPConnector.GetList(SP_EXP_CHECK_LIST_NAME, _siteUrl, camlapprover))
            {
                exitProcedureChecklist.Add(ConvertToExitProcedureChecklistForApprove(item));
            }

            return exitProcedureChecklist;
        }

        private ExitProcedureChecklistVM ConvertToExitProcedureChecklistForApprove(ListItem item)
        {
            return new ExitProcedureChecklistVM
            {
                ID = Convert.ToInt32(item["ID"]),
                ItemExitProcedure = Convert.ToString(item["Title"]),
                Remarks = Convert.ToString(item["remarks"]),
                DateOfApproval = Convert.ToDateTime(item["dateofapproval"]).ToLocalTime(),
                CheckListItemApproval = ExitProcedureChecklistVM.GetCheckListItemApprovalDefaultValue(
                    new Model.ViewModel.Control.InGridComboBoxVM
                    {
                        Text = Convert.ToString(item["checklistitemapproval"])
                    }
                    )
            };

        }

        private IEnumerable<ExitProcedureChecklistVM> GetExitProcedureChecklist(int? iD)
        {
          var caml = @"<View>
            <Query> 
               <Where><Eq><FieldRef Name='exitprocedure' LookupId='True' /><Value Type='Lookup'>" + iD + @"</Value></Eq></Where> 
            </Query>  
            </View>";

            
            var exitProcedureChecklist = new List<ExitProcedureChecklistVM>();

            foreach (var item in SPConnector.GetList(SP_EXP_CHECK_LIST_NAME, _siteUrl, caml))
            {
                exitProcedureChecklist.Add(ConvertToExitProcedureChecklist(item));
            }

            return exitProcedureChecklist;
        }

        private ExitProcedureChecklistVM ConvertToExitProcedureChecklist(ListItem item)
        {
            var viewModel = new ExitProcedureChecklistVM();

            viewModel.ID = Convert.ToInt32(item["ID"]);
            viewModel.ItemExitProcedure = Convert.ToString(item["Title"]);
            viewModel.Remarks = Convert.ToString(item["remarks"]);
            viewModel.ApproverUnit = ExitProcedureChecklistVM.GetUnitDefaultValue(
                    new Model.ViewModel.Control.InGridComboBoxVM
                    {
                        Text = Convert.ToString(item["approverunit"])
                    }
                );
            viewModel.DateOfApproval = Convert.ToDateTime(item["dateofapproval"]).ToLocalTime();
            viewModel.CheckListItemApproval = ExitProcedureChecklistVM.GetCheckListItemApprovalDefaultValue(
                    new Model.ViewModel.Control.InGridComboBoxVM
                    {
                        Text = Convert.ToString(item["checklistitemapproval"])
                    }
                );
            viewModel.ApproverPosition = ExitProcedureChecklistVM.GetPositionDefaultValue(FormatUtil.ConvertToInGridAjaxComboBox(item, "approverposition"));
            viewModel.ApproverUserName = ExitProcedureChecklistVM.GetApproverUserNameDefaultValue(FormatUtil.ConvertToInGridAjaxComboBox(item, "approverusername"));
            viewModel.Level = Convert.ToString(item["approverlevel"]);
            
            if(viewModel.CheckListItemApproval.Text == "Pending Approval")
            {
                viewModel.ApprovalIndicator = "red";
            }
            else
            {
                viewModel.ApprovalIndicator = "green";
            }

            return viewModel;
        }

        private string GetDocumentUrl(int? iD)
        {
            return string.Format(UrlResource.ExitProcedureDocumentByID, _siteUrl, iD);
        }

        public bool UpdateExitProcedure(ExitProcedureVM exitProcedure)
        {
            var columnValues = new Dictionary<string, object>();
            int ID = exitProcedure.ID.Value;
            
            columnValues.Add("requestdate", exitProcedure.RequestDate.Value);
            //columnValues.Add("professional", new FieldLookupValue { LookupId = Convert.ToInt32(exitProcedure.Professional.Value) });
            columnValues.Add("Title", exitProcedure.FullName);
            columnValues.Add("projectunit", exitProcedure.ProjectUnit);
            columnValues.Add("position", exitProcedure.Position);
            columnValues.Add("mobilenumber", exitProcedure.PhoneNumber);
            columnValues.Add("officeemail", exitProcedure.ProfessionalPersonalMail);
            columnValues.Add("currentaddress", exitProcedure.CurrentAddress);
            columnValues.Add("joindate", exitProcedure.JoinDate.Value);
            columnValues.Add("lastworkingdate", exitProcedure.LastWorkingDate.Value);
            columnValues.Add("exitreason", exitProcedure.ExitReason.Value);
            columnValues.Add("reasondescription", exitProcedure.ReasonDesc);
            columnValues.Add("psanumber", exitProcedure.PSANumber);

            if(exitProcedure.StatusForm == "Draft")
            {
                columnValues.Add("status", exitProcedure.StatusForm);
            }
            if(exitProcedure.StatusForm == "Pending Approval")
            {
                exitProcedure.StartDateApproval = DateTime.Now;

                //foreach(var professionalRecord in SPConnector.GetList())

                var professionalData = SPConnector.GetListItem(SP_PROMAS_LIST_NAME, exitProcedure.ProfessionalID, _siteUrl);
                string professionalOfficeMail = Convert.ToString(professionalData["officeemail"]);

                columnValues.Add("visibleto", SPConnector.GetUser(professionalOfficeMail, _siteUrl));
                columnValues.Add("startdateapproval", exitProcedure.StartDateApproval.ToLocalTime());
                columnValues.Add("status", exitProcedure.StatusForm);
            }

            try
            {
                SPConnector.UpdateListItem(SP_EXP_LIST_NAME, ID, columnValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                return false;
            }

            var entitiy = new ExitProcedureVM();
            entitiy = exitProcedure;
            return true;
        }

        public bool UpdateExitProcedureHR(ExitProcedureVM exitProcedure)
        {
            var columnValues = new Dictionary<string, object>();
            int ID = exitProcedure.ID.Value;

            columnValues.Add("requestdate", exitProcedure.RequestDate.Value);
            //columnValues.Add("professional", new FieldLookupValue { LookupId = Convert.ToInt32(exitProcedure.Professional.Value) });
            columnValues.Add("Title", exitProcedure.FullName);
            columnValues.Add("projectunit", exitProcedure.ProjectUnit);
            columnValues.Add("position", exitProcedure.Position);
            columnValues.Add("mobilenumber", exitProcedure.PhoneNumber);
            columnValues.Add("officeemail", exitProcedure.ProfessionalPersonalMail);
            columnValues.Add("currentaddress", exitProcedure.CurrentAddress);
            columnValues.Add("joindate", exitProcedure.JoinDate.Value);
            columnValues.Add("lastworkingdate", exitProcedure.LastWorkingDate.Value);
            columnValues.Add("exitreason", exitProcedure.ExitReason.Value);
            columnValues.Add("reasondescription", exitProcedure.ReasonDesc);
            columnValues.Add("psanumber", exitProcedure.PSANumber);

            if (exitProcedure.StatusForm == "Saved by HR")
            {
                string statusDraft = "Draft";

                columnValues.Add("status", statusDraft);
            }
            if (exitProcedure.StatusForm == "Approved by HR")
            {
                exitProcedure.StartDateApproval = DateTime.Now;

                string statusApproved = "Approved";

                var professionalData = SPConnector.GetListItem(SP_PROMAS_LIST_NAME, exitProcedure.ProfessionalID, _siteUrl);
                string professionalOfficeMail = Convert.ToString(professionalData["officeemail"]);

                columnValues.Add("visibleto", SPConnector.GetUser(professionalOfficeMail, _siteUrl));
                columnValues.Add("startdateapproval", exitProcedure.StartDateApproval.ToLocalTime());
                columnValues.Add("status", statusApproved);
            }

            try
            {
                SPConnector.UpdateListItem(SP_EXP_LIST_NAME, ID, columnValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                return false;
            }

            var entitiy = new ExitProcedureVM();
            entitiy = exitProcedure;
            return true;
        }

        public bool UpdateExitChecklist(ExitProcedureVM exitProcedure, IEnumerable<ExitProcedureChecklistVM> ExitProcedureChecklist)
        {
            foreach (var viewModel in ExitProcedureChecklist)
            {
                if (Item.CheckIfSkipped(viewModel))
                    continue;
                if (Item.CheckIfDeleted(viewModel))
                {
                    try
                    {
                        SPConnector.DeleteListItem(SP_EXP_CHECK_LIST_NAME, viewModel.ID, _siteUrl);

                    }
                    catch (Exception e)
                    {
                        logger.Error(e);
                        throw e;
                    }
                    continue;
                }
                var updatedValue = new Dictionary<string, object>();

                updatedValue.Add("checklistitemapproval", viewModel.CheckListItemApproval.Text);
                updatedValue.Add("dateofapproval", viewModel.DateOfApproval);
                
                try
                {
                    if (Item.CheckIfUpdated(viewModel))
                    {
                            SPConnector.UpdateListItem(SP_EXP_CHECK_LIST_NAME, viewModel.ID, updatedValue, _siteUrl);
                    }
                    else
                    {
                        SPConnector.AddListItem(SP_EXP_CHECK_LIST_NAME, updatedValue, _siteUrl);
                    }

                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw new Exception(ErrorResource.SPInsertError);
                }


                var itemJustApproved = SPConnector.GetListItem(SP_EXP_CHECK_LIST_NAME, viewModel.ID, _siteUrl);
                string statusApprovalItem = Convert.ToString(itemJustApproved["checklistitemapproval"]);

                if (statusApprovalItem == "Approved")
                {
                    var exitChecklistRecords = SPConnector.GetListItem(SP_EXP_CHECK_LIST_NAME, viewModel.ID, _siteUrl);
                    int exitProcedureID = Convert.ToInt32((exitChecklistRecords["exitprocedure"] as FieldLookupValue).LookupId);
                    string approvalMail = Convert.ToString(exitChecklistRecords["approvalmail"]);

                    var camlApprover = @"<View>  
            <Query> 
               <Where><Eq><FieldRef Name='officeemail' /><Value Type='Text'>" + approvalMail + @"</Value></Eq></Where> 
            </Query> 
      </View>";
                    string approverFullName = "";

                    foreach(var approver in SPConnector.GetList(SP_PROMAS_LIST_NAME, _siteUrl, camlApprover))
                    {
                        approverFullName = Convert.ToString(approver["Title"]) + " " + Convert.ToString(approver["lastname"]);
                        break;
                    }

                    var exitProcedureRecords = SPConnector.GetListItem(SP_EXP_LIST_NAME, exitProcedureID, _siteUrl);
                    int professionalID = Convert.ToInt32((exitProcedureRecords["professional_x003a_ID"] as FieldLookupValue).LookupId);
                    
                    var professionalRecords = SPConnector.GetListItem(SP_PROMAS_LIST_NAME, professionalID, _siteUrl);
                    string professionalMail = Convert.ToString(professionalRecords["officeemail"]);
                    string professionalName = Convert.ToString(professionalRecords["Title"]);


                    //SendAlreadyApproved(professionalMail, string.Format("Your Item: {0} already approved by {1}", viewModel.ItemExitProcedure, approverFullName));
                    SendAlreadyApproved(professionalMail, string.Format("Dear {0},{1}{2}Your item : {3} has been approved by {4}. Please contact respective person for any queries.{5}{6}Thank you.", professionalName, Environment.NewLine, Environment.NewLine, viewModel.ItemExitProcedure, approverFullName, Environment.NewLine, Environment.NewLine));
                }
            }

            return true;
        }

        public ExitProcedureVM ViewExitProcedure(int? ID)
        {
            var viewModel = new ExitProcedureVM();
            if (ID == null)
                return viewModel;

            var listItem = SPConnector.GetListItem(SP_EXP_LIST_NAME, ID, _siteUrl);
            viewModel = ConvertToViewExitProcedureVM(listItem);

            return viewModel;

        }

        private ExitProcedureVM ConvertToViewExitProcedureVM(ListItem listItem)
        {
            var viewModel = new ExitProcedureVM();

            viewModel.ID = Convert.ToInt32(listItem["ID"]);
            viewModel.RequestDate = Convert.ToDateTime(listItem["requestdate"]).ToLocalTime();
            viewModel.Professional.Text = FormatUtil.ConvertLookupToValue(listItem, "professional");
            viewModel.FullName = Convert.ToString(listItem["Title"]);
            viewModel.ProjectUnit = Convert.ToString(listItem["projectunit"]);
            viewModel.Position = Convert.ToString(listItem["position"]);
            viewModel.PhoneNumber = Convert.ToString(listItem["mobilenumber"]);
            viewModel.EmailAddress = Convert.ToString(listItem["officeemail"]);
            viewModel.CurrentAddress = Convert.ToString(listItem["currentaddress"]);
            viewModel.JoinDate = Convert.ToDateTime(listItem["joindate"]).ToLocalTime();
            viewModel.LastWorkingDate = Convert.ToDateTime(listItem["lastworkingdate"]).ToLocalTime();
            viewModel.ExitReason.Value = Convert.ToString(listItem["exitreason"]);
            viewModel.ReasonDesc = Convert.ToString(listItem["reasondescription"]);
            viewModel.PSANumber = Convert.ToString(listItem["psanumber"]);

            viewModel.DocumentUrl = GetDocumentUrl(viewModel.ID);

            return viewModel;
        }

        public void CreateExitProcedureDocuments(int? exProcID, IEnumerable<HttpPostedFileBase> documents, ExitProcedureVM exitProcedure)
        {
            foreach (var doc in documents)
            {
                var updateValue = new Dictionary<string, object>();

                exitProcedure.DocumentType = "Exit Procedure";

                updateValue.Add("documenttype", "Exit Procedure");
                updateValue.Add("exitprocedureid", new FieldLookupValue { LookupId = Convert.ToInt32(exProcID) });
                updateValue.Add("professional", new FieldLookupValue { LookupId = Convert.ToInt32(exitProcedure.ProfessionalID) });

                try
                {
                    SPConnector.UploadDocument(SP_EXP_DOC_LIST_NAME, updateValue, doc.FileName, doc.InputStream, _siteUrl);
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw e;
                }
            }
        }

        public IEnumerable<ProfessionalMaster> GetApproverNames(string position)
        {
            var caml = @"<View>  
            <Query> 
               <Where><Eq><FieldRef Name='Position' /><Value Type='Lookup'>" + position + @"</Value></Eq></Where> 
            </Query> 
             <ViewFields><FieldRef Name='ID' /><FieldRef Name='Title' /><FieldRef Name='officeemail' /></ViewFields> 
            </View>";

            var viewModel = new List<ProfessionalMaster>();
            foreach (var item in SPConnector.GetList(SP_PROMAS_LIST_NAME, _siteUrl, caml))
            {
                viewModel.Add(ConvertToProfessionalMasterVM(item));
            }

            return viewModel;
        }

        public IEnumerable<ProfessionalMaster> GetApproverUserName(string position, string projectunit)
        {
            var caml = @"<View>  
            <Query>
                <Where>
      <And>
         <Eq>
            <FieldRef Name='Project_x002f_Unit' />
            <Value Type='Choice'>" + projectunit + @"</Value>
         </Eq>
         <Eq>
            <FieldRef Name='Position' />
            <Value Type='Lookup'>" + position + @"</Value>
         </Eq>
      </And>
   </Where>
     </Query> 
             <ViewFields><FieldRef Name='ID' /><FieldRef Name='Title' /><FieldRef Name='officeemail' /></ViewFields> 
            </View>";

            var viewModel = new List<ProfessionalMaster>();
            foreach (var item in SPConnector.GetList(SP_PROMAS_LIST_NAME, _siteUrl, caml))
            {
                viewModel.Add(ConvertToProfessionalMasterVM(item));
            }

            return viewModel;
        }

        private ProfessionalMaster ConvertToProfessionalMasterVM(ListItem item)
        {
            return new ProfessionalMaster
            {
                ID = Convert.ToInt32(item["ID"]),
                Name = Convert.ToString(item["Title"]),
                UserLogin = Convert.ToString(item["officeemail"])
            };
        }

        public IEnumerable<PositionMaster> GetPositionsInWorkflow(string listName,
            string approverUnit, string requestorUnit, string requestorPosition)
        {
            var caml = @"<View>  
            <Query> 
               <Where><And><And><And><Eq>
                <FieldRef Name='approverunit' /><Value Type='Choice'>"
                    + approverUnit + @"</Value></Eq><Eq>
                <FieldRef Name='requestorposition' /><Value Type='Lookup'>"
                    + requestorPosition + @"</Value></Eq></And><Eq>
                <FieldRef Name='requestorunit' /><Value Type='Choice'>"
                    + requestorUnit + @"</Value></Eq></And><Eq>
                <FieldRef Name='transactiontype' /><Value Type='Choice'>"
                    + listName + @"</Value></Eq></And></Where> 
            </Query> 
                <ViewFields><FieldRef Name='approverposition' /></ViewFields> 
            </View>";

            var positions = new List<PositionMaster>();
            foreach (var item in SPConnector.GetList(SP_WORKFLOW_LISTNAME, _siteUrl, caml))
            {
                positions.Add(new PositionMaster
                {
                    ID = FormatUtil.ConvertLookupToID(item, "approverposition"),
                    PositionName = FormatUtil.ConvertLookupToValue(item, "approverposition")
                });
            }

            return positions;
        }

        public string GetPositionName(int position)
        {
            var item = SPConnector.GetListItem(SP_POSMAS_LIST_NAME, position, _siteUrl);
            return Convert.ToString(item["Title"]);
        }

        public void SendEmail(ExitProcedureVM header, string workflowTransactionListName, string transactionLookupColumnName, int exitProcID, string siteUrl, string urlResource, string requestorMail, string messageForRequestor)
        {
            var camlrequestor = @"<View>  
          <Query> 
               <Where><Eq><FieldRef Name='exitprocedure' /><Value Type='Lookup'>" + exitProcID + @" </Value></Eq></Where> 
            </Query> 
             <ViewFields>
                <FieldRef Name='requestor' />
             </ViewFields> 
            </View>";

            var camlapprover = @"<View>  
            <Query> 
               <Where><Eq><FieldRef Name='exitprocedure' /><Value Type='Lookup'>" + exitProcID + @"</Value></Eq></Where> 
            </Query> 
      </View>";
            
            var emails = new List<string>();
                        
            if (header.StatusForm == "Pending Approval")
            {
                foreach (var item in SPConnector.GetList(SP_EXP_CHECK_LIST_NAME, _siteUrl, camlapprover))
                {
                    emails.Add(Convert.ToString(item["approvalmail"]));
                }
                foreach (var item in emails)
                {
                    int professionalID = GetProfessionalID(item);
                    var professionalRecord = SPConnector.GetListItem(SP_PROMAS_LIST_NAME, professionalID, _siteUrl);
                    string professionalName = Convert.ToString(professionalRecord["Title"]);

                    int requestorID = GetProfessionalID(requestorMail);
                    var requestorRecord = SPConnector.GetListItem(SP_PROMAS_LIST_NAME, requestorID, _siteUrl);
                    string requestorName = Convert.ToString(requestorRecord["Title"]);

                    EmailUtil.Send(item, "Request for Approval of Exit Checklist", string.Format("Dear {0}{1}{2}You are authorized as an approver for Exit Checklist Form.{3}This Checklist is requested by {4}{5}Please complete the approval process immediately{6}{7}To view the detail, please click following link:{8}{9}/EditExitProcedureForApprover.aspx?ID={10}{11}{12}Thank you for your attention.", professionalName, Environment.NewLine, Environment.NewLine, Environment.NewLine, requestorName, Environment.NewLine, Environment.NewLine, Environment.NewLine, siteUrl, urlResource, exitProcID, Environment.NewLine, Environment.NewLine));
                }
            }

            int countApprover = emails.Count;
            var columnValues = new Dictionary<string, object>();
            int i = 1;
            foreach(var item in emails)
            {
                if(i == 1)
                {
                    columnValues.Add("visibletoapprover1", SPConnector.GetUser(item, _siteUrl));
                }
                else if(i == 2)
                {
                    columnValues.Add("visibletoapprover2", SPConnector.GetUser(item, _siteUrl));
                }
                else if(i == 3)
                {
                    columnValues.Add("visibletoapprover3", SPConnector.GetUser(item, _siteUrl));
                }
                else if(i == 4)
                {
                    columnValues.Add("visibletoapprover4", SPConnector.GetUser(item, _siteUrl));
                }
                else if (i == 5)
                {
                    columnValues.Add("visibletoapprover5", SPConnector.GetUser(item, _siteUrl));
                }
                else if (i == 6)
                {
                    columnValues.Add("visibletoapprover6", SPConnector.GetUser(item, _siteUrl));
                }
                else if (i == 7)
                {
                    columnValues.Add("visibletoapprover7", SPConnector.GetUser(item, _siteUrl));
                }
                else if (i == 8)
                {
                    columnValues.Add("visibletoapprover8", SPConnector.GetUser(item, _siteUrl));
                }
                else if (i == 9)
                {
                    columnValues.Add("visibletoapprover9", SPConnector.GetUser(item, _siteUrl));
                }
                else if (i == 10)
                {
                    columnValues.Add("visibletoapprover10", SPConnector.GetUser(item, _siteUrl));
                }
                else if (i == 11)
                {
                    columnValues.Add("visibletoapprover11", SPConnector.GetUser(item, _siteUrl));
                }
                else if (i == 12)
                {
                    columnValues.Add("visibletoapprover12", SPConnector.GetUser(item, _siteUrl));
                }
                else if (i == 13)
                {
                    columnValues.Add("visibletoapprover13", SPConnector.GetUser(item, _siteUrl));
                }
                else if (i == 14)
                {
                    columnValues.Add("visibletoapprover14", SPConnector.GetUser(item, _siteUrl));
                }

                i++;
            }

            try
            {
                SPConnector.UpdateListItem(SP_EXP_LIST_NAME, exitProcID, columnValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
            }
        }

        private int GetProfessionalID(string professionalEmail)
        {
            int professionalID = 0;

            var camlProfessionalData = @"<View>  
            <Query> 
               <Where><Eq><FieldRef Name='officeemail' /><Value Type='Text'>" + professionalEmail + @"</Value></Eq></Where> 
            </Query> 
      </View>";

            foreach(var professionalData in SPConnector.GetList(SP_PROMAS_LIST_NAME, _siteUrl, camlProfessionalData))
            {
                professionalID = Convert.ToInt32(professionalData["ID"]);
                break;
            }

            return professionalID;
        }

        public int GetProfessionalIDNumber (string professionalName, string projectUnit, string position)
        {
            int professionalID = 0;

            var camlProfessionalData = @"<View>  
            <Query> 
               <Where><And><And><Eq><FieldRef Name='Title' /><Value Type='Text'>" + professionalName + @"</Value></Eq><Eq><FieldRef Name='Position' /><Value Type='Lookup'>" + position + @"</Value></Eq></And><Eq><FieldRef Name='Project_x002f_Unit' /><Value Type='Choice'>" + projectUnit + @"</Value></Eq></And></Where> 
            </Query> 
      </View>";

            foreach (var professionalData in SPConnector.GetList(SP_PROMAS_LIST_NAME, _siteUrl, camlProfessionalData))
            {
                professionalID = Convert.ToInt32(professionalData["ID"]);
                break;
            }

            return professionalID;
        }

        public void SendMailDocument(string requestorMail, string documentExitProcedure)
        {
            EmailUtil.Send(requestorMail, " ", documentExitProcedure);
        }

        public bool CheckPendingApproval(int? id, string statusApproved)
        {
            int number = 0;
            
            var camlCheckPendingApproval = @"<View>  
            <Query> 
               <Where><And><Eq><FieldRef Name='exitprocedure' /><Value Type='Lookup'>" + id + @"</Value></Eq><Eq><FieldRef Name='checklistitemapproval' /><Value Type='Choice'>" + statusApproved + @"</Value></Eq></And></Where> 
            </Query> 
            </View>";

            foreach (var item in SPConnector.GetList(SP_EXP_CHECK_LIST_NAME, _siteUrl, camlCheckPendingApproval))
            {
                if (item["ID"] != null)
                {
                    number = 1;
                    break;
                }
                else
                {
                    number = 0;
                }
            }

            if(number == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SendAlreadyApproved(string professionalMail, string messageToProfessional)
        {
            EmailUtil.Send(professionalMail, "Approval of Exit Checklist", messageToProfessional);
        }

        public bool UpdateExitProcedureStatus(int? id, string checklistStatusApproved)
        {
            var columnValues = new Dictionary<string, object>();

            columnValues.Add("status", checklistStatusApproved);

            try
            {
                SPConnector.UpdateListItem(SP_EXP_LIST_NAME, id, columnValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                return false;
            }

            return true;
        }

        public string GetPSANumberOnExitProcedure(int? id)
        {
            string psaNumber;

            var caml = @"<View>  
            <Query> 
               <Where><Eq><FieldRef Name='ID' /><Value Type='Counter'>" + id + @"</Value></Eq></Where> 
            </Query> 
      </View>";

            var item = SPConnector.GetListItem(SP_EXP_LIST_NAME, id, _siteUrl);

            psaNumber = Convert.ToString(item["psanumber"]);

            return psaNumber;
        }

        public int GetPSAId(string psaNumber)
        {
            int psaID = 0;

            var caml = @"<View>  
            <Query> 
               <Where><Eq><FieldRef Name='Title' /><Value Type='Text'>" + psaNumber + @"</Value></Eq></Where> 
            </Query> 
      </View>";

            foreach (var item in SPConnector.GetList(SP_PSA_LIST_NAME, _siteUrl, caml))
            {
                psaID = Convert.ToInt32(item["ID"]);

                if (psaID != 0)
                    break;
            }

            return psaID;
        }

        public DateTime GetLastWorkingDate(int? exitProcID)
        {
            var item = SPConnector.GetListItem(SP_EXP_LIST_NAME, exitProcID, _siteUrl);
            DateTime lastWorkingDate = Convert.ToDateTime(item["lastworkingdate"]);

            return lastWorkingDate;
        }

        public bool UpdateLastWorkingDateOnPSA(int? psaID, DateTime lastWorkingDate)
        {
            var columnValues = new Dictionary<string, object>();

            columnValues.Add("lastworkingdate", lastWorkingDate);

            try
            {
                SPConnector.UpdateListItem(SP_PSA_LIST_NAME, psaID, columnValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                return false;
            }

            return true;
        }

        public bool UpdateLastWorkingDateOnProfessional(int professionalID, DateTime lastWorkingDate)
        {
            var columnValues = new Dictionary<string, object>();

            columnValues.Add("lastworkingdate", lastWorkingDate);

            try
            {
                SPConnector.UpdateListItem(SP_PROMAS_LIST_NAME, professionalID, columnValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                return false;
            }

            return true;
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

            foreach (var item in SPConnector.GetList(SP_POSMAS_LIST_NAME, _siteUrl, caml))
            {
                if(item["ID"] != null)
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

            if(number == 1)
            {
                return positionID;
            }
            else
            {
                return 0;
            }
            
        }

        public string GetExitProcedureStatus(int? exitProcID)
        {
            var caml = @"<View>  
            <Query> 
               <Where><Eq><FieldRef Name='ID' /><Value Type='Counter'>" + exitProcID + @"</Value></Eq></Where> 
            </Query> 
      </View>";

            var item = SPConnector.GetListItem(SP_EXP_LIST_NAME, exitProcID, _siteUrl);

            string exitProcedureStatus = Convert.ToString(item["status"]);

            return exitProcedureStatus;
        }

        public string GetProjectUnit(string requestor)
        {
            var camlProjectUnit = @"<View>  
            <Query> 
               <Where><Eq><FieldRef Name='officeemail' /><Value Type='Text'>" + requestor + @"</Value></Eq></Where> 
            </Query> 
      </View>";

            string projectUnit = "";

            foreach (var projectunit in SPConnector.GetList(SP_PROMAS_LIST_NAME, _siteUrl, camlProjectUnit))
            {
                projectUnit = Convert.ToString(projectunit["Project_x002f_Unit"]);
                break;
            }

            return projectUnit;
        }

        public bool UpdateLastWorkingDateOnProfessional(int? professionalID, DateTime lastWorkingDate)
        {
            var columnValues = new Dictionary<string, object>();

            columnValues.Add("lastworkingdate", lastWorkingDate);

            try
            {
                SPConnector.UpdateListItem(SP_PROMAS_LIST_NAME, professionalID, columnValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                return false;
            }

            return true;
        }

        public string GetProfessionalData(int? professionalID)
        {
            var professionalData = SPConnector.GetListItem(SP_PROMAS_LIST_NAME, professionalID, _siteUrl);
            string professionalMail = Convert.ToString(professionalData["officeemail"]);

            return professionalMail;
        }

        public string GetProfessionalName(int? exitProcID)
        {
            string professionalName = "";

            var camlExitProcedure = @"<View>  
            <Query> 
               <Where><Eq><FieldRef Name='ID' /><Value Type='Counter'>" + exitProcID + @"</Value></Eq></Where> 
            </Query> 
      </View>";

            foreach(var exitProcedureData in SPConnector.GetList(SP_EXP_LIST_NAME, _siteUrl, camlExitProcedure))
            {
                professionalName = Convert.ToString(exitProcedureData["Title"]);
                break;
            }

            return professionalName;
        }

        public string GetUnitBasedExitID(int? exitProcID)
        {
            string projectUnit = "";

            var camlExitProcedure = @"<View>  
            <Query> 
               <Where><Eq><FieldRef Name='ID' /><Value Type='Counter'>" + exitProcID + @"</Value></Eq></Where> 
            </Query> 
      </View>";

            foreach(var exitProcedureData in SPConnector.GetList(SP_EXP_LIST_NAME, _siteUrl, camlExitProcedure))
            {
                projectUnit = Convert.ToString(exitProcedureData["projectunit"]);
                break;
            }

            return projectUnit;
        }

        public string GetPositionBasedExitID(int? exitProcID)
        {
            string position = "";

            var camlExitProcedure = @"<View>  
            <Query> 
               <Where><Eq><FieldRef Name='ID' /><Value Type='Counter'>" + exitProcID + @"</Value></Eq></Where> 
            </Query> 
      </View>";

            foreach(var exitProcedureData in SPConnector.GetList(SP_EXP_LIST_NAME, _siteUrl, camlExitProcedure))
            {
                position = Convert.ToString(exitProcedureData["position"]);
                break;
            }

            return position;
        }
    }
}
