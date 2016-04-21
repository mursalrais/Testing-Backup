﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;

namespace MCAWebAndAPI.Service.Asset
{
    public interface IAssignmentofAssetService
    {
        void SetSiteUrl(string siteUrl);

        IEnumerable<AssignmentofAssetVM> GetAssignmentofAsset();

        bool CreateAssignmentofAsset(AssignmentofAssetVM assignmentofAsset);

        bool UpdateAssignmentofAsset(AssignmentofAssetVM assignmentofAsset);
    }
}
