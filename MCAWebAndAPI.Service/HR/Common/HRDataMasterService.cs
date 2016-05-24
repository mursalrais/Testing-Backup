using System;
using System.Collections.Generic;
using MCAWebAndAPI.Model.HR.DataMaster;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using System.Linq;

namespace MCAWebAndAPI.Service.HR.Common
{
    /// <summary>
    /// <ViewFields>
    //    <FieldRef Name = 'Title' />
    //    < FieldRef Name='lastname' />
    //    <FieldRef Name = 'Position' />
    //    < FieldRef Name='placeofbirth' />
    //    <FieldRef Name = 'dateofbirth' />
    //    < FieldRef Name='maritalstatus' />
    //    <FieldRef Name = 'bloodtype' />
    //    < FieldRef Name='Gender' />
    //    <FieldRef Name = 'Nationality' />
    //    < FieldRef Name='Religion' />
    //    <FieldRef Name = 'idcardtype' />
    //    < FieldRef Name='idcardnumber' />
    //    <FieldRef Name = 'idcardexpirydate' />
    //    < FieldRef Name='permanentaddress' />
    //    <FieldRef Name = 'permanentlandlinephone' />
    //    < FieldRef Name='currentaddress' />
    //    <FieldRef Name = 'currentlandlinephone' />
    //    < FieldRef Name='emergencynumber' />
    //    <FieldRef Name = 'officephone' />
    //    < FieldRef Name='Extension' />
    //    <FieldRef Name = 'NIK' />
    //    < FieldRef Name='officeemail' />
    //    <FieldRef Name = 'personalemail' />
    //    < FieldRef Name='personalemail2' />
    //    <FieldRef Name = 'mobilephonenr' />
    //    < FieldRef Name='mobilephonenr2' />
    //    <FieldRef Name = 'hiaccountname' />
    //    < FieldRef Name='hiaccountnr' />
    //    <FieldRef Name = 'hicurrency' />
    //    < FieldRef Name='hibankname' />
    //    <FieldRef Name = 'hibankbranchoffice' />
    //    < FieldRef Name='hieffectivedate' />
    //    <FieldRef Name = 'hienddate' />
    //    < FieldRef Name='hiriaccountnr' />
    //    <FieldRef Name = 'hirjaccountnr' />
    //    < FieldRef Name='hirgaccountnr' />
    //    <FieldRef Name = 'himaaccountnr' />
    //    < FieldRef Name='spaccountname' />
    //    <FieldRef Name = 'spaccountnr' />
    //    < FieldRef Name='spcurrency' />
    //    <FieldRef Name = 'spbankname' />
    //    < FieldRef Name='spbranchoffice' />
    //    <FieldRef Name = 'speffectivedate' />
    //    < FieldRef Name='spenddate' />
    //    <FieldRef Name = 'payrollaccountname' />
    //    < FieldRef Name='payrollaccountnr' />
    //    <FieldRef Name = 'payrollcurrency' />
    //    < FieldRef Name='payrollbankname' />
    //    <FieldRef Name = 'payrollbranchoffice' />
    //    < FieldRef Name='payrollbankswiftcode' />
    //    <FieldRef Name = 'payrolltaxstatus' />
    //    < FieldRef Name='taxid' />
    //    <FieldRef Name = 'nameintaxid' />
    //    < FieldRef Name='taxaddress' />
    //    <FieldRef Name = 'datavalidationstatus' /></ ViewFields >
    //</ View >
    /// </summary>
    public class HRDataMasterService : IHRDataMasterService
    {
        string _siteUrl;
        const string SP_PROMAS_LIST_NAME = "Professional Master";
        const string SP_POSMAS_LIST_NAME = "Position Master";
        const string SP_MONFEE_LIST_NAME = "Monthly Fee";

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
                collectionIDMonthlyFee.Add(Convert.ToInt32(item["ProfessionalId"]));
            }
            foreach (var item in SPConnector.GetList(SP_PROMAS_LIST_NAME, _siteUrl))
            {
                tempID = Convert.ToInt32(item["ID"]);
                if (!(collectionIDMonthlyFee.Any(e => e == tempID)))
                {
                    models.Add(ConvertToProfessionalModel_Light(item));
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
                Name = Convert.ToString(item["Title"]),
                Status = Convert.ToString(item["maritalstatus"]),
                Position = item["Position"] == null ? "" :
               Convert.ToString((item["Position"] as FieldLookupValue).LookupValue)
            };
        }

