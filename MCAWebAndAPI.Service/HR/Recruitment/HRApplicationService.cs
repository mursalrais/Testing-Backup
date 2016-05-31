﻿using System;
using System.Collections.Generic;
using System.Web;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.Utils;
using NLog;
using Microsoft.SharePoint.Client;
using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Model.Common;

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

        const string SP_PROMAS_LIST_NAME = "Professional Master";

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
            updatedValue.Add("religion", viewModel.Religion.Value);
            updatedValue.Add("gender", viewModel.Gender.Value);
            updatedValue.Add("idcardtype", viewModel.IDCardType.Value);
            updatedValue.Add("idcardexpirydate", viewModel.IDCardExpiry);
            updatedValue.Add("nationality", new FieldLookupValue { LookupId = (int)viewModel.Nationality.Value });
            updatedValue.Add("applicationstatus", Workflow.ApplicationStatus.NEW.ToString());

            try
            {
                SPConnector.AddListItem(SP_APPDATA_LIST_NAME, updatedValue, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                throw e;
            }

            return SPConnector.GetInsertedItemID(SP_APPDATA_LIST_NAME, _siteUrl);
        }

        public void CreateEducationDetails(int? headerID, IEnumerable<EducationDetailVM> viewModels)
        {
            foreach (var viewModel in viewModels)
            {
                var updatedValue = new Dictionary<string, object>();
                updatedValue.Add("Title", viewModel.Subject);
                updatedValue.Add("university", viewModel.University);
                updatedValue.Add("yearofgraduation", FormatUtil.ConvertToYearString(viewModel.YearOfGraduation));
                updatedValue.Add("applications", new FieldLookupValue { LookupId = Convert.ToInt32(headerID) });
                updatedValue.Add("remarks", viewModel.Remarks);

                try
                {
                    SPConnector.AddListItem(SP_APPEDU_LIST_NAME, updatedValue, _siteUrl);
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw e;
                }
            }
        }

        public void CreateApplicationDocument(int? headerID, IEnumerable<HttpPostedFileBase> documents)
        {
            foreach (var doc in documents)
            {
                var updateValue = new Dictionary<string, object>();
                updateValue.Add("application", new FieldLookupValue { LookupId = Convert.ToInt32(headerID) });
                try
                {
                    SPConnector.UploadDocument(SP_APPDOC_LIST_NAME, updateValue, doc.FileName, doc.InputStream, _siteUrl);
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw e;
                }
            }
        }

        public void CreateTrainingDetails(int? headerID, IEnumerable<TrainingDetailVM> trainingDetails)
        {
            foreach (var viewModel in trainingDetails)
            {
                var updatedValue = new Dictionary<string, object>();
                updatedValue.Add("Title", viewModel.Subject);
                updatedValue.Add("traininginstitution", viewModel.Institution);
                updatedValue.Add("trainingremarks", viewModel.Remarks);
                updatedValue.Add("trainingyear", FormatUtil.ConvertToYearString(viewModel.Year));
                updatedValue.Add("application", new FieldLookupValue { LookupId = Convert.ToInt32(headerID) });

                try
                {
                    SPConnector.AddListItem(SP_APPTRAIN_LIST_NAME, updatedValue, _siteUrl);
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw e;
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
                updatedValue.Add("applicationfrom", viewModel.From);
                updatedValue.Add("applicationto", viewModel.To);
                updatedValue.Add("application", new FieldLookupValue { LookupId = Convert.ToInt32(headerID) });
                updatedValue.Add("applicationjobdescription", viewModel.JobDescription);

                try
                {
                    SPConnector.AddListItem(SP_APPWORK_LIST_NAME, updatedValue, _siteUrl);
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw e;
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
            viewModel.PlaceOfBirth = Convert.ToString(listItem["placeofbirth"]);
            viewModel.DateOfBirth = Convert.ToDateTime(listItem["dateofbirth"]);
            viewModel.PermanentAddress =
                FormatUtil.ConvertMultipleLine(Convert.ToString(listItem["permanentaddress"]));
            viewModel.CurrentAddress =
                FormatUtil.ConvertMultipleLine(Convert.ToString(listItem["currentaddress"]));
            viewModel.IDCardNumber = Convert.ToString(listItem["idcardnumber"]);
            viewModel.IDCardExpiry = Convert.ToDateTime(listItem["idcardexpirydate"]);
            viewModel.Telephone = Convert.ToString(listItem["permanentlandlinephone"]);
            viewModel.CurrentTelephone = Convert.ToString(listItem["currentlandlinephone"]);
            viewModel.EmailAddresOne = Convert.ToString(listItem["personalemail"]);
            viewModel.EmailAddresTwo = Convert.ToString(listItem["personalemail2"]);
            viewModel.MobileNumberOne = Convert.ToString(listItem["mobilephonenr"]);
            viewModel.MobileNumberTwo = Convert.ToString(listItem["mobilephonenr2"]);
            viewModel.SpecializationField =
                FormatUtil.ConvertMultipleLine(Convert.ToString(listItem["specializationfield"]));
            viewModel.YearRelevanWork = Convert.ToInt32(listItem["totalrelevantexperienceyears"]);
            viewModel.MonthRelevantWork = Convert.ToInt32(listItem["totalrelevantexperiencemonths"]);
            viewModel.MaritalStatus.Value = Convert.ToString(listItem["maritalstatus"]);
            viewModel.BloodType.Value = Convert.ToString(listItem["bloodtype"]);
            viewModel.Religion.Value = Convert.ToString(listItem["religion"]);
            viewModel.Gender.Value = Convert.ToString(listItem["gender"]);
            viewModel.IDCardType.Value = Convert.ToString(listItem["idcardtype"]);
            viewModel.IDCardExpiry = Convert.ToDateTime(listItem["idcardexpirydate"]);
            viewModel.Nationality.Value = FormatUtil.ConvertLookupToID(listItem, "nationality");
            viewModel.ApplicationStatus = Convert.ToString(listItem["applicationstatus"]);


            // Convert Details
            viewModel.EducationDetails = GetEducationDetails(viewModel.ID);
            viewModel.TrainingDetails = GetTrainingDetails(viewModel.ID);
            viewModel.WorkingExperienceDetails = GetWorkingExperienceDetails(viewModel.ID);
            viewModel.DocumentUrl = GetDocumentUrl(viewModel.ID);

            return viewModel;

        }

        private string GetDocumentUrl(int? iD)
        {
            return string.Format(UrlResource.ApplicationDocumentByID, _siteUrl, iD);
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
             <ViewFields>
            <FieldRef Name='ID' />
            <FieldRef Name='application' />
            <FieldRef Name='Title' />
            <FieldRef Name='applicationcompany' />
            <FieldRef Name='applicationfrom' />
            <FieldRef Name='applicationto' />
            <FieldRef Name='applicationjobdescription' /></ViewFields> 
            </View>";

            var workingExperienceDetails = new List<WorkingExperienceDetailVM>();
            foreach(var item in SPConnector.GetList(SP_APPWORK_LIST_NAME, _siteUrl, caml))
            {
                workingExperienceDetails.Add(ConvertToWorkingExperienceDetailVM(item));
            }

            return workingExperienceDetails;
        }

        /// <summary>
        //<ViewFields>
        //  <FieldRef Name = 'applicationjobdescription' />
        //  < FieldRef Name='applicationfrom' />
        //  <FieldRef Name = 'applicationcompany' />
        //  < FieldRef Name='applicationto' />
        //</ViewFields>
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
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
             <ViewFields>
                <FieldRef Name='ID' />
                <FieldRef Name='Title' />
                <FieldRef Name='traininginstitution' />
                <FieldRef Name='trainingyear' />
                <FieldRef Name='trainingremarks' />
                <FieldRef Name='application' />
            </ViewFields> 
            </View>";

            var trainingDetails = new List<TrainingDetailVM>();
            foreach (var item in SPConnector.GetList(SP_APPTRAIN_LIST_NAME, _siteUrl, caml))
            {
                trainingDetails.Add(ConvertToTrainingDetailVM(item));
            }

            return trainingDetails;
        }


        /// <summary>
       //<ViewFields>
       //   <FieldRef Name = 'Title' />
       //   < FieldRef Name='trainingyear' />
       //   <FieldRef Name = 'traininginstitution' />
       //   < FieldRef Name='trainingremarks' />
       //</ViewFields>
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private TrainingDetailVM ConvertToTrainingDetailVM(ListItem item)
        {
            return new TrainingDetailVM
            {
                ID = Convert.ToInt32(item["ID"]),
                Subject = Convert.ToString(item["Title"]),
                Institution = Convert.ToString(item["traininginstitution"]),
                Remarks = Convert.ToString(item["trainingremarks"]),
                Year = Convert.ToDateTime(item["trainingyear"])
            };
        }

       //<ViewFields>
       //   <FieldRef Name = 'Title' />
       //   < FieldRef Name='applications' />
       //   <FieldRef Name = 'university' />
       //   < FieldRef Name='yearofgraduation' />
       //   <FieldRef Name = 'remarks' />
       //</ ViewFields >
        private IEnumerable<EducationDetailVM> GetEducationDetails(int? iD)
        {
            var caml = @"<View>  
            <Query> 
               <Where><Eq><FieldRef Name='applications' LookupId='True' /><Value Type='Lookup'>" + iD + @"</Value></Eq></Where> 
            </Query> 
             <ViewFields>
                <FieldRef Name='applications' />
                <FieldRef Name='university' />
                <FieldRef Name='yearofgraduation' />
                <FieldRef Name='remarks' />
                <FieldRef Name='Title' />
                <FieldRef Name='ID' />
             </ViewFields> 
            </View>";

            var eduacationDetails = new List<EducationDetailVM>();
            foreach (var item in SPConnector.GetList(SP_APPEDU_LIST_NAME, _siteUrl, caml))
            {
                eduacationDetails.Add(ConvertToEducationDetailVM(item));
            }

            return eduacationDetails;
        }

        /// <summary>
           // <ViewFields>
           //   <FieldRef Name = 'Title' />
           //   < FieldRef Name='university' />
           //   <FieldRef Name = 'yearofgraduation' />
           //   < FieldRef Name='remarks' />
           //   <FieldRef Name = 'applications' />
           //</ ViewFields >
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private EducationDetailVM ConvertToEducationDetailVM(ListItem item)
        {
            return new EducationDetailVM
            {
                ID = Convert.ToInt32(item["ID"]),
                Subject = Convert.ToString(item["Title"]),
                University = Convert.ToString(item["university"]),
                YearOfGraduation = Convert.ToDateTime(item["yearofgraduation"]),
                Remarks = Convert.ToString(item["remarks"])
            };
        }

        public void SetSiteUrl(string siteUrl = null)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
        }

        public void SetApplicationStatus(ApplicationDataVM viewModel)
        {
            var updatedValue = new Dictionary<string, object>();
            updatedValue.Add("applicationstatus", viewModel.WorkflowStatusOptions.Value);

            try
            {
                SPConnector.UpdateListItem(SP_APPDATA_LIST_NAME, viewModel.ID, updatedValue, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e);
                throw e;
            }
        }
    }
}
