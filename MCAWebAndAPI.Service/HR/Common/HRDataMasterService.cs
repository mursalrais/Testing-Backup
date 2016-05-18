using System;
using System.Collections.Generic;
using MCAWebAndAPI.Model.HR.DataMaster;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;

namespace MCAWebAndAPI.Service.HR.Common
{
    public class HRDataMasterService : IHRDataMasterService
    {
        string _siteUrl;
        const string SP_PROMAS_LIST_NAME = "Professional Master";


        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
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
                ContactNo = Convert.ToString(item["ContactNo"]),
                Position = Convert.ToString(item["Position"]),
                ProjectUnit = Convert.ToString(item["ProjectUnit"])
            };
        }
    }
}
