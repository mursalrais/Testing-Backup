using MCAWebAndAPI.Model.HR.DataMaster;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using System.Collections.Generic;
using System.Web;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Service.HR.Recruitment
{
    public interface IHRCompensatoryService
    {
        void SetSiteUrl(string siteUrl = null);

        CompensatoryVM GetComplistbyCmpid(int? iD);

        CompensatoryVM GetComplistbyProfid(int? iD);

        CompensatoryVM GetComplistActive();

        bool UpdateHeader(CompensatoryVM header);

        void CreateCompensatoryData(int? headerID, CompensatoryVM CompensatoryList);

        IEnumerable<CompensatoryMasterVM> GetCompensatoryId(int? idProf);

        Task<CompensatoryVM> GetCompensatoryDetailGrid(int? idComp);

    }
}
