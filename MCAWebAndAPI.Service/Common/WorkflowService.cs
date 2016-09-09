using System;
using System.Collections.Generic;
using MCAWebAndAPI.Model.HR.DataMaster;
using MCAWebAndAPI.Model.ViewModel.Form.Common;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using System.Threading.Tasks;
using System.Linq;
using MCAWebAndAPI.Model.ViewModel.Control;

namespace MCAWebAndAPI.Service.Common
{
    public class WorkflowService : IWorkflowService
    {
        string _siteUrl;
        const string SP_WORKFLOW_LISTNAME = "Workflow Mapping Master";
        const string SP_PROMAS_LIST_NAME = "Professional Master";
        const string SP_POSMAS_LIST_NAME = "Position Master";
        const string SP_EXIT_CHECKLIST_LIST_NAME = "Exit Procedure Checklist";

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


        public async Task<WorkflowRouterVM> GetWorkflowRouter(string listName, string requestor)
        {
            var viewModel = new WorkflowRouterVM();
            viewModel.ListName = listName;

            // Get Position in Professional Master
            var caml = @"<View><Query><Where><Eq>
                <FieldRef Name='officeemail' /><Value Type='Text'>" + requestor +
                @"</Value></Eq></Where></Query><ViewFields><FieldRef Name='Position' /><FieldRef Name='Project_x002f_Unit' /></ViewFields><QueryOptions /></View>";

            int? positionID = 0;
            foreach (var item in SPConnector.GetList(SP_PROMAS_LIST_NAME, _siteUrl, caml))
            {
                viewModel.RequestorPosition = FormatUtil.ConvertLookupToValue(item, "Position");
                positionID = FormatUtil.ConvertLookupToID(item, "Position");

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

            var workflowItems = new List<WorkflowItemVM>();
            foreach (var item in SPConnector.GetList(SP_WORKFLOW_LISTNAME, _siteUrl, caml))
            {
                if (string.Compare(Convert.ToString(item["isdefault"]), "No",
                    StringComparison.OrdinalIgnoreCase) == 0
                    &&
                    string.Compare(Convert.ToString(item["workflowtype"]), "Sequential",
                    StringComparison.OrdinalIgnoreCase) == 0)
                    continue;

                var vm = await ConvertToWorkflowItemVM(item);
                workflowItems.Add(vm);
            }

            viewModel.WorkflowItems = workflowItems;

            return viewModel;
        }

        private async Task<WorkflowItemVM> ConvertToWorkflowItemVM(ListItem item)
        {

            var viewModel = new WorkflowItemVM();
            viewModel.ApproverPosition = FormatUtil.ConvertToInGridAjaxComboBox(item, "approverposition");
            Task<IEnumerable<ProfessionalMaster>> getApproverNamesTask =
                GetApproverNamesAsync(viewModel.ApproverPosition.Text);

            viewModel.Level = Convert.ToString(item["approverlevel"]);

            if (viewModel.Level == "1")
            {
                viewModel.ItemExitProcedure = "Close-Out/Handover Report";
            }
            else if (viewModel.Level == "2")
            {
                viewModel.ItemExitProcedure = "MCA Indonesia Propietary Information";
            }
            else if (viewModel.Level == "3")
            {
                viewModel.ItemExitProcedure = "Laptop/Desktop";
            }
            else if (viewModel.Level == "4")
            {
                viewModel.ItemExitProcedure = "SAP Password, Computer Password";
            }
            else if (viewModel.Level == "5")
            {
                viewModel.ItemExitProcedure = "IT Tools";
            }
            else if (viewModel.Level == "6")
            {
                viewModel.ItemExitProcedure = "Keys (Drawers,desk,etc)";
            }
            else if (viewModel.Level == "7")
            {
                viewModel.ItemExitProcedure = "Car";
            }
            else if (viewModel.Level == "8")
            {
                viewModel.ItemExitProcedure = "Advance Statement";
            }
            else if (viewModel.Level == "9")
            {
                viewModel.ItemExitProcedure = "Travel Statement";
            }
            else if (viewModel.Level == "10")
            {
                viewModel.ItemExitProcedure = "Resignation/Separation Letter";
            }
            else if (viewModel.Level == "11")
            {
                viewModel.ItemExitProcedure = "Timesheet/Leave Form";
            }
            else if (viewModel.Level == "12")
            {
                viewModel.ItemExitProcedure = "Exit Interview/NDA";
            }
            else if (viewModel.Level == "13")
            {
                viewModel.ItemExitProcedure = "Insurance Card";
            }
            else if (viewModel.Level == "14")
            {
                viewModel.ItemExitProcedure = "ID Card & Access Card";
            }

            viewModel.ApproverUnit =
                WorkflowItemVM.GetUnitDefaultValue(new InGridComboBoxVM
                {
                    Text = Convert.ToString(item["approverunit"])
                });

            var userNames = await getApproverNamesTask;
            var userName = userNames.FirstOrDefault();
            viewModel.ApproverUserName = AjaxComboBoxVM.GetDefaultValue(new AjaxComboBoxVM
            {
                Text = userName.Name,
                Value = userName.ID
            });

            return viewModel;
        }

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
        }

