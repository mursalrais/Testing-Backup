using MCAWebAndAPI.Model.ViewModel.Form.HR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.HR.DataMaster;
using System.Web;


namespace MCAWebAndAPI.Service.HR.Recruitment
{
    public interface IPSAManagementService
    {
        void SetSiteUrl(string siteUrl);

        IEnumerable<PSAMaster> GetPSAs();
        IEnumerable<PSAMaster> GetPSAs(DateTime period);

        int CreatePSAManagement(PSAManagementVM psaManagement);

        PSAManagementVM GetPSAManagement(int? ID);

        PSAManagementVM ViewPSAManagementData(int? ID);

        bool UpdatePSAManagement(PSAManagementVM psaManagement);

        void CreatePSAManagementDocuments(int? headerID, IEnumerable<HttpPostedFileBase> documents, PSAManagementVM psaManagement);

        IEnumerable<PSAManagementVM> GetRenewalNumber(int? professionalID);

        //PSAManagementVM GetRenewalNumber(int? professionalID);

        IEnumerable<PSAManagementVM> GetJoinDate(int? professionalID);

        bool UpdateStatusPSA(PSAManagementVM psaManagement);

        void SendMailPerformancePlan(int? professionalID, string siteUrl, DateTime today);

        string GetProfessionalFullName(int? professionalID);

        bool UpdateProfessionalFromPSA(PSAManagementVM psaManagement, int? psaID);

        int GetPSALatestID(string _siteUrl);

        bool UpdateStatusPSABefore(int? psaRenewalNumberMinusOne, string professionalName);

        string GetProfessionalName(int? professionalID);

        IEnumerable<PositionMaster> GetPosition(string ProjectUnit);
    }
}
