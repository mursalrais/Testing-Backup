using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using NLog;
using MCAWebAndAPI.Model.ViewModel.Form.HR;


namespace MCAWebAndAPI.Service.HR.AdjustmentDayOffBalance
{
    public class AdjustmentDayOffBalanceService : IAdjustmentDayOffBalanceService
    {
        string _siteUrl;
        static Logger logger = LogManager.GetCurrentClassLogger();

        const string SP_ADJUSTMENT_DAY_OFF_BALANCE_LIST_NAME = "Adjustment Day-Off Balance";
        const string SP_PROFESSIONAL_MASTER_LIST_NAME = "Professional Master";
        const string SP_COMPENSATORY_REQUEST = "Compensatory Request";
        const string SP_DAY_OFF_BALANCE_LIST_NAME = "Day-Off Balance";

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
        }

        public string GetProfessionalName(int? professionalID)
        {
            var professionalData = SPConnector.GetListItem(SP_PROFESSIONAL_MASTER_LIST_NAME, professionalID, _siteUrl);

            string professionalName = Convert.ToString(professionalData["Title"]);

            return professionalName;
        }

        public AdjustmentDayOffBalanceVM GetAdjustmentDayOffBalance(int? ID)
        {
            var viewModel = new AdjustmentDayOffBalanceVM();
            if (ID == null)
            {
                return viewModel;
            }

            var listItem = SPConnector.GetListItem(SP_ADJUSTMENT_DAY_OFF_BALANCE_LIST_NAME, ID, _siteUrl);
            viewModel = ConvertToAdjustmentDayOffBalanceVM(listItem);

            return viewModel;
        }

        private AdjustmentDayOffBalanceVM ConvertToAdjustmentDayOffBalanceVM(ListItem listItem)
        {
            var viewModel = new AdjustmentDayOffBalanceVM();

            viewModel.ID = Convert.ToInt32(listItem["ID"]);
            viewModel.Professional.Value = Convert.ToInt32((listItem["professional"] as FieldLookupValue).LookupId);
            viewModel.Professional.Text = Convert.ToString((listItem["professional"] as FieldLookupValue).LookupValue);
            viewModel.ProfessionalName = Convert.ToString((listItem["professional"] as FieldLookupValue).LookupValue);
            viewModel.ProjectUnit = Convert.ToString(listItem["Title"]);
            viewModel.Position = Convert.ToString(listItem["position"]);
            viewModel.AdjustmentDate = Convert.ToDateTime(listItem["adjustmentdate"]).ToLocalTime();
            viewModel.DayOffType.Value = Convert.ToString(listItem["dayofftype"]);
            viewModel.Adjustment = Convert.ToInt32(listItem["adjustment"]);
            viewModel.DebitCredit.Value = Convert.ToString(listItem["debitcredit"]);
            viewModel.Remarks = Convert.ToString(listItem["remarks"]);

            var professionalData = SPConnector.GetListItem(SP_PROFESSIONAL_MASTER_LIST_NAME, viewModel.Professional.Value, _siteUrl);
            string professionalFullName = Convert.ToString(professionalData["Title"]);

            if (viewModel.DayOffType.Value == "Annual Day-Off")
            {
                var camlAnnualDayOffBalance = @"<View>  
            <Query> 
               <Where><And><And><Eq><FieldRef Name='dayoffname' /><Value Type='Choice'>" + viewModel.DayOffType.Value + @"</Value></Eq><Eq><FieldRef Name='professional' /><Value Type='Lookup'>" + professionalFullName + @"</Value></Eq></And><Eq><FieldRef Name='psastatus' /><Value Type='Choice'>Active</Value></Eq></And></Where> 
            </Query> 
      </View>";

                foreach (var annualDayOff in SPConnector.GetList(SP_DAY_OFF_BALANCE_LIST_NAME, _siteUrl, camlAnnualDayOffBalance))
                {
                    viewModel.LastBalance = Convert.ToInt32(annualDayOff["finalbalance"]);
                }
            }
            if(viewModel.DayOffType.Value == "Special Day-Off")
            {
                var camlSpecialDayOffBalance = @"<View>  
            <Query> 
               <Where><And><Eq><FieldRef Name='dayoffname' /><Value Type='Choice'>" + viewModel.DayOffType.Value + @"</Value></Eq><Eq><FieldRef Name='professional' /><Value Type='Lookup'>" + professionalFullName + @"</Value></Eq></And></Where> 
            </Query> 
      </View>";

                foreach(var specialDayOff in SPConnector.GetList(SP_DAY_OFF_BALANCE_LIST_NAME, _siteUrl, camlSpecialDayOffBalance))
                {
                    viewModel.LastBalance = Convert.ToInt32(specialDayOff["finalbalance"]);
                }
            }
            if(viewModel.DayOffType.Value == "Paternity")
            {
                var camlPaternityProfessional = @"<View>  
            <Query> 
               <Where><And><Eq><FieldRef Name='dayoffname' /><Value Type='Choice'>" + viewModel.DayOffType.Value + @"</Value></Eq><Eq><FieldRef Name='professional' /><Value Type='Lookup'>" + professionalFullName + @"</Value></Eq></And></Where> 
            </Query> 
      </View>";

                var listOfPaternityID = new List<int>();

                foreach (var paternityData in SPConnector.GetList(SP_DAY_OFF_BALANCE_LIST_NAME, _siteUrl, camlPaternityProfessional))
                {
                    listOfPaternityID.Add(Convert.ToInt32(paternityData["ID"]));
                }

                int paternityID = listOfPaternityID.LastOrDefault();

                var paternityBalance = SPConnector.GetListItem(SP_DAY_OFF_BALANCE_LIST_NAME, paternityID, _siteUrl);

                viewModel.LastBalance = Convert.ToInt32(paternityBalance["finalbalance"]);
            }
            if (viewModel.DayOffType.Value == "Day-off due to Compensatory time")
            {
                var camlCompensatoryBalance = @"<View>  
            <Query> 
               <Where><And><Eq><FieldRef Name='dayoffname' /><Value Type='Choice'>" + viewModel.DayOffType.Value + @"</Value></Eq><Eq><FieldRef Name='professional' /><Value Type='Lookup'>" + professionalFullName + @"</Value></Eq></And></Where> 
            </Query> 
      </View>";

                foreach(var compensatoryData in SPConnector.GetList(SP_DAY_OFF_BALANCE_LIST_NAME, _siteUrl, camlCompensatoryBalance))
                {
                    viewModel.LastBalance = Convert.ToInt32(compensatoryData["finalbalance"]);
                }
            }

            return viewModel;
        }