        private async Task<IEnumerable<ProfessionalMaster>> GetApproverNamesAsync(string position)
        {
            return GetApproverNames(position);
        }

        private string GetApproverUserLogin(int userID)
        {
            var caml = @"<View>  
            <Query> 
               <Where><Eq><FieldRef Name='ID' /><Value Type='Counter'>"
                + userID + @"</Value></Eq></Where> 
            </Query>
             <ViewFields><FieldRef Name='officeemail' /></ViewFields> 
            </View>";

            foreach (var item in SPConnector.GetList(SP_PROMAS_LIST_NAME, _siteUrl, caml))
            {
                return Convert.ToString(item["officeemail"]);
            }
            return null;
        }

        public IEnumerable<ProfessionalMaster> GetApproverNames(string position)
        {
            var caml = @"<View>  
            <Query> 
               <Where><Eq><FieldRef Name='Position' /><Value Type='Lookup'>" + position + @"</Value></Eq></Where> 
            </Query> 
             <ViewFields><FieldRef Name='ID' /><FieldRef Name='Title' /><FieldRef Name='Title' /><FieldRef Name='officeemail' /></ViewFields> 
            </View>";

            var viewModel = new List<ProfessionalMaster>();
            foreach (var item in SPConnector.GetList(SP_PROMAS_LIST_NAME, _siteUrl, caml))
            {
                viewModel.Add(ConvertToProfessionalMasterVM(item));
            }

            return viewModel;
        }

        ProfessionalMaster ConvertToProfessionalMasterVM(ListItem item)
        {
            return new ProfessionalMaster
            {
                ID = Convert.ToInt32(item["ID"]),
                Name = Convert.ToString(item["Title"]),
                UserLogin = Convert.ToString(item["officeemail"]),
                Desc = string.Format("{0} - {1} ({2})", Convert.ToString(item["Title"]), Convert.ToString(item["Project_x002f_Unit"]), FormatUtil.ConvertLookupToValue(item, "Position"))
            };
        }

        public void CreateTransactionWorkflow(string workflowTransactionListName, string transactionLookupColumnName, int headerID, IEnumerable<WorkflowItemVM> workflowItems, string requestor = null)
        {
            foreach (var item in workflowItems)
            {
                CreateTransactionWorkflowItem(workflowTransactionListName, transactionLookupColumnName, headerID, item, requestor);
            }
        }

        public void CreateWorkflow(string workflowTransactionListName, string transactionLookupColumnName, int headerID, IEnumerable<WorkflowItemVM> workflowItems)
        {
            foreach (var item in workflowItems)
            {
                CreateWorkflow(workflowTransactionListName, transactionLookupColumnName, headerID, item);
            }
        }

        public void CreateExitProcedureChecklistWorkflow(string workflowTransactionListName, string transactionLookupColumnName, int exitProcID, IEnumerable<ExitProcedureChecklistVM> exitProcedureChecklist, string requestor = null)
        {
            foreach (var item in exitProcedureChecklist)
            {
                CreateExitProcedureWorkflowItem(workflowTransactionListName, transactionLookupColumnName, exitProcID, item, requestor);
            }
        }

