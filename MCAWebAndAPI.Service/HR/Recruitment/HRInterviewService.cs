﻿using System;
using System.Collections.Generic;
using System.Web;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.Utils;
using NLog;
using Microsoft.SharePoint.Client;
using MCAWebAndAPI.Service.Resources;
using System.Threading.Tasks;
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

        private ApplicationShortlistVM ConvertToResultInterviewDataVM(ListItem listItem, int? posMan)
        {
            var viewModel = new ApplicationShortlistVM();
            viewModel.ID = Convert.ToInt32(listItem["ID"]);
            viewModel.Position = FormatUtil.ConvertLookupToID(listItem, "vacantposition") + string.Empty;
            viewModel.OtherPosition.Value = FormatUtil.ConvertLookupToID(listItem, "recommendedforposition");
            viewModel.Candidate = Convert.ToString(listItem["Title"]) + " " + Convert.ToString(listItem["lastname"]);
            viewModel.SendTo = Convert.ToString(listItem["personalemail"]);
            viewModel.InterviewerUrl = string.Format(UrlResource.AddInterviewInvitation, _siteUrl, Convert.ToInt32(listItem["ID"]), posMan);
            viewModel.Remarks = Convert.ToString(listItem["applicationremarks"]); 
            viewModel.NeedNextInterviewer.Value = Convert.ToString(listItem["neednextinterview"]);
            viewModel.RecommendedForPosition.Value = Convert.ToString(listItem["applicationstatus"]);
            viewModel.Attachmentname = GetAttachment(viewModel.ID);
            viewModel.InterviewResultOption.Value = GetLastResult(viewModel.ID).ToString();

            // Convert Details
            viewModel.InterviewlistDetails = GetInputInterviewResult(viewModel.ID);
            return viewModel;
        }

        public ApplicationShortlistVM GetResultlistInterview(int? ID, int? posMan)
        {
            var viewModel = new ApplicationShortlistVM();

            ListItem listItemdt;

            if (ID == null)
                return viewModel;

            try
            {
                listItemdt = SPConnector.GetListItem(SP_APPDATA_LIST_NAME, ID, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                throw e;
            }

            viewModel = ConvertToResultInterviewDataVM(listItemdt, posMan);

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
            var caml = @"<View>
                        <Query>
                           <Where>
                              <Eq>
                                 <FieldRef Name='application_x003a_ID' />
                                 <Value Type='Lookup'>" + ID + @"</Value>
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
                        </ViewFields></View>";

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

            viewModel.DateString = Convert.ToDateTime(listItem["interviewdatetime"]).ToShortDateString();
            viewModel.InterviewPanel = Convert.ToString(listItem["interviewpanel"]);
            viewModel.InterviewSummary = Convert.ToString(listItem["interviewsummary"]);
            viewModel.Result = Convert.ToString(listItem["interviewresult"]);


            return viewModel;
        }

        private string GetLastResult(int? ID)
        {
            var caml = @"
                         <Query> 
                            <Where>
                              <Eq>
                                 <FieldRef Name='ID' />
                                 <Value Type='Counter'>" + ID + @"</Value>
                              </Eq>
                           </Where>
                           <OrderBy><FieldRef Name='ID' Ascending='FALSE' /><FieldRef Name='Editor' Ascending='FALSE' /></OrderBy> 
                        </Query> 
                        <ViewFields> 
                           <FieldRef Name='interviewresult' /></ViewFields> 
                        <RowLimit>1</RowLimit> ";

            var result = "";
            var list = SPConnector.GetList(SP_APPINTV_LIST_NAME, _siteUrl, caml);
            foreach (var item in list)
            {
                result = Convert.ToString(item["interviewresult"]);
            }

            return result;
        }

        private string GetAttachment(int? ID)
        {
            var caml = @"<view>
                         <Query> 
                            <Where>
                            <Eq>
                                <FieldRef Name='application_x003a_ID' />
                                <Value Type='Lookup'>" + ID + @"</Value>
                            </Eq>
                           </Where>
                           <OrderBy><FieldRef Name='ID' Ascending='FALSE' /><FieldRef Name='Created' Ascending='FALSE' /></OrderBy> 
                        </Query> 
                        <ViewFields> 
                              <FieldRef Name='FileLeafRef' />
                        </ViewFields><RowLimit>4</RowLimit></view>";

            List<string> filelist = new List<string>();

            var list = SPConnector.GetList(SP_APPDOC_LIST_NAME, _siteUrl, caml);
            foreach (var item in list)
            {
                filelist.Add(Convert.ToString(item["FileLeafRef"]));
            }

            var result = string.Join(", ", filelist.ToArray());

            return result;
        }

        private ApplicationShortlistVM ConvertToLastResultDataVM(ListItem listItem)
        {
            var viewModel = new ApplicationShortlistVM();

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

        public ApplicationShortlistVM GetInterviewlist(int? position, string username, string useraccess)
        {
            if (position == null)
                return null;

            var caml = @"<View>  
                    <Query> 
                       <Where>
                             <And>
                                 <Eq>
                                    <FieldRef Name='positionrequested_x003a_ID' />
                                    <Value Type='Lookup'>" + position + @"</Value>
                                 </Eq>
                                 <Eq>
                                    <FieldRef Name='manpowerrequeststatus' />
                                    <Value Type='Text'>Active</Value>
                                 </Eq>
                              </And>
                       </Where>
                    </Query> 
                     <ViewFields><FieldRef Name='ID' /></ViewFields> 
                    </View>";

            var applicationID = 0;
            foreach (var item in SPConnector.GetList(SP_MANPOW_LIST_NAME, _siteUrl, caml))
            {
                applicationID = Convert.ToInt32(item["ID"]);
            }

            return GetInterviewlist(applicationID, username, useraccess, position);
        }

        public ApplicationShortlistVM GetInterviewlist(int ID, string username, string useraccess, int? position)
        {
            var viewModel = new ApplicationShortlistVM();
            if (ID == 0)
                return viewModel;

            viewModel.ShortlistDetails = GetDetailInterviewlist(ID, useraccess, position);
            viewModel.ActivePosition.Value = Convert.ToInt32(position);
            viewModel.Position = Convert.ToString(position);
            viewModel.useraccess = Convert.ToString(useraccess);

            return viewModel;

        }

        //<ViewFields>
        //   <FieldRef Name = 'Title' />
        //   < FieldRef Name='applications' />
        //   <FieldRef Name = 'university' />
        //   < FieldRef Name='yearofgraduation' />
        //   <FieldRef Name = 'remarks' />
        //</ ViewFields >
        private IEnumerable<ShortlistDetailVM> GetDetailInterviewlist(int Position, string useraccess, int? manPos)
        {
            var caml = "";

            if (useraccess == "REQ")
            {
                caml = @"
                        <View>
                           <Query>
                           <Where>
                                <And>
                                    <Eq>
                                    <FieldRef Name='manpowerrequisition' />
                                    <Value Type='Lookup'>" + Position + @"</Value>
                                    </Eq>
                                    <And>
                                    <Eq>
                                        <FieldRef Name='manpowerrequisition_x003a_Manpow' />
                                        <Value Type='Lookup'>Active</Value>
                                    </Eq>
                                    <Or>
                                        <Eq>
                                            <FieldRef Name='applicationstatus' />
                                            <Value Type='Text'>Shortlisted</Value>
                                        </Eq>
                                        <Or>
                                            <Eq>
                                                <FieldRef Name='applicationstatus' />
                                                <Value Type='Text'>Recommended</Value>
                                            </Eq>
                                            <Or>
                                                <Eq>
                                                <FieldRef Name='applicationstatus' />
                                                <Value Type='Text'>Not Recommended</Value>
                                                </Eq>
                                                <Eq>
                                                <FieldRef Name='applicationstatus' />
                                                <Value Type='Text'>Pending MCC Approval</Value>
                                                </Eq>
                                            </Or>
                                        </Or>
                                    </Or>
                                    </And>
                                </And>
                           </Where>
                        </Query>
                        <ViewFields>
                           <FieldRef Name='Title' />
                          <FieldRef Name='lastname' />
                          <FieldRef Name='ID' />
                          <FieldRef Name='applicationstatus' />
                          <FieldRef Name='applicationremarks' />
                          <FieldRef Name='position' />
                          <FieldRef Name='neednextinterview' />
                          <FieldRef Name='personalemail' />
                        </ViewFields>
                        </View>";
            }
            else if (useraccess == "PAN")
            {
                caml = @"
                        <View>
                        <Query>
                           <Where>
                                <And>
                                    <Eq>
                                    <FieldRef Name='manpowerrequisition' />
                                    <Value Type='Lookup'>" + Position + @"</Value>
                                    </Eq>
                                    <And>
                                    <Eq>
                                        <FieldRef Name='manpowerrequisition_x003a_Manpow' />
                                        <Value Type='Lookup'>Active</Value>
                                    </Eq>
                                    <And>
                                        <Eq>
                                            <FieldRef Name='neednextinterview' />
                                            <Value Type='Choice'>Yes</Value>
                                        </Eq>
                                        <Or>
                                            <Eq>
                                                <FieldRef Name='applicationstatus' />
                                                <Value Type='Text'>For Other Position</Value>
                                            </Eq>
                                            <Or>
                                                <Eq>
                                                <FieldRef Name='applicationstatus' />
                                                <Value Type='Text'>Recommended</Value>
                                                </Eq>
                                                <Eq>
                                                <FieldRef Name='applicationstatus' />
                                                <Value Type='Text'>Pending MCC Approval</Value>
                                                </Eq>
                                            </Or>
                                        </Or>
                                    </And>
                                    </And>
                                </And>
                           </Where>
                        </Query>
                        <ViewFields>
                                <FieldRef Name='Title' />
                                <FieldRef Name='lastname' />
                                <FieldRef Name='ID' />
                                <FieldRef Name='applicationstatus' />
                                <FieldRef Name='applicationremarks' />
                                <FieldRef Name='position' />
                                <FieldRef Name='neednextinterview' />
                                <FieldRef Name='personalemail' />
                        </ViewFields>
                        </View>";
            }

            var shortlistDetails = new List<ShortlistDetailVM>();
            foreach (var item in SPConnector.GetList(SP_APPDATA_LIST_NAME, _siteUrl, caml))
            {
                shortlistDetails.Add(ConvertToInterviewDetailVM(item, manPos));
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
        private ShortlistDetailVM ConvertToInterviewDetailVM(ListItem item, int? pos)
        {
            return new ShortlistDetailVM
            {
                Candidate = Convert.ToString(item["Title"]) + " " + Convert.ToString(item["lastname"]),
                Candidatemail = Convert.ToString(item["personalemail"]),
                ID = Convert.ToInt32(item["ID"]),
                CandidateUrl = GetCandidateUrl(Convert.ToInt32(item["ID"]), pos),
                ProfesionalUrl = GetProfesionalUrl(Convert.ToInt32(item["ID"])),
                DocumentUrl = GetDocumentUrl(Convert.ToInt32(item["ID"])),
                Status = ShortlistDetailVM.GetStatusDefaultValue(
                    new Model.ViewModel.Control.AjaxComboBoxVM
                    {
                        Text = Convert.ToString(item["applicationstatus"]),
                    }),

                GetStat = Convert.ToString(item["applicationstatus"]),
                Remarks = Convert.ToString(item["applicationremarks"]),
                neednextintv = Convert.ToString(item["neednextinterview"]),
            };
        }

        private string GetDocumentUrl(int? iD)
        {
            return string.Format(UrlResource.CVDocumentByID, _siteUrl, iD);
        }

        private string GetCandidateUrl(int? iD, int? pos)
        {
            return string.Format(UrlResource.InterviewInputResult, _siteUrl, iD, pos);
        }

        private string GetProfesionalUrl(int? iD)
        {
            return string.Format(UrlResource.ApplicationDisplayByID, _siteUrl, iD);
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
            
                createdValue.Add("applicationremarks", viewModel.Remarks);
                createdValue.Add("applicationstatus", viewModel.RecommendedForPosition.Value);
                createdValue.Add("recommendedforposition", viewModel.OtherPosition.Value);
                createdValue.Add("neednextinterview", viewModel.NeedNextInterviewer.Value);

            try
            {
                SPConnector.UpdateListItem(SP_APPDATA_LIST_NAME, headerID, createdValue, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                throw e;
            }
            
        }

        public void CreateInputIntvResult(int? headerID, ApplicationShortlistVM viewModel)
        {
             var createdValue = new Dictionary<string, object>();

            createdValue.Add("application",  headerID);
            createdValue.Add("interviewdatetime", viewModel.InterviewerDate);
            createdValue.Add("interviewpanel", viewModel.InterviewerPanel);
            createdValue.Add("interviewsummary", viewModel.InterviewSummary);
            createdValue.Add("Title", viewModel.Candidate);
            createdValue.Add("interviewresult", viewModel.InterviewResultOption.Value);

            try
            {
                SPConnector.AddListItem(SP_APPINTV_LIST_NAME, createdValue, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                throw e;
            }
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

        public Dictionary<int, string> GetStatusResult()
        {
            int index = 0;
            var choice = new Dictionary<int, string>
            {
                { ++index, "Recomended"},
                { ++index, "Not Recomended"},
                { ++index, "For Other Position"},
                { ++index, "Pending MCC Approval"},
                { ++index, "Rejected by MCC"},
                { ++index, "On Board"},
                { ++index, "Decline to Join"}
            };

            return choice;
        }

        public async Task CreateInterviewDocumentsSync(int? headerID, IEnumerable<HttpPostedFileBase> documents)
        {
            CreateInterviewDocuments(headerID, documents);
        }

        public void CreateInterviewDocuments(int? headerID, IEnumerable<HttpPostedFileBase> documents)
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
                    throw new Exception(ErrorResource.SPInsertError);
                }
            }
        }

        public IEnumerable<ShortlistDetailVM> GetUpdatedata(string useraccess)
        {
            var caml = @"<view>
                        <Query> 
                              <Where>
                                   <And>
                                        <Eq>
                                           <FieldRef Name='manpowerrequisition' />
                                            <Value Type='Counter'>45</Value>
                                        </Eq>
                                    <Or>
                                        <Eq>
                                           <FieldRef Name='applicationstatus' />
                                           <Value Type='Text'>Shortlisted</Value>
                                        </Eq>
                                        <Or>
                                           <Eq>
                                              <FieldRef Name='applicationstatus' />
                                              <Value Type='Text'>New</Value>
                                           </Eq>
                                           <Eq>
                                              <FieldRef Name='applicationstatus' />
                                              <Value Type='Text'>Declined</Value>
                                           </Eq>
                                        </Or>
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
                shortlistDetails.Add(ConvertToShortlistDetailVM(item));
            }

            return shortlistDetails;
        }

        private ShortlistDetailVM ConvertToShortlistDetailVM(ListItem item)
        {
            return new ShortlistDetailVM
            {
                ID = Convert.ToInt32(item["ID"])
            };
        }

        public void UpdateManualDataDetail(ShortlistDetailVM viewModel, int? manID)
        {
            var createdValue = new Dictionary<string, object>();

            createdValue.Add("applicationstatus", "New");

            try
            {

                SPConnector.UpdateListItem(SP_APPDATA_LIST_NAME, viewModel.ID, createdValue, _siteUrl);

            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                throw e;
            }

        }

    }
}
