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
        private const string FIELD_NAME_ID = "ID";
        private const string FIELD_NAME_NO = "No";
        private const string FIELD_NAME_TITLE = "Title";
        private const string FIELD_NAME_LASTNAME = "lastname";
        private const string FIELD_NAME_POSITION = "Position";

        public IEnumerable<ProfessionalMaster> GetProfessionals()
        {
            var models = new List<ProfessionalMaster>();
            foreach (var item in SPConnector.GetList(SP_PROMAS_LIST_NAME, _siteUrl))
            {
                models.Add(ConvertToProfessionalModel(item));
            }

            return models;
        }

        public IEnumerable<AjaxComboBoxVM> GetEventBudgets()
        {
            var models = new List<AjaxComboBoxVM>();
            foreach (var item in SPConnector.GetList(SP_EBUDGET_LIST_NAME, _siteUrl))
            {
                models.Add(
                    new AjaxComboBoxVM
                    {
                        Value = Convert.ToInt32(item[FIELD_NAME_ID]),
                        Text = Convert.ToString(item[FIELD_NAME_NO]) + " - " + Convert.ToString(item[FIELD_NAME_TITLE])
                    }
                );
            }

            return models;
        }

        public IEnumerable<AjaxComboBoxVM> GetSubActivities(int activityID)
        {
            var models = new List<AjaxComboBoxVM>();
            var caml = @"<View><Query><Where><Eq><FieldRef Name='Activity_x003a_ID' /><Value Type='Lookup'>" + activityID.ToString() + "</Value></Eq></Where></Query></View>";

            foreach (var item in SPConnector.GetList(SP_SUB_ACTIVITY_LIST_NAME, _siteUrl, caml))
            {
                models.Add(
                    new AjaxComboBoxVM
                    {
                        Value = Convert.ToInt32(item[FIELD_NAME_ID]),
                        Text = Convert.ToString(item[FIELD_NAME_TITLE])
                    }
                );
            }

            return models;
        }

        private ProfessionalMaster ConvertToProfessionalModel(ListItem item)
        {
            return new ProfessionalMaster
            {
                ID = Convert.ToInt32(item[FIELD_NAME_ID]),
                Name = Convert.ToString(item[FIELD_NAME_TITLE]),
                Position = item[FIELD_NAME_POSITION] == null ? string.Empty : item[FIELD_NAME_POSITION].ToString()
            };
        }

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = siteUrl;
        }
    }
}