        public int GetLastBalanceAnnualDayOff(int? professionalID, string dayOffType)
        {
            //Get Professional Name
            var professionalData = SPConnector.GetListItem(SP_PROFESSIONAL_MASTER_LIST_NAME, professionalID, _siteUrl);
            string professionalFullName = Convert.ToString(professionalData["Title"]);

            var camlAnnualDayOffBalance = @"<View>  
            <Query> 
               <Where><And><And><Eq><FieldRef Name='dayoffname' /><Value Type='Choice'>" + dayOffType + @"</Value></Eq><Eq><FieldRef Name='professional' /><Value Type='Lookup'>" + professionalFullName + @"</Value></Eq></And><Eq><FieldRef Name='psastatus' /><Value Type='Choice'>Active</Value></Eq></And></Where> 
            </Query> 
      </View>";

            int annualDayOffBalance = 0;

            foreach (var annualDayOffData in SPConnector.GetList(SP_DAY_OFF_BALANCE_LIST_NAME, _siteUrl, camlAnnualDayOffBalance))
            {
                annualDayOffBalance = Convert.ToInt32(annualDayOffData["finalbalance"]);
            }

            return annualDayOffBalance;
        }

        public int GetLastBalanceSpecialDayOff(int? professionalID, string dayOffType)
        {
            //Get Professional Name
            var professionalData = SPConnector.GetListItem(SP_PROFESSIONAL_MASTER_LIST_NAME, professionalID, _siteUrl);
            string professionalFullName = Convert.ToString(professionalData["Title"]);

            var camlSpecialDayOffBalance = @"<View>  
            <Query> 
               <Where><And><Eq><FieldRef Name='dayoffname' /><Value Type='Choice'>" + dayOffType + @"</Value></Eq><Eq><FieldRef Name='professional' /><Value Type='Lookup'>" + professionalFullName + @"</Value></Eq></And></Where> 
            </Query> 
      </View>";

            int specialDayOffBalance = 0;

            foreach (var specialDayOffData in SPConnector.GetList(SP_DAY_OFF_BALANCE_LIST_NAME, _siteUrl, camlSpecialDayOffBalance))
            {
                specialDayOffBalance = Convert.ToInt32(specialDayOffData["finalbalance"]);
            }

            return specialDayOffBalance;
        }

