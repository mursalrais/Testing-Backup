using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.HR.DataMaster;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;

namespace MCAWebAndAPI.Service.HR.Common
{
    public class DataMasterService : IDataMasterService
    {
        string _siteUrl;
        const string SP_PROMAS_LIST_NAME = "Professional Master";
        const string SP_POSMAS_LIST_NAME = "Position Master";
        
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