        public void SendApprovalRequest(string workflowTransactionListName,
            string transactionLookupColumnName, int headerID, int level, string message)
        {
            var caml = @"<View>  
            <Query> 
               <Where><And><Eq><FieldRef Name='" + transactionLookupColumnName + @"' />
               <Value Type='Lookup'>" + headerID + @"</Value></Eq><Eq>
               <FieldRef Name='approverlevel' /><Value Type='Choice'>" + level + @"</Value></Eq></And></Where> 
            </Query> 
            </View>";

            var emails = new List<string>();
            foreach (var item in SPConnector.GetList(workflowTransactionListName, _siteUrl, caml))
            {
                emails.Add(Convert.ToString(item["approver0"]));
            }
            foreach (var item in emails)
            {
                SPConnector.SendEmail(item, message, "Ask for Approval", _siteUrl);
            }
        }

        private void CreateTransactionWorkflowItem(string workflowTransactionListName, string transactionLookupColumnName, int headerID, WorkflowItemVM workflowItem, string requestor = null)
        {
            var updatedValue = new Dictionary<string, object>();
            updatedValue.Add(transactionLookupColumnName, new FieldLookupValue { LookupId = headerID });
            updatedValue.Add("approverlevel", workflowItem.Level);
            updatedValue.Add("position", workflowItem.ApproverPositionId);
            updatedValue.Add("projectunit", workflowItem.ApproverUnitText);
            updatedValue.Add("approvername", workflowItem.ApproverName.Value);
            SPConnector.AddListItem(workflowTransactionListName, updatedValue, _siteUrl);
        }