        public int GetLastBalanceCompensatory(int? professionalID, string dayOffType)
        {
            //Get Professional Name
            var professionalData = SPConnector.GetListItem(SP_PROFESSIONAL_MASTER_LIST_NAME, professionalID, _siteUrl);
            string professionalFullName = Convert.ToString(professionalData["Title"]);

            var camlCompensatoryBalance = @"<View>  
            <Query> 
               <Where><And><Eq><FieldRef Name='dayoffname' /><Value Type='Choice'>" + dayOffType + @"</Value></Eq><Eq><FieldRef Name='professional' /><Value Type='Lookup'>" + professionalFullName + @"</Value></Eq></And></Where> 
            </Query> 
      </View>";

            int compensatoryBalance = 0;

            foreach(var compensatoryData in SPConnector.GetList(SP_DAY_OFF_BALANCE_LIST_NAME, _siteUrl, camlCompensatoryBalance))
            {
                compensatoryBalance = Convert.ToInt32(compensatoryData["finalbalance"]);
            }

            return compensatoryBalance;
        }

        public int GetLastBalancePaternity(int? professionalID, string dayOffType)
        {
            //Get Professional Name
            var professionalData = SPConnector.GetListItem(SP_PROFESSIONAL_MASTER_LIST_NAME, professionalID, _siteUrl);
            string professionalFullName = Convert.ToString(professionalData["Title"]);

            var camlPaternityProfessional = @"<View>  
            <Query> 
               <Where><And><Eq><FieldRef Name='dayoffname' /><Value Type='Choice'>" + dayOffType + @"</Value></Eq><Eq><FieldRef Name='professional' /><Value Type='Lookup'>" + professionalFullName + @"</Value></Eq></And></Where> 
            </Query> 
      </View>";

            var paternityRecordID = new List<int>();
            var endcontractDate = new List<DateTime>();

            foreach (var paternityData in SPConnector.GetList(SP_DAY_OFF_BALANCE_LIST_NAME, _siteUrl, camlPaternityProfessional))
            {
                paternityRecordID.Add(Convert.ToInt32(paternityData["ID"]));
                endcontractDate.Add(Convert.ToDateTime(paternityData["endofcontract"]).ToLocalTime());
            }

            int latestPaternity = paternityRecordID.LastOrDefault();
            DateTime latestPaternityDate = endcontractDate.LastOrDefault().ToLocalTime();

            var paternityBalanceRecord = SPConnector.GetListItem(SP_DAY_OFF_BALANCE_LIST_NAME, latestPaternity, _siteUrl);

            int paternityBalance = Convert.ToInt32(paternityBalanceRecord["finalbalance"]);
            
            return paternityBalance;
        }

