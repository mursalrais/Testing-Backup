﻿using MCAWebAndAPI.Model.HR.DataMaster;
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

        CompensatoryVM GetProfessional(string userAccess);
        
        bool UpdateHeader(CompensatoryVM header);

        void CreateCompensatoryData(int? headerID, CompensatoryVM CompensatoryList);

        void CreateHeaderCompensatory(CompensatoryVM CompensatoryList);

        IEnumerable<CompensatoryMasterVM> GetCompensatoryId(int? idComp);

        IEnumerable<CompensatoryMasterVM> GetCompensatoryIdbyProf(int? idProf);

        Task<CompensatoryVM> GetCompensatoryDetailGrid(int? idComp);

        string GetPosition(string username = null);

        void SendEmail(string workflowTransactionListName, string transactionLookupColumnName,
          int headerID, int level, string message);
       
    }
}
