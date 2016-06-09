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
        const string SP_APPEDU_LIST_NAME = "Application Education";
        const string SP_APPWORK_LIST_NAME = "Application Working Experience";
        const string SP_APPTRAIN_LIST_NAME = "Application Training";
        const string SP_APPDOC_LIST_NAME = "Application Documents";

        const string SP_PROMAS_LIST_NAME = "Professional Master";
        const string SP_POSMAS_LIST_NAME = "Position Master";

        const string SP_MANPOW_LIST_NAME = "Manpower Requisition";

        private ApplicationShortlistVM ConvertToApplicationDataVM(ListItem listItem)
        {
            var viewModel = new ApplicationShortlistVM();

            viewModel.ID = Convert.ToInt32(listItem["ID"]);
            viewModel.Candidate = Convert.ToString(listItem["placeofbirth"]);
            viewModel.EditMode = Convert.ToInt32(listItem["dateofbirth"]);
            //viewModel.EmailMessage = Convert.ToString(listItem["permanentaddress"]);
            //viewModel.InterviewerDate = Convert.ToString(listItem["idcardexpirydate"]);
            //viewModel.InterviewerPanel = Convert.ToString(listItem["idcardnumber"]);
            //viewModel.SendTo = Convert.ToString(listItem["idcardexpirydate"]);
            //viewModel.Time = Convert.ToString(listItem["idcardexpirydate"]);
            viewModel.Title = Convert.ToString(listItem["idcardexpirydate"]);

            // Convert Details
            //viewModel.ShortlistDetail = GetDetailShortlist();
            //viewModel.DocumentUrl = GetDocumentUrl(viewModel.ID);

            return viewModel;
        }

        public ApplicationShortlistVM GetShortlist(string Position)
        {
            var viewModel = new ApplicationShortlistVM();
            if (Position == null)
            return viewModel;

            viewModel.ShortlistDetail = GetDetailShortlist(Position);

            return viewModel;

        }

        //<ViewFields>
        //   <FieldRef Name = 'Title' />
        //   < FieldRef Name='applications' />
        //   <FieldRef Name = 'university' />
        //   < FieldRef Name='yearofgraduation' />
        //   <FieldRef Name = 'remarks' />
        //</ ViewFields >
        private IEnumerable<ShortlistDetailVM> GetDetailShortlist(string Position)
        {
            var caml = @"<View>  
            <Query> 
   <Where>
      <Eq>
         <FieldRef Name='position' />
         <Value Type='Text'>AD, Community-based Renewable Energy</Value>
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
            return new ShortlistDetailVM
            {
                Candidate = Convert.ToString(item["Title"]),
                ID = Convert.ToInt32(item["ID"]),
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

        public IEnumerable<ApplicationShortlistVM> GetShortlists()
        {
            throw new NotImplementedException();
        }

        public void SetSiteUrl(string siteUrl = null)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
        }

        public int? EditShortlistData(ShortlistDetailVM viewModel)
        {
            var updatedValue = new Dictionary<string, object>();

            updatedValue.Add("Candidate", viewModel.Candidate);
            updatedValue.Add("CV", viewModel.CV);
            updatedValue.Add("EditMode", viewModel.EditMode);
            updatedValue.Add("ID", viewModel.ID);
            updatedValue.Add("Remarks", viewModel.Remarks);
            updatedValue.Add("Status", viewModel.Status);
            updatedValue.Add("Title", viewModel.Title);


            try
            {
                SPConnector.UpdateListItem(SP_PROMAS_LIST_NAME, viewModel.ID, updatedValue);
            }
            catch (Exception e)
            {
                logger.Error(e);
                throw e;
            }

            return viewModel.ID;
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
                updatedValue.Add("Candidate", viewModel.Candidate);
                updatedValue.Add("CV", viewModel.CV);
                updatedValue.Add("EditMode", viewModel.EditMode);
                updatedValue.Add("ID", viewModel.ID);
                updatedValue.Add("Remarks", viewModel.Remarks);
                updatedValue.Add("Status", viewModel.Status);
                updatedValue.Add("Title", viewModel.Title);

                try
                {
                    if (Item.CheckIfUpdated(viewModel))
                        SPConnector.UpdateListItem(SP_APPDATA_LIST_NAME, viewModel.ID, updatedValue, _siteUrl);
                    else
                        SPConnector.AddListItem(SP_APPDATA_LIST_NAME, updatedValue, _siteUrl);
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw e;
                }
            }
        }

    }
}
