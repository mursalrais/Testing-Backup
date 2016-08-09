using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.HR;


namespace MCAWebAndAPI.Service.HR.MedicalCheckUp
{
   public interface IMedicalCheckUpService
    {

        void SetSiteUrl(string siteUrl = null);

        void CreateMedical(MedicalCheckUpVM header);

        void UpdateMedical(MedicalCheckUpVM header);

        MedicalCheckUpVM GetPopulatedModel(string useremail = null);

        MedicalCheckUpVM GetMedical(int? ID, string useremail = null);

       IEnumerable<MedicalCheckUpVM> GetMedicalByUser(int? ID);
    }
}
