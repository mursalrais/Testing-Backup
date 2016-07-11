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
using System.Globalization;
using MCAWebAndAPI.Model.ViewModel.Form.Common;
using MCAWebAndAPI.Model.ViewModel.Control;


namespace MCAWebAndAPI.Service.HR.Recruitment
{
    public class ExitProcedureService : IExitProcedureService
    {
        string _siteUrl;
        static Logger logger = LogManager.GetCurrentClassLogger();

        const string SP_EXP_LIST_NAME = "Exit Procedure";
        const string SP_EXP_DOC_LIST_NAME = "ExitProcedureDocuments";
        const string SP_WORKFLOW_LISTNAME = "Workflow Mapping Master";
        const string SP_PROMAS_LIST_NAME = "Professional Master";
        const string SP_POSMAS_LIST_NAME = "Position Master";

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
        }

        //Display Exit Procedure Data based on ID

        public int CreateExitProcedure(ExitProcedureVM exitProcedure)
        {
            var updatedValues = new Dictionary<string, object>();

            updatedValues.Add("Title", exitProcedure.FullName);
            updatedValues.Add("requestdate", exitProcedure.RequestDate);
            updatedValues.Add("professional", new FieldLookupValue { LookupId = (int)exitProcedure.Professional.Value });
            updatedValues.Add("projectunit", exitProcedure.ProjectUnit);
            updatedValues.Add("position", exitProcedure.Position);
            updatedValues.Add("mobilenumber", exitProcedure.PhoneNumber);
            updatedValues.Add("officeemail", exitProcedure.EmailAddress);
            updatedValues.Add("currentaddress", exitProcedure.CurrentAddress);
            updatedValues.Add("joindate", exitProcedure.JoinDate);
            updatedValues.Add("lastworkingdate", exitProcedure.LastWorkingDate);
            updatedValues.Add("exitreason", exitProcedure.ExitReason.Value);
            updatedValues.Add("reasondescription", exitProcedure.ReasonDesc);
            updatedValues.Add("psanumber", exitProcedure.PSANumber);

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

        public ExitProcedureVM GetExitProcedure(int? ID)
        {
            var viewModel = new ExitProcedureVM();

            if (ID == null)
            {
                return viewModel;
            }

            var listItem = SPConnector.GetListItem(SP_EXP_LIST_NAME, ID, _siteUrl);
            viewModel = ConvertToExitProcedureVM(listItem);

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

            viewModel.DocumentUrl = GetDocumentUrl(viewModel.ID);

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
                updateValue.Add("professional", new FieldLookupValue { LookupId = Convert.ToInt32(exitProcedure.Professional.Value) });

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

        public async Task<ExitProcedureVM> GetWorkflowRouterExitProcedure(string listName, string requestor)
        {
            var viewModel = new ExitProcedureVM();
            //var viewModel = new WorkflowRouterVM();
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

            var exitProcedureCheckList = new List<ExitProcedureChecklistVM>();
            foreach (var item in SPConnector.GetList(SP_WORKFLOW_LISTNAME, _siteUrl, caml))
            {
                if (string.Compare(Convert.ToString(item["isdefault"]), "No",
                    StringComparison.OrdinalIgnoreCase) == 0
                    &&
                    string.Compare(Convert.ToString(item["workflowtype"]), "Sequential",
                    StringComparison.OrdinalIgnoreCase) == 0)
                    continue;

                var vm = await ConvertToExitProcedureChecklistVM(item);
                exitProcedureCheckList.Add(vm);
            }

            //viewModel.WorkflowItems = workflowItems;
            viewModel.ExitProcedureChecklist = exitProcedureCheckList;

            return viewModel;
        }

        private async Task<ExitProcedureChecklistVM> ConvertToExitProcedureChecklistVM(ListItem item)
        {

            var viewModel = new ExitProcedureChecklistVM();
            viewModel.ApproverPosition = FormatUtil.ConvertToInGridAjaxComboBox(item, "approverposition");
            Task<IEnumerable<ProfessionalMaster>> getApproverNamesTask =
                GetApproverNamesAsync(viewModel.ApproverPosition.Text);

            viewModel.Level = Convert.ToString(item["approverlevel"]);

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
            
            //else if (viewModel.Level == "10")
            //{
            //    viewModel.ItemExitProcedure = "Resignation/Separation Letter";
            //}
            //else if (viewModel.Level == "11")
            //{
            //    viewModel.ItemExitProcedure = "Timesheet/Leave Form";
            //}
            //else if (viewModel.Level == "12")
            //{
            //    viewModel.ItemExitProcedure = "Exit Interview/NDA";
            //}
            //else if (viewModel.Level == "13")
            //{
            //    viewModel.ItemExitProcedure = "Insurance Card";
            //}
            //else if (viewModel.Level == "14")
            //{
            //    viewModel.ItemExitProcedure = "ID Card & Access Card";
            //}

            viewModel.ApproverUnit =
                ExitProcedureChecklistVM.GetUnitDefaultValue(new InGridComboBoxVM
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

        private async Task<IEnumerable<ProfessionalMaster>> GetApproverNamesAsync(string position)
        {
            return GetApproverNames(position);
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

        //public void CreateExitProcedureChecklist(ExitProcedureChecklistVM exitProcedureChecklist)
        //{
        //    var updatedValues = new Dictionary<string, object>();

        //    updatedValues.Add("Title", exitProcedure.FullName);
        //    updatedValues.Add("requestdate", exitProcedure.RequestDate);
        //    updatedValues.Add("professional", new FieldLookupValue { LookupId = (int)exitProcedure.Professional.Value });
        //    updatedValues.Add("projectunit", exitProcedure.ProjectUnit);
        //    updatedValues.Add("position", exitProcedure.Position);
        //    updatedValues.Add("mobilenumber", exitProcedure.PhoneNumber);
        //    updatedValues.Add("officeemail", exitProcedure.EmailAddress);
        //    updatedValues.Add("currentaddress", exitProcedure.CurrentAddress);
        //    updatedValues.Add("joindate", exitProcedure.JoinDate);
        //    updatedValues.Add("lastworkingdate", exitProcedure.LastWorkingDate);
        //    updatedValues.Add("exitreason", exitProcedure.ExitReason.Value);
        //    updatedValues.Add("reasondescription", exitProcedure.ReasonDesc);
        //    updatedValues.Add("psanumber", exitProcedure.PSANumber);

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

        //    /*
        //    var entity = new ExitProcedureChecklistVM();

        //    //var updatedValues = new Dictionary<string, object>();
        //    entity.ApproverUnit.Value = (int)exitProcedureChecklist.ApproverUnit.Value;
        //    entity.ApproverPosition.Value = (int)exitProcedureChecklist.ApproverPosition.Value;
        //    entity.ApproverUserName.Value = (int)exitProcedureChecklist.ApproverUserName.Value;
        //    entity.Level = exitProcedureChecklist.Level;

        //    entity.

        //    /*
        //    if (entity.CategoryID == null)
        //    {
        //        entity.CategoryID = 1;
        //    }
        //    */

        //    /*
        //    if (product.Category != null)
        //    {
        //        entity.CategoryID = product.Category.CategoryID;
        //    }
        //    */

        //    /*
        //    exit.Products.Add(entity);
        //    entities.SaveChanges();

        //    product.ProductID = entity.ProductID;
        //    */
        //}
    }
}
