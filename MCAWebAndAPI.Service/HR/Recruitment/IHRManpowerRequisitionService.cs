﻿using MCAWebAndAPI.Model.ViewModel.Form.HR;
using System.Collections.Generic;
using System.Web;

namespace MCAWebAndAPI.Service.HR.Recruitment
{
    public interface IHRManpowerRequisitionService
    {
        void SetSiteUrl(string siteUrl = null);

        ManpowerRequisitionVM GetManpowerRequisition(int? ID);

        int CreateManpowerRequisition(ManpowerRequisitionVM viewModel);

        void CreateWorkingRelationshipDetails(int? headerID, IEnumerable<WorkingRelationshipDetailVM> workingRelationshipDetails);        

        void CreateManpowerRequisitionDocuments(int? headerID, IEnumerable<HttpPostedFileBase> documents);
    }
}