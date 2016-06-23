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
    public interface IExitProcedureService
    {

        void SetSiteUrl(string siteUrl);

        //Display Exit Procedure Data based on ID
        ExitProcedureVM GetExitProcedure(int? ID);

        int CreateExitProcedure(ExitProcedureVM exitProcedure);

        bool UpdateExitProcedure(ExitProcedureVM exitProcedure);

        ExitProcedureVM ViewExitProcedure(int? ID);
    }
}
