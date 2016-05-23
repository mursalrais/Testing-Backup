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
                }
                catch (Exception e)
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
                }
                catch (Exception e)
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
            viewModel.LastName = Convert.ToString(listItem["lastname"]);
            viewModel.PlaceOfBirth = Convert.ToString("placeofbirth");
            viewModel.DateOfBirth = Convert.ToDateTime("dateofbirth");
            viewModel.CurrentAddress = Convert.ToString(listItem["currentaddress"]);
            viewModel.IDCardNumber = Convert.ToString(listItem["idcardnumber"]);
            viewModel.Telephone = Convert.ToString(listItem["permanentlandlinephone"]);
            viewModel.CurrentTelephone = Convert.ToString(listItem["currentlandlinephone"]);
            viewModel.EmailAddresOne = Convert.ToString(listItem["personalemail"]);
            viewModel.EmailAddresTwo = Convert.ToString(listItem["personalemail2"]);
            viewModel.MobileNumberOne = Convert.ToString(listItem["mobilephonenr"]);
            viewModel.MobileNumberTwo = Convert.ToString(listItem["mobilephonenr2"]);
            viewModel.SpecializationField = Convert.ToString(listItem["specializationfield"]);
            viewModel.YearRelevanWork = Convert.ToInt32(listItem["totalrelevantexperienceyears"]);
            viewModel.MonthRelevantWork = Convert.ToInt32(listItem["totalrelevantexperiencemonths"]);
            viewModel.MaritalStatus.DefaultValue = Convert.ToString(listItem["maritalstatus"]);
            viewModel.BloodType.DefaultValue = Convert.ToString(listItem["bloodtype"]);
            viewModel.Religion.DefaultValue = Convert.ToString(listItem["Religion"]);
            viewModel.Gender.DefaultValue = Convert.ToString(listItem["Gender"]);
            viewModel.IDCardType.DefaultValue = Convert.ToString(listItem["idcardtype"]);
            viewModel.IDCardExpiry = Convert.ToDateTime(listItem["idcardexpirydate"]);
            viewModel.Nationality.DefaultValue = FormatUtil.ConvertLookupToID(listItem, "Nationality") + string.Empty;

            // Convert Details
            viewModel.EducationDetails = GetEducationDetails(viewModel.ID);
            viewModel.TrainingDetails = GetTrainingDetails(viewModel.ID);
            viewModel.WorkingExperienceDetails = GetWorkingExperienceDetails(viewModel.ID);
            viewModel.Documents = GetDocuments(viewModel.ID);

            return viewModel;

        }

        private IEnumerable<HttpPostedFileBase> GetDocuments(int? iD)
        {
            var caml = @"<View>  
            <Query> 
               <Where><Eq><FieldRef Name='application' LookupId='True' /><Value Type='Lookup'>" + iD 
               + @"</Value></Eq></Where> 
            </Query>
            <ViewFields><FieldRef Name='Title' /><FieldRef Name='ID' /><FieldRef Name='FileRef' /></ViewFields></View>";

            throw new NotImplementedException();
        }

        private IEnumerable<WorkingExperienceDetailVM> GetWorkingExperienceDetails(int? iD)
        {
            var caml = @"<View>  
            <Query> 
               <Where><Eq><FieldRef Name='application' LookupId='True' /><Value Type='Lookup'>" + iD + @"</Value></Eq></Where> 
            </Query> 
             <ViewFields><FieldRef Name='ID' /><FieldRef Name='application' /><FieldRef Name='Title' /><FieldRef Name='applicationcompany' /><FieldRef Name='applicationfrom' /><FieldRef Name='applicationto' /><FieldRef Name='applicationjobdescription' /></ViewFields> 
      </View>";

            var workingExperienceDetails = new List<WorkingExperienceDetailVM>();
            foreach(var item in SPConnector.GetList(SP_APPWORK_LIST_NAME, _siteUrl, caml))
            {
                workingExperienceDetails.Add(ConvertToWorkingExperienceDetailVM(item));
            }

            return workingExperienceDetails;
        }

        private WorkingExperienceDetailVM ConvertToWorkingExperienceDetailVM(ListItem item)
        {
            return new WorkingExperienceDetailVM
            {
                ID = Convert.ToInt32(item["ID"]),
                Company = Convert.ToString(item["applicationcompany"]),
                Position = Convert.ToString(item["Title"]),
                JobDescription = Convert.ToString(item["applicationjobdescription"]),
                From = Convert.ToDateTime(item["applicationfrom"]),
                To = Convert.ToDateTime(item["applicationto"])
            };
        }

        private IEnumerable<TrainingDetailVM> GetTrainingDetails(int? iD)
        {
            var caml = @"<View>  
            <Query> 
               <Where><Eq><FieldRef Name='application' LookupId='True' /><Value Type='Lookup'>" + iD 
               + @"</Value></Eq></Where> 
            </Query> 
             <ViewFields><FieldRef Name='ID' /><FieldRef Name='Title' /><FieldRef Name='applicationtraininginstitution' /><FieldRef Name='applicationtrainingyear' /><FieldRef Name='applicationtrainingremarks' /><FieldRef Name='application' /></ViewFields> 
            </View>";

            var trainingDetails = new List<TrainingDetailVM>();
            foreach (var item in SPConnector.GetList(SP_APPTRAIN_LIST_NAME, _siteUrl, caml))
            {
                trainingDetails.Add(ConvertToTrainingDetailVM(item));
            }

            return trainingDetails;
        }

        private TrainingDetailVM ConvertToTrainingDetailVM(ListItem item)
        {
            return new TrainingDetailVM
            {
                ID = Convert.ToInt32(item["ID"]),
                Subject = Convert.ToString(item["Title"]),
                Institution = Convert.ToString(item["applicationtraininginstitution"]),
                Remarks = Convert.ToString(item["applicationtrainingremarks"]),
                Year = Convert.ToDateTime(item["applicationtrainingyear"])
            };
        }

        private IEnumerable<EducationDetailVM> GetEducationDetails(int? iD)
        {
            var caml = @"<View>  
            <Query> 
               <Where><Eq><FieldRef Name='applications' LookupId='True' /><Value Type='Lookup'>" + iD + @"</Value></Eq></Where> 
            </Query> 
             <ViewFields><FieldRef Name='applications' /><FieldRef Name='applicationuniversity' /><FieldRef Name='applicationyearofgraduation' /><FieldRef Name='Title' /><FieldRef Name='ID' /></ViewFields> 
            </View>";

            var eduacationDetails = new List<EducationDetailVM>();
            foreach (var item in SPConnector.GetList(SP_APPEDU_LIST_NAME, _siteUrl, caml))
            {
                eduacationDetails.Add(ConvertToEducationDetailVM(item));
            }

            return eduacationDetails;
        }

        private EducationDetailVM ConvertToEducationDetailVM(ListItem item)
        {
            return new EducationDetailVM
            {
                ID = Convert.ToInt32(item["ID"]),
                Subject = Convert.ToString(item["Title"]),
                University = Convert.ToString(item["applicationuniversity"]),
                YearOfGraduation = Convert.ToDateTime(item["applicationyearofgraduation"])
            };
        }

        public void SetSiteUrl(string siteUrl = null)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
        }
    }
}
