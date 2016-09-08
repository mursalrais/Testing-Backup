using System;
using System.Linq;
using MCAWebAndAPI.Model.ProjectManagement.Schedule;
using MCAWebAndAPI.Service.Utils;
using System.Collections.Generic;
using MCAWebAndAPI.Model.ViewModel.Chart;
using Microsoft.SharePoint.Client;
using NLog;
using MCAWebAndAPI;


namespace MCAWebAndAPI.Service.ProjectManagement.Schedule
{
    public class PSAScheduleService : IPSAScheduleService
    {
        static Logger logger = LogManager.GetCurrentClassLogger();
        
        const string SP_PSA_LIST_NAME = "PSA";
        const string SP_PROFESSIONAL_LIST_NAME = "Professional Master";

        string _siteUrl = null;

        
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
                DateTime expireDate = Convert.ToDateTime(psaData["psaexpirydates"]).ToLocalTime();
                DateTime twoMonthsBeforeExpired = expireDate.AddMonths(-2);

                string strExpireDate = Convert.ToDateTime(psaData["psaexpirydates"]).ToLocalTime().ToShortDateString();
                string strTwoMonthBeforeExpired = twoMonthsBeforeExpired.ToLocalTime().ToShortDateString();

                logger.Info("expireDate: " + expireDate + "--" + "twoMonthsBeforeExpired: " + twoMonthsBeforeExpired + "--" + "strExpireDate: " + strExpireDate + "--" + "strTwoMonthBeforeExpired: " + strTwoMonthBeforeExpired);

                DateTime today = DateTime.Now;
                string strToday = today.ToLocalTime().ToShortDateString();

                string psaNumber = Convert.ToString(psaData["Title"]);

                int? professionalID = FormatUtil.ConvertLookupToID(psaData, "professional");//GetProfessionalID(psaNumber);

                if (professionalID != Convert.ToInt32(null))
                {
                    var professionalData = SPConnector.GetListItem(SP_PROFESSIONAL_LIST_NAME, professionalID, _siteUrl);

                    if(professionalData != null)
                    {
                        string professionalMail = Convert.ToString(professionalData["officeemail"]);
                        string professionalFullName = Convert.ToString(professionalData["Title"]);

                        if (strToday == strTwoMonthBeforeExpired)
                        {
                            string mailSubject = "Notification of PSA Expired";
                            string mailContent = string.Format("Dear Mr./Mrs. {0}, {1}{2}This email is sent to you to notify that your PSA will be expired in the next two months. Please kindly communicate to HR dept. for any further action.{3}{4}Thank You", professionalFullName, Environment.NewLine, Environment.NewLine, Environment.NewLine, Environment.NewLine);
                            
                            SendMailTwoMonthBeforeExpired(professionalMail, mailSubject, mailContent);
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    continue;
                }
            }

            return true;
        }

