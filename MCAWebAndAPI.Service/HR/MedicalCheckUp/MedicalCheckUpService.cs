using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security;

//using System.Reflection;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.HR.DataMaster;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.Resources;
//using MCAWebAndAPI.Service.HR.Common;
//using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using NLog;

namespace MCAWebAndAPI.Service.HR.MedicalCheckUp
{
    public class MedicalCheckUpService : IMedicalCheckUpService
    {

        string _siteUrl;
        static Logger logger = LogManager.GetCurrentClassLogger();

        const string SpMedicalListName = "Medical Check Up";
      
        public void SetSiteUrl(string siteUrl = null)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
        }

        public void CreateMedical(MedicalCheckUpVM header)
        {
            var columnValues = new Dictionary<string, object>
           {
               {"Title", header.ProfessionalTextName},
               {"position", header.Position},
               {"unit", header.Unit},
               {"claimdate", header.ClaimDate},
               {"status", header.ClaimStatus},
               {"claimamount", header.Amount},
               {"Year", header.ClaimDate.Value.Year},
               {"remarks", header.Remarks},
               {"officeemail", header.OfficeEmail}
           };

            if (header.ProfessionalName.Value != null)
            {
                columnValues.Add("professional",
                    new FieldLookupValue { LookupId = Convert.ToInt32(header.ProfessionalName.Value) });
                header.ProfessionalID = Convert.ToInt32(header.ProfessionalName.Value);
            }
            else
            {
                columnValues.Add("professional", header.ProfessionalID);
            }

            columnValues.Add("visibleto", SPConnector.GetUser(header.OfficeEmail, _siteUrl));

            SPConnector.AddListItem(SpMedicalListName, columnValues, _siteUrl);

        }

        public void UpdateMedical(MedicalCheckUpVM header)
        {

            var columnValues = new Dictionary<string, object>();
            int? id = header.ID;

            if (header.ProfessionalName.Value != null)
            {
                columnValues.Add("professional",
                    new FieldLookupValue { LookupId = Convert.ToInt32(header.ProfessionalName.Value) });
            }
            else
            {
                columnValues.Add("professional", header.ProfessionalID);
            }
            columnValues.Add("Title", header.ProfessionalTextName);
            columnValues.Add("position", header.Position);
            columnValues.Add("unit", header.Position);
            columnValues.Add("claimdate", header.ClaimDate);
            columnValues.Add("status", header.ClaimStatus);
            columnValues.Add("claimamount", header.Amount);
            columnValues.Add("Year", header.ClaimDate.Value.Year);
            columnValues.Add("remarks", header.Remarks);
            columnValues.Add("officeemail", header.OfficeEmail);

            try
            {
                SPConnector.UpdateListItem(SpMedicalListName, id, columnValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                
            }

        }

        public MedicalCheckUpVM GetPopulatedModel(string useremail = null)
        {
            var viewModel = new MedicalCheckUpVM();


            if (useremail == null)
                return viewModel;


            var caml = @"<View><Query><Where><Eq><FieldRef Name='officeemail' /><Value Type='Text'>" + useremail +
                "</Value></Eq></Where></Query></View>";

            foreach (var item in SPConnector.GetList("Professional Master", _siteUrl, caml))
            {
                viewModel.ProfessionalID = Convert.ToInt32(item["ID"]);
                viewModel.ProfessionalTextName = Convert.ToString(item["Title"]) + " " + Convert.ToString(item["lastname"]);
                viewModel.ProfessionalName.Value = Convert.ToInt32(item["ID"]);
                viewModel.ProfessionalName.Text = Convert.ToString(item["Title"]);
                viewModel.Position = FormatUtil.ConvertLookupToValue(item, "Position");
                viewModel.Unit = Convert.ToString(item["Project_x002f_Unit"]);
                viewModel.OfficeEmail = Convert.ToString(item["officeemail"]);
                var strUnit = Convert.ToString(item["Project_x002f_Unit"]);
                viewModel.UserPermission = strUnit == "Human Resources Unit" ? "HR" : "Professional";

            }

            return viewModel;
        }

        public MedicalCheckUpVM GetMedical(int? ID, string useremail = null)
        {
          
            var viewModel = new MedicalCheckUpVM();


            if (ID == null)
                return viewModel;

            viewModel = ConvertToMedicalVm(
                SPConnector.GetListItem(SpMedicalListName, ID, _siteUrl));

            var caml = @"<View><Query><Where><Eq><FieldRef Name='officeemail' /><Value Type='Text'>" + useremail +
              "</Value></Eq></Where></Query></View>";

            foreach (var item in SPConnector.GetList("Professional Master", _siteUrl, caml))
            {
                var strUnit = Convert.ToString(item["Project_x002f_Unit"]);

                viewModel.UserPermission = strUnit == "Human Resources Unit" ? "HR" : "Professional";
            }

       
            return viewModel;
        
    }

        public IEnumerable<MedicalCheckUpVM> GetMedicalByUser(int? id)
        {
            var models = new List<MedicalCheckUpVM>();

          
            var caml = @"<View><Query><Where><Eq><FieldRef Name='professional' />
                        <Value Type='Lookup'>" + id +
                       "</Value></Eq></Where></Query></View>";

            foreach (var item in SPConnector.GetList(SpMedicalListName, _siteUrl, caml))
            {
                var detail = new MedicalCheckUpVM
                {
                    ID = Convert.ToInt32(item["ID"]),Year = Convert.ToString(item["Year"]),
                    ClaimStatus = Convert.ToString(item["status"])
                };
                models.Add(detail);
            }

            return models;
        }

        private MedicalCheckUpVM ConvertToMedicalVm(ListItem listItem)
        {

           

            var viewModel = new MedicalCheckUpVM
            {

                ProfessionalName =
                {
                    Value = FormatUtil.ConvertLookupToID(listItem, "professional"),
                    Text = Convert.ToString(listItem["Title"])
                },
                ProfessionalTextName = Convert.ToString(listItem["Title"]),
                ProfessionalID = FormatUtil.ConvertLookupToID(listItem, "professional"),
                Position = Convert.ToString(listItem["position"]),
                Unit = Convert.ToString(listItem["unit"]),

                ClaimDate = Convert.ToDateTime(listItem["claimdate"]).ToLocalTime(),

                Amount = Convert.ToDouble(listItem["claimamount"]),
                Remarks = Convert.ToString(listItem["remarks"]),
                OfficeEmail = Convert.ToString(listItem["officeemail"]),

                ID = Convert.ToInt32(listItem["ID"]),


                ClaimStatus = Convert.ToString(listItem["status"]),
                Year = Convert.ToString(listItem["Year"]),

                URL = _siteUrl
            };


            viewModel.UserPermission = viewModel.Unit == "Human Resources Unit" ? "HR" : "Professional";

            return viewModel;
        }

    }
}
