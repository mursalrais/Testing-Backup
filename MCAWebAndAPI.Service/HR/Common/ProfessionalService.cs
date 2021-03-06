﻿using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Service.HR.Common
{
    public partial class ProfessionalService : IProfessionalService
    {
        string _siteUrl;
        const string SP_PROMAS_LIST_NAME = "Professional Master";
        const string SP_POSMAS_LIST_NAME = "Position Master";
        const string SP_MONFEE_LIST_NAME = "Monthly Fee";
        const string SP_PROEDU_LIST_NAME = "Professional Education";
        const string SP_PROTRAIN_LIST_NAME = "Professional Training";
        const string SP_PROORG_LIST_NAME = "Professional Organization Detail";
        const string SP_PRODEP_LIST_NAME = "Dependent";

        static Logger logger = LogManager.GetCurrentClassLogger();

        public ProfessionalService()
        {

        }

        public ProfessionalService(string siteUrl)
        {
            SetSiteUrl(siteUrl);
        }

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ProfessionalDataVM GetProfessionalData(int? ID)
        {
            if (ID == null)
                return new ProfessionalDataVM();

            var listItem = SPConnector.GetListItem(SP_PROMAS_LIST_NAME, ID, _siteUrl);
            var viewModel = ConvertToProfessionalModel(listItem);
            viewModel = GetProfessionalDetails(viewModel);
            return viewModel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public async Task<ProfessionalDataVM> GetProfessionalDataAsync(int? ID)
        {
            if (ID == null)
                return new ProfessionalDataVM();

            var listItem = SPConnector.GetListItem(SP_PROMAS_LIST_NAME, ID, _siteUrl);
            var viewModel = ConvertToProfessionalModel(listItem);
            viewModel = await GetProfessionalDetailsAsync(viewModel);
            return viewModel;
        }

        private async Task<ProfessionalDataVM> GetProfessionalDetailsAsync(ProfessionalDataVM viewModel)
        {
            Task<IEnumerable<OrganizationalDetailVM>> getOrganizationDetailsTask = GetOrganizationalDetailsAsync(viewModel.ID);
            Task<IEnumerable<EducationDetailVM>> getEducationDetailsTask = GetEducationDetailsAsync(viewModel.ID);
            Task<IEnumerable<TrainingDetailVM>> getTrainingDetailsTask = GetTrainingDetailsAsync(viewModel.ID);
            Task<IEnumerable<DependentDetailVM>> getDependantDetailsTask = GetDependentDetailsAsync(viewModel.ID);

            try
            {
                viewModel.OrganizationalDetails = await getOrganizationDetailsTask;
                viewModel.EducationDetails = await getEducationDetailsTask;
                viewModel.TrainingDetails = await getTrainingDetailsTask;
                viewModel.DependentDetails = await getDependantDetailsTask;
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                throw e;
            }
            return viewModel;
        }

        private async Task<IEnumerable<DependentDetailVM>> GetDependentDetailsAsync(int? iD)
        {
            return GetDependentDetails(iD);
        }

        private async Task<IEnumerable<TrainingDetailVM>> GetTrainingDetailsAsync(int? iD)
        {
            return GetTrainingDetails(iD);
        }

        private async Task<IEnumerable<EducationDetailVM>> GetEducationDetailsAsync(int? iD)
        {
            return GetEducationDetails(iD);
        }

        private async Task<IEnumerable<OrganizationalDetailVM>> GetOrganizationalDetailsAsync(int? iD)
        {
            return GetOrganizationalDetails(iD);
        }

        private ProfessionalDataVM GetProfessionalDetails(ProfessionalDataVM viewModel)
        {
            viewModel.OrganizationalDetails = GetOrganizationalDetails(viewModel.ID);
            viewModel.EducationDetails = GetEducationDetails(viewModel.ID);
            viewModel.TrainingDetails = GetTrainingDetails(viewModel.ID);
            viewModel.DependentDetails = GetDependentDetails(viewModel.ID);

            return viewModel;
        }

        private ProfessionalDataVM ConvertToProfessionalModel(ListItem listItem)
        {
            var viewModel = new ProfessionalDataVM();
            DateTime LastWorkingDate = Convert.ToDateTime(listItem["lastworkingdate"]);
            DateTime CurrentDate = DateTime.UtcNow;
            viewModel.ID = Convert.ToInt32(listItem["ID"]);
            viewModel.FirstMiddleName = Convert.ToString(listItem["Title"]);
            viewModel.CurrentPosition.Value = FormatUtil.ConvertLookupToID(listItem, "Position");
            viewModel.DivisionProjectUnit.Value = Convert.ToString(listItem["Project_x002f_Unit"]);
            viewModel.JoinDate = Convert.ToDateTime(listItem["Join_x0020_Date"]).ToLocalTime();
            if (viewModel.JoinDate.Value.Year == 1)
            {
                viewModel.JoinDate = null;
            }
            //if ((LastWorkingDate.Year ==1)||(CurrentDate <= LastWorkingDate))
            //{
            //    viewModel.ProfessionalStatus.Value = "Active";
            //}
            //else
            //{
            //    viewModel.ProfessionalStatus.Value = "Inactive";
            //}
            //viewModel.ProfessionalStatus.Value = Convert.ToString(listItem["Professional_x0020_Status"]);
            viewModel.PlaceOfBirth = Convert.ToString(listItem["placeofbirth"]);
            viewModel.DateOfBirth = Convert.ToDateTime(listItem["dateofbirth"]);
            viewModel.PermanentAddress =
                FormatUtil.ConvertMultipleLine(Convert.ToString(listItem["permanentaddress"]));
            viewModel.CurrentAddress =
                FormatUtil.ConvertMultipleLine(Convert.ToString(listItem["currentaddress"]));
            viewModel.IDCardNumber = Convert.ToString(listItem["idcardnumber"]);
            viewModel.IDCardExpiry = Convert.ToDateTime(listItem["idcardexpirydate"]).ToLocalTime();
            if (viewModel.IDCardExpiry.Value.Year == 1)
            {
                viewModel.IDCardExpiry = null;
            }
            viewModel.Telephone = Convert.ToString(listItem["permanentlandlinephone"]);
            viewModel.CurrentTelephone = Convert.ToString(listItem["currentlandlinephone"]);
            viewModel.EmailAddresOne = Convert.ToString(listItem["personalemail"]);
            viewModel.MobileNumberOne = Convert.ToString(listItem["mobilephonenr"]);
            viewModel.MaritalStatus.Value = Convert.ToString(listItem["maritalstatus"]);
            viewModel.BloodType.Value = Convert.ToString(listItem["bloodtype"]);
            viewModel.Religion.Value = Convert.ToString(listItem["religion"]);
            viewModel.Gender.Value = Convert.ToString(listItem["gender"]);
            viewModel.IDCardType.Text = Convert.ToString(listItem["idcardtype"]);
            viewModel.Nationality.Value = FormatUtil.ConvertLookupToID(listItem, "nationality");

            // Fields not from Application Data
            viewModel.EmergencyNumber = Convert.ToString(listItem["emergencynumber"]);
            viewModel.OfficePhone = Convert.ToString(listItem["officephone"]);
            viewModel.OfficeEmail = Convert.ToString(listItem["officeemail"]);
            viewModel.Extension = Convert.ToString(listItem["Extension"]);

            viewModel.AccountNameForHI = Convert.ToString(listItem["hiaccountname"]);
            viewModel.BankNameForHI = Convert.ToString(listItem["hibankname"]);
            viewModel.EffectiveDateForHI = Convert.ToDateTime(listItem["hieffectivedate"]).ToLocalTime();
            if (viewModel.EffectiveDateForHI.Value.Year == 1)
            {
                viewModel.EffectiveDateForHI = null;
            }
            viewModel.AccountNumberForHI = Convert.ToString(listItem["hiaccountnr"]);
            viewModel.BranchOfficeForHI = Convert.ToString(listItem["hibankbranchoffice"]);
            viewModel.EndDateForHI = Convert.ToDateTime(listItem["hienddate"]).ToLocalTime();
            if (viewModel.EndDateForHI.Value.Year == 1)
            {
                viewModel.EndDateForHI = null;
            }
            viewModel.CurrencyForHI.Value = Convert.ToString(listItem["hicurrency"]);

            viewModel.VendorAccountNumberRIForHI = Convert.ToString(listItem["hiriaccountnr"]);
            viewModel.VendorAccountNumberRJForHI = Convert.ToString(listItem["hirjaccountnr"]);
            viewModel.VendorAccountNumberRGForHI = Convert.ToString(listItem["hirgaccountnr"]);
            viewModel.VendorAccountNumberMAForHI = Convert.ToString(listItem["himaaccountnr"]);

            viewModel.AccountNameForSP = Convert.ToString(listItem["spaccountname"]);
            viewModel.BankNameForSP = Convert.ToString(listItem["spbankname"]);
            viewModel.EffectiveDateForSP = Convert.ToDateTime(listItem["speffectivedate"]).ToLocalTime();
            if (viewModel.EffectiveDateForSP.Value.Year == 1)
            {
                viewModel.EffectiveDateForSP = null;
            }
            viewModel.AccountNumberForSP = Convert.ToString(listItem["spaccountnr"]);
            viewModel.BranchOfficeForSP = Convert.ToString(listItem["spbranchoffice"]);
            viewModel.EndDateForSP = Convert.ToDateTime(listItem["spenddate"]).ToLocalTime();
            if (viewModel.EndDateForSP.Value.Year == 1)
            {
                viewModel.EndDateForSP = null;
            }
            viewModel.CurrencyForSP.Value = Convert.ToString(listItem["spcurrency"]);

            viewModel.AccountNameForPayroll = Convert.ToString(listItem["payrollaccountname"]);
            viewModel.BankNameForPayroll = Convert.ToString(listItem["payrollbankname"]);
            viewModel.AccountNumberForPayroll = Convert.ToString(listItem["payrollaccountnr"]);
            viewModel.BranchOfficeForPayroll = Convert.ToString(listItem["payrollbranchoffice"]);
            viewModel.CurrencyForPayroll.Value = Convert.ToString(listItem["payrollcurrency"]);
            viewModel.BankSwiftCodeForPayroll = Convert.ToString(listItem["payrollbankswiftcode"]);
            viewModel.TaxStatusForPayroll.Value = Convert.ToString(listItem["payrolltaxstatus"]);

            //added by yaya
            viewModel.ValidationStatus = Convert.ToString(listItem["datavalidationstatus"]);
            //added by yaya

            // Convert Details


            return viewModel;
        }

        private IEnumerable<OrganizationalDetailVM> GetOrganizationalDetails(int? ID)
        {
            var caml = @"<View>  
            <Query> 
               <Where><Eq><FieldRef Name='professional' LookupId='True' /><Value Type='Lookup'>" + ID + @"</Value></Eq></Where> 
            </Query> 
             <ViewFields>
                <FieldRef Name='Title' />
                <FieldRef Name='projectunit' />
                <FieldRef Name='Status' />
                <FieldRef Name='Position' />
                <FieldRef Name='Level' />
                <FieldRef Name='psanr' />
                <FieldRef Name='startdate' />
                <FieldRef Name='lastworkingday' />
                <FieldRef Name='PSA_x0020_Number_x003a_PSA_x0020' />
                <FieldRef Name='ID' />
             </ViewFields> 
            </View>";

            var organizationalDetails = new List<OrganizationalDetailVM>();
            foreach (var item in SPConnector.GetList(SP_PROORG_LIST_NAME, _siteUrl, caml))
            {
                organizationalDetails.Add(ConvertToOrganizationalDetailVM(item));
            }
            return organizationalDetails;
        }

        OrganizationalDetailVM ConvertToOrganizationalDetailVM(ListItem item)
        {
            var viewModel = new OrganizationalDetailVM();
            viewModel.ID = Convert.ToInt32(item["ID"]);
            viewModel.LastWorkingDay = Convert.ToDateTime(item["lastworkingday"]).ToLocalTime();
            viewModel.Level = Convert.ToString(item["Level"]);

            var ListPosition = SPConnector.GetListItem(SP_POSMAS_LIST_NAME, (item["Position"] as FieldLookupValue).LookupId, _siteUrl);
            viewModel.Position.Value = (item["Position"] as FieldLookupValue).LookupId;
            viewModel.Position.Text = Convert.ToString(ListPosition["projectunit"]) + " - " + Convert.ToString(ListPosition["Title"]);
            viewModel.PSANumber.Value = (item["psanr"] as FieldLookupValue).LookupId;
            viewModel.PSANumber.Text = (item["psanr"] as FieldLookupValue).LookupValue;
            viewModel.PSAStatus = (item["PSA_x0020_Number_x003a_PSA_x0020"] as FieldLookupValue).LookupValue;
            viewModel.StartDate = Convert.ToDateTime(item["startdate"]).ToLocalTime();
            //viewModel.Project = OrganizationalDetailVM.GetProjectDefaultValue(
              //      new Model.ViewModel.Control.InGridComboBoxVM { Text = Convert.ToString(item["projectunit"]) }
                //);
            return viewModel;

            //return new OrganizationalDetailVM
            //{
            //    ID = Convert.ToInt32(item["ID"]),
            //    LastWorkingDay = Convert.ToDateTime(item["lastworkingday"]).ToLocalTime(),
            //    Level = Convert.ToString(item["Level"]),
            //    Position = FormatUtil.ConvertToInGridAjaxComboBox(item, "Position"),
            //    PSANumber = FormatUtil.ConvertToInGridAjaxComboBox(item, "psanr"),
            //    StartDate = Convert.ToDateTime(item["startdate"]).ToLocalTime(),
            //    Project = OrganizationalDetailVM.GetProjectDefaultValue(
            //        FormatUtil.ConvertToInGridComboBox(item, "projectunit")),
            //    PSAStatus = OrganizationalDetailVM.GetPSAStatusDefaultValue(
            //        FormatUtil.ConvertToInGridComboBox(item, "Status"))
            //};
        }

        private IEnumerable<DependentDetailVM> GetDependentDetails(int? ID)
        {
            var caml = @"<View>  
            <Query> 
               <Where><Eq><FieldRef Name='professional' LookupId='True' /><Value Type='Lookup'>" + ID + @"</Value></Eq></Where> 
            </Query> 
             <ViewFields>
                <FieldRef Name='Title' />
                <FieldRef Name='relationship' />
                <FieldRef Name='placeofbirth' />
                <FieldRef Name='dateofbirth' />
                <FieldRef Name='insurancenr' />
                <FieldRef Name='remarks' />
                <FieldRef Name='ID' />
             </ViewFields> 
            </View>";
            var dependentDetail = new List<DependentDetailVM>();

            foreach (var item in SPConnector.GetList(SP_PRODEP_LIST_NAME, _siteUrl, caml))
            {
                dependentDetail.Add(ConvertToDependentDetailVM(item));
            }
            var tes = dependentDetail;
            return dependentDetail;
        }

        private DependentDetailVM ConvertToDependentDetailVM(ListItem item)
        {
            return new DependentDetailVM
            {
                ID = Convert.ToInt32(item["ID"]),
                FullName = Convert.ToString(item["Title"]),
                DateOfBirthGrid = Convert.ToDateTime(item["dateofbirth"]).ToLocalTime(),
                InsuranceNumber = Convert.ToString(item["insurancenr"]),
                PlaceOfBirth = Convert.ToString(item["placeofbirth"]),
                Remark = FormatUtil.ConvertMultipleLine(Convert.ToString(item["remarks"])),
                Relationship = DependentDetailVM.GetRelationshipDefaultValue(
                    new Model.ViewModel.Control.InGridComboBoxVM { Text = Convert.ToString(item["relationship"]) }
                )
            };
        }

        private IEnumerable<EducationDetailVM> GetEducationDetails(int? iD)
        {
            var caml = @"<View>  
            <Query>
               <Where>
                  <Eq>
                     <FieldRef Name='professional_x003a_ID' />
                     <Value Type='Lookup'>"+iD+@"</Value>
                  </Eq>
               </Where>
            </Query> 
             <ViewFields>
                <FieldRef Name='professional' />
                <FieldRef Name='university' />
                <FieldRef Name='yearofgraduation' />
                <FieldRef Name='remarks' />
                <FieldRef Name='Title' />
                <FieldRef Name='ID' />
             </ViewFields> 
            </View>";

            var eduacationDetails = new List<EducationDetailVM>();
            foreach (var item in SPConnector.GetList(SP_PROEDU_LIST_NAME, _siteUrl, caml))
            {
                eduacationDetails.Add(ConvertToEducationDetailVM(item));
            }
            return eduacationDetails;
        }

        private EducationDetailVM ConvertToEducationDetailVM(ListItem item)
        {
            var tes = new EducationDetailVM();
            tes.ID = Convert.ToInt32(item["ID"]);
            tes.Subject = Convert.ToString(item["Title"]);
            tes.University = Convert.ToString(item["university"]);
            tes.YearOfGraduation = FormatUtil.ConvertDateStringToDateTimeProfessional(item, "yearofgraduation");
            tes.Remarks = Convert.ToString(item["remarks"]);
            return tes;
        }


        private IEnumerable<TrainingDetailVM> GetTrainingDetails(int? iD)
        {
            var caml = @"<View>  
            <Query> 
               <Where><Eq><FieldRef Name='professional' LookupId='True' /><Value Type='Lookup'>" + iD
               + @"</Value></Eq></Where> 
            </Query> 
             <ViewFields>
                <FieldRef Name='ID' />
                <FieldRef Name='Title' />
                <FieldRef Name='traininginstitution' />
                <FieldRef Name='trainingyear' />
                <FieldRef Name='trainingremarks' />
                <FieldRef Name='professional' />
            </ViewFields> 
            </View>";

            var trainingDetails = new List<TrainingDetailVM>();
            foreach (var item in SPConnector.GetList(SP_PROTRAIN_LIST_NAME, _siteUrl, caml))
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
                Institution = Convert.ToString(item["traininginstitution"]),
                Remarks = Convert.ToString(item["trainingremarks"]),
                Year = FormatUtil.ConvertDateStringToDateTimeProfessional(item, "trainingyear")
            };
        }

        public int? EditProfessionalData(ProfessionalDataVM viewModel)
        {
            var updatedValue = new Dictionary<string, object>();

            updatedValue.Add("Title", viewModel.FirstMiddleName);
            updatedValue.Add("Position", new FieldLookupValue
            { LookupId = (int)viewModel.CurrentPosition.Value });
            updatedValue.Add("Project_x002f_Unit",viewModel.DivisionProjectUnit.Value);
            //updatedValue.Add("Professional_x0020_Status", viewModel.ProfessionalStatus.Value);
            updatedValue.Add("Join_x0020_Date", viewModel.JoinDate);
            updatedValue.Add("placeofbirth", viewModel.PlaceOfBirth);
            updatedValue.Add("dateofbirth", viewModel.DateOfBirth);
            updatedValue.Add("idcardnumber", viewModel.IDCardNumber);
            updatedValue.Add("permanentaddress", viewModel.PermanentAddress);
            updatedValue.Add("permanentlandlinephone", FormatUtil.ConvertToCleanPhoneNumber(viewModel.Telephone));
            updatedValue.Add("currentaddress", viewModel.CurrentAddress);
            updatedValue.Add("currentlandlinephone", FormatUtil.ConvertToCleanPhoneNumber(viewModel.CurrentTelephone));
            updatedValue.Add("personalemail", viewModel.EmailAddresOne);
            updatedValue.Add("mobilephonenr", FormatUtil.ConvertToCleanPhoneNumber(viewModel.MobileNumberOne));
            updatedValue.Add("maritalstatus", viewModel.MaritalStatus.Value);
            updatedValue.Add("bloodtype", viewModel.BloodType.Value);
            updatedValue.Add("religion", viewModel.Religion.Value);
            updatedValue.Add("gender", viewModel.Gender.Value);
            updatedValue.Add("idcardtype", viewModel.IDCardType.Value);
            updatedValue.Add("idcardexpirydate", viewModel.IDCardExpiry);

            if (viewModel.Nationality.Value.Value != 0)
            {
                updatedValue.Add("nationality", new FieldLookupValue { LookupId = (int)viewModel.Nationality.Value });
            }
            else
            {
                updatedValue.Add("nationality", null);
            }


            //// Fields not from Application Data
            updatedValue.Add("emergencynumber", viewModel.EmergencyNumber);
            updatedValue.Add("officephone", viewModel.OfficePhone);
            updatedValue.Add("officeemail", viewModel.OfficeEmail);
            updatedValue.Add("Extension", viewModel.Extension);

            updatedValue.Add("hiaccountname", viewModel.AccountNameForHI);
            updatedValue.Add("hibankname", viewModel.BankNameForHI);
            updatedValue.Add("hieffectivedate", viewModel.EffectiveDateForHI);
            updatedValue.Add("hiaccountnr", viewModel.AccountNumberForHI);
            updatedValue.Add("hibankbranchoffice", viewModel.BranchOfficeForHI);
            updatedValue.Add("hienddate", viewModel.EndDateForHI);
            updatedValue.Add("hicurrency", viewModel.CurrencyForHI.Value);

            updatedValue.Add("hiriaccountnr", viewModel.VendorAccountNumberRIForHI);
            updatedValue.Add("hirjaccountnr", viewModel.VendorAccountNumberRJForHI);
            updatedValue.Add("hirgaccountnr", viewModel.VendorAccountNumberRGForHI);
            updatedValue.Add("himaaccountnr", viewModel.VendorAccountNumberMAForHI);

            updatedValue.Add("spaccountname", viewModel.AccountNameForSP);
            updatedValue.Add("spbankname", viewModel.BankNameForSP);
            updatedValue.Add("speffectivedate", viewModel.EffectiveDateForSP);
            updatedValue.Add("spaccountnr", viewModel.AccountNumberForSP);
            updatedValue.Add("spbranchoffice", viewModel.BranchOfficeForSP);
            updatedValue.Add("spenddate", viewModel.EndDateForSP);
            updatedValue.Add("spcurrency", viewModel.CurrencyForSP.Value);

            updatedValue.Add("payrollaccountname", viewModel.AccountNameForPayroll);
            updatedValue.Add("payrollbankname", viewModel.BankNameForPayroll);
            updatedValue.Add("payrollaccountnr", viewModel.AccountNumberForPayroll);
            updatedValue.Add("payrollbranchoffice", viewModel.BranchOfficeForPayroll);
            updatedValue.Add("payrollcurrency", viewModel.CurrencyForPayroll.Value);
            updatedValue.Add("payrollbankswiftcode", viewModel.BankSwiftCodeForPayroll);
            updatedValue.Add("payrolltaxstatus", viewModel.TaxStatusForPayroll.Value);
            updatedValue.Add("datavalidationstatus", Workflow.GetProfessionalValidationStatus(Workflow.ProfessionalValidationStatus.NEED_VALIDATION));

            try
            {
                if (viewModel.ID == null)
                    SPConnector.AddListItem(SP_PROMAS_LIST_NAME, updatedValue, _siteUrl);
                else
                    SPConnector.UpdateListItem(SP_PROMAS_LIST_NAME, viewModel.ID, updatedValue, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                throw e;
            }
            if (viewModel.ID == null)
            {
               viewModel.ID = SPConnector.GetLatestListItemID(SP_PROMAS_LIST_NAME, _siteUrl);
            }
            return viewModel.ID;
        }


        public void CreateEducationDetails(int? headerID, IEnumerable<EducationDetailVM> viewModels)
        {
            foreach (var viewModel in viewModels)
            {
                if (Item.CheckIfSkipped(viewModel))
                    continue;

                if (Item.CheckIfDeleted(viewModel))
                {
                    try
                    {
                        SPConnector.DeleteListItem(SP_PROEDU_LIST_NAME, viewModel.ID, _siteUrl);

                    }
                    catch (Exception e)
                    {
                        logger.Debug(e.Message);
                        throw e;
                    }
                    continue;
                }

                var updatedValue = new Dictionary<string, object>();
                updatedValue.Add("Title", viewModel.Subject);
                updatedValue.Add("university", viewModel.University);
                updatedValue.Add("yearofgraduation", FormatUtil.ConvertToDateString(viewModel.YearOfGraduation));
                updatedValue.Add("remarks", viewModel.Remarks);
                updatedValue.Add("professional", new FieldLookupValue { LookupId = Convert.ToInt32(headerID) });

                try
                {
                    if (Item.CheckIfUpdated(viewModel))
                        SPConnector.UpdateListItem(SP_PROEDU_LIST_NAME, viewModel.ID, updatedValue, _siteUrl);
                    else
                        SPConnector.AddListItem(SP_PROEDU_LIST_NAME, updatedValue, _siteUrl);
                }
                catch (Exception e)
                {
                    logger.Debug(e.Message);                    
                    throw e;
                }
            }
        }

        public void CreateTrainingDetails(int? headerID, IEnumerable<TrainingDetailVM> trainingDetails)
        {
            foreach (var viewModel in trainingDetails)
            {
                if (Item.CheckIfSkipped(viewModel))
                    continue;

                if (Item.CheckIfDeleted(viewModel))
                {
                    try
                    {
                        SPConnector.DeleteListItem(SP_PROTRAIN_LIST_NAME, viewModel.ID, _siteUrl);

                    }
                    catch (Exception e)
                    {
                        logger.Error(e);
                        throw e;
                    }
                    continue;
                }

                var updatedValue = new Dictionary<string, object>();
                updatedValue.Add("Title", viewModel.Subject);
                updatedValue.Add("traininginstitution", viewModel.Institution);
                updatedValue.Add("trainingremarks", viewModel.Remarks);
                updatedValue.Add("trainingyear", FormatUtil.ConvertToDateString(viewModel.Year));
                updatedValue.Add("professional", new FieldLookupValue { LookupId = Convert.ToInt32(headerID) });

                try
                {
                    if (viewModel.ID == null)
                        SPConnector.AddListItem(SP_PROTRAIN_LIST_NAME, updatedValue, _siteUrl);
                    else
                        SPConnector.UpdateListItem(SP_PROTRAIN_LIST_NAME, viewModel.ID, updatedValue, _siteUrl);
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw e;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="headerID"></param>
        /// <param name="dependentDetails"></param>
        public void CreateDependentDetails(int? headerID, IEnumerable<DependentDetailVM> dependentDetails)
        {
            foreach (var viewModel in dependentDetails)
            {
                if (Item.CheckIfSkipped(viewModel))
                    continue;

                if (Item.CheckIfDeleted(viewModel))
                {
                    try
                    {
                        SPConnector.DeleteListItem(SP_PRODEP_LIST_NAME, viewModel.ID, _siteUrl);
                    }
                    catch (Exception e)
                    {
                        logger.Error(e);
                        throw e;
                    }
                    continue;
                }

                var updatedValue = new Dictionary<string, object>();
                updatedValue.Add("Title", viewModel.FullName);
                updatedValue.Add("relationship", viewModel.Relationship.Text);
                updatedValue.Add("placeofbirth", viewModel.PlaceOfBirth);
                updatedValue.Add("dateofbirth", viewModel.DateOfBirthGrid);
                updatedValue.Add("insurancenr", viewModel.InsuranceNumber);
                updatedValue.Add("remarks", viewModel.Remark);
                updatedValue.Add("professional", new FieldLookupValue { LookupId = Convert.ToInt32(headerID) });

                try
                {
                    if (viewModel.ID == null)
                        SPConnector.AddListItem(SP_PRODEP_LIST_NAME, updatedValue, _siteUrl);
                    else
                        SPConnector.UpdateListItem(SP_PRODEP_LIST_NAME, viewModel.ID, updatedValue, _siteUrl);
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw e;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="headerID"></param>
        /// <param name="organizationalDetails"></param>
        public void CreateOrganizationalDetails(int? headerID, IEnumerable<OrganizationalDetailVM> organizationalDetails)
        {
            var index = 0;
            var length = organizationalDetails.Count();
            foreach (var viewModel in organizationalDetails)
            {
                if (Item.CheckIfSkipped(viewModel))
                    continue;

                if (Item.CheckIfDeleted(viewModel))
                {
                    try
                    {
                        SPConnector.DeleteListItem(SP_PROORG_LIST_NAME, viewModel.ID, _siteUrl);
                    }
                    catch (Exception e)
                    {
                        logger.Error(e);
                        throw e;
                    }
                    continue;
                }

                var updatedValue = new Dictionary<string, object>();
                updatedValue.Add("Position", new FieldLookupValue { LookupId = Convert.ToInt32(viewModel.Position.Value) });
                updatedValue.Add("psanr", new FieldLookupValue { LookupId = Convert.ToInt32(viewModel.PSANumber.Value) });
                updatedValue.Add("Level", viewModel.Level);
                //updatedValue.Add("Status", viewModel.PSAStatus.Text);
                updatedValue.Add("projectunit", viewModel.Project.Text);
                updatedValue.Add("startdate", viewModel.StartDate);
                updatedValue.Add("lastworkingday", viewModel.LastWorkingDay);
                updatedValue.Add("professional", new FieldLookupValue { LookupId = Convert.ToInt32(headerID) });

                try
                {
                    if (viewModel.ID == null)
                        SPConnector.AddListItem(SP_PROORG_LIST_NAME, updatedValue, _siteUrl);
                    else
                        SPConnector.UpdateListItem(SP_PROORG_LIST_NAME, viewModel.ID, updatedValue, _siteUrl);
                }
                catch (Exception e)
                {
                    logger.Debug(e.Message);
                    throw e;
                }

                if (++index == length)
                {
                    UpdateCurrentPSAAndOrganization(headerID, viewModel);
                }
            }
        }

        private void UpdateCurrentPSAAndOrganization(int? headerID, OrganizationalDetailVM viewModel)
        {
            var updatedValue = new Dictionary<string, object>();
            updatedValue.Add("Position",
                new FieldLookupValue { LookupId = (int)viewModel.Position.Value });
            updatedValue.Add("PSAnumber",
                new FieldLookupValue { LookupId = (int)viewModel.PSANumber.Value });

            try
            {
                SPConnector.UpdateListItem(SP_PROMAS_LIST_NAME, headerID, updatedValue, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                throw e;
            }
        }

        public ProfessionalDataVM GetProfessionalData(string userLogin = null)
        {
            if (userLogin == null)
                return null;

            var caml = @"<View>  
                    <Query> 
                       <Where><Eq><FieldRef Name='officeemail' /><Value Type='Text'>" + userLogin + @"</Value></Eq></Where> 
                    </Query> 
                     <ViewFields><FieldRef Name='officeemail' /><FieldRef Name='ID' /></ViewFields> 
                    </View>";

            var professionalID = 0;
            foreach (var item in SPConnector.GetList(SP_PROMAS_LIST_NAME, _siteUrl, caml))
            {
                professionalID = Convert.ToInt32(item["ID"]);

            }
            return GetProfessionalData(professionalID);
        }

        public void SendEmailValidation(string emailTo, string emailMessages)
        {
            try
            {
                //SPConnector.SendEmail(emailTo, emailMessages, "Professional Data Validation", _siteUrl);
                EmailUtil.Send(emailTo, "Professional Data Validation", emailMessages);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                throw e;
            }
        }

        public void SendEmailValidation(string emailTo, string emailMessages, bool isApproved)
        {
            SendEmailValidation(emailTo, string.Format(emailMessages, isApproved ? "Approved" : "Rejected"));
        }

        public void UpdateValidation(int? ID, string status)
        {
            var updatedValue = new Dictionary<string, object>();
            updatedValue.Add("datavalidationstatus", status);

            try
            {
                SPConnector.UpdateListItem(SP_PROMAS_LIST_NAME, ID, updatedValue);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);                
                throw e;
            }
        }

        public void SetValidationStatus(int? id, Workflow.ProfessionalValidationStatus validationStatus)
        {
            var updatedValue = new Dictionary<string, object>();
            updatedValue.Add("datavalidationstatus", Workflow.GetProfessionalValidationStatus(validationStatus));

            try
            {
                SPConnector.UpdateListItem(SP_PROMAS_LIST_NAME, id, updatedValue, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                throw e;
            }
        }

        public async Task CreateEducationDetailsAsync(int? headerID, IEnumerable<EducationDetailVM> educationDetails)
        {
            CreateEducationDetails(headerID, educationDetails);
        }

        public async Task CreateTrainingDetailsAsync(int? headerID, IEnumerable<TrainingDetailVM> trainingDetails)
        {
            CreateTrainingDetails(headerID, trainingDetails);
        }

        public async Task CreateDependentDetailsAsync(int? headerID, IEnumerable<DependentDetailVM> documents)
        {
            CreateDependentDetails(headerID, documents);
        }

        public async Task CreateOrganizationalDetailsAsync(int? headerID, IEnumerable<OrganizationalDetailVM> organizationalDetails)
        {
            CreateOrganizationalDetails(headerID, organizationalDetails);
        }

        public List<string> GetEmailHR()
        {
            List<string> EmailHR = new List<string>();
            string caml = @"<View><Query><Where><Contains><FieldRef Name='Position' />
            <Value Type='Lookup'>HR</Value></Contains></Where></Query><ViewFields><FieldRef Name='officeemail' />
            <FieldRef Name='Position' /></ViewFields><QueryOptions /></View>";
            foreach (var item in SPConnector.GetList(SP_PROMAS_LIST_NAME, _siteUrl, caml))
            {
                EmailHR.Add(Convert.ToString(item["officeemail"]));
            }
            return EmailHR;
        }

        public string GetPosition(string username, string siteUrl)
        {
            var caml = @"<View><Query><Where><Eq><FieldRef Name='officeemail' /><Value Type='Text'>" + username + @"</Value></Eq></Where></Query><ViewFields><FieldRef Name='Position' /></ViewFields><QueryOptions /></View>";
            var listItem = SPConnector.GetList("Professional Master", siteUrl, caml);
            string position = "";
            foreach (var item in listItem)
            {
                position = FormatUtil.ConvertLookupToValue(item, "Position");
            }
            if (position == null)
            {
                position = "";
            }
            return position;
        }

        public string GetUser(string email, string siteUrl)
        {
            var caml = @"<View><Query>
                       <Where>
                          <Eq>
                             <FieldRef Name='officeemail' />
                             <Value Type='Text'>" + email + @"</Value>
                          </Eq>
                       </Where>
                    </Query>
                    <ViewFields>
                       <FieldRef Name='Title' />
                    </ViewFields>
                    <QueryOptions /></View>";
            var result = SPConnector.GetList("Professional Master", siteUrl, caml);
            var user = "";
            if (result.Count != 0)
            {
                foreach (var r in result)
                {
                    user = Convert.ToString(r["Title"]);
                }
            }

            return user;
        }
    }
}
