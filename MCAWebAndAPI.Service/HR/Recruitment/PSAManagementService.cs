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

namespace MCAWebAndAPI.Service.HR.Recruitment
{
    public class PSAManagementService : IPSAManagementService
    {
        string _siteUrl = "https://eceos2.sharepoint.com/sites/mca-dev/hr/";
        static Logger logger = LogManager.GetCurrentClassLogger();
        const string SP_PSA_LIST_NAME = "PSA";

       
        public int CreatePSA(PSAManagementVM psaManagement)
        {
            var updatedValues = new Dictionary<string, object>();
            updatedValues.Add("Title", psaManagement.psaNumber);
            updatedValues.Add("isrenewal", psaManagement.IsRenewal.Value);
            updatedValues.Add("renewalnumber", psaManagement.renewalnumber);
            updatedValues.Add("ProjectOrUnit", psaManagement.ProjectOrUnit1.Value);
            updatedValues.Add("position", new FieldLookupValue { LookupId = Convert.ToInt32(psaManagement.Position.Value) });
            updatedValues.Add("professional", new FieldLookupValue { LookupId = Convert.ToInt32(psaManagement.Professional.Value) });
            updatedValues.Add("joindate", psaManagement.joindate);
            updatedValues.Add("dateofnewpsa", psaManagement.dateofnewpsa);
            updatedValues.Add("tenure", psaManagement.tenure);
            updatedValues.Add("psaexpirydate", psaManagement.psaexpirydate);

            try
            {
                SPConnector.AddListItem(SP_PSA_LIST_NAME, updatedValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }

            return SPConnector.GetInsertedItemID(SP_PSA_LIST_NAME, _siteUrl); 
        }

        public IEnumerable<PSAMaster> GetPSAs()
        {
            var models = new List<PSAMaster>();
                foreach (var item in SPConnector.GetList(SP_PSA_LIST_NAME, _siteUrl))
            {
                models.Add(ConvertToPSAModel(item));
            }

            return models;
        }
        
        private PSAMaster ConvertToPSAModel(ListItem item)
        {
            return new PSAMaster
            {
                ID = item["professional_x003a_ID"] == null ? "" :
               Convert.ToString((item["professional_x003a_ID"] as FieldLookupValue).LookupValue),
                PSAID = Convert.ToString(item["ID"]),
                JoinDate = Convert.ToString(item["joindate"]),
                DateOfNewPSA = Convert.ToString(item["dateofnewpsa"]),
                PsaExpiryDate = Convert.ToString(item["psaexpirydate"]),
                ProjectOrUnit = Convert.ToString(item["ProjectOrUnit"]),
                Position = item["position"] == null ? "" :
               Convert.ToString((item["position"] as FieldLookupValue).LookupValue)

            };
        }

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
        }

        public PSAManagementVM GetPopulatedModel(int? id = default(int?))
        {
            var model = new PSAManagementVM();
            return model;
        }

        public PSAManagementVM GetPSAManagement(int ID)
        {
            var listItem = SPConnector.GetListItem(SP_PSA_LIST_NAME, ID, _siteUrl);
            var viewModel = new PSAManagementVM();

            viewModel.psaNumber = Convert.ToString(listItem["Title"]);
            viewModel.IsRenewal.Choices = SPConnector.GetChoiceFieldValues(SP_PSA_LIST_NAME, "isrenewal");
            viewModel.IsRenewal.DefaultValue = Convert.ToString(listItem["isrenewal"]);

            viewModel.renewalnumber = Convert.ToInt32(listItem["renewalnumber"]);

            viewModel.ProjectOrUnit1.Choices = SPConnector.GetChoiceFieldValues(SP_PSA_LIST_NAME, "ProjectUnit");
            viewModel.ProjectOrUnit1.DefaultValue = Convert.ToString(listItem["ProjectUnit"]);

            viewModel.Position.DefaultValue = Convert.ToString(listItem["position"]);
            viewModel.Professional.DefaultValue = Convert.ToString(listItem["professional"]);

            viewModel.joindate = Convert.ToDateTime(listItem["joindate"]).ToLocalTime();
            viewModel.dateofnewpsa = Convert.ToDateTime(listItem["dateofnewpsa"]).ToLocalTime();
            viewModel.tenure = Convert.ToInt32(listItem["tenure"]);
            viewModel.psaexpirydate = Convert.ToDateTime(listItem["psaexpirydate"]).ToLocalTime();

            
            return viewModel;
        }

        public bool UpdatePSAManagement(PSAManagementVM psaManagement)
        {
            var columnValues = new Dictionary<string, object>();
            int ID = psaManagement.ID.Value;

            columnValues.Add("Title", psaManagement.psaNumber);
            columnValues.Add("isrenewal", psaManagement.IsRenewal.Value);
            columnValues.Add("renewalnumber", psaManagement.renewalnumber);
            columnValues.Add("ProjectOrUnit", psaManagement.ProjectOrUnit1.Value);
            columnValues.Add("position", new FieldLookupValue { LookupId = Convert.ToInt32(psaManagement.Position.Value) });
            columnValues.Add("professional", new FieldLookupValue { LookupId = Convert.ToInt32(psaManagement.Professional.Value) });
            columnValues.Add("joindate", psaManagement.joindate.Value);
            columnValues.Add("dateofnewpsa", psaManagement.dateofnewpsa.Value);
            columnValues.Add("tenure", psaManagement.tenure);
            columnValues.Add("psaexpirydate", psaManagement.psaexpirydate);

            try
            {
                SPConnector.UpdateListItem(SP_PSA_LIST_NAME, ID, columnValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                return false;
            }

            var entitiy = new PSAManagementVM();
            entitiy = psaManagement;
            return true;
        }

    }
}
