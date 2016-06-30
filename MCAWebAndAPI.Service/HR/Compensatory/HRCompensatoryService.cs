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
    public class HRCompensatoryService : IHRCompensatoryService
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

        private CompensatoryVM ConvertToShortlistDataVM(ListItem listItem)
        {
            var viewModel = new CompensatoryVM();

            viewModel.ID = Convert.ToInt32(listItem["ID"]);
            viewModel.CmpName = Convert.ToString(listItem["Title"]);

            return viewModel;
        }

        public CompensatoryVM GetComplist(int? ID)
        {
            var viewModel = new CompensatoryVM();

            if (ID == null)
                return null;

            var caml = @"<View>  
                    <Query> 
                       <Where>
                             <Eq>
                                <FieldRef Name='positionrequested_x003a_ID' />
                                <Value Type='Lookup'>" + ID + @"</Value>
                             </Eq>
                       </Where>
                    </Query> 
                     <ViewFields> <FieldRef Name='Title' />
                       <FieldRef Name='ID' /></ViewFields> 
                    </View>";

            var compID = 0;
            foreach (var item in SPConnector.GetList(SP_PROMAS_LIST_NAME, _siteUrl, caml))
            {
                compID = Convert.ToInt32(item["ID"]);
            }

            return GetComplisted(compID);
        }

        private CompensatoryVM GetComplisted(int? ID)
        {
            var viewModel = new CompensatoryVM();
            if (ID == 0)
                return viewModel;

            var listItemdt = SPConnector.GetListItem(SP_APPDATA_LIST_NAME, ID, _siteUrl);
            viewModel = ConvertToShortlistDataVM(listItemdt);

            viewModel.CompensatoryDetails = GetCompDetailist(ID);

            return viewModel;

        }

        //<ViewFields>
        //   <FieldRef Name = 'Title' />
        //   < FieldRef Name='applications' />
        //   <FieldRef Name = 'university' />
        //   < FieldRef Name='yearofgraduation' />
        //   <FieldRef Name = 'remarks' />
        //</ ViewFields >
        private IEnumerable<CompensatoryDetailVM> GetCompDetailist(int? ID)
        {
            var caml = @"<View>  
                              <Query> 
                            <Where>
                             <Eq>
                                <FieldRef Name='compensatoryrequest' />
                                <Value Type='Lookup'>" + ID + @"</Value>
                             </Eq>
                            </Where>
                                </Query> 
                               <Query />
                            <ViewFields>
                               <FieldRef Name='Title' />
                               <FieldRef Name='compensatoryrequest' />
                               <FieldRef Name='compensatorydate' />
                               <FieldRef Name='compensatorystarttime' />
                               <FieldRef Name='compensatoryendtime' />
                               <FieldRef Name='totalhours' />
                               <FieldRef Name='totaldays' />
                               <FieldRef Name='remarks' />
                               <FieldRef Name='compensatorystatus' />
                               <FieldRef Name='visibleto' />
                               <FieldRef Name='ID' />
                               <FieldRef Name='Attachments' />
                               <FieldRef Name='LinkTitleNoMenu' />
                               <FieldRef Name='LinkTitle' />
                               <FieldRef Name='DocIcon' />
                               <FieldRef Name='ItemChildCount' />
                               <FieldRef Name='FolderChildCount' />
                               <FieldRef Name='AppAuthor' />
                               <FieldRef Name='AppEditor' />
                            </ViewFields>
                           </View>";

            var shortlistDetails = new List<CompensatoryDetailVM>();
            foreach (var item in SPConnector.GetList(SP_APPDATA_LIST_NAME, _siteUrl, caml))
            {
                shortlistDetails.Add(ConvertToShortlistDetailVM(item));
            }
            return shortlistDetails;
        }

        /// <summary>
        /// </summary>

        private CompensatoryDetailVM ConvertToShortlistDetailVM(ListItem item)
        {
            return new CompensatoryDetailVM
            {
                CmpID = Convert.ToInt32(item["Title"]),
                CmpDate = Convert.ToDateTime(item["Title"]),
                StartTime = Convert.ToDateTime(item["ID"]),
                FinishTime = Convert.ToDateTime(item["ID"]),
                CmpTotalHours = Convert.ToInt32(item["applicationstatus"]),
                TotalDay = Convert.ToInt32(item["applicationremarks"]),
                remarks = Convert.ToString(item["applicationremarks"]),
                AppStatus = Convert.ToString(item["applicationremarks"]),
            };
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

        public IEnumerable<PositionMaster> GetPositions()
        {
            var caml = @"<View>  
                    <Query> 
                       <Where><Eq><FieldRef Name='manpowerrequeststatus' /><Value Type='Text'>Active</Value></Eq></Where><OrderBy><FieldRef Name='positionrequested_x003a_Position' /></OrderBy> 
                    </Query> 
                    <ViewFields><FieldRef Name='manpowerrequeststatus' /> <FieldRef Name='ID' /><FieldRef Name='positionrequested' /><FieldRef Name='positionrequested_x003a_Position' /><FieldRef Name='positionrequested_x003a_ID' /></ViewFields></View>";

            var models = new List<PositionMaster>();

            foreach (var item in SPConnector.GetList(SP_MANPOW_LIST_NAME, _siteUrl, caml))
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

            viewModel.ID = Convert.ToInt32(FormatUtil.ConvertLookupToValue(item, "positionrequested_x003a_ID"));
            viewModel.PositionName = FormatUtil.ConvertLookupToValue(item, "positionrequested");
            return viewModel;
        }
    }
}

