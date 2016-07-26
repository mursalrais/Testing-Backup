using System;
using System.Collections.Generic;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.HR.DataMaster;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.Resources;
//using MCAWebAndAPI.Service.HR.Common;
//using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using NLog;

namespace MCAWebAndAPI.Service.HR.InsuranceClaim
{
    public class InsuranceClaimService : IInsuranceClaimService
    {

        string _siteUrl;
        static Logger logger = LogManager.GetCurrentClassLogger();

        const string SP_HEADER_LIST_NAME = "Insurance Claim";
        const string SP_COMPONENT_LIST_NAME = "Claim Component";
        const string SP_PAYMENT_LIST_NAME = "Claim Payment";
        public void SetSiteUrl(string siteUrl = null)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
        }


        private ClaimComponentDetailVM ConvertToComponentDetailVM(ListItem item)
        {
            return new ClaimComponentDetailVM
            {
                ID = Convert.ToInt32(item["ID"]),
                Amount = Convert.ToDouble(item["claimcomponentamount"]),
                ReceiptDate = Convert.ToDateTime(item["claimcomponentreceiptdate"]),
                Remarks = Convert.ToString(item["claimcomponentremarks"]),
                Currency = ClaimComponentDetailVM.GetCurrencyDefaultValue(
                     new Model.ViewModel.Control.InGridComboBoxVM
                     {
                         Text = Convert.ToString(item["claimcomponentcurrency"])
                     }),
                Type = ClaimComponentDetailVM.GetTypeDefaultValue(
                     new Model.ViewModel.Control.InGridComboBoxVM
                     {
                         Text = Convert.ToString(item["claimcomponenttype"])
                     }),
            };
        }

        private IEnumerable<ClaimComponentDetailVM> GetComponentDetails(int? ID)
        {
            var caml = @"<View><Query><Where><Eq><FieldRef Name='insuranceclaim' /><Value Type='Lookup'>" + ID.ToString() + "</Value></Eq></Where></Query></View>";

            var ComponentDetails = new List<ClaimComponentDetailVM>();
            foreach (var item in SPConnector.GetList(SP_COMPONENT_LIST_NAME, _siteUrl, caml))
            {
                ComponentDetails.Add(ConvertToComponentDetailVM(item));
            }

            return ComponentDetails;
        }

        private ProfessionalMaster GetProfessionalPosition(int? ID)
        {
            var models = new ProfessionalMaster();
            var item = SPConnector.GetListItem("Professional Master", ID, _siteUrl);

            models.Position = FormatUtil.ConvertLookupToValue(item, "Position");
            models.Project_Unit = Convert.ToString(item["Project_x002f_Unit"]);

            return models;


        }

        private DependentMaster GetDependent(int? ID)
        {
            var models = new DependentMaster();



            var item = SPConnector.GetListItem("Dependent", ID, _siteUrl);

            models.InsuranceNumber = Convert.ToString(item["insurancenr"]);
            models.OrganizationInsurance = Convert.ToString(item["insurancenr"]);


            return models;
        }

        private InsuranceClaimVM ConvertToInsuranceClaimVM(ListItem listItem)
        {

            var viewModel = new InsuranceClaimVM
            {

                ProfessionalName =
                {
                    Value = FormatUtil.ConvertLookupToID(listItem, "professional"),
                    Text = FormatUtil.ConvertLookupToValue(listItem, "professional")
                },
                DependantName =
                {
                    Value = FormatUtil.ConvertLookupToID(listItem, "dependent"),
                    Text = FormatUtil.ConvertLookupToValue(listItem, "dependent")
                },
                ProfessionalID = FormatUtil.ConvertLookupToID(listItem, "professional_x003a_ID"),
                ProfessionalTextName = FormatUtil.ConvertLookupToValue(listItem, "professional"),
                DependentID = FormatUtil.ConvertLookupToID(listItem, "dependent_x003a_ID"),
                ClaimDate = Convert.ToDateTime(listItem["claimdate"]).ToLocalTime(),
                ID = Convert.ToInt32(listItem["ID"]),
                ClaimStatus = Convert.ToString(listItem["claimstatus"]),
                Type =
                {
                    Text = Convert.ToString(listItem["claimtype"]),
                    Value = Convert.ToString(listItem["claimtype"])
                }
            };
            var professional = GetProfessionalPosition(viewModel.ProfessionalID);

            viewModel.Position = professional.Position;
        

            if (viewModel.ID != null)
            {
                viewModel.ClaimComponentDetails = GetComponentDetails(viewModel.ID);
            }

            if (viewModel.DependentID != null)
            {
                var dependent = GetDependent(viewModel.DependentID);
                viewModel.IndividualInsuranceNumber = dependent.InsuranceNumber;
                viewModel.OrganizationInsuranceID = dependent.OrganizationInsurance;
            }

            return viewModel;
        }