        public void changePSAstatus()
        {
            var camlPSAInactive = "";

            foreach (var psaData in SPConnector.GetList(SP_PSA_LIST_NAME, _siteUrl, camlPSAInactive))
            {
                int id = Convert.ToInt32(psaData["ID"]);
                DateTime expireDate = Convert.ToDateTime(psaData["lastworkingdate"]).ToLocalTime();
                DateTime newpsadate = Convert.ToDateTime(psaData["dateofnewpsa"]).ToLocalTime();
                string strStatus = Convert.ToString(psaData["psastatus"]);
                DateTime dateToday = DateTime.Now.ToLocalTime();
                
                if (dateToday < newpsadate || dateToday > expireDate)
                {
                    UpdatePSAStatus("Inactive", id);

                    int? professionalID = FormatUtil.ConvertLookupToID(psaData, "professional");//GetProfessionalID(psaNumber);

                    if(professionalID != null)
                    {
                        var professionalData = SPConnector.GetListItem(SP_PROFESSIONAL_LIST_NAME, professionalID, _siteUrl);

                        if(professionalData != null)
                        {
                            string professionalFullName = Convert.ToString(professionalData["Title"]);

                            int professionalPSA = checkProfessionalPSA(professionalFullName);

                            if (id == professionalPSA)
                            {
                                var latestProfessionalPSA = SPConnector.GetListItem(SP_PSA_LIST_NAME, professionalPSA, _siteUrl);

                                int? profID = FormatUtil.ConvertLookupToID(latestProfessionalPSA, "professional");//GetProfessionalID(psaNumber);
                                string psaNumber = Convert.ToString(latestProfessionalPSA["Title"]);
                                DateTime professionalJoinDate = Convert.ToDateTime(latestProfessionalPSA["joindate"]);
                                DateTime professionalStartPSA = Convert.ToDateTime(latestProfessionalPSA["dateofnewpsa"]);
                                DateTime professionalEndPSA = Convert.ToDateTime(latestProfessionalPSA["psaexpirydates"]);
                                DateTime professionalLastDate = Convert.ToDateTime(latestProfessionalPSA["lastworkingdate"]);
                                string psaStatus = Convert.ToString(latestProfessionalPSA["psastatus"]);

                                UpdateProfessionalData(profID, psaNumber, professionalJoinDate, professionalStartPSA, professionalEndPSA, professionalLastDate, psaStatus);
                            }
                            else if(id < professionalPSA)
                            {
                                var currentProfessionalPSA = SPConnector.GetListItem(SP_PSA_LIST_NAME, id, _siteUrl);
                                var latestProfessionalPSA = SPConnector.GetListItem(SP_PSA_LIST_NAME, professionalPSA, _siteUrl);

                                DateTime latestDateOfNewPSA = Convert.ToDateTime(latestProfessionalPSA["dateofnewpsa"]).ToLocalTime();
                                DateTime latestExpiryDate = Convert.ToDateTime(latestProfessionalPSA["lastworkingdate"]).ToLocalTime();

                                if ((dateToday <= latestDateOfNewPSA) && (dateToday >= latestExpiryDate))
                                {
                                    UpdatePSAStatus("Active", professionalPSA);
                                    
                                    int? profID = FormatUtil.ConvertLookupToID(latestProfessionalPSA, "professional");//GetProfessionalID(psaNumber);
                                    string psaNumber = Convert.ToString(latestProfessionalPSA["Title"]);
                                    DateTime professionalJoinDate = Convert.ToDateTime(latestProfessionalPSA["joindate"]);
                                    DateTime professionalStartPSA = Convert.ToDateTime(latestProfessionalPSA["dateofnewpsa"]);
                                    DateTime professionalEndPSA = Convert.ToDateTime(latestProfessionalPSA["psaexpirydates"]);
                                    DateTime professionalLastDate = Convert.ToDateTime(latestProfessionalPSA["lastworkingdate"]);
                                    string psaStatus = Convert.ToString(latestProfessionalPSA["psastatus"]);

                                    UpdateProfessionalData(profID, psaNumber, professionalJoinDate, professionalStartPSA, professionalEndPSA, professionalLastDate, psaStatus);
                                }
                                if((dateToday < latestDateOfNewPSA) && (dateToday > latestExpiryDate))
                                {
                                    UpdatePSAStatus("Inactive", professionalPSA);

                                    int? profID = FormatUtil.ConvertLookupToID(latestProfessionalPSA, "professional");//GetProfessionalID(psaNumber);
                                    string psaNumber = Convert.ToString(latestProfessionalPSA["Title"]);
                                    DateTime professionalJoinDate = Convert.ToDateTime(latestProfessionalPSA["joindate"]);
                                    DateTime professionalStartPSA = Convert.ToDateTime(latestProfessionalPSA["dateofnewpsa"]);
                                    DateTime professionalEndPSA = Convert.ToDateTime(latestProfessionalPSA["psaexpirydates"]);
                                    DateTime professionalLastDate = Convert.ToDateTime(latestProfessionalPSA["lastworkingdate"]);
                                    string psaStatus = Convert.ToString(latestProfessionalPSA["psastatus"]);

                                    UpdateProfessionalData(profID, psaNumber, professionalJoinDate, professionalStartPSA, professionalEndPSA, professionalLastDate, psaStatus);

                                }
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                else if ((dateToday >= newpsadate) && (dateToday <= expireDate))
                {
                    int? professionalID = FormatUtil.ConvertLookupToID(psaData, "professional");//GetProfessionalID(psaNumber);

                    if(professionalID != null)
                    {
                        var professionalData = SPConnector.GetListItem(SP_PROFESSIONAL_LIST_NAME, professionalID, _siteUrl);

                        if(professionalData != null)
                        {
                            string professionalFullName = Convert.ToString(professionalData["Title"]);
                            int professionalPSA = checkProfessionalPSA(professionalFullName);

                            if (id < professionalPSA)
                            {
                                UpdatePSAStatus("Active", id);
                                UpdatePSAStatus("Inactive", professionalPSA);

                                ComparePSAID(professionalFullName, id);

                                var latestProfessionalPSA = SPConnector.GetListItem(SP_PSA_LIST_NAME, professionalPSA, _siteUrl);
                                var currentProfessionalPSA = SPConnector.GetListItem(SP_PSA_LIST_NAME, id, _siteUrl);

                                DateTime latestPSAExpiryDate = Convert.ToDateTime(latestProfessionalPSA["lastworkingdate"]).ToLocalTime();
                                DateTime lastestPSANewPSA = Convert.ToDateTime(latestProfessionalPSA["dateofnewpsa"]).ToLocalTime();

                                DateTime currentPSAExpiryDate = Convert.ToDateTime(currentProfessionalPSA["lastworkingdate"]).ToLocalTime();
                                DateTime currentPSANewPSA = Convert.ToDateTime(currentProfessionalPSA["dateofnewpsa"]).ToLocalTime();

                                int? profID = FormatUtil.ConvertLookupToID(currentProfessionalPSA, "professional");//GetProfessionalID(psaNumber);
                                string psaNumber = Convert.ToString(currentProfessionalPSA["Title"]);
                                DateTime professionalJoinDate = Convert.ToDateTime(currentProfessionalPSA["joindate"]);
                                DateTime professionalStartPSA = Convert.ToDateTime(currentProfessionalPSA["dateofnewpsa"]);
                                DateTime professionalEndPSA = Convert.ToDateTime(currentProfessionalPSA["psaexpirydates"]);
                                DateTime professionalLastDate = Convert.ToDateTime(currentProfessionalPSA["lastworkingdate"]);
                                string psaStatus = Convert.ToString(currentProfessionalPSA["psastatus"]);

                                UpdateProfessionalData(profID, psaNumber, professionalJoinDate, professionalStartPSA, professionalEndPSA, professionalLastDate, psaStatus);
                            }
                            if (id == professionalPSA)
                            {
                                UpdatePSAStatus("Active", professionalPSA);

                                ComparePSAID(professionalFullName, id);

                                var latestProfessionalPSA = SPConnector.GetListItem(SP_PSA_LIST_NAME, professionalPSA, _siteUrl);

                                DateTime latestPSAExpiryDate = Convert.ToDateTime(psaData["lastworkingdate"]).ToLocalTime();
                                DateTime lastestPSANewPSA = Convert.ToDateTime(psaData["dateofnewpsa"]).ToLocalTime();

                                int? profID = FormatUtil.ConvertLookupToID(latestProfessionalPSA, "professional");//GetProfessionalID(psaNumber);
                                string psaNumber = Convert.ToString(latestProfessionalPSA["Title"]);
                                DateTime professionalJoinDate = Convert.ToDateTime(latestProfessionalPSA["joindate"]);
                                DateTime professionalStartPSA = Convert.ToDateTime(latestProfessionalPSA["dateofnewpsa"]);
                                DateTime professionalEndPSA = Convert.ToDateTime(latestProfessionalPSA["psaexpirydates"]);
                                DateTime professionalLastDate = Convert.ToDateTime(latestProfessionalPSA["lastworkingdate"]);
                                string psaStatus = Convert.ToString(latestProfessionalPSA["psastatus"]);

                                UpdateProfessionalData(profID, psaNumber, professionalJoinDate, professionalStartPSA, professionalEndPSA, professionalLastDate, psaStatus);
                            }
                        }
                    }
                }
            }
        }

        private void UpdatePSAStatus(string psaStatus, int id)
        {
            var columnValues = new Dictionary<string, object>();
            
            columnValues.Add("psastatus", psaStatus);
            
            try
            {
                SPConnector.UpdateListItem(SP_PSA_LIST_NAME, id, columnValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
            }
        }

        private void ComparePSAID (string professionalFullName, int id)
        {
            var camlPSARecord = @"<View>  
            <Query> 
               <Where><Eq><FieldRef Name='professional' /><Value Type='Lookup'>" + professionalFullName + @"</Value></Eq></Where> 
            </Query> 
      </View>";

            foreach(var psaData in SPConnector.GetList(SP_PSA_LIST_NAME, _siteUrl, camlPSARecord))
            {
                int psaID = Convert.ToInt32(psaData["ID"]);

                if(psaID != id)
                {
                    UpdatePSAStatus("Inactive", psaID);
                }
            }
        }

        private int checkProfessionalPSA(string professionalFullName)
        {
            var camlProfessionalPSA = @"<View>  
            <Query> 
               <Where><Eq><FieldRef Name='professionalfullname' /><Value Type='Text'>" + professionalFullName + @"</Value></Eq></Where> 
            </Query> 
      </View>";

            var listProfessionalPSAID = new List<int>();

            foreach(var professionalPSA in SPConnector.GetList(SP_PSA_LIST_NAME, _siteUrl, camlProfessionalPSA))
            {
                int psaID = Convert.ToInt32(professionalPSA["ID"]);
                listProfessionalPSAID.Add(psaID);
            }

            return listProfessionalPSAID.LastOrDefault();
        }

        private bool UpdateProfessionalData(int? professionalID, string psaNumber, DateTime professionalJoinDate, DateTime professionalStartPSA, DateTime professionalEndPSA, DateTime professionalLastDate, string psaStatus)
        {
            var columnValues = new Dictionary<string, object>();

            columnValues.Add("Join_x0020_Date", professionalJoinDate);
            columnValues.Add("PSAnumber", psaNumber);
            columnValues.Add("PSAstartdate", professionalStartPSA);
            columnValues.Add("PSAexpirydate", professionalEndPSA);
            columnValues.Add("lastworkingdate", professionalLastDate);
            columnValues.Add("psastatus", psaStatus);

            try
            {
                SPConnector.UpdateListItemNoVersionConflict(SP_PSA_LIST_NAME, professionalID, columnValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
            }
            
            return true;
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

        private string GetProfessionalHR(string hrUnit, string hrPosition)
        {
            string professionalMail = "";

            var camlProfessionalHR = @"<View>  
            <Query> 
               <Where><And><Eq><FieldRef Name='Project_x002f_Unit' /><Value Type='Choice'>" + hrUnit + @"</Value></Eq><Eq><FieldRef Name='Position' /><Value Type='Lookup'>" + hrPosition + @"</Value></Eq></And></Where> 
            </Query> 
      </View>";

            foreach(var professionalData in SPConnector.GetList(SP_PROFESSIONAL_LIST_NAME, _siteUrl, camlProfessionalHR))
            {
                professionalMail = Convert.ToString(professionalData["officeemail"]);
                break;
            }

            return professionalMail; 
        }

        private void SendMailChangeStatus(string professionalMail, string mailSubject, string mailContent)
        {
            EmailUtil.Send(professionalMail, mailSubject, mailContent);
        }
    }
}
