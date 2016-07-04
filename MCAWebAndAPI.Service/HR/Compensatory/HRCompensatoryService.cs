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
        const string SP_COMDET_LIST_NAME = "Compensatory Request Detail";
        const string SP_COMREQ_LIST_NAME = "Compensatory Request";

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

        private CompensatoryVM ConvertCompInputTolistDataVM(ListItem listItem)
        {
            var viewModel = new CompensatoryVM();

            viewModel.CmpID = Convert.ToInt32(listItem["ID"]);
            viewModel.CmpName = Convert.ToString(listItem["Title"]);
            viewModel.CmpProjUnit = Convert.ToString(listItem["Project_x002f_Unit"]);
            viewModel.CmpPosition = FormatUtil.ConvertLookupToValue(listItem, "Position");

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
                                <FieldRef Name='professional_x003a_ID' />
                                <Value Type='Lookup'>" + ID + @"</Value>
                             </Eq>
                       </Where>
                    </Query> 
                     <ViewFields> <FieldRef Name='Title' />
                       <FieldRef Name='ID' /></ViewFields> 
                    </View>";

            var compID = 0;
            foreach (var item in SPConnector.GetList(SP_COMREQ_LIST_NAME, _siteUrl, caml))
            {
                compID = Convert.ToInt32(item["ID"]);
            }

            return GetComplisted(compID, ID);
        }

        private CompensatoryVM GetComplisted(int? ID, int? idPro )
        {
            var viewModel = new CompensatoryVM();

            if (idPro == 0)
                return viewModel;

            var listItem = SPConnector.GetListItem(SP_PROMAS_LIST_NAME, idPro, _siteUrl);
            viewModel = ConvertCompInputTolistDataVM(listItem);


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
                            </ViewFields>
                           </View>";

            var shortlistDetails = new List<CompensatoryDetailVM>();
            foreach (var item in SPConnector.GetList(SP_COMDET_LIST_NAME, _siteUrl, caml))
            {
                shortlistDetails.Add(ConvertToCompDetailVM(item));
            }
            return shortlistDetails;
        }

        /// <summary>
        /// </summary>

        private CompensatoryDetailVM ConvertToCompDetailVM(ListItem item)
        {
            return new CompensatoryDetailVM
            {
                CmpActiv = Convert.ToString(item["Title"]),
                CmpID = Convert.ToInt32(item["ID"]),
                CmpDate = Convert.ToDateTime(item["compensatorydate"]),
                StartTime = Convert.ToDateTime(item["compensatorystarttime"]),
                FinishTime = Convert.ToDateTime(item["compensatoryendtime"]),
                CmpTotalHours = Convert.ToInt32(item["totalhours"]),
                TotalDay = Convert.ToInt32(item["totaldays"]),
                remarks = Convert.ToString(item["remarks"]),
                AppStatus = Convert.ToString(item["compensatorystatus"]),
            };
        }

        public void SetSiteUrl(string siteUrl = null)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
        }

        public void CreateCompensatoryData(int? headerID, CompensatoryVM viewModels)
        {
            var CreateValue = new Dictionary<string, object>();

            CreateValue.Add("professional", new FieldLookupValue { LookupId = Convert.ToInt32(viewModels.CmpID) });

            try
            {
                SPConnector.AddListItem(SP_COMREQ_LIST_NAME, CreateValue, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                throw e;
            }

            var caml = @"<View>  
                  <Query> 
                    <Where>
                        <Eq>
                        <FieldRef Name='professional_x003a_ID' />
                        <Value Type='Lookup'>" + viewModels.CmpID + @"</Value>
                        </Eq>
                    </Where>
                        <OrderBy>
                            <FieldRef Name='Created' Ascending='False' />
                        </OrderBy>
                  </Query> 
                  <ViewFields>
                      <FieldRef Name='ID' />
                  </ViewFields>
                <RowLimit>1</RowLimit> 
            </View>";

            var CompID = 0;
            foreach (var item in SPConnector.GetList(SP_COMREQ_LIST_NAME, _siteUrl, caml))
            {
                CompID = Convert.ToInt32(item["ID"]);
            }

            foreach (var viewModel in viewModels.CompensatoryDetails)
            {
                if (Item.CheckIfSkipped(viewModel))
                    continue;

                if (Item.CheckIfDeleted(viewModel))
                {
                    try
                    {
                        SPConnector.DeleteListItem(SP_COMDET_LIST_NAME, viewModel.ID, _siteUrl);
                    }
                    catch (Exception e)
                    {
                        logger.Error(e);
                        throw e;
                    }
                    continue;
                }

                var updatedValue = new Dictionary<string, object>();
                
                updatedValue.Add("compensatoryrequest", CompID);
                updatedValue.Add("Title", viewModel.CmpActiv);
                updatedValue.Add("compensatorydate", viewModel.CmpDate);
                updatedValue.Add("compensatorystarttime", viewModel.StartTime);
                updatedValue.Add("compensatoryendtime", viewModel.FinishTime);
                updatedValue.Add("totalhours", viewModel.CmpTotalHours);
                updatedValue.Add("totaldays", viewModel.TotalDay);
                updatedValue.Add("remarks", viewModel.remarks);

                try
                {
                    SPConnector.AddListItem(SP_COMDET_LIST_NAME, updatedValue, _siteUrl);
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

