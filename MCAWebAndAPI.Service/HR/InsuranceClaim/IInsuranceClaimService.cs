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

      
        InsuranceClaimVM GetInsuranceHeader(int? ID, string useremail = null);

        InsuranceClaimVM GetPopulatedModel(string useremail = null);
        int CreateHeader(InsuranceClaimVM header);

       void CreateClaimComponentDetails(int? headerId, IEnumerable<ClaimComponentDetailVM> claimComponentDetails);

        void CreateClaimPaymentDetails(int? headerId, IEnumerable<ClaimPaymentDetailVM> claimPaymentDetails);

        bool UpdateHeader(InsuranceClaimVM header);

    }
}
