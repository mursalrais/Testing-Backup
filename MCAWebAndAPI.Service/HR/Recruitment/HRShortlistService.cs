using System;
using System.Collections.Generic;
using System.Web;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.Utils;
using NLog;
using Microsoft.SharePoint.Client;
using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.HR.DataMaster;

namespace MCAWebAndAPI.Service.HR.Recruitment
{
    public class HRShortlistService : IHRShortlistService
    {
        string _siteUrl;
        static Logger logger = LogManager.GetCurrentClassLogger();

        const string SP_APPDATA_LIST_NAME = "Application";
        const string SP_APPINTV_LIST_NAME = "Application Interview";
        const string SP_APPINVPANEL_LIST_NAME = "Interview Invitation to Panel";
        const string SP_APPEDU_LIST_NAME = "Application Education";
        const string SP_APPWORK_LIST_NAME = "Application Working Experience";
        const string SP_APPTRAIN_LIST_NAME = "Application Training";
        const string SP_APPDOC_LIST_NAME = "Application Documents";

        const string SP_PROMAS_LIST_NAME = "Professional Master";
        const string SP_POSMAS_LIST_NAME = "Position Master";

        const string SP_MANPOW_LIST_NAME = "Manpower Requisition";

        public string GetAccessData(string userLoginName = null)
        {
            if (userLoginName == null)
                return null;

            var caml = @"<View>  
                    <Query> 
                       <Where><Eq><FieldRef Name='hiaccountname' /><Value Type='Text'>" + userLoginName + @"</Value></Eq></Where> 
                    </Query> 
                     <ViewFields><FieldRef Name='Position' /><FieldRef Name='ID' /></ViewFields> 
                    </View>";

            var useraccess = "";
            foreach (var item in SPConnector.GetList(SP_PROMAS_LIST_NAME, _siteUrl, caml))
            {
                useraccess = Convert.ToString(item["ID"]);
            }

            return useraccess;
        }

        private ApplicationShortlistVM ConvertToShortlistDataVM(ListItem listItem)
        {
            var viewModel = new ApplicationShortlistVM();

            viewModel.ID = Convert.ToInt32(listItem["ID"]);
            viewModel.Position = FormatUtil.ConvertLookupToID(listItem, "vacantposition") + string.Empty;
            viewModel.Candidate = Convert.ToString(listItem["Title"]);
            viewModel.SendTo = Convert.ToString(listItem[""]);

            return viewModel;
        }

        private ApplicationShortlistVM ConvertToSendIntvDataVM(ListItem listItem)
        {
            var viewModel = new ApplicationShortlistVM();

            viewModel.Position = Convert.ToString(listItem["vacantposition"]);
            viewModel.Candidate = Convert.ToString(listItem["Title"]);
            viewModel.SendTo = Convert.ToString(listItem["personalemail"]);

            return viewModel;
        }

        private ApplicationShortlistVM ConvertToInviteIntvDataVM(ListItem listItem)
        {
            var viewModel = new ApplicationShortlistVM();
            
            viewModel.InterviewerDate = Convert.ToDateTime(listItem["interviewdatetime"]);

            viewModel.EmailMessage = Convert.ToString(listItem["EmailMessage"]);

            return viewModel;
        }

        public ApplicationShortlistVM GetShortlist(string position, string username, string useraccess)
        {
            var viewModel = new ApplicationShortlistVM();
            if (position == null)
            return viewModel;

            if (username != null)
            useraccess = GetAccessData(username);

            viewModel.ShortlistDetails = GetDetailShortlist(position, useraccess);

            return viewModel;

        }

        public ApplicationShortlistVM GetShortlistSend(int? ID)
        {
            var viewModel = new ApplicationShortlistVM();
            if (ID == null)
                return viewModel;

            var listItemdt = SPConnector.GetListItem(SP_APPDATA_LIST_NAME, ID, _siteUrl);
            viewModel = ConvertToSendIntvDataVM(listItemdt);

            return viewModel;

        }

