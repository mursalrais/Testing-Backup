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

namespace MCAWebAndAPI.Service.HR.Recruitment
{
    public class ExitProcedureService : IExitProcedureService
    {
        string _siteUrl;
        static Logger logger = LogManager.GetCurrentClassLogger();

        const string SP_EXP_LIST_NAME = "Exit Procedure";
        const string SP_PSA_DOC_LIST_NAME = "Professional Documents";

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
            else
            {
                var listItem = SPConnector.GetListItem(SP_EXP_LIST_NAME, ID, _siteUrl);
                viewModel = ConvertToExitProcedureVM(listItem);
            }

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

            return viewModel;
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
    }
}
