using System;
using System.Collections.Generic;
using MCAWebAndAPI.Model.HR.DataMaster;
using MCAWebAndAPI.Model.ViewModel.Form.Common;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;

namespace MCAWebAndAPI.Service.Common
{
    public class WorkflowService : IWorkflowService
    {
        string _siteUrl;
        const string SP_WORKFLOW_LISTNAME = "Workflow Mapping Master";
        const string SP_PROMAS_LIST_NAME = "Professional Master";
        const string SP_POSMAS_LIST_NAME = "Position Master";

        public IEnumerable<PositionMaster> GetPositionsInWorkflow(string listName, 
            string requestorUnit, string requestorPosition)
        {
            var caml = @"<View>  
            <Query> 
               <Where><And><And><Eq><FieldRef Name='requestorposition' /><Value Type='Lookup'>" + 
                requestorPosition + @"</Value></Eq><Eq><FieldRef Name='requestorunit' /><Value Type='Choice'>" + 
                requestorUnit + @"</Value></Eq></And><Eq><FieldRef Name='transactiontype' /><Value Type='Choice'>" + 
                listName + @"</Value></Eq></And></Where> 
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
       

        public WorkflowRouterVM GetWorkflowRouter(string listName, string requestor)
        {
            var viewModel = new WorkflowRouterVM();
            viewModel.ListName = listName; 

            // Get Position in Professional Master
            var caml = @"<View><Query><Where><Eq>
                <FieldRef Name='officeemail' /><Value Type='Text'>" + requestor + 
                @"</Value></Eq></Where></Query><ViewFields><FieldRef Name='Position' /></ViewFields><QueryOptions /></View>";

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
               @"</Value></Eq><Eq><FieldRef Name='requestorunit' /><Value Type='Choice'>" + viewModel.RequestorUnit +  @"</Value></Eq></And><Eq>
               <FieldRef Name='transactiontype' /><Value Type='Choice'>" + listName + @"</Value></Eq></And></Where> 
            <OrderBy><FieldRef Name='approverlevel' /></OrderBy>
            </Query> 
            </View>";

            var workflowItems = new List<WorkflowItemVM>();
            foreach (var item in SPConnector.GetList(SP_WORKFLOW_LISTNAME, _siteUrl, caml))
            {
                if( string.Compare(Convert.ToString(item["isdefault"]), "No",
                    StringComparison.OrdinalIgnoreCase) == 0
                    && 
                    string.Compare(Convert.ToString(item["workflowtype"]), "Sequential", 
                    StringComparison.OrdinalIgnoreCase) == 0)
                continue;

                var vm = ConvertToWorkflowItemVM(item);
                workflowItems.Add(vm);
            }

            viewModel.WorkflowItems = workflowItems;

            return viewModel;
        }

        private WorkflowItemVM ConvertToWorkflowItemVM(ListItem item)
        {
            var viewModel = new WorkflowItemVM();
            viewModel.Level = Convert.ToString(item["approverlevel"]);
            viewModel.ApproverUnit = FormatUtil.ConvertToInGridComboBox(item, "approverunit");
            viewModel.ApproverPosition = FormatUtil.ConvertToInGridAjaxLookup(item, "approverposition");

            return viewModel;
        }

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
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

        private ProfessionalMaster ConvertToProfessionalMasterVM(ListItem item)
        {
            return new ProfessionalMaster
            {
                ID = Convert.ToInt32(item["ID"]),
                Name = Convert.ToString(item["Title"]),
                UserLogin = Convert.ToString(item["officeemail"])
            };
        }

        public void CreateTransactionWorkflow(string workflowTransactionListName, string transactionLookupColumnName, int headerID, IEnumerable<WorkflowItemVM> workflowItems)
        {
            foreach (var item in workflowItems)
            {
                CreateTransactionWorkflowItem(workflowTransactionListName, transactionLookupColumnName, headerID, item);
            }
        }

        public void SendApprovalRequest(string workflowTransactionListName, string transactionLookupColumnName, int headerID, int level, string message)
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
                emails.Add(Convert.ToString(item["approver"]));
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
            updatedValue.Add("approver", workflowItem.ApproverUserName);
            updatedValue.Add("requestor", requestor);
            SPConnector.AddListItem(workflowTransactionListName, updatedValue, _siteUrl);
        }
    }
}
