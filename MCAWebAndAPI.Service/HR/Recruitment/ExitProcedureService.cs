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
            viewModel.Professional.Text = FormatUtil.ConvertLookupToValue(listItem, "professional");
            viewModel.Position = Convert.ToString(listItem["position"]);
            viewModel.PhoneNumber = Convert.ToString(listItem["mobilenumber"]);
            viewModel.EmailAddress = Convert.ToString(listItem["officeemail"]);
            viewModel.CurrentAddress = FormatUtil.ConvertMultipleLine(Convert.ToString(listItem["currentaddress"]));
            viewModel.LastWorkingDate = Convert.ToDateTime(listItem["lastworkingdate"]).ToLocalTime();
            viewModel.ExitReason.Value =  Convert.ToString(listItem["exitreason"]);
            viewModel.ReasonDesc = FormatUtil.ConvertMultipleLine(Convert.ToString(listItem["reasondescription"]));

            /*
            viewModel.IsRenewal.Text = Convert.ToString(listItem["isrenewal"]);
            viewModel.RenewalNumber = Convert.ToInt32(listItem["renewalnumber"]);
            viewModel.ProjectOrUnit.Value = Convert.ToString(listItem["ProjectOrUnit"]);
            viewModel.Position.Value = FormatUtil.ConvertLookupToID(listItem, "position");
            viewModel.Professional.Text = FormatUtil.ConvertLookupToValue(listItem, "professional");
            viewModel.JoinDate = Convert.ToDateTime(listItem["joindate"]).ToLocalTime();
            viewModel.DateOfNewPSA = Convert.ToDateTime(listItem["dateofnewpsa"]).ToLocalTime();
            viewModel.Tenure = Convert.ToInt32(listItem["tenure"]);
            viewModel.PSAExpiryDate = Convert.ToDateTime(listItem["psaexpirydate"]).ToLocalTime();
            viewModel.PSAStatus.Text = Convert.ToString(listItem["psastatus"]);

            viewModel.DocumentUrl = GetDocumentUrl(viewModel.ID);
            */

            return viewModel;
        }
    }
}
