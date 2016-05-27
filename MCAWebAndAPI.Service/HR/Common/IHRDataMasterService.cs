﻿using MCAWebAndAPI.Model.HR.DataMaster;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using System.Collections.Generic;
using System.Web;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Client;

namespace MCAWebAndAPI.Service.HR.Common
{
    public interface IHRDataMasterService
    {
        void SetSiteUrl(string siteUrl);

        /// <summary>
        /// Used to get professional master form
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        ProfessionalDataVM GetProfessionalData(int? ID);

        /// <summary>
        /// Used to get lightweight professional data, e.g., name, position.
        /// Suitable for combobox
        /// </summary>
        /// <returns></returns>
        IEnumerable<ProfessionalMaster> GetProfessionals();

        IEnumerable<ProfessionalMaster> GetProfessionalMonthlyFees();

        IEnumerable<PositionsMaster> GetPositions();
        int? CreateProfessionalData(ProfessionalDataVM viewModel);
        void CreateEducationDetails(int? headerID, IEnumerable<EducationDetailVM> educationDetails);
        void CreateTrainingDetails(int? headerID, IEnumerable<TrainingDetailVM> trainingDetails);
        void CreateDependentDetails(int? headerID, IEnumerable<HttpPostedFileBase> documents);
        void CreateOrganizationalDetails(int? headerID, IEnumerable<EducationDetailVM> educationDetails);
    }
}
