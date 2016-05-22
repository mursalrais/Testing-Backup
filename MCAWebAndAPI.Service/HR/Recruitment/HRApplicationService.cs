using System;
using System.Collections.Generic;
using System.Web;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.Utils;
using NLog;
using Microsoft.SharePoint.Client;
using MCAWebAndAPI.Service.Resources;

namespace MCAWebAndAPI.Service.HR.Recruitment
{
    public class HRApplicationService : IHRApplicationService
    {
        string _siteUrl;
        static Logger logger = LogManager.GetCurrentClassLogger();

        const string SP_APPDATA_LIST_NAME = "Application";
        const string SP_APPEDU_LIST_NAME = "Application Education";
        const string SP_APPWORK_LIST_NAME = "Application Working Experience";
        const string SP_APPTRAIN_LIST_NAME = "Application Training";
        const string SP_APPDOC_LIST_NAME = "Application Documents";
        
        public int CreateApplicationData(ApplicationDataVM viewModel)
        {
            var updatedValue = new Dictionary<string, object>();

            updatedValue.Add("Title", viewModel.FirstMiddleName);
            updatedValue.Add("lastname", viewModel.LastName);
            updatedValue.Add("placeofbirth", viewModel.PlaceOfBirth);
            updatedValue.Add("dateofbirth", viewModel.DateOfBirth);
            updatedValue.Add("idcardnumber", viewModel.IDCardNumber);
            updatedValue.Add("permanentaddress", viewModel.PermanentAddress);
            updatedValue.Add("permanentlandlinephone", FormatUtil.ConvertToCleanPhoneNumber(viewModel.Telephone));
            updatedValue.Add("currentaddress", viewModel.CurrentAddress);
            updatedValue.Add("currentlandlinephone", FormatUtil.ConvertToCleanPhoneNumber(viewModel.CurrentTelephone));
            updatedValue.Add("personalemail", viewModel.EmailAddresOne);
            updatedValue.Add("personalemail2", viewModel.EmailAddresTwo);
            updatedValue.Add("mobilephonenr", FormatUtil.ConvertToCleanPhoneNumber(viewModel.MobileNumberOne));
            updatedValue.Add("mobilephonenr2", FormatUtil.ConvertToCleanPhoneNumber(viewModel.MobileNumberTwo));
            updatedValue.Add("specializationfield", viewModel.SpecializationField);
            updatedValue.Add("totalrelevantexperienceyears", viewModel.YearRelevanWork);
            updatedValue.Add("totalrelevantexperiencemonths", viewModel.MonthRelevantWork);
            updatedValue.Add("maritalstatus", viewModel.MaritalStatus.Value);
            updatedValue.Add("bloodtype", viewModel.BloodType.Value);
            updatedValue.Add("Religion", viewModel.Religion.Value);
            updatedValue.Add("Gender", viewModel.Gender.Value);
            updatedValue.Add("idcardtype", viewModel.IDCardType.Value);
            updatedValue.Add("idcardexpirydate", viewModel.IDCardExpiry);
            updatedValue.Add("Nationality", new FieldLookupValue { LookupId = viewModel.Nationality.Value });
            updatedValue.Add("applicationstatus", WorkflowUtil.DRAFT);

            try
            {
                SPConnector.AddListItem(SP_APPDATA_LIST_NAME, updatedValue, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                throw new Exception(ErrorResource.SPInsertError);
            }

            return SPConnector.GetInsertedItemID(SP_APPDATA_LIST_NAME, _siteUrl);
        }

        public void CreateEducationDetails(int? headerID, IEnumerable<EducationDetailVM> viewModels)
        {
            foreach (var viewModel in viewModels)
            {
                var updatedValue = new Dictionary<string, object>();
                updatedValue.Add("Title", viewModel.Subject);
                updatedValue.Add("applicationuniversity", viewModel.University);
                updatedValue.Add("applicationyearofgraduation", FormatUtil.ConvertToYearString(viewModel.YearOfGraduation));
                updatedValue.Add("applications", new FieldLookupValue { LookupId = Convert.ToInt32(headerID) });

                try
                {
                    SPConnector.AddListItem(SP_APPEDU_LIST_NAME, updatedValue, _siteUrl);
                }catch(Exception e)
                {
                    logger.Error(e.Message);
                    throw new Exception(ErrorResource.SPInsertError);
                }
            }
        }

        public void CreateProfessionalDocuments(int? headerID, IEnumerable<HttpPostedFileBase> documents)
        {
            foreach (var doc in documents)
            {
                try
                {
                    SPConnector.UploadDocument(SP_APPDOC_LIST_NAME, doc.FileName, doc.InputStream, _siteUrl);
                }catch(Exception e)
                {
                    logger.Error(e.Message);
                    throw new Exception(ErrorResource.SPInsertError);
                }
            }
        }

        public void CreateTrainingDetails(int? headerID, IEnumerable<TrainingDetailVM> trainingDetails)
        {
            foreach (var viewModel in trainingDetails)
            {
                var updatedValue = new Dictionary<string, object>();
                updatedValue.Add("Title", viewModel.Subject);
                updatedValue.Add("applicationtraininginstitution", viewModel.Institution);
                updatedValue.Add("applicationtrainingremarks", viewModel.Remarks);
                updatedValue.Add("applicationtrainingyear", FormatUtil.ConvertToYearString(viewModel.Year));
                updatedValue.Add("application", new FieldLookupValue { LookupId = Convert.ToInt32(headerID) });

                try
                {
                    SPConnector.AddListItem(SP_APPTRAIN_LIST_NAME, updatedValue, _siteUrl);
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw new Exception(ErrorResource.SPInsertError);
                }
            }
        }

        public void CreateWorkingExperienceDetails(int? headerID, IEnumerable<WorkingExperienceDetailVM> workingExperienceDetails)
        {
            foreach (var viewModel in workingExperienceDetails)
            {
                var updatedValue = new Dictionary<string, object>();
                updatedValue.Add("Title", viewModel.Position);
                updatedValue.Add("applicationcompany", viewModel.Company);
                updatedValue.Add("applicationfrom", FormatUtil.ConvertToYearString(viewModel.From));
                updatedValue.Add("applicationto", FormatUtil.ConvertToYearString(viewModel.To));
                updatedValue.Add("application", new FieldLookupValue { LookupId = Convert.ToInt32(headerID) });
                updatedValue.Add("applicationjobdescription", viewModel.JobDescription);

                try
                {
                    SPConnector.AddListItem(SP_APPWORK_LIST_NAME, updatedValue, _siteUrl);
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw new Exception(ErrorResource.SPInsertError);
                }
            }
        }

        public ApplicationDataVM GetApplicationData(int? ID)
        {
            var viewModel = new ApplicationDataVM();
            if (ID == null)
                return viewModel;

            var listItem = SPConnector.GetListItem(SP_APPDATA_LIST_NAME, ID, _siteUrl);
            viewModel = ConvertToApplicationDataVM(listItem);

            return viewModel;

        }

        private ApplicationDataVM ConvertToApplicationDataVM(ListItem listItem)
        {
            var viewModel = new ApplicationDataVM();
            viewModel.ID = Convert.ToInt32(listItem["ID"]);
            viewModel.FirstMiddleName = Convert.ToString(listItem["Title"]);

            viewModel.EducationDetails = GetEducationDetails(viewModel.ID);
            viewModel.TrainingDetails = GetTrainingDetails(viewModel.ID);
            viewModel.WorkingExperienceDetails = GetWorkingExperienceDetails(viewModel.ID);
            viewModel.Documents = GetDocuments(viewModel.ID);

            //TODO: To Map one by one
            return viewModel;

        }

        private IEnumerable<HttpPostedFileBase> GetDocuments(int? iD)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<WorkingExperienceDetailVM> GetWorkingExperienceDetails(int? iD)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<TrainingDetailVM> GetTrainingDetails(int? iD)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<EducationDetailVM> GetEducationDetails(int? iD)
        {
            throw new NotImplementedException();
        }

        public void SetSiteUrl(string siteUrl = null)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
        }
    }
}
