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
using System.Threading.Tasks;
using System.Linq;

namespace MCAWebAndAPI.Service.HR.Recruitment
{
    public class HRCompensatoryService : IHRCompensatoryService
    {
        string _siteUrl;
        static Logger logger = LogManager.GetCurrentClassLogger();

        const string SP_APPDATA_LIST_NAME = "Application";
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

        private CompensatoryVM ConvertCompInputTolistDataVM(ListItem listItem, CompensatoryVM viewModel)
        {
            viewModel.cmpEmail = Convert.ToString(listItem["officeemail"]);
            viewModel.cmpName = Convert.ToString(listItem["Title"]) + Convert.ToString(listItem["lastname"]);
            viewModel.cmpProjUnit = Convert.ToString(listItem["Project_x002f_Unit"]);
            viewModel.cmpPosition = FormatUtil.ConvertLookupToValue(listItem, "Position");
            viewModel.ddlProfessional.Value = Convert.ToInt32(listItem["ID"]);
            viewModel.ddlProfessional.Text = Convert.ToString(listItem["Title"]);

            return viewModel;
        }

        public CompensatoryVM GetComplistbyCmpid(int? iD)
        {
            var viewModel = new CompensatoryVM();
            string crstatus = "";

            if (iD == null)
                return viewModel;

            var caml = @"<View>  
                    <Query> 
                       <Where>
                             <Eq>
                                <FieldRef Name='ID' />
                                <Value Type='Lookup'>" + iD + @"</Value>
                             </Eq>
                       </Where>
                    </Query> 
                     <ViewFields> 
                          <FieldRef Name='Title' />
                          <FieldRef Name='professional_x003a_ID' />
                          <FieldRef Name='crstatus' />
                          <FieldRef Name='Created' />
                     </ViewFields> 
                    </View>";

            var profID = 0;
            foreach (var item in SPConnector.GetList(SP_COMREQ_LIST_NAME, _siteUrl, caml))
            {
                profID = Convert.ToInt32(FormatUtil.ConvertLookupToID(item, "professional_x003a_ID") + string.Empty);
                crstatus = Convert.ToString(item["crstatus"]);
                viewModel.cmpTitle = Convert.ToString(item["Title"]);
                viewModel.cmpYearDate = Convert.ToString(item["Created"]);
            }

            return GetComplisted(iD, profID, crstatus, viewModel);
        }

        public CompensatoryVM GetComplistbyProfid(int? iD)
        {
            var viewModel = new CompensatoryVM();
            string crstatus = "";

            if (iD == null)
                return null;

            var caml = @"<View>  
                    <Query> 
                       <Where>
                             <Eq>
                                <FieldRef Name='professional_x003a_ID' />
                                <Value Type='Lookup'>" + iD + @"</Value>
                             </Eq>
                       </Where>
                    </Query> 
                     <ViewFields> 
                          <FieldRef Name='Title' />
                          <FieldRef Name='ID' />
                          <FieldRef Name='Created' />
                          <FieldRef Name='crstatus' />
                     </ViewFields> 
                    </View>";

            var cmpID = 0;
            foreach (var item in SPConnector.GetList(SP_COMREQ_LIST_NAME, _siteUrl, caml))
            {
                cmpID = Convert.ToInt32(item["ID"]);
                crstatus = Convert.ToString(item["crstatus"]);
                viewModel.cmpTitle = Convert.ToString(item["Title"]);
                viewModel.cmpYearDate = Convert.ToString(item["Created"]);
            }

            if (crstatus == "")
            {
                var updatedValue = new Dictionary<string, object>();

                updatedValue.Add("crstatus", "Pending Approval 1 of 2");

                try
                {
                    SPConnector.UpdateListItem(SP_COMREQ_LIST_NAME, iD, updatedValue, _siteUrl);
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw e;
                }
            }

            return GetComplisted(cmpID, iD, crstatus, viewModel);
        }

        private int GetCompID(int? ID)
        {
            var viewModel = new CompensatoryVM();
            string appstatus;

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
                       <FieldRef Name='ID' /> 
                       <FieldRef Name='crstatus' />
                     </ViewFields> 
                    </View>";

            var compID = 0;
            foreach (var item in SPConnector.GetList(SP_COMREQ_LIST_NAME, _siteUrl, caml))
            {
                appstatus = Convert.ToString(item["crstatus"]); 
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
                    viewModel.cmpProjUnit = Convert.ToString(item2["Project_x002f_Unit"]);
                    viewModel.cmpPosition = FormatUtil.ConvertLookupToValue(item2, "Position");
                    viewModel.CompensatoryDetails = GetCompDetailist(GetCompID(Convert.ToInt32(item2["ID"])), viewModel);
                }

            }
            return viewModel;
        }

