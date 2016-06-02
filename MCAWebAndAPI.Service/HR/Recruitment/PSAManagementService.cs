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
    public class PSAManagementService : IPSAManagementService
    {
        string _siteUrl;
        static Logger logger = LogManager.GetCurrentClassLogger();

        const string SP_PSA_LIST_NAME = "PSA";
        const string SP_PSA_DOC_LIST_NAME = "PSA Documents";

        public int CreatePSAManagement(PSAManagementVM psaManagement)
        {
            var updatedValues = new Dictionary<string, object>();
            //updatedValues.Add("Title", psaManagement.PSANumber);
            updatedValues.Add("isrenewal", psaManagement.IsRenewal.Value);
            updatedValues.Add("renewalnumber", psaManagement.RenewalNumber);
            updatedValues.Add("ProjectOrUnit", psaManagement.ProjectOrUnit.Value);
            updatedValues.Add("position", new FieldLookupValue { LookupId =  (int)psaManagement.Position.Value});
            //updatedValues.Add("position", psaManagement.Position);
            //updatedValues.Add("position_x003a_ID", new FieldLookupValue { LookupId = (int)psaManagement.PositionID });
            updatedValues.Add("professional", new FieldLookupValue { LookupId = (int)psaManagement.Professional.Value });
            updatedValues.Add("joindate", psaManagement.JoinDate);
            updatedValues.Add("dateofnewpsa", psaManagement.DateOfNewPSA);
            updatedValues.Add("tenure", psaManagement.Tenure);

            try
            {
                SPConnector.AddListItem(SP_PSA_LIST_NAME, updatedValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                throw e;
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
                JoinDate = Convert.ToDateTime(item["joindate"]).ToLocalTime().ToShortDateString(),
                DateOfNewPSA = Convert.ToDateTime(item["dateofnewpsa"]).ToLocalTime().ToShortDateString(),
                PsaExpiryDate = Convert.ToDateTime(item["psaexpirydate"]).ToLocalTime().ToShortDateString(),
                ProjectOrUnit = Convert.ToString(item["ProjectOrUnit"]),
                Position = item["position"] == null ? "" :
               Convert.ToString((item["position"] as FieldLookupValue).LookupValue)
            };
        }

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
        }

        public PSAManagementVM GetPSAManagement(int? ID)
        {
            var viewModel = new PSAManagementVM();
            if (ID == null)
            {
                return viewModel;
            }
                

            var listItem = SPConnector.GetListItem(SP_PSA_LIST_NAME, ID, _siteUrl);
            viewModel = ConvertToPSAManagementVM(listItem);

            return viewModel;

        }

        private PSAManagementVM ConvertToPSAManagementVM(ListItem listItem)
        {
            var viewModel = new PSAManagementVM();

            viewModel.ID = Convert.ToInt32(listItem["ID"]);
            //viewModel.PSANumber = Convert.ToString(listItem["Title"]);
            viewModel.IsRenewal.Text = Convert.ToString(listItem["isrenewal"]);
            viewModel.RenewalNumber = Convert.ToInt32(listItem["renewalnumber"]);
            viewModel.ProjectOrUnit.Value = Convert.ToString(listItem["ProjectOrUnit"]);
            viewModel.Position.Value = FormatUtil.ConvertLookupToID(listItem, "position");
            /*viewModel.Position = listItem["Position"] == null ? "" :
               Convert.ToString((listItem["Position"] as FieldLookupValue).LookupValue);*/
            //viewModel.Position = FormatUtil.ConvertLookupToValue(listItem, "position");
            viewModel.Professional.Text = FormatUtil.ConvertLookupToValue(listItem, "professional");
            viewModel.JoinDate = Convert.ToDateTime(listItem["joindate"]).ToLocalTime();
            viewModel.DateOfNewPSA = Convert.ToDateTime(listItem["dateofnewpsa"]).ToLocalTime();
            viewModel.Tenure = Convert.ToInt32(listItem["tenure"]);
            
            viewModel.PSAExpiryDate = Convert.ToDateTime(listItem["psaexpirydate"]).ToLocalTime();

            viewModel.DocumentUrl = GetDocumentUrl(viewModel.ID);

            return viewModel;
        }

        public PSAManagementVM ViewPSAManagementData(int? ID)
        {
            var viewModel = new PSAManagementVM();
            if (ID == null)
                return viewModel;

            var listItem = SPConnector.GetListItem(SP_PSA_LIST_NAME, ID, _siteUrl);
            viewModel = ConvertToViewPSAManagementVM(listItem);

            return viewModel;

        }

        private PSAManagementVM ConvertToViewPSAManagementVM(ListItem listItem)
        {
            var viewModel = new PSAManagementVM();

            viewModel.ID = Convert.ToInt32(listItem["ID"]);
            viewModel.PSANumber = Convert.ToString(listItem["Title"]);
            viewModel.IsRenewal.Value = Convert.ToString(listItem["isrenewal"]);
            viewModel.RenewalNumber = Convert.ToInt32(listItem["renewalnumber"]);
            viewModel.ProjectOrUnit.Value = Convert.ToString(listItem["ProjectOrUnit"]);
            /*viewModel.Position.Text = FormatUtil.ConvertLookupToValue(listItem, "position");*/
            //viewModel.Position = Convert.ToString(listItem["Position"]);
            viewModel.Professional.Text = FormatUtil.ConvertLookupToValue(listItem, "professional");
            viewModel.JoinDate = Convert.ToDateTime(listItem["joindate"]).ToLocalTime();
            viewModel.DateOfNewPSA = Convert.ToDateTime(listItem["dateofnewpsa"]).ToLocalTime();
            viewModel.Tenure = Convert.ToInt32(listItem["tenure"]);

            viewModel.PSAExpiryDate = Convert.ToDateTime(listItem["psaexpirydate"]).ToLocalTime();

            viewModel.DocumentUrl = GetDocumentUrl(viewModel.ID);

            return viewModel;
        }

        private string GetDocumentUrl(int? iD)
        {
            return string.Format(UrlResource.PSAManagementDocumentByID, _siteUrl, iD);
        }
        
        public bool UpdatePSAManagement(PSAManagementVM psaManagement)
        {
            var columnValues = new Dictionary<string, object>();
            int ID = psaManagement.ID.Value;

            //columnValues.Add("Title", psaManagement.PSANumber);
            columnValues.Add("isrenewal", psaManagement.IsRenewal.Value);
            columnValues.Add("renewalnumber", psaManagement.RenewalNumber);
            columnValues.Add("ProjectOrUnit", psaManagement.ProjectOrUnit.Value);
            columnValues.Add("position", new FieldLookupValue { LookupId = Convert.ToInt32(psaManagement.Position.Value) });
            //columnValues.Add("position", psaManagement.Position);
            //columnValues.Add("professional", new FieldLookupValue { LookupId = Convert.ToInt32(psaManagement.Professional.Value) });
            columnValues.Add("joindate", psaManagement.JoinDate.Value);
            columnValues.Add("dateofnewpsa", psaManagement.DateOfNewPSA.Value);
            columnValues.Add("tenure", psaManagement.Tenure);

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

        private IEnumerable<HttpPostedFileBase> GetDocuments(int? iD)
        {
            var caml = @"<View>  
            <Query> 
               <Where><Eq><FieldRef Name='psa' LookupId='True' /><Value Type='Lookup'>" + iD
               + @"</Value></Eq></Where> 
            </Query>
            <ViewFields><FieldRef Name='Title' /><FieldRef Name='ID' /><FieldRef Name='FileRef' /></ViewFields></View>";

            throw new NotImplementedException();
        }

        public void CreatePSAManagementDocuments(int? psaID, IEnumerable<HttpPostedFileBase> documents, PSAManagementVM psaManagmement)
        {
            foreach (var doc in documents)
            {
                var updateValue = new Dictionary<string, object>();
                updateValue.Add("psa", new FieldLookupValue { LookupId = Convert.ToInt32(psaID) });
                updateValue.Add("documenttype", psaManagmement.DocumentType.Value);

                try
                {
                    SPConnector.UploadDocument(SP_PSA_DOC_LIST_NAME, updateValue, doc.FileName, doc.InputStream, _siteUrl);
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw e;
                }
            }
        }
    }
}
