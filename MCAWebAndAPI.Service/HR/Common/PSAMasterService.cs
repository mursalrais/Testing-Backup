using System;
using System.Collections.Generic;
using MCAWebAndAPI.Model.HR.DataMaster;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;

namespace MCAWebAndAPI.Service.HR.Common
{
    public class PSAMasterService : IPSAMasterService
    {
        string _siteUrl;
        const string SP_PSAMSA_LIST_NAME = "PSA";

        public IEnumerable<PSAMaster> GetPSAs()
        {
            var models = new List<PSAMaster>();

            foreach (var item in SPConnector.GetList(SP_PSAMSA_LIST_NAME, _siteUrl))
            {
                models.Add(ConvertToPSAModel(item));
            }

            return models;
    }

        private PSAMaster ConvertToPSAModel(ListItem item)
        {
            return new PSAMaster
            {
                ID = item["professional_x003a_ID"] == null ? "" :
               Convert.ToString((item["professional_x003a_ID"] as FieldLookupValue).LookupValue),
                JoinDate = Convert.ToString(item["joindate"]),
                DateOfNewPSA = Convert.ToString(item["dateofnewpsa"]),
                PsaExpiryDate = Convert.ToString(item["psaexpirydate"]),
                ProjectOrUnit = Convert.ToString(item["ProjectOrUnit"]),
            };
        }

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
        }
    }
}