        //<ViewFields>
        //   <FieldRef Name = 'Title' />
        //   < FieldRef Name='applications' />
        //   <FieldRef Name = 'university' />
        //   < FieldRef Name='yearofgraduation' />
        //   <FieldRef Name = 'remarks' />
        //</ ViewFields >
        private IEnumerable<ShortlistDetailVM> GetDetailShortlist(string Position, string useraccess)
        {
            var caml = "";
            if (useraccess == "HR")
            {
                caml = @"<View>  
            <Query> 
      <Where>
    <And>
      <Or>
         <Or>
            <Or>
               <Or>
                  <Or>
                     <Eq>
                        <FieldRef Name='applicationstatus' />
                        <Value Type='Text'>New</Value>
                     </Eq>
                     <Eq>
                        <FieldRef Name='applicationstatus' />
                        <Value Type='Text'>Shortlisted</Value>
                     </Eq>
                  </Or>
                  <Eq>
                     <FieldRef Name='applicationstatus' />
                     <Value Type='Text'>Declined</Value>
                  </Eq>
               </Or>
               <Eq>
                  <FieldRef Name='applicationstatus' />
                  <Value Type='Text'>NEW</Value>
               </Eq>
            </Or>
            <Eq>
               <FieldRef Name='applicationstatus' />
               <Value Type='Text'>SHORTLISTED</Value>
            </Eq>
         </Or>
         <Eq>
            <FieldRef Name='applicationstatus' />
            <Value Type='Text'>DECLINED</Value>
         </Eq>
      </Or>
         <Eq>
            <FieldRef Name='position' LookupId='True' /><Value Type='Lookup'>" + Position + @"</Value>
         </Eq>
      </And>
   </Where>
            </Query> 
              <ViewFields>
      <FieldRef Name='Title' />
      <FieldRef Name='ID' />
      <FieldRef Name='applicationstatus' />
      <FieldRef Name='position' />
   </ViewFields>
   
            </View>";

            }
            else if(useraccess == "REQ")
            {
                caml = @"<View>  
            <Query> 
      <Where>
       <Eq>
         <FieldRef Name='applicationstatus' />
         <Value Type='Text'>Shortlisted</Value>
      </Eq>
   </Where>
            </Query> 
              <ViewFields>
      <FieldRef Name='Title' />
      <FieldRef Name='ID' />
      <FieldRef Name='applicationstatus' />
      <FieldRef Name='position' />
   </ViewFields>
            </View>";

            }
            
            var shortlistDetails = new List<ShortlistDetailVM>();
            foreach (var item in SPConnector.GetList(SP_APPDATA_LIST_NAME, _siteUrl, caml))
            {
                shortlistDetails.Add(ConvertToShortlistDetailVM(item));
            }

            return shortlistDetails;
        }

