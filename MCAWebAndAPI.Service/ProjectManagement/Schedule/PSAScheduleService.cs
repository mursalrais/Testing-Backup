using System;
using System.Linq;
using MCAWebAndAPI.Model.ProjectManagement.Schedule;
using MCAWebAndAPI.Service.Utils;
using System.Collections.Generic;
using MCAWebAndAPI.Model.ViewModel.Chart;
using Microsoft.SharePoint.Client;
using NLog;
using MCAWebAndAPI.Model.ViewModel.Gantt;


namespace MCAWebAndAPI.Service.ProjectManagement.Schedule
{
    public class PSAScheduleService : IPSAScheduleService
    {
        static Logger logger = LogManager.GetCurrentClassLogger();
        
        const string SP_PSA_LIST_NAME = "PSA";
        const string SP_PROFESSIONAL_LIST_NAME = "Professional Master";

        string _siteUrl = null;

        public PSAScheduleService()
        {
            _updatedTaskCandidates = new Dictionary<int, TaskManager>();
            _baseLineTotal = new Dictionary<DateTime, int>();
            _planTotal = new Dictionary<DateTime, int>();
            _actualTotal = new Dictionary<DateTime, int>();
        }

        Dictionary<int, TaskManager> _updatedTaskCandidates;
        Dictionary<DateTime, int> _baseLineTotal;
        Dictionary<DateTime, int> _planTotal;
        Dictionary<DateTime, int> _actualTotal;

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
        }


        public bool CheckTwoMonthsBeforeExpireDate()
        {
            string status = "Active";

            var caml = @"<View>  
            <Query> 
               <Where><Eq><FieldRef Name='psastatus' /><Value Type='Text'>" + status + @"</Value></Eq></Where> 
            </Query> 
      </View>";

            foreach (var psaData in SPConnector.GetList(SP_PSA_LIST_NAME, _siteUrl, caml))
            {
                DateTime expireDate = Convert.ToDateTime(psaData["psaexpirydate"]).ToLocalTime();
                DateTime twoMonthsBeforeExpired = expireDate.AddMonths(-2);

                string strExpireDate = Convert.ToDateTime(psaData["psaexpirydate"]).ToLocalTime().ToShortDateString();
                string strTwoMonthBeforeExpired = twoMonthsBeforeExpired.ToLocalTime().ToShortDateString();

                logger.Info("expireDate: " + expireDate + "--" + "twoMonthsBeforeExpired: " + twoMonthsBeforeExpired + "--" + "strExpireDate: " + strExpireDate + "--" + "strTwoMonthBeforeExpired: " + strTwoMonthBeforeExpired);

                DateTime today = DateTime.Now;
                string strToday = today.ToLocalTime().ToShortDateString();

                string psaNumber = Convert.ToString(psaData["Title"]);

                int? professionalID = FormatUtil.ConvertLookupToID(psaData, "professional");//GetProfessionalID(psaNumber);

                if (professionalID==null)continue;
                var professionalData = SPConnector.GetListItem(SP_PROFESSIONAL_LIST_NAME, professionalID, _siteUrl);
                string professionalMail = Convert.ToString(professionalData["officeemail"]);
                string professionalFullName = Convert.ToString(professionalData["Title"]) + " " + Convert.ToString(professionalData["lastname"]);

                if(strToday == strTwoMonthBeforeExpired)
                {
                    string mailSubject = "Notification of PSA Expired";
                    string mailContent = string.Format("Dear Mr./Mrs. {0}. This email is sent to you to notify that your PSA will be expired in the next two months. Please kindly communicate to HR dept. for any further action.", professionalFullName);

                    SendMailTwoMonthBeforeExpired(professionalMail, mailSubject, mailContent);
                }
                
            }

            return true;
        }

        public void changePSAstatus()
        {
            var caml = "";
            foreach (var psaData in SPConnector.GetList(SP_PSA_LIST_NAME, _siteUrl, caml))
            {
                int id = Convert.ToInt32(psaData["ID"]);
                DateTime expireDate = Convert.ToDateTime(psaData["psaexpirydate"]).ToLocalTime();
                DateTime newpsadate = Convert.ToDateTime(psaData["dateofnewpsa"]).ToLocalTime();
                string strStatus = Convert.ToString(psaData["psastatus"]);
                DateTime dateToday = DateTime.Now.ToLocalTime();
                var columnValues = new Dictionary<string, object>();
                if (dateToday < newpsadate || dateToday > expireDate)
                {
                    columnValues.Add("psastatus", "Non Active");
                    SPConnector.UpdateListItemNoVersionConflict(SP_PSA_LIST_NAME, id, columnValues, _siteUrl);
                }
                else if (dateToday >= newpsadate && dateToday < expireDate)
                {
                    if (strStatus != "Active")
                    {
                        columnValues.Add("psastatus", "Active");
                        SPConnector.UpdateListItemNoVersionConflict(SP_PSA_LIST_NAME, id, columnValues, _siteUrl);
                    }
                   
                }

            }
        }

        public int GetProfessionalID(string psaNumber)
        {
            var caml = @"<View>  
            <Query> 
               <Where><Eq><FieldRef Name='PSAnumber' /><Value Type='Text'>" + psaNumber + @"</Value></Eq></Where> 
            </Query> 
      </View>";

            int professionalID = 0;

            foreach(var item in SPConnector.GetList(SP_PROFESSIONAL_LIST_NAME, _siteUrl, caml))
            {
                if(Convert.ToInt32(item["ID"]) != 0)
                {
                    professionalID = Convert.ToInt32(item["ID"]);
                    break;
                }
            }

            return professionalID;
        }

        public void SendMailTwoMonthBeforeExpired(string professionalMail, string mailSubject, string mailContent)
        {
            EmailUtil.Send(professionalMail, mailSubject, mailContent);
        }

        //public bool UpdateStatusPSA()
        //{
        //    int psaID = SPConnector.GetLatestListItemID(SP_PSA_LIST_NAME, _siteUrl);

        //    UpdatePSAStatus(psaID);
        //    GetPSAData(psaID);
            

        //    var psaData = SPConnector.GetListItem(SP_PSA_LIST_NAME, psaID, _siteUrl);
        //    DateTime datePSAStart = Convert.ToDateTime(psaData["dateofnewpsa"]).ToLocalTime();
        //    DateTime lastWorkingDate = Convert.ToDateTime(psaData["lastworkingdate"]).ToLocalTime();
        //    DateTime today = DateTime.Now;

            
        //}

        //public bool UpdatePSAStatus(int? psaID)
        //{
        //    var updateValues = new Dictionary<string, object>();

        //    string psaStatus = "Active";

        //    updateValues.Add("psastatus", psaStatus);
            
        //    try
        //    {
        //        SPConnector.UpdateListItem(SP_PSA_LIST_NAME, psaID, updateValues, _siteUrl);
        //    }
        //    catch (Exception e)
        //    {
        //        logger.Debug(e.Message);
        //        return false;
        //    }

        //    return true;
        //}

        //public bool GetPSAData(int? psaID)
        //{
        //    var psaData = SPConnector.GetListItem(SP_PSA_LIST_NAME, psaID, _siteUrl);

        //    string professionalName = Convert.ToString(psaData["Title"]);
        //}
    }
}
