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
using System.Linq;

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

            viewModel.ActivePosition.Value = FormatUtil.ConvertLookupToID(listItem, "manpowerrequisition") ;

            return viewModel;
        }

        private ApplicationShortlistVM ConvertToSendIntvDataVM(ListItem listItem)
        {
            var viewModel = new ApplicationShortlistVM();

            viewModel.Position = FormatUtil.ConvertLookupToID(listItem, "vacantposition") + string.Empty;
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

        public ApplicationShortlistVM GetShortlist( int? position, string username, string useraccess)
        {
            var viewModel = new ApplicationShortlistVM();

            if (position == null)
                return null;

            var caml = @"<View>  
                    <Query> 
                       <Where>
                             <Eq>
                                <FieldRef Name='positionrequested_x003a_ID' />
                                <Value Type='Lookup'>" + position +@"</Value>
                             </Eq>
                       </Where>
                    </Query> 
                     <ViewFields><FieldRef Name='ID' /></ViewFields> 
                    </View>";

            var manpowerID = 0;
            foreach (var item in SPConnector.GetList(SP_MANPOW_LIST_NAME, _siteUrl, caml))
            {
                manpowerID = Convert.ToInt32(item["ID"]);
            }

            return GetShortlist(manpowerID, username, useraccess, position);
        }

        public ApplicationShortlistVM GetShortlist(int ID, string username, string useraccess, int? position)
        {
            var viewModel = new ApplicationShortlistVM();
            if (ID == 0)
            return viewModel;
            
            viewModel.ShortlistDetails = GetDetailShortlist(ID, useraccess);
            viewModel.ActivePosition.Value = Convert.ToInt32(position);
            viewModel.Position = Convert.ToString(position);
            viewModel.useraccess = Convert.ToString(useraccess);

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
        private IEnumerable<ShortlistDetailVM> GetDetailShortlist(int manPosition, string useraccess)
         {
            var caml = "";
            if (useraccess == "HR")
            {
                caml = @"<View>  
          <Query> 
      <Where>
           <And>
         <Eq>
            <FieldRef Name='manpowerrequisition' />
            <Value Type='Lookup'>"+ manPosition + @"</Value>
         </Eq>
            <Or>
               <Eq>
                  <FieldRef Name='applicationstatus' />
                  <Value Type='Text'>New</Value>
               </Eq>
               <Eq>
                  <FieldRef Name='applicationstatus' />
                  <Value Type='Text'>NEW</Value>
               </Eq>
         </Or>
      </And>
   </Where>
            </Query> 
              <ViewFields>
      <FieldRef Name='Title' />
      <FieldRef Name='ID' />
      <FieldRef Name='applicationstatus' />
      <FieldRef Name='applicationremarks' />
      <FieldRef Name='position' />
      <FieldRef Name='personalemail' />
   </ViewFields>
   
            </View>";

            }
            else if(useraccess == "REQ")
            {
                caml = @"<View>  
            <Query> 
      <Where>
          <And>
             <Eq>
                <FieldRef Name='manpowerrequisition' />
                <Value Type='Lookup'>" + manPosition + @"</Value>
             </Eq>
                <Eq>
                   <FieldRef Name='applicationstatus' />
                   <Value Type='Text'>Shortlisted</Value>
                </Eq>
          </And>
   </Where>
            </Query> 
              <ViewFields>
      <FieldRef Name='Title' />
      <FieldRef Name='ID' />
      <FieldRef Name='applicationstatus' />
      <FieldRef Name='applicationremarks' />
      <FieldRef Name='position' />
      <FieldRef Name='personalemail' />
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
            return new ShortlistDetailVM
            {
                Candidate = Convert.ToString(item["Title"]),
                Candidatemail = Convert.ToString(item["personalemail"]),
                ID = Convert.ToInt32(item["ID"]),
                DocumentUrl = GetDocumentUrl(Convert.ToInt32(item["ID"])),
                Status = ShortlistDetailVM.GetStatusDefaultValue(
                    new Model.ViewModel.Control.AjaxComboBoxVM
                    {
                        Text = Convert.ToString(item["applicationstatus"]),
                    }),

                GetStat = Convert.ToString(item["applicationstatus"]),
                Remarks = Convert.ToString(item["applicationremarks"]),
                

            };
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

        public void UpdateShortlistDataDetail(int? headerID, IEnumerable<ShortlistDetailVM> viewModels)
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
                updatedValue.Add("applicationstatus", viewModel.Status.Text);
                updatedValue.Add("applicationremarks", viewModel.Remarks);

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

        public void CreateShortlistInviteIntv(int? headerID, ApplicationShortlistVM viewModel, string mailsubject)
        {
            foreach (var list in viewModel.ShortlistDetails)
            {
                var updatedValue = new Dictionary<string, object>();
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
                EmailUtil.Send(list.Candidatemail, "Interview Invitation", viewModel.EmailMessage + "   " + "https://eceos2.sharepoint.com/sites/ims/hr/Lists/Professional%20Master/DispForm_Custom.aspx?ID="+ viewModel.ID +"" + mailsubject);
            }

            char[] delimiterChars = { ' ', ',', ';' };

            string[] words = viewModel.InterviewerPanel.Split(delimiterChars);

            foreach (string mail in words)
            {
                EmailUtil.Send(mail, "Interview Invitation", "<div><label>" + viewModel.EmailMessage + "</label></div>" + mailsubject);
            }
        }

        public void CreateShorlistSendintv(int? headerID, ApplicationShortlistVM viewModel)
        {
            var updatedValue = new Dictionary<string, object>();

            updatedValue.Add("interviewdatetime", viewModel.InterviewerDate);
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

            SendEmailValidation(viewModel.SendTo, viewModel.EmailMessage);

        }

        public void SendEmailValidation(string emailTo, string emailMessages)
        {
            try
            {
                EmailUtil.Send(emailTo, "Shortlist Candidate Data", emailMessages);
            }
            catch (Exception e)
            {
                logger.Error(e);
                throw e;
            }
        }

        public IEnumerable<PositionMaster> GetPositions()
        {
            var caml = @"<View>  
                    <Query> 
                       <Where><Eq><FieldRef Name='manpowerrequeststatus' /><Value Type='Text'>Active</Value></Eq></Where><OrderBy><FieldRef Name='positionrequested_x003a_Position' /></OrderBy> 
                    </Query> 
                    <ViewFields><FieldRef Name='manpowerrequeststatus' /> <FieldRef Name='ID' /><FieldRef Name='positionrequested' /><FieldRef Name='positionrequested_x003a_Position' /><FieldRef Name='positionrequested_x003a_ID' /></ViewFields></View>";
             
            var models = new List<PositionMaster>();

            //foreach (var item in SPConnector.GetList(SP_MANPOW_LIST_NAME, _siteUrl, caml))
            //{
            //    models.Add(ConvertToPositionsModel(item));
            //}

            int tempID;
            List<int> collectionIDShortlist = new List<int>();
            foreach (var item in SPConnector.GetList(SP_MANPOW_LIST_NAME, _siteUrl, caml))
            {
                collectionIDShortlist.Add(item["positionrequested_x003a_ID"] == null ? 0 :
               Convert.ToInt16((item["positionrequested_x003a_ID"] as FieldLookupValue).LookupValue));
            }
            foreach (var item in SPConnector.GetList(SP_MANPOW_LIST_NAME, _siteUrl, caml))
            {
                tempID = Convert.ToInt32(item["ID"]);
                if (!(collectionIDShortlist.Any(e => e == tempID)))
                {
                    models.Add(ConvertToPositionsModel(item));
                }
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

            viewModel.ID = Convert.ToInt32(FormatUtil.ConvertLookupToValue(item, "positionrequested_x003a_ID"));
            viewModel.PositionName = FormatUtil.ConvertLookupToValue(item, "positionrequested");
            return viewModel;
        }
    }
}