        private void CreateWorkflow(string workflowTransactionListName, string transactionLookupColumnName, int headerID, WorkflowItemVM workflowItem)
        {
            var updatedValue = new Dictionary<string, object>();
            updatedValue.Add(transactionLookupColumnName, new FieldLookupValue { LookupId = headerID });
            updatedValue.Add("approverlevel", workflowItem.Level);
            updatedValue.Add("position", workflowItem.ApproverPositionId);
            updatedValue.Add("projectunit", workflowItem.ApproverUnitText);
            updatedValue.Add("approvername", workflowItem.ApproverName.Value);

            try
            { 
                SPConnector.AddListItem(workflowTransactionListName, updatedValue, _siteUrl);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void CreateExitProcedureWorkflowItem(string workflowTransactionListName, string transactionLookupColumnName, int exitProcID, ExitProcedureChecklistVM exiProcedureChecklist, string requestor = null)
        {
            var updatedValue = new Dictionary<string, object>();
            updatedValue.Add(transactionLookupColumnName, new FieldLookupValue { LookupId = exitProcID });
            updatedValue.Add("approverlevel", exiProcedureChecklist.Level);
            updatedValue.Add("approver", GetApproverUserLogin((int)exiProcedureChecklist.ApproverUserName.Value));
            updatedValue.Add("requestor", requestor);
            updatedValue.Add("Title", exiProcedureChecklist.ItemExitProcedure);

            SPConnector.AddListItem(workflowTransactionListName, updatedValue, _siteUrl);
        }

        public string GetPositionName(int position)
        {
            var item = SPConnector.GetListItem(SP_POSMAS_LIST_NAME, position, _siteUrl);
            return Convert.ToString(item["Title"]);
        }

        public string GetUnitName(int unit)
        {
            var item = SPConnector.GetListItem(SP_POSMAS_LIST_NAME, unit, _siteUrl);
            return Convert.ToString(item["Project_x002f_Unit"]);
        }

        public async Task<IEnumerable<PendingApprovalItemVM>> GetPendingApprovalItemsAsync(string userLogin)
        {
            var getManpowerRequisitionWorkflow = GetPendingApprovalItemsAsync(userLogin, "Manpower Requisition", "manpowerrequisition");
            var getCompensatoryRequestWorkflow = GetPendingApprovalItemsAsync(userLogin, "Compensatory Request", "compensatoryrequest");
            var getDayOffRequestWorkflow = GetPendingApprovalItemsAsync(userLogin, "Day-Off Request", "dayoffrequest");
            var getExitProcedureWorkflow = GetPendingApprovalItemsAsync(userLogin, "Exit Procedure", "exitprocedure");
            var getPayrollRunWorkflow = GetPendingApprovalItemsAsync(userLogin, "Payroll Run", "payrollrun");
            var getProfessionalPerformanceEvaluationWorkflow = GetPendingApprovalItemsAsync(userLogin, "Professional Performance Evaluation", "professionalperformanceevaluation");
            var getProfessionalPerformancePlanWorkflow = GetPendingApprovalItemsAsync(userLogin, "Professional Performance Plan", "professionalperformanceplan");
            var getTimesheetWorkflow = GetPendingApprovalItemsAsync(userLogin, "Timesheet", "timesheet");

            var allTask = Task.WhenAll(
                getManpowerRequisitionWorkflow,
                getCompensatoryRequestWorkflow,
                getDayOffRequestWorkflow,
                getExitProcedureWorkflow,
                getPayrollRunWorkflow,
                getProfessionalPerformanceEvaluationWorkflow,
                getProfessionalPerformancePlanWorkflow,
                getTimesheetWorkflow);

            await allTask;
            var pendingApprovalItems = new List<PendingApprovalItemVM>();
            pendingApprovalItems.AddRange(await getManpowerRequisitionWorkflow);
            pendingApprovalItems.AddRange(await getCompensatoryRequestWorkflow);
            pendingApprovalItems.AddRange(await getDayOffRequestWorkflow);
            pendingApprovalItems.AddRange(await getExitProcedureWorkflow);
            pendingApprovalItems.AddRange(await getPayrollRunWorkflow);
            pendingApprovalItems.AddRange(await getProfessionalPerformanceEvaluationWorkflow);
            pendingApprovalItems.AddRange(await getProfessionalPerformancePlanWorkflow);
            pendingApprovalItems.AddRange(await getTimesheetWorkflow);

            return pendingApprovalItems;
        }

        async Task<IEnumerable<PendingApprovalItemVM>> GetPendingApprovalItemsAsync(string userLogin,
            string listName, string lookupColumnName)
        {
            var caml = @"<View><Query><Where><And><Contains><FieldRef Name='approver0' /><Value Type='Text'>"
                + userLogin + @"</Value></Contains><Eq><FieldRef Name='currentstate' /><Value Type='Choice'>Yes</Value></Eq></And></Where></Query></View>";
            var viewModel = new List<PendingApprovalItemVM>();
            var workflowListName = string.Format("{0} {1}", listName, "Workflow");

            foreach (var item in SPConnector.GetList(workflowListName, _siteUrl, caml))
            {
                viewModel.Add(ConvertToPendingApprovalItemVM(item, listName, lookupColumnName));
            }
            return viewModel;
        }

        PendingApprovalItemVM ConvertToPendingApprovalItemVM(ListItem item, string listName, string lookupColumnName)
        {

            return new PendingApprovalItemVM
            {
                Requestor = Convert.ToString(item["requestor0"]),
                Level = Convert.ToString(item["approverlevel"]),
                DateOfRequest = Convert.ToDateTime(item["Created"]),
                LookupID = (int)FormatUtil.ConvertLookupToID(item, lookupColumnName),
                TransactionName = listName
            };
        }

        public async Task<IEnumerable<WorkflowItemVM>> GetWorkflowDetails(string requestor, string listName)
        {
            var viewModel = new WorkflowRouterVM();

            viewModel.ListName = listName;

            // Get Position in Professional Master
            var caml = @"<View><Query><Where><Eq>
                <FieldRef Name='officeemail' /><Value Type='Text'>" + requestor +
                @"</Value></Eq></Where></Query><ViewFields><FieldRef Name='Position' /><FieldRef Name='Project_x002f_Unit' /></ViewFields><QueryOptions /></View>";

            int? positionID = 0;
            foreach (var item in SPConnector.GetList(SP_PROMAS_LIST_NAME, _siteUrl, caml))
            {
                viewModel.RequestorPosition = FormatUtil.ConvertLookupToValue(item, "Position");
                positionID = FormatUtil.ConvertLookupToID(item, "Position");

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

            var workflowItems = new List<WorkflowItemVM>();
            foreach (var item in SPConnector.GetList(SP_WORKFLOW_LISTNAME, _siteUrl, caml))
            {
                var vm = await ConvertToWorkflowDetail(item);
                workflowItems.Add(vm);
            }

            return workflowItems;
        }

        private async Task<WorkflowItemVM> ConvertToWorkflowDetail(ListItem item)
        {
            var viewModel = new WorkflowItemVM();
            viewModel.ApproverPositionText = FormatUtil.ConvertLookupToValue(item, "approverposition");
            viewModel.ApproverPositionId = FormatUtil.ConvertLookupToID(item, "approverposition");
            Task<IEnumerable<ProfessionalMaster>> getApproverNamesTask =
                GetApproverUserNames(viewModel.ApproverPositionText);

            viewModel.Level = Convert.ToString(item["approverlevel"]);

            viewModel.ApproverUnitText = Convert.ToString(item["approverunit"]);

            var userNames = await getApproverNamesTask;
            var userName = userNames.FirstOrDefault();
            viewModel.ApproverName = AjaxComboBoxVM.GetDefaultValue(new AjaxComboBoxVM
            {
                Text = userName.Name,
                Value = userName.ID,
            });

            return viewModel;
        }

        private async Task<IEnumerable<ProfessionalMaster>> GetApproverUserNamesAsync(string position)
        {
            return await GetApproverUserNames(position);
        }

        public async Task<IEnumerable<ProfessionalMaster>> GetApproverUserNames(string position)
        {
            var caml = @"<View><Query><Where><Or><And><Gt><FieldRef Name='lastworkingdate' /><Value IncludeTimeValue='TRUE' Type='DateTime'>" + DateTime.UtcNow.ToString("yyyy-MM-dd") + "T23:57:44Z</Value></Gt><IsNull><FieldRef Name='lastworkingdate' /></IsNull></And><Eq><FieldRef Name='Position' /><Value Type='Lookup'>"+ position +@"</Value></Eq></Or></Where></Query><ViewFields><FieldRef Name='ID' /><FieldRef Name='Title' /><FieldRef Name='Position' /><FieldRef Name='Project_x002f_Unit' /><FieldRef Name='officeemail' /></ViewFields><QueryOptions /></View>";

            var viewModel = new List<ProfessionalMaster>();
            foreach (var item in SPConnector.GetList(SP_PROMAS_LIST_NAME, _siteUrl, caml))
            {
                viewModel.Add(ConvertToProfessionalMasterVM(item));
            }

            return viewModel;
        }

        public IEnumerable<ProfessionalMaster> GetApproverUser()
        {
            var caml = @"<View><Query><Where><Or><Gt><FieldRef Name='lastworkingdate' /><Value IncludeTimeValue='TRUE' Type='DateTime'>" + DateTime.UtcNow.ToString("yyyy-MM-dd") + "T23:57:44Z</Value></Gt><IsNull><FieldRef Name='lastworkingdate' /></IsNull></Or></Where></Query><ViewFields><FieldRef Name='ID' /><FieldRef Name='Title' /><FieldRef Name='Position' /><FieldRef Name='Project_x002f_Unit' /><FieldRef Name='officeemail' /></ViewFields><QueryOptions /></View>";

            var viewModel = new List<ProfessionalMaster>();
            foreach (var item in SPConnector.GetList(SP_PROMAS_LIST_NAME, _siteUrl, caml))
            {
                viewModel.Add(ConvertToProfessionalMasterVM(item));
            }

            return viewModel;
        }

        public async Task<IEnumerable<WorkflowItemVM>> CheckWorkflow(int headerID, string workflowTransactionListName, string transactionLookupColumnName)
        {
            var caml = @"<View>  
            <Query> 
               <Where><Eq><FieldRef Name='" + transactionLookupColumnName + @"' /><Value Type='Lookup'>" + headerID + @"</Value></Eq></Where> 
            </Query> 
      </View>";

            var count = SPConnector.GetList(workflowTransactionListName, _siteUrl, caml).Count();
            var WorkflowItems = new List<WorkflowItemVM>();
            var viewModel = new WorkflowItemVM();
            foreach (var item in SPConnector.GetList(workflowTransactionListName, _siteUrl, caml))
            {
                WorkflowItems.Add(ConvertToWorkflowItem(item));
            }
            return WorkflowItems;
        }

        private WorkflowItemVM ConvertToWorkflowItem(ListItem item)
        {
            return new WorkflowItemVM
            {
                Level = Convert.ToString(item["approverlevel"]),
                ApproverUnitText = Convert.ToString(item["projectunit"]),
                ApproverPositionText = FormatUtil.ConvertLookupToValue(item, "position"),
                ApproverPositionId = FormatUtil.ConvertLookupToID(item, "position"),
                ApproverName = AjaxComboBoxVM.GetDefaultValue(new AjaxComboBoxVM
                {
                    Text = FormatUtil.ConvertLookupToValue(item, "approvername"),
                    Value = FormatUtil.ConvertLookupToID(item, "approvername_x003a_ID"),
                })
            };
        }



        private WorkflowItemVM ConvertToWorkflowItemTimesheet(ListItem item)
        {
            return new WorkflowItemVM
            {
                ID = Convert.ToInt32(item["ID"]),
                ApproverEmail = FormatUtil.ConvertLookupToValue(item, "approvername_x003a_Office_x0020_"),
                Status = Convert.ToString(item["status"]),
                Level = Convert.ToString(item["approverlevel"]),
                ApproverUnitText = Convert.ToString(item["projectunit"]),
                ApproverPositionText = FormatUtil.ConvertLookupToValue(item, "position"),
                ApproverPositionId = FormatUtil.ConvertLookupToID(item, "position"),
                ApproverNameText = FormatUtil.ConvertLookupToValue(item, "approvername"),
                ApproverName = AjaxComboBoxVM.GetDefaultValue(new AjaxComboBoxVM
                {
                    Text = FormatUtil.ConvertLookupToValue(item, "approvername"),
                    Value = FormatUtil.ConvertLookupToID(item, "approvername_x003a_ID"),
                })
            };
        }

        public async Task<IEnumerable<WorkflowItemVM>> CheckWorkflowTimesheet(int headerID, string workflowTransactionListName, string transactionLookupColumnName)
        {
            var caml = @"<View>  
            <Query> 
               <Where><Eq><FieldRef Name='" + transactionLookupColumnName + @"' /><Value Type='Lookup'>" + headerID + @"</Value></Eq></Where> 
            </Query> 
      </View>";

            var count = SPConnector.GetList(workflowTransactionListName, _siteUrl, caml).Count();
            var WorkflowItems = new List<WorkflowItemVM>();
            var viewModel = new WorkflowItemVM();
            foreach (var item in SPConnector.GetList(workflowTransactionListName, _siteUrl, caml))
            {
                WorkflowItems.Add(ConvertToWorkflowItemTimesheet(item));
            }
            return WorkflowItems;
        }


        private async Task<WorkflowItemVM> ConvertToWorkflowDetailTimeSheet(ListItem item)
        {
            var viewModel = new WorkflowItemVM();
            viewModel.ApproverPositionText = FormatUtil.ConvertLookupToValue(item, "approverposition");
            viewModel.ApproverPositionId = FormatUtil.ConvertLookupToID(item, "approverposition");
            Task<IEnumerable<ProfessionalMaster>> getApproverNamesTask =
                GetApproverUserNames(viewModel.ApproverPositionText);

            viewModel.Level = Convert.ToString(item["approverlevel"]);

            viewModel.ApproverUnitText = Convert.ToString(item["approverunit"]);

            var userNames = await getApproverNamesTask;
            var userName = userNames.FirstOrDefault();
            viewModel.ApproverEmail = userName.UserLogin;
            viewModel.Status = "Pending";
            viewModel.ApproverNameText = userName.Name;
            viewModel.ApproverName = AjaxComboBoxVM.GetDefaultValue(new AjaxComboBoxVM
            {
                Text = userName.Name,
                Value = userName.ID,
            });

            return viewModel;
        }


        public async Task<IEnumerable<WorkflowItemVM>> GetWorkflowDetailsTimeSheet(string requestor,
         string listName, int? headerid = null)
        {
            var viewModel = new WorkflowRouterVM();

            viewModel.ListName = listName;

            // Get Position in Professional Master
            var caml = @"<View><Query><Where><Eq>
                <FieldRef Name='officeemail' /><Value Type='Text'>" + requestor +
                @"</Value></Eq></Where></Query><ViewFields><FieldRef Name='Position' /><FieldRef Name='Project_x002f_Unit' /></ViewFields><QueryOptions /></View>";

            int? positionID = 0;
            foreach (var item in SPConnector.GetList(SP_PROMAS_LIST_NAME, _siteUrl, caml))
            {
                viewModel.RequestorPosition = FormatUtil.ConvertLookupToValue(item, "Position");
                positionID = FormatUtil.ConvertLookupToID(item, "Position");

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

            var workflowItems = new List<WorkflowItemVM>();
            foreach (var item in SPConnector.GetList(SP_WORKFLOW_LISTNAME, _siteUrl, caml))
            {
                var vm = await ConvertToWorkflowDetailTimeSheet(item);
                workflowItems.Add(vm);
            }

            return workflowItems;
        }
    }
}
