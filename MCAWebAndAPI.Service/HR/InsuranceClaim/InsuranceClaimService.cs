using System;
using System.Collections.Generic;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using NLog;

namespace MCAWebAndAPI.Service.HR.InsuranceClaim
{
   public class InsuranceClaimService : IInsuranceClaimService
    {

        string _siteUrl;
        static Logger logger = LogManager.GetCurrentClassLogger();

        const string SP_HEADER_LIST_NAME = "InsuranceClaim";
        const string SP_COMPONENT_LIST_NAME = "ClaimComponent";
        const string SP_MEDIC_LIST_NAME = "MedicalClaim";
        public void SetSiteUrl(string siteUrl = null)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
        }

  
        private InsuranceClaimVM ConvertToApplicationDataVM(ListItem listItem)
        {

            var viewModel = new InsuranceClaimVM
            {
                Name = Convert.ToString(listItem["Title"]),
                ProfessionalName =
                {
                    Value = FormatUtil.ConvertLookupToID(listItem, "professional"),
                    Text = FormatUtil.ConvertLookupToValue(listItem, "professional")
                },
                ProfessionalID = FormatUtil.ConvertLookupToID(listItem, "professional_x003a_ID"),
                ClaimDate = Convert.ToDateTime(listItem["claimdate"]).ToLocalTime(),
                Type = {Text = Convert.ToString(listItem["claimtype"])}
            };


            return viewModel;
        }

        public InsuranceClaimVM GetInsurance(int? ID)
       {
            var viewModel = new InsuranceClaimVM();
           

            if (ID == null)
                return viewModel;

            viewModel = ConvertToApplicationDataVM(
                SPConnector.GetListItem(SP_HEADER_LIST_NAME, ID, _siteUrl));

            // Convert List Item to ViewModel
            return viewModel;
        }

       public InsuranceClaimVM GetPopulatedModel(int? id = null)
       {
            var model = new InsuranceClaimVM();
            return model;
        }

       public int CreateHeader(InsuranceClaimVM header)
       {
           var columnValues = new Dictionary<string, object>
           {
               {"professional", new FieldLookupValue {LookupId = Convert.ToInt32(header.ProfessionalName.Value)}},
               {"Title", header.Type.Text},
               {"claimtype", header.Type.Text},
               {"claimdate", header.ClaimDate}
           };

           try
            {
                SPConnector.AddListItem(SP_HEADER_LIST_NAME, columnValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }

            return SPConnector.GetLatestListItemID(SP_HEADER_LIST_NAME, _siteUrl);
        }

       public void CreateClaimComponentDetails(int? headerId, IEnumerable<ClaimComponentDetailVM> claimComponentDetails)
       {
            foreach (var viewModel in claimComponentDetails)
            {
                if (Item.CheckIfSkipped(viewModel))
                    continue;
                if (Item.CheckIfDeleted(viewModel))
                {
                    try
                    {
                        SPConnector.DeleteListItem(SP_COMPONENT_LIST_NAME, viewModel.ID, _siteUrl);

                    }
                    catch (Exception e)
                    {
                        logger.Error(e);
                        throw e;
                    }
                    continue;
                }
                var updatedValue = new Dictionary<string, object>();
                updatedValue.Add("insuranceclaim", new FieldLookupValue { LookupId = Convert.ToInt32(headerId) });
                updatedValue.Add("claimcomponenttype", viewModel.Type.Text);
                updatedValue.Add("claimcomponentcurrency", viewModel.Currency.Text);
                updatedValue.Add("claimcomponentamount", viewModel.Amount);
                updatedValue.Add("claimcomponentreceiptdate", viewModel.ReceiptDate);
                updatedValue.Add("claimcomponentremarks", viewModel.Remarks);
                try
                {
                    if (Item.CheckIfUpdated(viewModel))
                        SPConnector.UpdateListItem(SP_COMPONENT_LIST_NAME, viewModel.ID, updatedValue, _siteUrl);
                    else
                        SPConnector.AddListItem(SP_COMPONENT_LIST_NAME, updatedValue, _siteUrl);
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    //throw new Exception(ErrorResource.SPInsertError);
                    throw new Exception(e.Message);
                }
            }
        }

       public void CreateMedicalClaimDetails(int? headerId, IEnumerable<MedicalClaimDetailVM> medicalClaimDetails)
       {
            foreach (var viewModel in medicalClaimDetails)
            {
                if (Item.CheckIfSkipped(viewModel))
                    continue;
                if (Item.CheckIfDeleted(viewModel))
                {
                    try
                    {
                        SPConnector.DeleteListItem(SP_MEDIC_LIST_NAME, viewModel.ID, _siteUrl);

                    }
                    catch (Exception e)
                    {
                        logger.Error(e);
                        throw e;
                    }
                    continue;
                }
                var updatedValue = new Dictionary<string, object>
                {
                    {"insuranceclaim", new FieldLookupValue {LookupId = Convert.ToInt32(headerId)}},
                    {"medicalclaimtype", viewModel.Type.Text},
                    {"medicalclaimcurrency", viewModel.Currency.Text},
                    {"medicalclaimamount", viewModel.Amount},
                    {"medicalclaimreceiptdate", viewModel.ReceiptDate},
                    {"medicalclaimremarks", viewModel.Remarks},
                    {"medicalclaimWBS", viewModel.WBS},
                    {"medicalclaimGL", viewModel.GLCode}
                };
                try
                {
                    if (Item.CheckIfUpdated(viewModel))
                        SPConnector.UpdateListItem(SP_MEDIC_LIST_NAME, viewModel.ID, updatedValue, _siteUrl);
                    else
                        SPConnector.AddListItem(SP_MEDIC_LIST_NAME, updatedValue, _siteUrl);
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    //throw new Exception(ErrorResource.SPInsertError);
                    throw new Exception(e.Message);
                }
            }
        }
    }
}
