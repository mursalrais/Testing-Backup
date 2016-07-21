using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.HR;


namespace MCAWebAndAPI.Service.HR.InsuranceClaim
{
   public interface IInsuranceClaimService
    {

        void SetSiteUrl(string siteUrl = null);

       // int CreateInsuranceClaim(InsuranceClaimVM viewModel);

        InsuranceClaimVM GetInsurance(int? ID);

        InsuranceClaimVM GetPopulatedModel(int? id = null);
        int CreateHeader(InsuranceClaimVM header);

       void CreateClaimComponentDetails(int? headerId, IEnumerable<ClaimComponentDetailVM> claimComponentDetails);

        void CreateMedicalClaimDetails(int? headerId, IEnumerable<MedicalClaimDetailVM> medicalClaimDetails);


    }
}
