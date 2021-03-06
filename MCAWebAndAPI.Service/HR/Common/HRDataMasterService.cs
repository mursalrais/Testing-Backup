﻿using System;
using System.Collections.Generic;
using MCAWebAndAPI.Model.HR.DataMaster;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using System.Linq;
using NLog;
using MCAWebAndAPI.Model.Common;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Service.HR.Common
{
    public class HRDataMasterService : IHRDataMasterService
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

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
        }

        public IEnumerable<ProfessionalMaster> GetProfessionalMonthlyFees()
        {
            var models = new List<ProfessionalMaster>();
            int tempID;
            List<int> collectionIDMonthlyFee = new List<int>();
            foreach (var item in SPConnector.GetList(SP_MONFEE_LIST_NAME, _siteUrl))
            {
                collectionIDMonthlyFee.Add(item["professional_x003a_ID"] == null ? 0 :
               Convert.ToInt16((item["professional_x003a_ID"] as FieldLookupValue).LookupValue));
            }
            foreach (var item in SPConnector.GetList(SP_PROMAS_LIST_NAME, _siteUrl))
            {
                tempID = Convert.ToInt32(item["ID"]);
                if (!(collectionIDMonthlyFee.Any(e => e == tempID)))
                {
                    models.Add(ConvertToProfessionalMonthlyFeeModel_Light(item));
                }
            }
            return models;
        }

        public IEnumerable<ProfessionalMaster> GetProfessionalMonthlyFeesEdit()
        {
            var models = new List<ProfessionalMaster>();
            int tempID;
            List<int> collectionIDMonthlyFee = new List<int>();
            foreach (var item in SPConnector.GetList(SP_MONFEE_LIST_NAME, _siteUrl))
            {
                collectionIDMonthlyFee.Add(item["professional_x003a_ID"] == null ? 0 :
               Convert.ToInt16((item["professional_x003a_ID"] as FieldLookupValue).LookupValue));
            }
            foreach (var item in SPConnector.GetList(SP_PROMAS_LIST_NAME, _siteUrl))
            {
                tempID = Convert.ToInt32(item["ID"]);
                if ((collectionIDMonthlyFee.Any(e => e == tempID)))
                {
                    models.Add(ConvertToProfessionalMonthlyFeeModel_Light(item));
                }
            }

            return models;
        }

        public IEnumerable<ProfessionalMaster> GetProfessionals()
        {
            var models = new List<ProfessionalMaster>();
            foreach(var item in SPConnector.GetList(SP_PROMAS_LIST_NAME, _siteUrl))
            {
                    models.Add(ConvertToProfessionalModel_Light(item));
            }

            return models;
        }

        private ProfessionalMaster ConvertToProfessionalMonthlyFeeModel_Light(ListItem item)
        {
            return new ProfessionalMaster
            {
                ID = Convert.ToInt32(item["ID"]),
                Name = Convert.ToString(item["Title"]),
                Status = Convert.ToString(item["maritalstatus"]),
            };
        }

        /// <summary>
        /// Convert to light-weight version of professional model.
        /// This is only used to display professional combo box
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private ProfessionalMaster ConvertToProfessionalModel_Light(ListItem item)
        {
            return new ProfessionalMaster
            {
                ID = Convert.ToInt32(item["ID"]),
                FirstMiddleName = Convert.ToString(item["Title"]),
                Name = Convert.ToString(item["Title"]) + " " + Convert.ToString(item["lastname"]),
                Status = Convert.ToString(item["maritalstatus"]),
                Position = item["Position"] == null ? string.Empty :
                        Convert.ToString((item["Position"] as FieldLookupValue).LookupValue),
                Project_Unit = Convert.ToString(item["Project_x002f_Unit"]),
                OfficeEmail = Convert.ToString(item["officeemail"]),
                PSANumber = Convert.ToString(item["PSAnumber"]),
                PersonalMail = Convert.ToString(item["personalemail"]),
                JoinDate = Convert.ToDateTime(item["Join_x0020_Date"]).ToLocalTime(),
                JoinDateTemp = Convert.ToDateTime(item["Join_x0020_Date"]).ToLocalTime().ToShortDateString(),
                InsuranceAccountNumber = Convert.ToString(item["hiaccountnr"])
            };
        }

        public IEnumerable<PositionMaster> GetPositions()
        {
            var models = new List<PositionMaster>();

            foreach (var item in SPConnector.GetList(SP_POSMAS_LIST_NAME, _siteUrl))
            {
                models.Add(ConvertToPositionsModel(item));
            }

            return models;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private PositionMaster ConvertToPositionsModel(ListItem item)
        {
            var viewModel = new PositionMaster();

            viewModel.ID = Convert.ToInt32(item["ID"]);
            viewModel.PositionName = Convert.ToString(item["Title"]);
            viewModel.IsKeyPosition = Convert.ToString(item["iskeyposition"]);
            viewModel.ProjectUnit = Convert.ToString(item["projectunit"]);
            return viewModel;
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
                logger.Error(e);
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

            viewModel.ID = Convert.ToInt32(listItem["ID"]);
            viewModel.FirstMiddleName = Convert.ToString(listItem["Title"]);
            viewModel.CurrentPosition.Value = FormatUtil.ConvertLookupToID(listItem, "Position");
            viewModel.JoinDate = Convert.ToDateTime(listItem["Join_x0020_Date"]);
            viewModel.ProfessionalStatus.Value = Convert.ToString(listItem["Professional_x0020_Status"]);
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
            viewModel.MobileNumberOne = Convert.ToString(listItem["mobilephonenr"]);
            viewModel.MaritalStatus.Value = Convert.ToString(listItem["maritalstatus"]);
            viewModel.BloodType.Value = Convert.ToString(listItem["bloodtype"]);
            viewModel.Religion.Value = Convert.ToString(listItem["religion"]);
            viewModel.Gender.Value = Convert.ToString(listItem["gender"]);
            viewModel.IDCardType.Text = Convert.ToString(listItem["idcardtype"]);
            viewModel.IDCardExpiry = Convert.ToDateTime(listItem["idcardexpirydate"]);
            viewModel.Nationality.Value = FormatUtil.ConvertLookupToID(listItem, "nationality");

            // Fields not from Application Data
            viewModel.EmergencyNumber = Convert.ToString(listItem["emergencynumber"]);
            viewModel.OfficePhone = Convert.ToString(listItem["officephone"]);
            viewModel.OfficeEmail = Convert.ToString(listItem["officeemail"]);
            viewModel.Extension = Convert.ToString(listItem["Extension"]);

            viewModel.AccountNameForHI = Convert.ToString(listItem["hiaccountname"]);
            viewModel.BankNameForHI = Convert.ToString(listItem["hibankname"]);
            viewModel.EffectiveDateForHI = Convert.ToDateTime(listItem["hieffectivedate"]);
            viewModel.AccountNumberForHI = Convert.ToString(listItem["hiaccountnr"]);
            viewModel.BranchOfficeForHI = Convert.ToString(listItem["hibankbranchoffice"]);
            viewModel.EndDateForHI = Convert.ToDateTime(listItem["hienddate"]);
            viewModel.CurrencyForHI.Value = Convert.ToString(listItem["hicurrency"]);

            viewModel.VendorAccountNumberRIForHI = Convert.ToString(listItem["hiriaccountnr"]);
            viewModel.VendorAccountNumberRJForHI = Convert.ToString(listItem["hirjaccountnr"]);
            viewModel.VendorAccountNumberRGForHI = Convert.ToString(listItem["hirgaccountnr"]);
            viewModel.VendorAccountNumberMAForHI = Convert.ToString(listItem["himaaccountnr"]);

            viewModel.AccountNameForSP = Convert.ToString(listItem["spaccountname"]);
            viewModel.BankNameForSP = Convert.ToString(listItem["spbankname"]);
            viewModel.EffectiveDateForSP = Convert.ToDateTime(listItem["speffectivedate"]);
            viewModel.AccountNumberForSP = Convert.ToString(listItem["spaccountnr"]);
            viewModel.BranchOfficeForSP = Convert.ToString(listItem["spbranchoffice"]);
            viewModel.EndDateForSP = Convert.ToDateTime(listItem["spenddate"]);
            viewModel.CurrencyForSP.Value = Convert.ToString(listItem["spcurrency"]);

            viewModel.AccountNameForPayroll = Convert.ToString(listItem["payrollaccountname"]);
            viewModel.BankNameForPayroll = Convert.ToString(listItem["payrollbankname"]);
            viewModel.AccountNumberForPayroll = Convert.ToString(listItem["payrollaccountnr"]);
            viewModel.BranchOfficeForPayroll = Convert.ToString(listItem["payrollbranchoffice"]);
            viewModel.CurrencyForPayroll.Value = Convert.ToString(listItem["payrollcurrency"]);
            viewModel.BankSwiftCodeForPayroll = Convert.ToString(listItem["payrollbankswiftcode"]);
            viewModel.TaxStatusForPayroll.Value = Convert.ToString(listItem["payrolltaxstatus"]);
            
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
            return new OrganizationalDetailVM
            {
                ID = Convert.ToInt32(item["ID"]),
                LastWorkingDay = Convert.ToDateTime(item["lastworkingday"]),
                Level = Convert.ToString(item["Level"]),
                Position = FormatUtil.ConvertToInGridAjaxComboBox(item, "Position"),
                PSANumber = FormatUtil.ConvertToInGridAjaxComboBox(item, "psanr"),
                StartDate = Convert.ToDateTime(item["startdate"]),
                Project = OrganizationalDetailVM.GetProjectDefaultValue(
                    FormatUtil.ConvertToInGridComboBox(item, "projectunit")),
                PSAStatus = OrganizationalDetailVM.GetPSAStatusDefaultValue(
                    FormatUtil.ConvertToInGridComboBox(item, "Status"))
            };
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
            return dependentDetail;
        }
        
        private DependentDetailVM ConvertToDependentDetailVM(ListItem item)
        {
            return new DependentDetailVM
            {
                ID = Convert.ToInt32(item["ID"]),
                FullName = Convert.ToString(item["Title"]),
                DateOfBirth = Convert.ToDateTime(item["dateofbirth"]),
                InsuranceNumber = Convert.ToString(item["insurancenr"]),
                PlaceOfBirth = Convert.ToString(item["placeofbirth"]),
                Remark = FormatUtil.ConvertMultipleLine(Convert.ToString(item["remark"])),
                Relationship = FormatUtil.ConvertToInGridComboBox(item, "relationship")
            };
        }

        private IEnumerable<EducationDetailVM> GetEducationDetails(int? iD)
        {
            var caml = @"<View>  
            <Query> 
               <Where><Eq><FieldRef Name='professional' LookupId='True' /><Value Type='Lookup'>" + iD + @"</Value></Eq></Where> 
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
            return new EducationDetailVM
            {
                ID = Convert.ToInt32(item["ID"]),
                Subject = Convert.ToString(item["Title"]),
                University = Convert.ToString(item["university"]),
                YearOfGraduation = FormatUtil.ConvertDateStringToDateTime(item, "yearofgraduation"),
                Remarks = Convert.ToString(item["remarks"])
            };
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
                Year = FormatUtil.ConvertDateStringToDateTime(item, "trainingyear")
            };
        }

        public int? EditProfessionalData(ProfessionalDataVM viewModel)
        {
            var updatedValue = new Dictionary<string, object>();

            updatedValue.Add("Title", viewModel.FirstMiddleName);
            updatedValue.Add("Position", new FieldLookupValue
                { LookupId = (int) viewModel.CurrentPosition.Value });
            updatedValue.Add("Professional_x0020_Status", viewModel.ProfessionalStatus.Value);
            updatedValue.Add("Join_x0020_Date", viewModel.JoinDate);
            updatedValue.Add("lastname", viewModel.LastName);
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
            updatedValue.Add("nationality", new FieldLookupValue { LookupId = (int)viewModel.Nationality.Value });

            // Fields not from Application Data
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
                if(viewModel.ID == null)
                    SPConnector.AddListItem(SP_PROMAS_LIST_NAME, updatedValue);
                else
                    SPConnector.UpdateListItem(SP_PROMAS_LIST_NAME, viewModel.ID, updatedValue);
            }
            catch(Exception e)
            {
                logger.Error(e);
                throw e;
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
                        logger.Error(e);
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
                    logger.Error(e.Message);
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
                updatedValue.Add("dateofbirth", viewModel.DateOfBirth);
                updatedValue.Add("insurancenr", viewModel.InsuranceNumber);
                updatedValue.Add("remark", viewModel.Remark);
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
                updatedValue.Add("Status", viewModel.PSAStatus.Text);
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
                    logger.Error(e.Message);
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
                new FieldLookupValue { LookupId = (int) viewModel.Position.Value });
            updatedValue.Add("PSAnumber",
                new FieldLookupValue { LookupId = (int) viewModel.PSANumber.Value });

            try
            {
                SPConnector.UpdateListItem(SP_PROMAS_LIST_NAME, headerID, updatedValue, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e);
                throw e;
            }
        }

        public ProfessionalDataVM GetProfessionalData(string userLoginName = null)
        {
            if (userLoginName == null)
                return null;

            var caml = @"<View>  
                    <Query> 
                       <Where><Eq><FieldRef Name='officeemail' /><Value Type='Text'>" + userLoginName + @"</Value></Eq></Where> 
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
                SPConnector.SendEmail(emailTo, emailMessages, "Professional Data Validation", _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e);
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
                logger.Error(e);
                throw e;
            }
        }

        public PositionMaster GetPosition(int id)
        {
            var caml = @"<View>  
            <Query> 
               <Where><Eq><FieldRef Name='ID' /><Value Type='Counter'>" + id + @"</Value></Eq></Where> 
            </Query> 
             <ViewFields><FieldRef Name='Title' /><FieldRef Name='ID' /></ViewFields> 
      </View>";

            var position = new PositionMaster();
            foreach (var item in SPConnector.GetList(SP_POSMAS_LIST_NAME, _siteUrl, caml))
            {
                position.ID = Convert.ToInt32(item["ID"]);
                position.PositionName = Convert.ToString(item["Title"]);
                //TODO: To add other neccessary property
            }
             
            return position;
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
                logger.Error(e);
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

        private DependentMaster ConvertToDependentModel_Light(ListItem item)
        {
          
            return new DependentMaster
            {
                ID = Convert.ToInt32(item["ID"]),
                InsuranceNumber = Convert.ToString(item["insurancenr"]),
                OrganizationInsurance = Convert.ToString(item["insurancenr"]) ,
                Name = FormatUtil.ConvertLookupToValue(item, "professional")
            };
        }
        public IEnumerable<DependentMaster> GetDependents()
        {
            var models = new List<DependentMaster>();
            foreach (var item in SPConnector.GetList(SP_PRODEP_LIST_NAME, _siteUrl))
            {
                models.Add(ConvertToDependentModel_Light(item));
            }

            return models;
        }

        public IEnumerable<PositionMaster> GetPositionsManpower(string Level)
        {
            var models = new List<PositionMaster>();
            string caml = @"<View><Query><Where><Eq><FieldRef Name='projectunit' /><Value Type='Choice'>"+Level+"</Value></Eq></Where></Query><ViewFields><FieldRef Name='ID' /><FieldRef Name='ContentType' /><FieldRef Name='Title' /><FieldRef Name='Modified' /><FieldRef Name='Created' /><FieldRef Name='Author' /><FieldRef Name='Editor' /><FieldRef Name='_UIVersionString' /><FieldRef Name='Attachments' /><FieldRef Name='Edit' /><FieldRef Name='LinkTitleNoMenu' /><FieldRef Name='LinkTitle' /><FieldRef Name='DocIcon' /><FieldRef Name='ItemChildCount' /><FieldRef Name='FolderChildCount' /><FieldRef Name='AppAuthor' /><FieldRef Name='AppEditor' /><FieldRef Name='projectunit' /><FieldRef Name='iskeyposition' /><FieldRef Name='positionstatus' /><FieldRef Name='Remarks' /></ViewFields><QueryOptions /></View>";

            foreach (var item in SPConnector.GetList(SP_POSMAS_LIST_NAME, _siteUrl,caml))
            {
                models.Add(ConvertToPositionsModel(item));
            }

            return models;
        }

        public IEnumerable<DependentMaster> GetDependentsForInsurance()
        {
            var models = new List<DependentMaster>();

            var _default = new DependentMaster
            {
                ID = 0,
                InsuranceNumber = "",
                OrganizationInsurance = "",
                Name = ""
            };
            models.Add(_default);
            foreach (var item in SPConnector.GetList(SP_PRODEP_LIST_NAME, _siteUrl))
            {
                models.Add(ConvertToDependentModel_Light(item));
            }

            return models;
        }
    }
}
