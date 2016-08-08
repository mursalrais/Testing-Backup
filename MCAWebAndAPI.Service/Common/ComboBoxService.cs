using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.HR.DataMaster;
using MCAWebAndAPI.Model.ViewModel.Control;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;

namespace MCAWebAndAPI.Service.Common
{
    public class ComboBoxService : IComboBoxService
    {
        private string _siteUrl;
        private const string SP_PROMAS_LIST_NAME = "Professional Master";
        private const string SP_EBUDGET_LIST_NAME = "Event Budget";
        private const string SP_SUB_ACTIVITY_LIST_NAME = "Sub Activity";

        public IEnumerable<ProfessionalMaster> GetProfessionals()
        {
            var models = new List<ProfessionalMaster>();
            foreach (var item in SPConnector.GetList(SP_PROMAS_LIST_NAME, _siteUrl))
            {
                models.Add(ConvertToProfessionalModel(item));
            }

            return models;
        }

        public IEnumerable<AjaxComboBoxVM> GetEventBudget()
        {
            var models = new List<AjaxComboBoxVM>();
            foreach (var item in SPConnector.GetList(SP_EBUDGET_LIST_NAME, _siteUrl))
            {
                models.Add(
                    new AjaxComboBoxVM
                    {
                        Value = Convert.ToInt32(item["ID"]),
                        Text = Convert.ToString(item["No"]) + " - " + Convert.ToString(item["Project"])
                    }
                );
            }

            return models;
        }

        public IEnumerable<AjaxComboBoxVM> GetSubActivity(int activityID)
        {
            var models = new List<AjaxComboBoxVM>();
            var caml = @"<View><Query><Where><Eq><FieldRef Name='Activity' /><Value Type='Lookup'>" + activityID.ToString() + "</Value></Eq></Where></Query></View>";

            foreach (var item in SPConnector.GetList(SP_SUB_ACTIVITY_LIST_NAME, _siteUrl, caml))
            {
                models.Add(
                    new AjaxComboBoxVM
                    {
                        Value = Convert.ToInt32(item["ID"]),
                        Text = Convert.ToString(item["Title"])
                    }
                );
            }

            return models;
        }

        private ProfessionalMaster ConvertToProfessionalModel(ListItem item)
        {
            return new ProfessionalMaster
            {
                ID = Convert.ToInt32(item["ID"]),
                FirstMiddleName = Convert.ToString(item["Title"]),
                Name = Convert.ToString(item["Title"]) + " " + Convert.ToString(item["lastname"]),
                Status = Convert.ToString(item["maritalstatus"]),
                Position = item["Position"] == null ? string.Empty :
                        Convert.ToString((item["Position"] as FieldLookupValue).LookupValue),
                Project_Unit = Convert.ToString(item["Project_x002f_Unit"]),
                OfficeEmail = Convert.ToString(item["officeemail"]),
                PSANumber = Convert.ToString(item["PSAnumber"]),
                JoinDateTemp = Convert.ToDateTime(item["Join_x0020_Date"]).ToLocalTime().ToShortDateString()
            };
        }

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = siteUrl;
        }
    }
}