        //<ViewFields>
        //   <FieldRef Name = 'Title' />
        //   < FieldRef Name='applications' />
        //   <FieldRef Name = 'university' />
        //   < FieldRef Name='yearofgraduation' />
        //   <FieldRef Name = 'remarks' />
        //</ ViewFields >
        private IEnumerable<ShortlistDetailVM> GetDetailShortlisted(string Position = null)
        {
            var caml = @"<View>  
            <Query> 
      <Where>
      <Or>
         <Or>
            <Or>
               <Or>
                  <Or>
                     <Eq>
                        <FieldRef Name='applicationstatus' />
                        <Value Type='Text'>New</Value>
                     </Eq>
                     <Eq>
                        <FieldRef Name='applicationstatus' />
                        <Value Type='Text'>Shortlisted</Value>
                     </Eq>
                  </Or>
                  <Eq>
                     <FieldRef Name='applicationstatus' />
                     <Value Type='Text'>Declined</Value>
                  </Eq>
               </Or>
               <Eq>
                  <FieldRef Name='applicationstatus' />
                  <Value Type='Text'>NEW</Value>
               </Eq>
            </Or>
            <Eq>
               <FieldRef Name='applicationstatus' />
               <Value Type='Text'>SHORTLISTED</Value>
            </Eq>
         </Or>
         <Eq>
            <FieldRef Name='applicationstatus' />
            <Value Type='Text'>DECLINED</Value>
         </Eq>
      </Or>
   </Where>
            </Query> 
              <ViewFields>
      <FieldRef Name='Title' />
      <FieldRef Name='ID' />
      <FieldRef Name='applicationstatus' />
      <FieldRef Name='position' />
   </ViewFields>
   
            </View>";

            var eduacationDetails = new List<ShortlistDetailVM>();
            foreach (var item in SPConnector.GetList(SP_APPDATA_LIST_NAME, _siteUrl, caml))
            {
                eduacationDetails.Add(ConvertToShortlistDetailVM(item));
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
        private ShortlistDetailVM ConvertToShortlistDetailVM(ListItem item)
        {
            int id = Convert.ToInt32(item["ID"]);
            return new ShortlistDetailVM
            {
                Candidate = Convert.ToString(item["Title"]),
                Candidatemail = Convert.ToString(item["Title"]),
                ID = Convert.ToInt32(item["ID"]),
                DocumentUrl = GetDocumentUrl(id),
                GetStat = Convert.ToString(item["applicationstatus"]),
                //Status = Convert.ToString(item["applicationstatus"]),
                Remarks = Convert.ToString(item["position"]),
                //Title = Convert.ToString(item["Title"])

            };
        }

        public void GetVacantPositions(PositionsMaster viewModel)
        {
            var updatedValue = new Dictionary<string, object>();
            updatedValue.Add("applicationstatus", viewModel.PositionName);

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

        private string GetDocumentUrl(int? iD)
        {
            return string.Format(UrlResource.ApplicationDocumentByID, _siteUrl, iD);
        }

        public IEnumerable<ApplicationShortlistVM> GetShortlists()
        {
            throw new NotImplementedException();
        }

        public void SetSiteUrl(string siteUrl = null)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
        }

        public void CreateShortlistDataDetail(int? headerID, IEnumerable<ShortlistDetailVM> viewModels)
        {
            foreach (var viewModel in viewModels)
            {
                if (Item.CheckIfSkipped(viewModel))
                    continue;

                if (Item.CheckIfDeleted(viewModel))
                {
                    try
                    {
                        SPConnector.DeleteListItem(SP_APPDATA_LIST_NAME, viewModel.ID, _siteUrl);

                    }
                    catch (Exception e)
                    {
                        logger.Error(e);
                        throw e;
                    }
                    continue;
                }

                var updatedValue = new Dictionary<string, object>();
                updatedValue.Add("Title", viewModel.Candidate);
                updatedValue.Add("applicationstatus", viewModel.GetStat);

                try
                {
                   SPConnector.UpdateListItem(SP_APPDATA_LIST_NAME, viewModel.ID, updatedValue, _siteUrl);
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw e;
                }
                
            }
        }

        public void CreateShortlistInviteIntv(int? headerID, ApplicationShortlistVM viewModel)
        {
            var updatedValue = new Dictionary<string, object>();

            foreach (var list in viewModel.ShortlistDetails)
            {
                updatedValue.Add("Title", list.Candidate);
                updatedValue.Add("emailfrom", list.Candidatemail);
                updatedValue.Add("emailto", viewModel.SendTo);
                updatedValue.Add("emailmessage", viewModel.EmailMessage);
                updatedValue.Add("emaildate", DateTime.Now);

                try
                {
                    SPConnector.AddListItem(SP_APPINVPANEL_LIST_NAME, updatedValue, _siteUrl);
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw e;
                }

                if (viewModel.SendToCandidate == true)
                {
                    SPConnector.SendEmail(viewModel.EmailFrom, viewModel.EmailMessage, "Interview Invitation", _siteUrl);
                }
                
            }
             SPConnector.SendEmail(viewModel.SendTo, viewModel.EmailMessage, "Interview Invitation", _siteUrl);
        }

        public void CreateShorlistSendintv(int? headerID, ApplicationShortlistVM viewModel)
        {
            var updatedValue = new Dictionary<string, object>();

            updatedValue.Add("interviewdatetime", viewModel.InterviewerDate);
            //updatedValue.Add("interviewerpanel", viewModel.InterviewerPanel);
            updatedValue.Add("invitationemailmessage", viewModel.Message);
            updatedValue.Add("invitationemaildate", DateTime.Now);

            try
            {
                SPConnector.AddListItem(SP_APPINTV_LIST_NAME, updatedValue, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                throw e;
            }

            SPConnector.SendEmail(viewModel.EmailFrom, viewModel.EmailMessage, "Interview Invitation", _siteUrl);
        }
    }
}
