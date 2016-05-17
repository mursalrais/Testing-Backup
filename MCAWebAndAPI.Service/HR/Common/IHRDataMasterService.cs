﻿using MCAWebAndAPI.Model.HR.DataMaster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Service.HR.Common
{
    public interface IHRDataMasterService
    {
        void SetSiteUrl(string siteUrl);

        IEnumerable<ProfessionalMaster> GetProfessionals();
    }
}