﻿using System;
using System.Collections.Generic;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using NLog;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Service.HR.Common;
using System.Threading.Tasks;
using System.IO;
using MCAWebAndAPI.Model.HR.DataMaster;

namespace MCAWebAndAPI.Service.HR.Leave
{
    public interface IDayOffService
    {
        void SetSiteUrl(string siteUrl);

        DayOffRequestVM GetPopulatedModel(string requestor = null);

        DayOffRequestVM GetHeader(int? ID, string requestor);

        int CreateHeader(DayOffRequestVM header);

        bool UpdateHeader(DayOffRequestVM header);

        void CreateDayOffDetails(int? headerID, IEnumerable<DayOffRequestDetailVM> dayOffDetails);

        Task CreateDayOffDetailsAsync(int? headerID, IEnumerable<DayOffRequestDetailVM> dayOffDetails);

        void CreateDayOffBalanceDetails(int? headerID, IEnumerable<DayOffBalanceVM> dayOffBalanceDetails);

        Task CreateDayOffBalanceDetailsAsync(int? headerID, IEnumerable<DayOffBalanceVM> dayOffBalanceDetails);

        void PopulateBalance(int idPSA,  PSAManagementVM viewModel, string action);

        IEnumerable<DayOffRequest> GetDayOffRequests(IEnumerable<int> professionalIDs);

        int GetUnpaidDayOffTotalDays(int professionalID, IEnumerable<DateTime> dateRange);

        bool IsUnpaidDayOff(int professionalID, DateTime date, IEnumerable<DateTime> dateRange);

        DayOffBalanceVM GetCalculateBalance(int? ID, string siteUrl, string requestor, string listName);

    }
}
