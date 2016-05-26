﻿using System;
using System.Collections.Generic;
using System.Web;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.Utils;
using NLog;
using Microsoft.SharePoint.Client;
using MCAWebAndAPI.Service.Resources;

namespace MCAWebAndAPI.Service.HR.Recruitment
{
    public class HRManpowerRequisitionService : IHRManpowerRequisitionService
    {
        string _siteUrl;
        static Logger logger = LogManager.GetCurrentClassLogger();

        const string SP_MANPOW_LIST_NAME = "Manpower%20Requisition";
        const string SP_WORKRE_LIST_NAME = "Manpower%20Requisition%20Working%20Relationship";
        const string SP_MANDOC_LIST_NAME = "Manpower%20Requisition%20Documents";

        public int CreateManpowerRequisition(ManpowerRequisitionVM viewModel)
        {
            var updatedValue = new Dictionary<string, object>();

            //updatedValue.Add("Title", viewModel.FirstMiddleName);
            //updatedValue.Add("lastname", viewModel.LastName);
            //updatedValue.Add("placeofbirth", viewModel.PlaceOfBirth);
            //updatedValue.Add("dateofbirth", viewModel.DateOfBirth);
            //updatedValue.Add("idcardnumber", viewModel.IDCardNumber);
            //updatedValue.Add("permanentaddress", viewModel.PermanentAddress);
            //updatedValue.Add("permanentlandlinephone", FormatUtil.ConvertToCleanPhoneNumber(viewModel.Telephone));
            //updatedValue.Add("currentaddress", viewModel.CurrentAddress);
            //updatedValue.Add("currentlandlinephone", FormatUtil.ConvertToCleanPhoneNumber(viewModel.CurrentTelephone));
            //updatedValue.Add("personalemail", viewModel.EmailAddresOne);
            //updatedValue.Add("personalemail2", viewModel.EmailAddresTwo);
            //updatedValue.Add("mobilephonenr", FormatUtil.ConvertToCleanPhoneNumber(viewModel.MobileNumberOne));
            //updatedValue.Add("mobilephonenr2", FormatUtil.ConvertToCleanPhoneNumber(viewModel.MobileNumberTwo));
            //updatedValue.Add("specializationfield", viewModel.SpecializationField);
            //updatedValue.Add("totalrelevantexperienceyears", viewModel.YearRelevanWork);
            //updatedValue.Add("totalrelevantexperiencemonths", viewModel.MonthRelevantWork);
            //updatedValue.Add("maritalstatus", viewModel.MaritalStatus.Value);
            //updatedValue.Add("bloodtype", viewModel.BloodType.Value);
            //updatedValue.Add("religion", viewModel.Religion.Value);
            //updatedValue.Add("gender", viewModel.Gender.Value);
            //updatedValue.Add("idcardtype", viewModel.IDCardType.Value);
            //updatedValue.Add("idcardexpirydate", viewModel.IDCardExpiry);
            //updatedValue.Add("nationality", new FieldLookupValue { LookupId = viewModel.Nationality.Value });
            //updatedValue.Add("applicationstatus", WorkflowUtil.DRAFT);

            try
            {
                SPConnector.AddListItem(SP_MANPOW_LIST_NAME, updatedValue, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                throw new Exception(ErrorResource.SPInsertError);
            }

            return SPConnector.GetInsertedItemID(SP_MANPOW_LIST_NAME, _siteUrl);
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
                    SPConnector.AddListItem(SP_WORKRE_LIST_NAME, updatedValue, _siteUrl);
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw new Exception(ErrorResource.SPInsertError);
                }
            }
        }

        public void CreateManpowerRequisitionDocuments(int? headerID, IEnumerable<HttpPostedFileBase> documents)
        {
            foreach (var doc in documents)
            {
                try
                {
                    SPConnector.UploadDocument(SP_MANDOC_LIST_NAME, doc.FileName, doc.InputStream, _siteUrl);
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw new Exception(ErrorResource.SPInsertError);
                }
            }
        }
       