        private CompensatoryVM GetComplisted(int? ID, int? idPro, string crstatus, CompensatoryVM viewModel)
        {
            if (idPro == 0)
                return viewModel;

            var listItem = SPConnector.GetListItem(SP_PROMAS_LIST_NAME, idPro, _siteUrl);
            viewModel.ddlProfessional.Value = Convert.ToInt32(idPro);
            viewModel = ConvertCompInputTolistDataVM(listItem, viewModel);
            viewModel.cmpID = ID;
            viewModel.ddlCompensatoryID.Value = ID;
            viewModel.StatusForm = crstatus;
            viewModel.CompensatoryDetails = GetCompDetailist(ID, viewModel);

            return viewModel;
        }

        //<ViewFields>
        //   <FieldRef Name = 'Title' />
        //   < FieldRef Name='applications' />
        //   <FieldRef Name = 'university' />
        //   < FieldRef Name='yearofgraduation' />
        //   <FieldRef Name = 'remarks' />
        //</ ViewFields >
        private IEnumerable<CompensatoryDetailVM> GetCompDetailist(int? ID, CompensatoryVM viewModel)
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
                CmpHID = Convert.ToInt32(FormatUtil.ConvertLookupToValue(item, "compensatoryrequest")),
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

        public void CreateHeaderCompensatory(CompensatoryVM viewModels)
        {
            var cratedValueDetail = new Dictionary<string, object>();

            
            cratedValueDetail.Add("professional", new FieldLookupValue { LookupId = Convert.ToInt32(viewModels.ddlProfessional.Value)});
            cratedValueDetail.Add("Title", Convert.ToString(viewModels.cmpYearDate));
            
            try
            {
                SPConnector.AddListItem(SP_COMREQ_LIST_NAME, cratedValueDetail, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                throw e;
            }

           int idCmp = SPConnector.GetLatestListItemID(SP_COMREQ_LIST_NAME, _siteUrl);

            CreateCompensatoryData(idCmp, viewModels);
        }

        public void CreateCompensatoryData(int? cmpID, CompensatoryVM viewModels)
        {
            foreach (var viewModel in viewModels.CompensatoryDetails)
            {
                if (viewModel.CmpActiv != null)
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
                        else
                        {
                            var updatedValue = new Dictionary<string, object>();

                            updatedValue.Add("Title", viewModel.CmpActiv);
                            updatedValue.Add("compensatoryrequest", viewModel.CmpHID);
                            updatedValue.Add("compensatorydate", viewModel.CmpDate);
                            updatedValue.Add("compensatorystarttime", viewModel.StartTime);
                            updatedValue.Add("compensatoryendtime", viewModel.FinishTime);
                            updatedValue.Add("totalhours", viewModel.CmpTotalHours);
                            updatedValue.Add("totaldays", viewModel.TotalDay);
                            updatedValue.Add("remarks", viewModel.remarks);

                            try
                            {
                                SPConnector.UpdateListItem(SP_COMDET_LIST_NAME, viewModel.CmpID, updatedValue, _siteUrl);

                            }
                            catch (Exception e)
                            {
                                logger.Error(e.Message);
                                throw e;
                            }
                        }
                        continue;
                    }
                        else if (Item.CheckIfCreated(viewModel))
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
                        else if (Item.CheckIfDeleted(viewModel))
                    {
                        try
                        {
                            SPConnector.DeleteListItem(SP_COMDET_LIST_NAME, viewModel.CmpID, _siteUrl);
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
        }

        public IEnumerable<CompensatoryMasterVM> GetCompensatoryIdbyProf(int? idProf)
        {
            var caml = @"<View>  
                    <Query> 
                        <Where>
                          <Eq>
                             <FieldRef Name='professional_x003a_ID' />
                             <Value Type='Lookup'>"+ idProf + @"</Value>
                          </Eq>
                       </Where>
                    </Query> 
                    <ViewFields>
                       <FieldRef Name='Title' />
                       <FieldRef Name='submitteddate' />
                       <FieldRef Name='crstatus' />
                       <FieldRef Name='ID' />
                    </ViewFields></View>";

            var models = new List<CompensatoryMasterVM>();

            foreach (var item in SPConnector.GetList(SP_COMREQ_LIST_NAME, _siteUrl, caml))
            {
                models.Add(ConvertToCompensatoryMasterModel(item));
            }

            models = models.OrderBy(e => e.CompensatoryDate).ToList();
            return models;

        }

        public IEnumerable<CompensatoryMasterVM> GetCompensatoryId(int? idComp)
        {
            var caml = @"<View>  
                    <Query> 
                        <Where>
                          <Eq>
                             <FieldRef Name='ID' />
                             <Value Type='Counter'>"+ idComp + @"</Value>
                          </Eq>
                       </Where>
                    </Query> 
                    <ViewFields>
                       <FieldRef Name='Title' />
                       <FieldRef Name='submitteddate' />
                       <FieldRef Name='crstatus' />
                       <FieldRef Name='ID' />
                    </ViewFields></View>";

            var models = new List<CompensatoryMasterVM>();

            foreach (var item in SPConnector.GetList(SP_COMREQ_LIST_NAME, _siteUrl, caml))
            {
                models.Add(ConvertToCompensatoryMasterModel(item));
            }

            models = models.OrderBy(e => e.CompensatoryDate).ToList();
            return models;

        }

        /// <summary>
        /// 
        /// </summary>
        private CompensatoryMasterVM ConvertToCompensatoryMasterModel(ListItem item)
        {
            var viewModel = new CompensatoryMasterVM();

            viewModel.ID = Convert.ToInt32(item["ID"]);
            viewModel.CompensatoryID = Convert.ToInt32(item["ID"]);
            viewModel.CompensatoryDate = Convert.ToDateTime(item["submitteddate"]);
            viewModel.CompensatoryTitle = Convert.ToString(item["Title"]);
            viewModel.CompensatoryStatus = Convert.ToString(item["crstatus"]);

            return viewModel;
        }

        public bool UpdateHeader(CompensatoryVM header)
        {
            var columnValues = new Dictionary<string, object>();

            int? ID = header.cmpID;

            if (header.StatusForm == " ")
            {
                columnValues.Add("crstatus", "Pending Approval 1 of 2");
            }

            if (header.StatusForm == "Draft" || header.StatusForm == "Unapprove")
            {
                columnValues.Add("crstatus", "Draft");
            }

            if (header.StatusForm == "Reject")
            {
                columnValues.Add("crstatus", "Rejected");
            }

            if (header.StatusForm == "Pending Approval 1 of 2")
            {
                columnValues.Add("crstatus", "Pending Approval 2 of 2");
            }

            if (header.StatusForm == "Pending Approval 2 of 2" || header.StatusForm == "submithr")
            {
                columnValues.Add("crstatus", "Approved");

                IEnumerable<CompensatoryDetailVM> getbalance = header.CompensatoryDetails;

                columnValues.Add("balance", getbalance.Count()); 
            }

            try
            {
                SPConnector.UpdateListItem(SP_COMREQ_LIST_NAME, ID, columnValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                return false;
            }
            var entitiy = new CompensatoryVM();
            entitiy = header;
            return true;
        }


       public async Task<CompensatoryVM> GetCompensatoryDetailGrid(int? idComp)
        {
            var viewModel = new CompensatoryVM();

            var caml = @"<View>  
                              <Query> 
                            <Where>
                             <Eq>
                                <FieldRef Name='compensatoryrequest' />
                                <Value Type='Lookup'>" + idComp + @"</Value>
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

            var compensatorylistDetails = new List<CompensatoryDetailVM>();
            foreach (var item in SPConnector.GetList(SP_COMDET_LIST_NAME, _siteUrl, caml))
            {
                compensatorylistDetails.Add(ConvertToCompDetailVM(item));
            }

            viewModel.CompensatoryDetails = compensatorylistDetails;

            return viewModel;
        }

        public string GetPosition(string username)
        {
            var caml = @"<View><Query><Where><Eq><FieldRef Name='officeemail' /><Value Type='Text'>" + username + @"</Value></Eq></Where></Query><ViewFields><FieldRef Name='Position' /></ViewFields><QueryOptions /></View>";
            var listItem = SPConnector.GetList("Professional Master", _siteUrl, caml);
            string position = "";
            foreach (var item in listItem)
            {
                position = FormatUtil.ConvertLookupToValue(item, "Position");
            }
            if (position == null)
            {
                position = "";
            }
            return position;
        }

        public int? GetProfid(string username)
        {
            var caml = @"<View><Query><Where><Eq><FieldRef Name='officeemail' /><Value Type='Text'>" + username + @"</Value></Eq></Where></Query><ViewFields><FieldRef Name='ID' /></ViewFields><QueryOptions /></View>";
            var listItem = SPConnector.GetList("Professional Master", _siteUrl, caml);
            int? position = null;

            foreach (var item in listItem)
            {
                position = FormatUtil.ConvertLookupToID(item, "ID");
            }
           
            return position;
        }

        public void SendEmail(string workflowTransactionListName,
            string transactionLookupColumnName, int headerID, int level, string message)
        {
            var caml = @"<View>  
            <Query> 
               <Where><And><Eq><FieldRef Name='" + transactionLookupColumnName + @"' />
               <Value Type='Lookup'>" + headerID + @"</Value></Eq><Eq>
               <FieldRef Name='approverlevel' /><Value Type='Choice'>" + level + @"</Value></Eq></And></Where> 
            </Query> 
            </View>";

            var emails = new List<string>();
            foreach (var item in SPConnector.GetList(workflowTransactionListName, _siteUrl, caml))
            {
                emails.Add(Convert.ToString(item["approver0"]));
            }
            foreach (var item in emails)
            {
                EmailUtil.Send(item, "Ask for Approval", message);
            }
        }
    }
}

