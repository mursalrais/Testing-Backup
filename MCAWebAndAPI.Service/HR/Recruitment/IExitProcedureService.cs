﻿using MCAWebAndAPI.Model.ViewModel.Form.HR;
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

        ExitProcedureVM GetExitProcedure(int? ID);
    }
}
