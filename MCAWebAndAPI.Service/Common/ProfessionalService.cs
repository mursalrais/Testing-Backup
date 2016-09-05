using System;
using System.Collections.Generic;
using MCAWebAndAPI.Model.HR.DataMaster;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.Common;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using static MCAWebAndAPI.Service.Common.CommonService;

namespace MCAWebAndAPI.Service.HR.Common
{
    /// <summary>
    /// Extension to MCAWebAndAPI.Service.HR.Common.ProfessionalService
    /// </summary>
    public partial class ProfessionalService
    {
        ///// <summary>
        ///// Gets an instance of ProfessionalDataVM based on its ID
        /////     
        /////  Be noticed that there are two different classes form Professional: ProfessionalMaster and ProfessionalDataVM
        ///// </summary>
        ///// <param name="siteUrl"></param>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public static ProfessionalDataVM Get(string siteUrl, int id)
        //{
        //    ProfessionalDataVM result;

        //    var siteUrlHR = CommonService.GetSiteUrlFromCurrent(siteUrl, Sites.HR);

        //    IProfessionalService service = new ProfessionalService();
        //    service.SetSiteUrl(siteUrlHR);

        //    result = Get(siteUrlHR,id);

        //    return result;
        //}


        /// <summary>
        /// Gets an instance of ProfessionalMaster based on its ID
        ///     Returns new instance if id parameter is null
        /// </summary>
        /// <param name="siteUrl"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ProfessionalMaster Get(string siteUrl, int? id)
        {
            ProfessionalMaster result = null;

            if (id == null)
            {
                result = new ProfessionalMaster();
            }
            else
            {
                var siteUrlHR = CommonService.GetSiteUrlFromCurrent(siteUrl, CommonService.Sites.HR);
                var listItem = SPConnector.GetListItem(SP_PROMAS_LIST_NAME, id, siteUrlHR);

                result = ConvertToProfessionalModel_Light(listItem);
            }

            return result;
        }


        private static ProfessionalMaster ConvertToProfessionalModel_Light(ListItem item)
        {
            return new ProfessionalMaster
            {
                ID = Convert.ToInt32(item["ID"]),
                FirstMiddleName = Convert.ToString(item["Title"]),
                Name = Convert.ToString(item["Title"]) + " " + Convert.ToString(item["lastname"]),
                Status = Convert.ToString(item["maritalstatus"]),
                Position = item["Position"] == null ? string.Empty :
                        Convert.ToString((item["Position"] as FieldLookupValue).LookupValue),
                PositionId = item["Position"] == null ? 0 :
                        Convert.ToInt32((item["Position"] as FieldLookupValue).LookupId),
                Project_Unit = Convert.ToString(item["Project_x002f_Unit"]),
                OfficeEmail = Convert.ToString(item["officeemail"]),
                PSANumber = Convert.ToString(item["PSAnumber"]),
                PersonalMail = Convert.ToString(item["personalemail"]),
                JoinDate = Convert.ToDateTime(item["Join_x0020_Date"]).ToLocalTime(),
                JoinDateTemp = Convert.ToDateTime(item["Join_x0020_Date"]).ToLocalTime().ToShortDateString(),
                InsuranceAccountNumber = Convert.ToString(item["hiaccountnr"]),
                MobileNumber = Convert.ToString(item["mobilephonenr"]),
                TaxStatus = Convert.ToString(item["payrolltaxstatus"]),

                // The followings are used in Payroll Worksheet
                BankAccountName = Convert.ToString(item["payrollbankname"]),
                BankAccountNumber = Convert.ToString(item["payrollaccountnr"]),
                BankBranchOffice = Convert.ToString(item["payrollbranchoffice"]),
                Currency = Convert.ToString(item["payrollcurrency"]),
                BankName = Convert.ToString(item["payrollbankname"])
            };
        }


    }
}