        public int CreateAdjustmentDayOffBalance(AdjustmentDayOffBalanceVM adjustmentDayOffBalance)
        {
            var updatedValues = new Dictionary<string, object>();

            updatedValues.Add("adjustmentdate", adjustmentDayOffBalance.AdjustmentDate);
            updatedValues.Add("professional", new FieldLookupValue { LookupId = (int)adjustmentDayOffBalance.Professional.Value });
            updatedValues.Add("dayofftype", adjustmentDayOffBalance.DayOffType.Value);
            updatedValues.Add("adjustment", adjustmentDayOffBalance.Adjustment);
            updatedValues.Add("debitcredit", adjustmentDayOffBalance.DebitCredit.Value);
            updatedValues.Add("Title", adjustmentDayOffBalance.ProjectUnit);
            updatedValues.Add("position", adjustmentDayOffBalance.Position);
            updatedValues.Add("remarks", adjustmentDayOffBalance.Remarks);

            try
            {
                SPConnector.AddListItem(SP_ADJUSTMENT_DAY_OFF_BALANCE_LIST_NAME, updatedValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                throw e;
            }

            return SPConnector.GetLatestListItemID(SP_ADJUSTMENT_DAY_OFF_BALANCE_LIST_NAME, _siteUrl);
        }

        public bool UpdateAnnualDayOffBalance(AdjustmentDayOffBalanceVM adjustmentDayOffBalance)
        {
            string professionalName;

            if (adjustmentDayOffBalance.ProfessionalName == null)
            {
                var professionalData = SPConnector.GetListItem(SP_PROFESSIONAL_MASTER_LIST_NAME, adjustmentDayOffBalance.Professional.Value, _siteUrl);
                professionalName = Convert.ToString(professionalData["Title"]);
            }
            else
            {
                professionalName = Convert.ToString(adjustmentDayOffBalance.ProfessionalName);
            }
                       

            var camlAnnualDayOff = @"<View>  
            <Query> 
               <Where><And><And><Eq><FieldRef Name='professional' /><Value Type='Lookup'>" + professionalName + @"</Value></Eq><Eq><FieldRef Name='psastatus' /><Value Type='Choice'>Active</Value></Eq></And><Eq><FieldRef Name='dayoffname' /><Value Type='Choice'>" + adjustmentDayOffBalance.DayOffType.Value + @"</Value></Eq></And></Where> 
            </Query> 
      </View>";

            int annualDayOffDataID = 0;

            foreach(var annualDayOffData in SPConnector.GetList(SP_DAY_OFF_BALANCE_LIST_NAME, _siteUrl, camlAnnualDayOff))
            {
                annualDayOffDataID = Convert.ToInt32(annualDayOffData["ID"]);
            }

            var updateValues = new Dictionary<string, object>();

            updateValues.Add("finalbalance", adjustmentDayOffBalance.NewBalance);

            try
            {
                SPConnector.UpdateListItem(SP_DAY_OFF_BALANCE_LIST_NAME, annualDayOffDataID, updateValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                return false;
            }

            return true;
        }

        public bool UpdateSpecialDayOffBalance(AdjustmentDayOffBalanceVM adjustmentDayOffBalance)
        {
            string professionalName;

            if (adjustmentDayOffBalance.ProfessionalName == null)
            {
                var professionalData = SPConnector.GetListItem(SP_PROFESSIONAL_MASTER_LIST_NAME, adjustmentDayOffBalance.Professional.Value, _siteUrl);
                professionalName = Convert.ToString(professionalData["Title"]);
            }
            else
            {
                professionalName = Convert.ToString(adjustmentDayOffBalance.ProfessionalName);
            }

            var camlSpecialDayOff = @"<View>  
            <Query> 
               <Where><And><Eq><FieldRef Name='dayoffname' /><Value Type='Choice'>" + adjustmentDayOffBalance.DayOffType.Value + @"</Value></Eq><Eq><FieldRef Name='professional' /><Value Type='Lookup'>" + professionalName + @"</Value></Eq></And></Where> 
            </Query> 
      </View>";

            int specialDayOffDataID = 0;

            foreach (var specialDayOffData in SPConnector.GetList(SP_DAY_OFF_BALANCE_LIST_NAME, _siteUrl, camlSpecialDayOff))
            {
                specialDayOffDataID = Convert.ToInt32(specialDayOffData["ID"]);
            }

            var updateValues = new Dictionary<string, object>();

            updateValues.Add("finalbalance", adjustmentDayOffBalance.NewBalance);

            try
            {
                SPConnector.UpdateListItem(SP_DAY_OFF_BALANCE_LIST_NAME, specialDayOffDataID, updateValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                return false;
            }

            return true;
        }

        public bool UpdatePaternityBalance(AdjustmentDayOffBalanceVM adjustmentDayOffBalance)
        {
            string professionalName;

            if (adjustmentDayOffBalance.ProfessionalName == null)
            {
                var professionalData = SPConnector.GetListItem(SP_PROFESSIONAL_MASTER_LIST_NAME, adjustmentDayOffBalance.Professional.Value, _siteUrl);
                professionalName = Convert.ToString(professionalData["Title"]);
            }
            else
            {
                professionalName = Convert.ToString(adjustmentDayOffBalance.ProfessionalName);
            }

            var camlPaternityRecord = @"<View>  
            <Query> 
               <Where><And><Eq><FieldRef Name='dayoffname' /><Value Type='Choice'>" + adjustmentDayOffBalance.DayOffType.Value + @"</Value></Eq><Eq><FieldRef Name='professional' /><Value Type='Lookup'>" + professionalName + @"</Value></Eq></And></Where> 
            </Query> 
      </View>";

            var paternityRecordID = new List<int>();

            foreach (var paternityData in SPConnector.GetList(SP_DAY_OFF_BALANCE_LIST_NAME, _siteUrl, camlPaternityRecord))
            {
                paternityRecordID.Add(Convert.ToInt32(paternityData["ID"]));
            }

            int paternityLatestID = paternityRecordID.LastOrDefault();

            var updateValues = new Dictionary<string, object>();

            updateValues.Add("finalbalance", adjustmentDayOffBalance.NewBalance);

            try
            {
                SPConnector.UpdateListItem(SP_DAY_OFF_BALANCE_LIST_NAME, paternityLatestID, updateValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                return false;
            }

            return true;
        }

        public bool UpdateCompensatoryBalance(AdjustmentDayOffBalanceVM adjustmentDayOffBalance)
        {
            string professionalName;

            if (adjustmentDayOffBalance.ProfessionalName == null)
            {
                var professionalData = SPConnector.GetListItem(SP_PROFESSIONAL_MASTER_LIST_NAME, adjustmentDayOffBalance.Professional.Value, _siteUrl);
                professionalName = Convert.ToString(professionalData["Title"]);
            }
            else
            {
                professionalName = Convert.ToString(adjustmentDayOffBalance.ProfessionalName);
            }

            var camlCompensatoryBalance = @"<View>  
            <Query> 
               <Where><And><Eq><FieldRef Name='dayoffname' /><Value Type='Choice'>" + adjustmentDayOffBalance.DayOffType.Value + @"</Value></Eq><Eq><FieldRef Name='professional' /><Value Type='Lookup'>" + professionalName + @"</Value></Eq></And></Where> 
            </Query> 
      </View>";

            int compensatoryBalanceDataID = 0;

            foreach (var compensatoryBalanceData in SPConnector.GetList(SP_DAY_OFF_BALANCE_LIST_NAME, _siteUrl, camlCompensatoryBalance))
            {
                compensatoryBalanceDataID = Convert.ToInt32(compensatoryBalanceData["ID"]);
            }

            var updateValues = new Dictionary<string, object>();

            updateValues.Add("finalbalance", adjustmentDayOffBalance.NewBalance);

            try
            {
                SPConnector.UpdateListItem(SP_DAY_OFF_BALANCE_LIST_NAME, compensatoryBalanceDataID, updateValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                return false;
            }

            return true;
        }

        public bool UpdateAdjustmentDayOffBalance(AdjustmentDayOffBalanceVM adjustmentDayOffBalance)
        {
            var columnValues = new Dictionary<string, object>();
            int ID = Convert.ToInt32(adjustmentDayOffBalance.ID);

            columnValues.Add("adjustmentdate", adjustmentDayOffBalance.AdjustmentDate.Value);
            columnValues.Add("adjustment", adjustmentDayOffBalance.Adjustment);
            columnValues.Add("debitcredit", adjustmentDayOffBalance.DebitCredit.Value);
            columnValues.Add("remarks", adjustmentDayOffBalance.Remarks);
            
            try
            {
                SPConnector.UpdateListItem(SP_ADJUSTMENT_DAY_OFF_BALANCE_LIST_NAME, adjustmentDayOffBalance.ID, columnValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                return false;
            }

            var entitiy = new AdjustmentDayOffBalanceVM();
            entitiy = adjustmentDayOffBalance;
            return true;
            
        }
    }
}