        public InsuranceClaimVM GetInsuranceHeader(int? ID, string useremail = null)
        {
            var viewModel = new InsuranceClaimVM();


            if (ID == null)
                return viewModel;

            viewModel = ConvertToInsuranceClaimVM(
                SPConnector.GetListItem(SP_HEADER_LIST_NAME, ID, _siteUrl));

            var caml = @"<View><Query><Where><Eq><FieldRef Name='officeemail' /><Value Type='Text'>" + useremail +
               "</Value></Eq></Where></Query></View>";

            foreach (var item in SPConnector.GetList("Professional Master", _siteUrl, caml))
            {
                var strUnit = Convert.ToString(item["Project_x002f_Unit"]);

                viewModel.UserPermission = strUnit == "Human Resources Unit" ? "HR" : "Professional";
            }


            return viewModel;
        }



        public InsuranceClaimVM GetPopulatedModel(string useremail = null)
        {
            var viewModel = new InsuranceClaimVM();


            if (useremail == null)
                return viewModel;


            var caml = @"<View><Query><Where><Eq><FieldRef Name='officeemail' /><Value Type='Text'>" + useremail +
                "</Value></Eq></Where></Query></View>";

            foreach (var item in SPConnector.GetList("Professional Master", _siteUrl, caml))
            {
                viewModel.ProfessionalID = Convert.ToInt32(item["ID"]);
                viewModel.ProfessionalTextName = Convert.ToString(item["Title"]);
                viewModel.ProfessionalName.Value = Convert.ToInt32(item["ID"]);
                viewModel.ProfessionalName.Text = Convert.ToString(item["Title"]);
                viewModel.Position = FormatUtil.ConvertLookupToValue(item, "Position");
                viewModel.VisibleTo = Convert.ToString(item["officeemail"]);
                var strUnit = Convert.ToString(item["Project_x002f_Unit"]);

                viewModel.UserPermission = strUnit == "Human Resources Unit" ? "HR" : "Professional";
            }

            return viewModel;

        }


        private void sendEmail(string strName,string strStatus,
            string strUserEmail=null,int? ID=null)
        {
            List<String> lstEmail = null;

            //string strName = "";

            //strName = viewModel.ProfessionalName.Text ?? viewModel.ProfessionalTextName;
            var strBody = "";
            if (strStatus == "Need HR to Validate")
            {
                var caml = @"<View><Query><Where><Eq><FieldRef Name='Project_x002f_Unit' />
                <Value Type='Text'>Human Resources Unit</Value></Eq></Where></Query></View>";

                foreach (var item in SPConnector.GetList("Professional Master", _siteUrl, caml))
                {
                    if (!string.IsNullOrEmpty(Convert.ToString(item["officeemail"])))
                    {
                        lstEmail.Add(Convert.ToString(item["officeemail"]));
                    }
                }

                if (lstEmail == null || lstEmail.Count <= 0) return;
               

                strBody += "Dear HR Team,<br><br>";
                strBody += @"This email is sent to you to notify that there is a request which required your action
                    to validate the insurance claim of " + strName + ". Please complete the validation process immediately.<br>";
                strBody += "<br>To view the detail, please click following link:<br>";
                strBody += _siteUrl + UrlResource.InsuranceClaim + "/editform_custom?ID="+ID;
                strBody += "<br><br>Thank you for your attention.";

                

                EmailUtil.SendMultiple(lstEmail, "Request for Validation of Insurance Claim", strBody);
            }
            else if (strStatus == "Need HR to Validate")
            {
                strBody += "Dear Mr/Ms " + strName +",<br><br>";
                strBody += @"This is to notify you that the Insurance Claim request has been validated by HR. 
                We will submit your claim immediately to AXA. 
                Please kindly check the latest status of your claim in the following link: <br>";
                strBody += _siteUrl + UrlResource.InsuranceClaim;
                strBody += "<br><br>Thank you for your attention.";

                EmailUtil.Send(strUserEmail, "Confirmation for Validation of Insurance Claim", strBody);
            }
        }



