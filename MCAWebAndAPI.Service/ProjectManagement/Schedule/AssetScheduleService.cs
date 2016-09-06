using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;

namespace MCAWebAndAPI.Service.ProjectManagement.Schedule
{
    public class AssetScheduleService : IAssetScheduleService
    {

        static Logger logger = LogManager.GetCurrentClassLogger();
        const string SP_PSA_LIST_NAME = "PSA";
        const string SP_ASSETLOANRETURNDETAIL_LIST_NAME = "Asset Loan Return Detail";
        const string SP_ASSETLOANRETURN_LIST_NAME = "Asset Loan Return";
        const string SP_PROFESSIONAL_LIST_NAME = "Professional Master";

        string _siteUrl = null;

        public bool CheckTwoMonthsBeforeExpireDate()
        {
           

            var caml = @"<View><Query>
                            <Where>
                                <IsNull><FieldRef Name='returndate' /></IsNull>
                            </Where>
                        </Query>
                        <ViewFields>
                                <FieldRef Name='estreturndate' />
                                <FieldRef Name='returndate' />
                                <FieldRef Name='status' />
                                <FieldRef Name='assetloanandreturn' />
                        </ViewFields><QueryOptions /></View>";

            foreach (var assetData in SPConnector.GetList(SP_ASSETLOANRETURNDETAIL_LIST_NAME, _siteUrl,caml))
            {
               
                
                DateTime returnDate = Convert.ToDateTime(assetData["returndate"]).ToLocalTime();
                DateTime estreturnDate = Convert.ToDateTime(assetData["estreturndate"]).ToLocalTime();


                string strReturnDate = Convert.ToDateTime(assetData["returndate"]).ToLocalTime().ToShortDateString();
                string strEstReturnDate = Convert.ToDateTime(assetData["estreturndate"]).ToLocalTime().ToShortDateString();

                logger.Info("expireDate: " + returnDate + "--" + "strReturnDate: " + strReturnDate );

                DateTime today = DateTime.Now;
                string strToday = today.ToLocalTime().ToShortDateString();

                var a = (assetData["assetloanandreturn"] as FieldLookupValue).LookupId; 
                

                   var professionalData = SPConnector.GetListItem(SP_ASSETLOANRETURN_LIST_NAME, a, _siteUrl);
               
                    if (professionalData != null)
                    {
                        string professionalMail = Convert.ToString(professionalData["name_x003a_Office_x0020_Email"]);
                        string professionalFullName = Convert.ToString(professionalData["professionalname"]);

                        if (strToday == strReturnDate)
                        {
                            string mailsubject = "notification of psa expired";
                            string mailcontent = string.Format("dear mr./mrs. {0}. this email is sent to you to notify that your psa will be expired in the next two months. please kindly communicate to hr dept. for any further action.", professionalFullName);

                            SendMailTwoMonthBeforeExpired(professionalMail, mailsubject, mailcontent);
                        }

                     }
                else
                {
                    continue;
                }
            }

            return true;
        }

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
        }

        public void SendMailTwoMonthBeforeExpired(string professionalMail, string mailSubject, string mailContent)
        {
            EmailUtil.Send(professionalMail, mailSubject, mailContent);
        }
    }
}
