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
    public class HRInterviewService : IHRInterviewService
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

        private ApplicationShortlistVM ConvertToResultInterviewDataVM(ListItem listItem)
        {
            var viewModel = new ApplicationShortlistVM();

            viewModel.ID = Convert.ToInt32(listItem["ID"]);
            viewModel.Position = FormatUtil.ConvertLookupToID(listItem, "vacantposition") + string.Empty;
            viewModel.OtherPosition.Value = FormatUtil.ConvertLookupToID(listItem, "recommendedforposition");
            viewModel.Candidate = Convert.ToString(listItem["Title"]);

            // Convert Details
            viewModel.InterviewlistDetails = GetInputInterviewResult(viewModel.ID);
            return viewModel;
        }

        public ApplicationShortlistVM GetResultlistInterview(int? ID)
        {
            var viewModel = new ApplicationShortlistVM();
            if (ID == null)
                return viewModel;

            var listItemdt = SPConnector.GetListItem(SP_APPDATA_LIST_NAME, ID, _siteUrl);
            viewModel = ConvertToResultInterviewDataVM(listItemdt);

            viewModel.InterviewlistDetails = GetInputInterviewResult(viewModel.ID);

            return viewModel;

        }

        //<ViewFields>
        //   <FieldRef Name = 'Title' />
        //   < FieldRef Name='applications' />
        //   <FieldRef Name = 'university' />
        //   < FieldRef Name='yearofgraduation' />
        //   <FieldRef Name = 'remarks' />
        //</ ViewFields >
        private IEnumerable<InterviewDetailVM> GetInputInterviewResult(int? ID)
        {
            var caml = @"<Query>
                           <Where>
                              <Eq>
                                 <FieldRef Name='ID' />
                                 <Value Type='Counter'>" + ID + @"</Value>
                              </Eq>
                           </Where>
                        </Query>
                        <ViewFields>
                           <FieldRef Name='interviewdatetime' />
                           <FieldRef Name='interviewpanel' />
                           <FieldRef Name='invitationemaildate' />
                           <FieldRef Name='invitationemailfrom' />
                           <FieldRef Name='invitationemailmessage' />
                           <FieldRef Name='interviewsummary' />
                           <FieldRef Name='interviewresult' />
                        </ViewFields>
                        <QueryOptions />";

            var Interviewdetails = new List<InterviewDetailVM>();
            foreach (var item in SPConnector.GetList(SP_APPINTV_LIST_NAME, _siteUrl, caml))
            {
                Interviewdetails.Add(ConvertToInterviewResultDataVM(item));
            }

            return Interviewdetails;
        }

        private InterviewDetailVM ConvertToInterviewResultDataVM(ListItem listItem)
        {
            var viewModel = new InterviewDetailVM();

            viewModel.Date = Convert.ToDateTime(listItem["interviewdatetime"]);
            viewModel.InterviewPanel = Convert.ToString(listItem["interviewpanel"]);
            viewModel.InterviewSummary = Convert.ToString(listItem["interviewsummary"]);
            viewModel.Result = Convert.ToString(listItem["interviewresult"]);

            return viewModel;
        }

        public void CreateApplicationDocument(int? headerID, IEnumerable<HttpPostedFileBase> documents)
        {
            foreach (var doc in documents)
            {
                var updateValue = new Dictionary<string, object>();
                updateValue.Add("application", new FieldLookupValue { LookupId = Convert.ToInt32(headerID) });
                try
                {
                    SPConnector.UploadDocument(SP_APPDOC_LIST_NAME, updateValue, doc.FileName, doc.InputStream, _siteUrl);
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw e;
                }
            }
        }

        public ApplicationShortlistVM GetInterviewlist(string position, string username, string useraccess)
        {
            if (position == null)
                return null;

            var caml = @"<View>  
                    <Query> 
                       <Where>
                          <And>
                             <Eq>
                                <FieldRef Name='iskeyposition' />
                                <Value Type='Boolean'>true</Value>
                             </Eq>
                             <Eq>
                                <FieldRef Name='positionrequested' />
                                <Value Type='Lookup'>Temporary Data Entry</Value>
                             </Eq>
                          </And>
                       </Where>
                    </Query> 
                     <ViewFields><FieldRef Name='Position' /><FieldRef Name='ID' /></ViewFields> 
                    </View>";

            var applicationID = 0;
            foreach (var item in SPConnector.GetList(SP_MANPOW_LIST_NAME, _siteUrl, caml))
            {
                applicationID = Convert.ToInt32(item["ID"]);
            }

            return GetInterviewlist(applicationID, username, useraccess, position);
        }

        public ApplicationShortlistVM GetInterviewlist(int ID, string username, string useraccess, string position)
        {
            var viewModel = new ApplicationShortlistVM();
            if (ID == 0)
                return viewModel;

            if (username != null)
                useraccess = GetAccessData(username);


            viewModel.ShortlistDetails = GetDetailInterviewlist(ID, useraccess);
            viewModel.Position = position;

            return viewModel;

        }

        //<ViewFields>
        //   <FieldRef Name = 'Title' />
        //   < FieldRef Name='applications' />
        //   <FieldRef Name = 'university' />
        //   < FieldRef Name='yearofgraduation' />
        //   <FieldRef Name = 'remarks' />
        //</ ViewFields >
        private IEnumerable<ShortlistDetailVM> GetDetailInterviewlist(int Position, string useraccess)
        {
            var caml = @"<View>  
                                <Query> 
                          <Where>
                              <And>
                                 <Eq>
                                    <FieldRef Name='manpowerrequisition' />
                                    <Value Type='Lookup'>" + Position + @"</Value>
                                 </Eq>
                                 <Or>
                                    <Eq>
                                       <FieldRef Name='applicationstatus' />
                                       <Value Type='Text'>Shortlisted</Value>
                                    </Eq>
                                    <Eq>
                                       <FieldRef Name='applicationstatus' />
                                       <Value Type='Text'>Recomended</Value>
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
                       </ViewFields>
                                </View>";

            var shortlistDetails = new List<ShortlistDetailVM>();
            foreach (var item in SPConnector.GetList(SP_APPDATA_LIST_NAME, _siteUrl, caml))
            {
                shortlistDetails.Add(ConvertToInterviewDetailVM(item));
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
        private ShortlistDetailVM ConvertToInterviewDetailVM(ListItem item)
        {
            return new ShortlistDetailVM
            {
                Candidate = Convert.ToString(item["Title"]),
                Candidatemail = Convert.ToString(item["Title"]),
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

        public IEnumerable<ApplicationShortlistVM> GetInterviewlists()
        {
            throw new NotImplementedException();
        }

        public void SetSiteUrl(string siteUrl = null)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
        }

        public void CreateInterviewDataDetail(int? headerID, ApplicationShortlistVM viewModel)
        {

            var createdValue = new Dictionary<string, object>();
            
                createdValue.Add("Title", viewModel.Candidate );
                createdValue.Add("applicationremarks", viewModel.Remarks);
                createdValue.Add("applicationstatus", viewModel.GetResultOptions.Value);
                createdValue.Add("recommendedforposition", viewModel.OtherPosition.Value);
                createdValue.Add("neednextinterview", viewModel.NeedNextInterviewer);
                //createdValue.Add("documenturl", viewModel.AttDocuments);

            try
            {
                SPConnector.UpdateListItem(SP_APPDATA_LIST_NAME, viewModel.ID, createdValue, _siteUrl);
            }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw e;
                }

            EmailUtil.Send(viewModel.SendTo, "Interview Result", "<div><label>" + viewModel.EmailMessage + "</label></div>" +
                           "<a href = 'https://eceos2.sharepoint.com/sites/mca-dev/hr/Lists/Professional%20Master/DispForm_Custom.aspx?ID=3' > Open candidates' profiles for this position</a>");

        }

        public void CreateInputIntvResult(int? headerID, ApplicationShortlistVM viewModel)
        {
            var createdValue = new Dictionary<string, object>();

            createdValue.Add("interviewdatetime", viewModel.InterviewerDate);
            createdValue.Add("interviewpanel", viewModel.InterviewerPanel);
            createdValue.Add("interviewsummary", viewModel.InterviewSummary);
            createdValue.Add("Title", viewModel.Candidate);
            createdValue.Add("interviewresult", viewModel.GetResultOptions.Value);


            try
            {
                SPConnector.AddListItem(SP_APPINTV_LIST_NAME, createdValue, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                throw e;
            }

            EmailUtil.Send(viewModel.EmailFrom, "Interview Invitation", viewModel.EmailMessage);

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
    }
}
