using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            if(exitProcedure.UserPermission == "Professional")
            {
                updatedValues.Add("position", exitProcedure.RequestorPosition);
                updatedValues.Add("joindate", exitProcedure.ProfessionalJoinDate);
            }
            if(exitProcedure.UserPermission == "HR")
            {
                updatedValues.Add("position", exitProcedure.Position);
                updatedValues.Add("joindate", exitProcedure.JoinDate);
            }

            updatedValues.Add("mobilenumber", exitProcedure.PhoneNumber);
            updatedValues.Add("officeemail", exitProcedure.EmailAddress);
            updatedValues.Add("currentaddress", exitProcedure.CurrentAddress);
            updatedValues.Add("lastworkingdate", exitProcedure.LastWorkingDate);
            updatedValues.Add("exitreason", exitProcedure.ExitReason.Value);
            updatedValues.Add("reasondescription", exitProcedure.ReasonDesc);
            updatedValues.Add("psanumber", exitProcedure.PSANumber);

            if(exitProcedure.StatusForm == "Pending Approval")
            {
                statusExitProcedure = "Pending Approval";
            }
            else if(exitProcedure.StatusForm == "Draft")
            {
                statusExitProcedure = "Draft";
            }
            else if(exitProcedure.StatusForm == "Saved by HR")
            {
                statusExitProcedure = "Draft";
            }
            else if(exitProcedure.StatusForm == "Approved by HR")
            {
                statusExitProcedure = "Approved";
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

            if ((ID == null && user == null))
            {
                viewModel = GetWorkflowExitProcedure(listName, requestor, user);

                return viewModel;
            }
            if (ID == null && user == "HR")
            {
                viewModel = GetWorkflowExitProcedure(listName, requestor, user);

                return viewModel;
            }

            var listItem = SPConnector.GetListItem(SP_EXP_LIST_NAME, ID, _siteUrl);
            viewModel = ConvertToExitProcedureVM(listItem);
            viewModel = GetExitProcedureDetails(viewModel);

            return viewModel;
        }

        public ExitProcedureVM GetExitProcedureHR(int? ID, string siteUrl, string requestor, string listName, string user)
        {
            var viewModel = new ExitProcedureVM();

            if (ID == null && user == "HR")
            {
                viewModel = GetWorkflowExitProcedureHR(listName, requestor, user);

                return viewModel;
            }

            var listItem = SPConnector.GetListItem(SP_EXP_LIST_NAME, ID, _siteUrl);
            viewModel = ConvertToExitProcedureVM(listItem);
            viewModel = GetExitProcedureDetails(viewModel);

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
                @"</Value></Eq></Where></Query><ViewFields><FieldRef Name='ID' /><FieldRef Name='Position' /><FieldRef Name='Project_x002f_Unit' /><FieldRef Name='Title' /><FieldRef Name='lastname' /><FieldRef Name='Join_x0020_Date' /><FieldRef Name='officeemail' /><FieldRef Name='PSAnumber' /></ViewFields><QueryOptions /></View>";

            int? positionID = 0;

            if(user == "Professional")
            {
                foreach (var item in SPConnector.GetList(SP_PROMAS_LIST_NAME, _siteUrl, caml))
                {
                    viewModel.RequestorPosition = FormatUtil.ConvertLookupToValue(item, "Position");
                    viewModel.FullName = Convert.ToString(item["Title"]) + " " + Convert.ToString(item["lastname"]);
                    viewModel.ProjectUnit = Convert.ToString(item["Project_x002f_Unit"]);
                    positionID = FormatUtil.ConvertLookupToID(item, "Position");
                    viewModel.ProfessionalID = Convert.ToInt32(item["ID"]);
                    viewModel.ProfessionalJoinDate = Convert.ToDateTime(item["Join_x0020_Date"]).ToLocalTime();
                    viewModel.RequestorMailAddress = Convert.ToString(item["officeemail"]);
                    viewModel.PSANumber = Convert.ToString(item["PSAnumber"]);
                    break;
                }
            }
            if(user == "HR")
            {
                foreach (var item in SPConnector.GetList(SP_PROMAS_LIST_NAME, _siteUrl, caml))
                {
                    positionID = FormatUtil.ConvertLookupToID(item, "Position");
                    viewModel.RequestorPosition = FormatUtil.ConvertLookupToValue(item, "Position");
                    break;
                }
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

        public ExitProcedureVM GetWorkflowExitProcedureHR(string listName, string requestor, string user)
        {
            var viewModel = new ExitProcedureVM();
            viewModel.ListName = listName;

            // Get Position in Professional Master
            var caml = @"<View><Query><Where><Eq>
                <FieldRef Name='officeemail' /><Value Type='Text'>" + requestor +
                @"</Value></Eq></Where></Query><ViewFields><FieldRef Name='ID' /><FieldRef Name='Position' /><FieldRef Name='Project_x002f_Unit' /><FieldRef Name='Title' /><FieldRef Name='lastname' /><FieldRef Name='Join_x0020_Date' /><FieldRef Name='officeemail' /><FieldRef Name='PSAnumber' /></ViewFields><QueryOptions /></View>";

            int? positionID = 0;

            if (user == "HR")
            {
                foreach (var item in SPConnector.GetList(SP_PROMAS_LIST_NAME, _siteUrl, caml))
                {
                    positionID = FormatUtil.ConvertLookupToID(item, "Position");
                    viewModel.RequestorPosition = FormatUtil.ConvertLookupToValue(item, "Position");
                    break;
                }
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

        public async Task CreateExitProcedureChecklistAsync(int? exitProcID, IEnumerable<ExitProcedureChecklistVM> exitProcedureChecklist, string requestorposition, string requestorunit)
        {
            CreateExitProcedureChecklist(exitProcID, exitProcedureChecklist, requestorposition, requestorunit);
        }

        

        public void CreateExitProcedureChecklist(int? exitProcID, IEnumerable<ExitProcedureChecklistVM> ExitProcedureChecklist, string requestorposition, string requestorunit)
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

                updatedValue.Add("Title", viewModel.ItemExitProcedure);
                updatedValue.Add("approverposition", new FieldLookupValue { LookupId = Convert.ToInt32(viewModel.ApproverPosition.Value) });
                updatedValue.Add("approvername", Convert.ToString(viewModel.ApproverUserName.Text));
                updatedValue.Add("checklistitemapproval", viewModel.CheckListItemApproval.Text);
                updatedValue.Add("dateofapproval", viewModel.DateOfApproval);
                updatedValue.Add("remarks", viewModel.Remarks);
                updatedValue.Add("requestorunit", viewModel.ProfessionalUnit.Text);
                updatedValue.Add("requestorposition", new FieldLookupValue { LookupId = Convert.ToInt32(viewModel.ProfessionalPosition.Value) });
                updatedValue.Add("approverlevel", viewModel.Level);
                updatedValue.Add("approverunit", viewModel.ApproverUnit.Text);
                updatedValue.Add("isdefault", viewModel.IsDefault);
                updatedValue.Add("approvalmail", viewModel.ApprovalMail);
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
            viewModel.Requestor = Convert.ToString(listItem["Title"]);
            viewModel.StatusForm = Convert.ToString(listItem["status"]);

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
             <ViewFields>
                <FieldRef Name='approverlevel' />
                <FieldRef Name='approvername' />
                <FieldRef Name='approverposition' />
                <FieldRef Name='approverposition_x003a_ID' />
                <FieldRef Name='checklistitemapproval' />
                <FieldRef Name='dateofapproval' />
                <FieldRef Name='approverunit' />
                <FieldRef Name='remarks' />
                <FieldRef Name='transactiontype' />
                <FieldRef Name='requestorunit' />
                <FieldRef Name='requestorposition' />
                <FieldRef Name='requestorposition_x003a_ID' />
                <FieldRef Name='approverlevel' />
                <FieldRef Name='approverunit' />
                <FieldRef Name='isdefault' />
                <FieldRef Name='workflowtype' />
                <FieldRef Name='exitprocedure' />
                <FieldRef Name='Title' />
                <FieldRef Name='ID' />
             </ViewFields> 
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
            return new ExitProcedureChecklistVM
            {
                ID = Convert.ToInt32(item["ID"]),
                ItemExitProcedure = Convert.ToString(item["Title"]),
                Remarks = Convert.ToString(item["remarks"])
            };
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
            columnValues.Add("professional", new FieldLookupValue { LookupId = Convert.ToInt32(exitProcedure.Professional.Value) });
            columnValues.Add("Title", exitProcedure.FullName);
            columnValues.Add("projectunit", exitProcedure.ProjectUnit);
            columnValues.Add("position", exitProcedure.Position);
            columnValues.Add("mobilenumber", exitProcedure.PhoneNumber);
            columnValues.Add("officeemail", exitProcedure.EmailAddress);
            columnValues.Add("currentaddress", exitProcedure.CurrentAddress);
            columnValues.Add("joindate", exitProcedure.JoinDate.Value);
            columnValues.Add("lastworkingdate", exitProcedure.LastWorkingDate.Value);
            columnValues.Add("exitreason", exitProcedure.ExitReason.Value);
            columnValues.Add("reasondescription", exitProcedure.ReasonDesc);
            columnValues.Add("psanumber", exitProcedure.PSANumber);


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

        public void SendEmail(ExitProcedureVM header, string workflowTransactionListName, string transactionLookupColumnName, int exitProcID, string messageForApprover, string messageForRequestor)
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
            var professionalEmail = new List<string>();

            if (header.StatusForm == "Pending Approval")
            {
                foreach (var item in SPConnector.GetList(SP_EXP_WF_LIST_NAME, _siteUrl, camlapprover))
                {
                    emails.Add(Convert.ToString(item["approver"]));
                }
                foreach (var item in emails)
                {
                    EmailUtil.Send(item, "Ask for Approval", messageForApprover);
                    
                }
            }
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
                var statusExitProcedure = "Approved";
                var columnValues = new Dictionary<string, object>();

                columnValues.Add("status", statusExitProcedure);

                try
                {
                    SPConnector.UpdateListItem(SP_EXP_LIST_NAME, id, columnValues, _siteUrl);
                }
                catch (Exception e)
                {
                    logger.Debug(e.Message);
                    return false;
                }

                return false;
            }
        }

        //public bool UpdateExitProcedureStatus(int? id, string checklistStatusApproved)
        //{
        //    var statusExitProcedure = "Approved";
        //    var columnValues = new Dictionary<string, object>();

        //    columnValues.Add("status", statusExitProcedure);

        //    try
        //    {
        //        SPConnector.UpdateListItem(SP_EXP_LIST_NAME, id, columnValues, _siteUrl);
        //    }
        //    catch (Exception e)
        //    {
        //        logger.Debug(e.Message);
        //        return false;
        //    }

        //    return true;
        //}
    }
}