        public int CreateHeader(InsuranceClaimVM header)
        {
            var columnValues = new Dictionary<string, object>
           {
               {"claimtype", header.Type.Text},
               {"claimdate", header.ClaimDate},
               {"claimstatus", header.ClaimStatus}
           };



            if (header.ProfessionalName.Value != null)
            {
                columnValues.Add("professional",
                    new FieldLookupValue { LookupId = Convert.ToInt32(header.ProfessionalName.Value) });
            }
            else
            {
                columnValues.Add("professional", header.ProfessionalID);
            }

            if (header.DependantName.Value != null)
            {
                columnValues.Add("dependent", new FieldLookupValue { LookupId = Convert.ToInt32(header.DependantName.Value) });
            }
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
                var updatedValue = new Dictionary<string, object>
                {
                    {"insuranceclaim", new FieldLookupValue {LookupId = Convert.ToInt32(headerId)}},
                    {"claimcomponenttype", viewModel.Type.Text},
                    {"claimcomponentcurrency", viewModel.Currency.Text},
                    {"claimcomponentamount", viewModel.Amount},
                    {"claimcomponentreceiptdate", viewModel.ReceiptDate},
                    {"claimcomponentremarks", viewModel.Remarks}
                };
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

        public void CreateClaimPaymentDetails(int? headerId, IEnumerable<ClaimPaymentDetailVM> claimPaymentDetails)
        {
            foreach (var viewModel in claimPaymentDetails)
            {
                if (Item.CheckIfSkipped(viewModel))
                    continue;
                if (Item.CheckIfDeleted(viewModel))
                {
                    try
                    {
                        SPConnector.DeleteListItem(SP_PAYMENT_LIST_NAME, viewModel.ID, _siteUrl);

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
                    {"paymentstatus", viewModel.Type.Text},
                    {"paymentcurrency", viewModel.Currency.Text},
                    {"paymentamount", viewModel.Amount},
                    {"paymentdate", viewModel.ReceiptDate},
                    {"paymentremarks", viewModel.Remarks},
                    {"paymentwbsid", viewModel.WBS},
                    {"paymentglcode", viewModel.GLCode}
                };
                try
                {
                    if (Item.CheckIfUpdated(viewModel))
                        SPConnector.UpdateListItem(SP_PAYMENT_LIST_NAME, viewModel.ID, updatedValue, _siteUrl);
                    else
                        SPConnector.AddListItem(SP_PAYMENT_LIST_NAME, updatedValue, _siteUrl);
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    //throw new Exception(ErrorResource.SPInsertError);
                    throw new Exception(e.Message);
                }
            }
        }

        public bool UpdateHeader(InsuranceClaimVM header)
        {
            var columnValues = new Dictionary<string, object>();
            int? ID = header.ID;

            if (header.ProfessionalName.Value != null)
            {
                columnValues.Add("professional",
                    new FieldLookupValue { LookupId = Convert.ToInt32(header.ProfessionalName.Value) });
            }
            else
            {
                columnValues.Add("professional", header.ProfessionalID);
            }
            columnValues.Add("claimtype", header.Type.Text);
            columnValues.Add("claimdate", header.ClaimDate);
            columnValues.Add("claimstatus", header.ClaimStatus);

            if (header.Type.Text == "Dependent")
            {
                if (header.DependantName.Value != null)
                {
                    columnValues.Add("dependent",
                        new FieldLookupValue {LookupId = Convert.ToInt32(header.DependantName.Value)});
                }

            }
            else
            {
                columnValues.Add("dependent", null);
            }

            try
            {
                SPConnector.UpdateListItem(SP_HEADER_LIST_NAME, ID, columnValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                return false;
            }
            //var entitiy = new InsuranceClaimVM();
            //entitiy = header;
            return true;
        }
    }
}
