﻿using System;
using System.Collections.Generic;
using MCAWebAndAPI.Model.HR.DataMaster;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using System.Linq;
using NLog;

namespace MCAWebAndAPI.Service.HR.Common
{
    /// <summary>
    /// The implementation of all common service methods in HR modules.
    /// </summary>
    public class DataMasterService : IDataMasterService
    {
        string _siteUrl;
        const string SP_PROMAS_LIST_NAME = "Professional Master";
        const string SP_POSMAS_LIST_NAME = "Position Master";
        const string SP_MONFEE_LIST_NAME = "Monthly Fee";
        const string SP_MONFEE_DETAIL_LIST_NAME = "Monthly Fee Detail";
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
            foreach (var item in SPConnector.GetList(SP_PROMAS_LIST_NAME, _siteUrl))
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
                InsuranceAccountNumber = Convert.ToString(item["hiaccountnr"]),

                BankAccountName = Convert.ToString(item["payrollbankname"]),
                BankAccountNumber = Convert.ToString(item["payrollaccountnr"]),
                BankBranchOffice = Convert.ToString(item["payrollbranchoffice"]),
                Currency = Convert.ToString(item["payrollcurrency"]),
                BankName = Convert.ToString(item["payrollbankname"])

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

        private DependentMaster ConvertToDependentModel_Light(ListItem item)
        {

            return new DependentMaster
            {
                ID = Convert.ToInt32(item["ID"]),
                InsuranceNumber = Convert.ToString(item["insurancenr"]),
                OrganizationInsurance = Convert.ToString(item["insurancenr"]),
                Name = Convert.ToString(item["Title"])
                //Name = FormatUtil.ConvertLookupToValue(item, "professional")
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
            string caml = @"<View><Query><Where><Eq><FieldRef Name='projectunit' /><Value Type='Choice'>" + Level + "</Value></Eq></Where></Query><ViewFields><FieldRef Name='ID' /><FieldRef Name='ContentType' /><FieldRef Name='Title' /><FieldRef Name='Modified' /><FieldRef Name='Created' /><FieldRef Name='Author' /><FieldRef Name='Editor' /><FieldRef Name='_UIVersionString' /><FieldRef Name='Attachments' /><FieldRef Name='Edit' /><FieldRef Name='LinkTitleNoMenu' /><FieldRef Name='LinkTitle' /><FieldRef Name='DocIcon' /><FieldRef Name='ItemChildCount' /><FieldRef Name='FolderChildCount' /><FieldRef Name='AppAuthor' /><FieldRef Name='AppEditor' /><FieldRef Name='projectunit' /><FieldRef Name='iskeyposition' /><FieldRef Name='positionstatus' /><FieldRef Name='Remarks' /></ViewFields><QueryOptions /></View>";

            foreach (var item in SPConnector.GetList(SP_POSMAS_LIST_NAME, _siteUrl, caml))
            {
                models.Add(ConvertToPositionsModel(item));
            }

            return models;
        }

        public IEnumerable<DependentMaster> GetDependentsForInsurance(int? id)
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

            var caml = @"<View><Query><Where><Eq><FieldRef Name='professional_x003a_ID' />
                        <Value Type='Lookup'>" + id +
                      "</Value></Eq></Where></Query></View>";

            foreach (var item in SPConnector.GetList(SP_PRODEP_LIST_NAME, _siteUrl, caml))
            {
                models.Add(ConvertToDependentModel_Light(item));
            }

            return models;
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

        public string GetProfessionalPosition(string userLogin)
        {
            var professional = GetProfessionals().FirstOrDefault(e => e.OfficeEmail == userLogin);
            return professional.Position;
        }

        public string GetProfessionalOfficeEmail(int professionalID)
        {
            var caml = @"<View>  
                    <Query> 
                       <Where><Eq><FieldRef Name='ID' /><Value Type='Number'>" + professionalID + @"</Value></Eq></Where> 
                    </Query> 
                     <ViewFields><FieldRef Name='officeemail' /><FieldRef Name='ID' /></ViewFields> 
                    </View>";

            var professionalOfficeEmail = string.Empty;
            foreach (var item in SPConnector.GetList(SP_PROMAS_LIST_NAME, _siteUrl, caml))
            {
                professionalOfficeEmail = Convert.ToString(item["officeemail"]);
                break;
            }

            return professionalOfficeEmail;
        }

        /// <summary>
        /// Get monthly fee details for given professional IDs
        /// </summary>
        /// <param name="professionalIDs"></param>
        /// <returns></returns>
        public IEnumerable<MonthlyFeeMaster> GetMonthlyFees(int[] professionalIDs)
        {
            var caml = @"<View>  
             <ViewFields><FieldRef Name='professional' /><FieldRef Name='ID' /></ViewFields> 
            </View>";

            //Item1 = ProfessionalID, Item2 = HeaderID
            var profIDAndHeaderIDs = new List<Tuple<int, int>>();

            foreach (var item in SPConnector.GetList(SP_MONFEE_LIST_NAME, _siteUrl, caml))
            {
                var profID = FormatUtil.ConvertLookupToID(item, "professional");
                if (professionalIDs.Contains((int)profID))
                {
                    profIDAndHeaderIDs.Add(new Tuple<int, int>((int)profID,Convert.ToInt32(item["ID"])));
                }
            }

            var monthlyFees = new List<MonthlyFeeMaster>();
            foreach (var header in profIDAndHeaderIDs)
            {
                caml = @"<View>  
                <Query> 
                <Where><Eq><FieldRef Name='monthlyfeeid' LookupId='True' /><Value Type='Lookup'>" + header.Item2 + 
                @"</Value></Eq></Where> 
                </Query> 
                <ViewFields><FieldRef Name='dateofnewfee' /><FieldRef Name='enddate' /><FieldRef Name='monthlyfee' /><FieldRef Name='monthlyfeeid' /></ViewFields> 
                </View>";

                foreach (var item in SPConnector.GetList(SP_MONFEE_DETAIL_LIST_NAME, _siteUrl, caml))
                {
                    monthlyFees.Add(new MonthlyFeeMaster
                    {
                        ProfessionalID = header.Item1, 
                        DateOfNewFee = Convert.ToDateTime(item["dateofnewfee"]),
                        EndDate = Convert.ToDateTime(item["enddate"]),
                        MonthlyFee = Convert.ToDouble(item["monthlyfee"])
                    });
                }
            }

            return monthlyFees;
        }
    }
}