        public IEnumerable<PositionsMaster> GetPositions()
        {
            var models = new List<PositionsMaster>();

            foreach (var item in SPConnector.GetList(SP_POSMAS_LIST_NAME, _siteUrl))
            {
                models.Add(ConvertToPositionsModel(item));
            }

            return models;
        }

        private PositionsMaster ConvertToPositionsModel(ListItem item)
        {
            return new PositionsMaster
            {
                ID = Convert.ToInt32(item["ID"]),
                Title = Convert.ToString(item["Title"]),
                //PositionStatus = Convert.ToString(item["positionstatus"]),
                //Position = Convert.ToString(item["Position"]),
                //ProjectUnit = Convert.ToString(item["ProjectUnit"])
            };
        }

        public ProfessionalDataVM GetProfessionalData(int? ID)
        {
            if (ID == null)
                return new ProfessionalDataVM();

            var listItem = SPConnector.GetListItem(SP_POSMAS_LIST_NAME, ID, _siteUrl);
            return ConvertToProfessionalModel(listItem);
        }

        /// <summary>
   //       <ViewFields>
   //   < FieldRef Name='nationality' />
   //   <FieldRef Name = 'idcardtype' />
   //   < FieldRef Name='idcardnumber' />
   //   <FieldRef Name = 'idcardexpirydate' />
   //   < FieldRef Name='permanentaddress' />
   //   <FieldRef Name = 'permanentlandlinephone' />
   //   < FieldRef Name='currentaddress' />
   //   <FieldRef Name = 'currentlandlinephone' />
   //   < FieldRef Name='emergencynumber' />
   //   <FieldRef Name = 'officephone' />
   //   < FieldRef Name='Extension' />
   //   <FieldRef Name = 'NIK' />
   //   < FieldRef Name='officeemail' />
   //   <FieldRef Name = 'personalemail' />
   //   < FieldRef Name='personalemail2' />
   //   <FieldRef Name = 'mobilephonenr' />
   //   < FieldRef Name='mobilephonenr2' />
   //   <FieldRef Name = 'hiaccountname' />
   //   < FieldRef Name='hiaccountnr' />
   //   <FieldRef Name = 'hicurrency' />
   //   < FieldRef Name='hibankname' />
   //   <FieldRef Name = 'hibankbranchoffice' />
   //   < FieldRef Name='hieffectivedate' />
   //   <FieldRef Name = 'hienddate' />
   //   < FieldRef Name='hiriaccountnr' />
   //   <FieldRef Name = 'hirjaccountnr' />
   //   < FieldRef Name='hirgaccountnr' />
   //   <FieldRef Name = 'himaaccountnr' />
   //   < FieldRef Name='spaccountname' />
   //   <FieldRef Name = 'spaccountnr' />
   //   < FieldRef Name='spcurrency' />
   //   <FieldRef Name = 'spbankname' />
   //   < FieldRef Name='spbranchoffice' />
   //   <FieldRef Name = 'speffectivedate' />
   //   < FieldRef Name='spenddate' />
   //   <FieldRef Name = 'payrollaccountname' />
   //   < FieldRef Name='payrollaccountnr' />
   //   <FieldRef Name = 'payrollcurrency' />
   //   < FieldRef Name='payrollbranchoffice' />
   //   <FieldRef Name = 'payrollbankname' />
   //   < FieldRef Name='payrollbankswiftcode' />
   //   <FieldRef Name = 'payrolltaxstatus' />
   //   < FieldRef Name='taxid' />
   //   <FieldRef Name = 'nameintaxid' />
   //   < FieldRef Name='taxaddress' />
   //   <FieldRef Name = 'datavalidationstatus' />
   //   < FieldRef Name='ID' />
   //</ViewFields>
        /// </summary>
        /// <param name="listItem"></param>
        /// <returns></returns>
        private ProfessionalDataVM ConvertToProfessionalModel(ListItem listItem)
        {
            var viewModel = new ProfessionalDataVM();
            viewModel.ID = Convert.ToInt32(listItem["ID"]);
            viewModel.FirstMiddleName = Convert.ToString(listItem["Title"]);
            viewModel.LastName = Convert.ToString(listItem["lastname"]);
            viewModel.PlaceOfBirth = Convert.ToString(listItem["placeofbirth"]);
            viewModel.DateOfBirth = Convert.ToDateTime(listItem["dateofbirth"]);
            viewModel.MaritalStatus.Value = Convert.ToString(listItem["maritalstatus"]);
            viewModel.BloodType.Value = Convert.ToString(listItem["bloodtype"]);
            viewModel.Gender.Value = Convert.ToString(listItem["gender"]);
            viewModel.Religion.Value = Convert.ToString("religion");

            return viewModel;

        }
    }
}