        public void CreateWorkingRelationshipDetails(int? headerID, IEnumerable<WorkingRelationshipDetailVM> workingRelationshipDetails)
        {
            foreach (var viewModel in workingRelationshipDetails)
            {
                var updatedValue = new Dictionary<string, object>();
                //updatedValue.Add("Title", viewModel.Position);
                //updatedValue.Add("applicationcompany", viewModel.Company);
                //updatedValue.Add("applicationfrom", FormatUtil.ConvertToYearString(viewModel.From));
                //updatedValue.Add("applicationto", FormatUtil.ConvertToYearString(viewModel.To));
                //updatedValue.Add("application", new FieldLookupValue { LookupId = Convert.ToInt32(headerID) });
                //updatedValue.Add("applicationjobdescription", viewModel.JobDescription);

                try
                {
                    SPConnector.AddListItem(SP_WORKRE_LIST_NAME, updatedValue, _siteUrl);
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw new Exception(ErrorResource.SPInsertError);
                }
            }
        }

        public ManpowerRequisitionVM GetManpowerRequisition(int? ID)
        {
            var viewModel = new ManpowerRequisitionVM();
            if (ID == null)
                return viewModel;

            var listItem = SPConnector.GetListItem(SP_WORKRE_LIST_NAME, ID, _siteUrl);
            viewModel = ConvertToManpowerRequisitionVM(listItem);

            return viewModel;

        }

        private ManpowerRequisitionVM ConvertToManpowerRequisitionVM(ListItem listItem)
        {
            var viewModel = new ManpowerRequisitionVM();
            viewModel.ID = Convert.ToInt32(listItem["ID"]);
            //viewModel.FirstMiddleName = Convert.ToString(listItem["Title"]);
            //viewModel.LastName = Convert.ToString(listItem["lastname"]);
            //viewModel.PlaceOfBirth = Convert.ToString(listItem["placeofbirth"]);
            //viewModel.DateOfBirth = Convert.ToDateTime(listItem["dateofbirth"]);
            //viewModel.PermanentAddress =
            //    FormatUtil.ConvertMultipleLine(Convert.ToString(listItem["permanentaddress"]));
            //viewModel.CurrentAddress =
            //    FormatUtil.ConvertMultipleLine(Convert.ToString(listItem["currentaddress"]));
            //viewModel.IDCardNumber = Convert.ToString(listItem["idcardnumber"]);
            //viewModel.IDCardExpiry = Convert.ToDateTime(listItem["idcardexpirydate"]);
            //viewModel.Telephone = Convert.ToString(listItem["permanentlandlinephone"]);
            //viewModel.CurrentTelephone = Convert.ToString(listItem["currentlandlinephone"]);
            //viewModel.EmailAddresOne = Convert.ToString(listItem["personalemail"]);
            //viewModel.EmailAddresTwo = Convert.ToString(listItem["personalemail2"]);
            //viewModel.MobileNumberOne = Convert.ToString(listItem["mobilephonenr"]);
            //viewModel.MobileNumberTwo = Convert.ToString(listItem["mobilephonenr2"]);
            //viewModel.SpecializationField =
            //    FormatUtil.ConvertMultipleLine(Convert.ToString(listItem["specializationfield"]));
            //viewModel.YearRelevanWork = Convert.ToInt32(listItem["totalrelevantexperienceyears"]);
            //viewModel.MonthRelevantWork = Convert.ToInt32(listItem["totalrelevantexperiencemonths"]);
            //viewModel.MaritalStatus.DefaultValue = Convert.ToString(listItem["maritalstatus"]);
            //viewModel.BloodType.DefaultValue = Convert.ToString(listItem["bloodtype"]);
            //viewModel.Religion.DefaultValue = Convert.ToString(listItem["religion"]);
            //viewModel.Gender.DefaultValue = Convert.ToString(listItem["gender"]);
            //viewModel.IDCardType.DefaultValue = Convert.ToString(listItem["idcardtype"]);
            //viewModel.IDCardExpiry = Convert.ToDateTime(listItem["idcardexpirydate"]);
            //viewModel.Nationality.DefaultValue = FormatUtil.ConvertLookupToID(listItem, "nationality") + string.Empty;

            //// Convert Details
            //viewModel.EducationDetails = GetEducationDetails(viewModel.ID);
            //viewModel.TrainingDetails = GetTrainingDetails(viewModel.ID);
            //viewModel.WorkingExperienceDetails = GetWorkingExperienceDetails(viewModel.ID);
            //viewModel.DocumentUrl = GetDocumentUrl(viewModel.ID);

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
            foreach(var item in SPConnector.GetList(SP_WORKRE_LIST_NAME, _siteUrl, caml))
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


        public void SetSiteUrl(string siteUrl = null)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
        }
    }
}
