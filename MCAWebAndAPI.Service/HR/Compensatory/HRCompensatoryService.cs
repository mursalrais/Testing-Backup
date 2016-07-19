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

            viewModel.cmpID = Convert.ToInt32(listItem["ID"]);
            viewModel.cmpName = Convert.ToString(listItem["Title"]);
            viewModel.cmpProjUnit = Convert.ToString(listItem["Project_x002f_Unit"]);
            viewModel.cmpPosition = FormatUtil.ConvertLookupToValue(listItem, "Position");

            return viewModel;
        }

        public CompensatoryVM GetComplist(int? iD)
        {
            var viewModel = new CompensatoryVM();

            if (iD == null)
                return null;

            var caml = @"<View>  
                    <Query> 
                       <Where>
                             <Eq>
                                <FieldRef Name='ID' />
                                <Value Type='Lookup'>" + iD + @"</Value>
                             </Eq>
                       </Where>
                    </Query> 
                     <ViewFields> <FieldRef Name='Title' />
                          <FieldRef Name='professional_x003a_ID' /></ViewFields> 
                    </View>";

            var profID = 0;
            foreach (var item in SPConnector.GetList(SP_COMREQ_LIST_NAME, _siteUrl, caml))
            {
                profID = Convert.ToInt32(FormatUtil.ConvertLookupToID(item, "professional_x003a_ID") + string.Empty);
            }

            return GetComplisted(iD, profID);
        }

        private int GetCompID(int? ID)
        {
            var viewModel = new CompensatoryVM();

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

            return compID;
        }

        public CompensatoryVM GetComplistActive()
        {
            var viewModel = new CompensatoryVM();

            var caml1 = @"<View>  
                     <ViewFields> 
                       <FieldRef Name='professional_x003a_ID' />
                     </ViewFields> 
                    </View>";

            foreach (var item1 in SPConnector.GetList(SP_COMREQ_LIST_NAME, _siteUrl, caml1))
            {

                var caml2 = @"<View>  
                    <Query> 
                        <Where>
                          <And>
                             <Eq>
                                <FieldRef Name='Professional_x0020_Status' />
                                <Value Type='Choice'>Active</Value>
                             </Eq>
                             <Eq>
                                <FieldRef Name='ID' />
                                <Value Type='Counter'>" + Convert.ToInt32(FormatUtil.ConvertLookupToValue(item1, "professional_x003a_ID")) + @"</Value>
                             </Eq>
                          </And>
                       </Where>
                    </Query> 
                     <ViewFields> 
                       <FieldRef Name='Title' />
                       <FieldRef Name='ID' />
                       <FieldRef Name='Project_x002f_Unit' />
                       <FieldRef Name='Position' />
                     </ViewFields> 
                    </View>";


                foreach (var item2 in SPConnector.GetList(SP_PROMAS_LIST_NAME, _siteUrl, caml2))
                {
                    viewModel.cmpID = Convert.ToInt32(item2["ID"]);
                    viewModel.cmpName = Convert.ToString(item2["Title"]);
                    viewModel.cmpProjUnit = Convert.ToString(item2["Project_x002f_Unit"]);
                    viewModel.cmpPosition = FormatUtil.ConvertLookupToValue(item2, "Position");
                    viewModel.CompensatoryDetails = GetCompDetailist(GetCompID(Convert.ToInt32(item2["ID"])));
                }

            }
            return viewModel;
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

        private int getidComp(int? cmpId)
        {
            var caml = @"<View>  
                  <Query> 
                    <Where>
                        <Eq>
                        <FieldRef Name='professional_x003a_ID' />
                        <Value Type='Lookup'>" + cmpId + @"</Value>
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
            return CompID;
        }

        public void CreateCompensatoryData(int? cmpID, CompensatoryVM viewModels)
        {
            foreach (var viewModel in viewModels.CompensatoryDetails)
            {
                if (Item.CheckIfUpdated(viewModel))
                {
                    if (viewModel.CmpID == null)
                    {
                        var cratedValueDetail = new Dictionary<string, object>();

                        cratedValueDetail.Add("compensatoryrequest", cmpID);
                        cratedValueDetail.Add("Title", viewModel.CmpActiv);
                        cratedValueDetail.Add("compensatorydate", viewModel.CmpDate);
                        cratedValueDetail.Add("compensatorystarttime", viewModel.StartTime);
                        cratedValueDetail.Add("compensatoryendtime", viewModel.FinishTime);
                        cratedValueDetail.Add("totalhours", viewModel.CmpTotalHours);
                        cratedValueDetail.Add("totaldays", viewModel.TotalDay);
                        cratedValueDetail.Add("remarks", viewModel.remarks);

                        try
                        {
                            SPConnector.AddListItem(SP_COMDET_LIST_NAME, cratedValueDetail, _siteUrl);
                        }
                        catch (Exception e)
                        {
                            logger.Error(e.Message);
                            throw e;
                        }
                    }

                    var updatedValue = new Dictionary<string, object>();

                    updatedValue.Add("Title", viewModel.CmpActiv);
                    updatedValue.Add("compensatorydate", viewModel.CmpDate);
                    updatedValue.Add("compensatorystarttime", viewModel.StartTime);
                    updatedValue.Add("compensatoryendtime", viewModel.FinishTime);
                    updatedValue.Add("totalhours", viewModel.CmpTotalHours);
                    updatedValue.Add("totaldays", viewModel.TotalDay);
                    updatedValue.Add("remarks", viewModel.remarks);

                    try
                    {
                        SPConnector.UpdateListItem(SP_APPDATA_LIST_NAME, cmpID, updatedValue, _siteUrl);

                    }
                    catch (Exception e)
                    {
                        logger.Error(e.Message);
                        throw e;
                    }
                    continue;
                }

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

