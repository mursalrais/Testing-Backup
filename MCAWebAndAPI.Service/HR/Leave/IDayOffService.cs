using System;
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

        DayOffRequestVM GetPopulatedModel(int? ID = null, string requestor = null);

        int CreateDayOffRequestHeader(DayOffRequestVM dayOffRequest);

        //Belum Pake
        DayOffRequestVM GetHeader(int? ID, string requestor);

        //Belum Pake
        int CreateHeader(DayOffRequestVM header);

        //Belum Pake
        bool UpdateHeader(DayOffRequestVM header);

        //Belum Pake
        void CreateDayOffDetails(int? headerID, IEnumerable<DayOffRequestDetailVM> dayOffDetails);

        //Belum Pake
        Task CreateDayOffDetailsAsync(int? headerID, IEnumerable<DayOffRequestDetailVM> dayOffDetails);

        //Belum Pake
        void CreateDayOffBalanceDetails(int? headerID, IEnumerable<DayOffBalanceVM> dayOffBalanceDetails);

        //Belum Pake
        Task CreateDayOffBalanceDetailsAsync(int? headerID, IEnumerable<DayOffBalanceVM> dayOffBalanceDetails);

        //Belum Pake
        void PopulateBalance(int idPSA,  PSAManagementVM viewModel, string action);

        //Belum Pake
        IEnumerable<DayOffRequest> GetDayOffRequests(IEnumerable<int> professionalIDs);

        //Belum Pake
        int GetUnpaidDayOffTotalDays(int professionalID, IEnumerable<DateTime> dateRange);

        //Belum Pake
        bool IsUnpaidDayOff(int professionalID, DateTime date, IEnumerable<DateTime> dateRange);

        //Belum Pake
        DayOffBalanceVM GetCalculateBalance(int? ID, string siteUrl, string requestor, string listName);

        //Belum Pake
        IEnumerable<DayOffTypeMaster> GetDayOffType();

        int GetPositionID(string requestorposition, string requestorunit, int positionID, int number);

        Task CreateDayOffRequestDetailAsync(DayOffRequestVM dayOffRequest, int? dayOffRequestHeaderID, IEnumerable<DayOffRequestDetailVM> dayOffRequestDetail, string requestorposition, string requestorunit, int? positionID);

        DayOffRequestVM GetRequestData(List<string> dayOffType, List<string> startDate, List<string> endDate, List<string> fullHalfDay, List<string> remarks);

    }
}
