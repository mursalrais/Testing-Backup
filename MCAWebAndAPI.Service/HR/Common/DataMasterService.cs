using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.HR.DataMaster;

namespace MCAWebAndAPI.Service.HR.Common
{
    public class DataMasterService : IDataMasterService
    {
        string _siteUrl; 

        //TODO: Retrieve from SharePoint list
        public IEnumerable<ProfessionalMaster> GetProfessionals()
        {
            return new List<ProfessionalMaster> {
                new ProfessionalMaster
                {
                    ID = 1, 
                    Name = "Rahadian Dewandono", 
                    Position = "Technical"
                }, 
                new ProfessionalMaster
                {
                    ID = 2, 
                    Name = "Jecky Prasetyo", 
                    Position = "Functional"
                }
            };
        }

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = siteUrl;
        }
    }
}
