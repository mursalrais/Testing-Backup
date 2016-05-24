using System;
using System.Collections.Generic;
using MCAWebAndAPI.Model.HR.DataMaster;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using System.Linq;

namespace MCAWebAndAPI.Service.HR.Common
{
    public class HRDataMasterService : IHRDataMasterService
    {
        string _siteUrl;
        const string SP_PROMAS_LIST_NAME = "Professional Master";
        const string SP_POSMAS_LIST_NAME = "Position Master";
        const string SP_MONFEE_LIST_NAME = "Monthly Fee";

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
        }

        public IEnumerable<ProfessionalMaster> GetProfessionalMonthlyFees()
        {
            var models = new List<ProfessionalMaster>();
            int tempID;
            List<int> collectionIDMonthlyFee = new List<int>();
            foreach (var item in SPConnector.GetList(SP_MONFEE_LIST_NAME, _siteUrl))
            {
                collectionIDMonthlyFee.Add(Convert.ToInt32(item["ProfessionalId"]));
            }
            foreach (var item in SPConnector.GetList(SP_PROMAS_LIST_NAME, _siteUrl))
            {
                tempID = Convert.ToInt32(item["ID"]);
                if (!(collectionIDMonthlyFee.Any(e => e == tempID)))
                {
                    models.Add(ConvertToProfessionalModel(item));
                }
            }

            return models;
        }

        public IEnumerable<ProfessionalMaster> GetProfessionals()
        {
            var models = new List<ProfessionalMaster>();
            foreach(var item in SPConnector.GetList(SP_PROMAS_LIST_NAME, _siteUrl))
            {
                    models.Add(ConvertToProfessionalModel(item));
            }

            return models;
        }

        private ProfessionalMaster ConvertToProfessionalModel(ListItem item)
        {
            return new ProfessionalMaster
            {
                ID = Convert.ToInt32(item["ID"]),
                Name = Convert.ToString(item["Title"]),
                Status = Convert.ToString(item["maritalstatus"]),
                Position = item["Position"] == null ? "" :
               Convert.ToString((item["Position"] as FieldLookupValue).LookupValue)
            };
        }

        public IEnumerable<PositionsMaster> GetPositions()
        {
            var models = new List<PositionsMaster>();

            foreach (var item in SPConnector.GetList(SP_POSMAS_LIST_NAME, _siteUrl))
            {
                models.Add(ConvertToPositionsModel(item));
            }

            return models;
        }

        private PositionsMaster ConvertToPositionsModel(ListItem item)
        {
            return new PositionsMaster
            {
                ID = Convert.ToInt32(item["ID"]),
                Title = Convert.ToString(item["Title"]),
                //PositionStatus = Convert.ToString(item["positionstatus"]),
                //Position = Convert.ToString(item["Position"]),
                //ProjectUnit = Convert.ToString(item["ProjectUnit"])
            };
        }
    }
}
