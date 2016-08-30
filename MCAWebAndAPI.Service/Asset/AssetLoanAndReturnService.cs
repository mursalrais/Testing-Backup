using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using NLog;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Model.ViewModel.Control;
using System.Data;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using MCAWebAndAPI.Model.ViewModel.Form.HR;

namespace MCAWebAndAPI.Service.Asset
{
    public class AssetLoanAndReturnService : IAssetLoanAndReturnService
    {
        string _siteUrl;
        static Logger logger = LogManager.GetCurrentClassLogger();
        const string SP_ASSLNR_LIST_NAME = "Asset Loan Return";
        const string SP_ASSLNRDetails_LIST_NAME = "Asset Loan Return Detail";
        const string SP_PROFMASTER_LIST_NAME = "Professional Master";
        const string SP_ASSSUBASS_LIST_NAME = "Asset Master";

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = siteUrl;
        }

        //public AssetLoanAndReturnHeaderVM GetPopulatedModel(int? id = null)
        //{
        //    var model = new AssetLoanAndReturnHeaderVM();
        //    model.Professional.Choices = GetChoicesFromList(SP_PROFMASTER_LIST_NAME, "ID", "Title");
        //    model.TransactionType = Convert.ToString("Asset Loan And Return");
        //    model.CancelURL = _siteUrl + UrlResource.AssetLoanAndReturn;
        //    return model;
        //}


        private IEnumerable<string> GetChoicesFromList(string listname, string v1, string v2 = null)
        {
            List<string> _choices = new List<string>();
            var listItems = SPConnector.GetList(listname, _siteUrl);
            foreach (var item in listItems)
            {
                if (v2 != null)
                {
                    _choices.Add(item[v1] + "-" + item[v2].ToString());
                }
                else
                {
                    _choices.Add(item[v1].ToString());
                }
            }
            return _choices.ToArray();
        }


        public AssetLoanAndReturnItemVM GetPopulatedModelItem(int? ID = default(int?))
        {
            var model = new AssetLoanAndReturnItemVM();
           

            return model;
        }

       

        public bool UpdateHeader(AssetLoanAndReturnHeaderVM viewmodel)
        {
            throw new NotImplementedException();
        }

        public void CreateDetails(int? headerID, IEnumerable<AssetLoanAndReturnItemVM> items)
        {
            foreach (var item in items)
            {
                if (Item.CheckIfSkipped(item)) continue;

                if (Item.CheckIfDeleted(item))
                {
                    try
                    {
                        SPConnector.DeleteListItem(SP_ASSLNRDetails_LIST_NAME, item.ID, _siteUrl);
                    }
                    catch (Exception e)
                    {
                        logger.Error(e);
                        throw e;
                    }
                    continue;
                }



                var updatedValues = new Dictionary<string, object>();
                updatedValues.Add("assetloanandreturn", new FieldLookupValue { LookupId = Convert.ToInt32(headerID) });
                updatedValues.Add("assetsubasset", new FieldLookupValue { LookupId = Convert.ToInt32(item.AssetSubAsset.Value.Value) });
                updatedValues.Add("estreturndate", item.EstReturnDate);
                updatedValues.Add("returndate", item.ReturnDate);
                updatedValues.Add("status", "LOAN");
                try
                {
                    SPConnector.AddListItem(SP_ASSLNRDetails_LIST_NAME, updatedValues, _siteUrl);
                }
                catch (Exception e)
                {
                    logger.Error(e);
                    throw new Exception(ErrorResource.SPInsertError);
                }

            }
        }
        //    public void CreateDetails(int? headerID, IEnumerable<AssetLoanAndReturnItemVM> items)
        //{
        //     var updatedValues = new Dictionary<string, object>();
        //    updatedValues.Add("assetloanandreturn", new FieldLookupValue { LookupId = Convert.ToInt32(headerID) });
        //    updatedValues.Add("assetsubasset", new FieldLookupValue { LookupId = Convert.ToInt32(item.AssetSubAsset.Value.Value) });
        //    updatedValues.Add("estreturndate", item.EstReturnDate);
        //    updatedValues.Add("returndate", item.ReturnDate);
        //    updatedValues.Add("status", "LOAN");
        //    try
        //    {
        //        SPConnector.AddListItem(SP_ASSLNRDetails_LIST_NAME, updatedValues, SiteUrl);
        //    }
        //    catch (Exception e)
        //    {
        //        logger.Error(e);
        //        throw new Exception(ErrorResource.SPInsertError);
        //    }

        //    return SPConnector.GetLatestListItemID(SP_ASSLNRDetails_LIST_NAME, SiteUrl);
        //}
        //}

        //public void CreateDetails(int? headerID, IEnumerable<AssetLoanAndReturnItemVM> items)
        //{
        //     var updatedValues = new Dictionary<string, object>();
        //    updatedValues.Add("assetloanandreturn", new FieldLookupValue { LookupId = Convert.ToInt32(headerID) });
        //    updatedValues.Add("assetsubasset", new FieldLookupValue { LookupId = Convert.ToInt32(item.AssetSubAsset.Value.Value) });
        //    updatedValues.Add("estreturndate", item.EstReturnDate);
        //    updatedValues.Add("returndate", item.ReturnDate);
        //    updatedValues.Add("status", "LOAN");
        //    try
        //    {
        //        SPConnector.AddListItem(SP_ASSLNRDetails_LIST_NAME, updatedValues, SiteUrl);
        //    }
        //    catch (Exception e)
        //    {
        //        logger.Error(e);
        //        throw new Exception(ErrorResource.SPInsertError);
        //    }

        //    return SPConnector.GetLatestListItemID(SP_ASSLNRDetails_LIST_NAME, SiteUrl);
        //}

        public void UpdateDetails(int? headerID, IEnumerable<AssetLoanAndReturnItemVM> items)
        {
            throw new NotImplementedException();
        }


        //public AssetLoanAndReturnHeaderVM GetHeader(int? ID, string SiteUrl)
        //{
        //    var listItem = SPConnector.GetListItem(SP_ASSLNR_LIST_NAME, ID, SiteUrl);
        //    var viewModel = new AssetLoanAndReturnHeaderVM();

        //    viewModel.TransactionType = Convert.ToString(listItem["Title"]);
        //    viewModel.Professional.Choices = GetChoicesFromList(SP_PROFMASTER_LIST_NAME, "ID", "Title");
        //    if ((listItem["Professional0"] as FieldLookupValue) != null)
        //    {
        //        viewModel.Professional.Value = (listItem["Professional0"] as FieldLookupValue).LookupId.ToString();
        //        viewModel.Professional.Text = (listItem["Professional0"] as FieldLookupValue).LookupId.ToString() + "-" + (listItem["Professional0"] as FieldLookupValue).LookupValue;
        //    }
        //    //viewModel.AccpMemo.Value = Convert.ToString(listItem["acceptancememono"]);
        //    viewModel.ContactNo = Convert.ToString(listItem["professionalmobilephonenr"]);
        //    viewModel.Project = Convert.ToString(listItem["projectunit"]);

        //    viewModel.LoanDate = Convert.ToDateTime(listItem["loandate"]);
        //    //viewModel.Spesifications = Regex.Replace(listItem["Spesifications"].ToString(), "<.*?>", string.Empty);
        //    viewModel.Purpose = Convert.ToString(listItem["Purpose"]);
        //    viewModel.ID = ID;

        //    viewModel.CancelURL = _siteUrl + UrlResource.AssetLoanAndReturn;

        //    return viewModel;
        //}

        public AssetLoanAndReturnHeaderVM GetHeader(int? ID, string SiteUrl)
        {
            var listItem = SPConnector.GetListItem(SP_ASSLNR_LIST_NAME, ID, SiteUrl);
            var viewModel = new AssetLoanAndReturnHeaderVM();
            viewModel.Purpose = listItem["Purpose"].ToString();
            viewModel.Project = listItem["projectunit"].ToString();
            viewModel.TransactionType = Convert.ToString(listItem["transactiontype"]);
            viewModel.Professional.Choices = GetFromListHR("Professional Master", "Title", "Position", SiteUrl);
            //var caml = @"<View><Query>
            //           <Where>
            //              <Eq>
            //                <FieldRef Name='ID' />
            //                 <Value Type='Counter'>" + (listItem["assetholder"] as FieldLookupValue).LookupId + @"</Value>
            //              </Eq>
            //           </Where>
            //        </Query>
            //        <ViewFields>
            //           <FieldRef Name='Title' />
            //           <FieldRef Name='Position' />
            //        </ViewFields>
            //        <QueryOptions /></View>";

            var caml = @"<View><Query><Where><Eq><FieldRef Name='ID' /><Value Type='Counter'>"+ (listItem["name"] as FieldLookupValue).LookupId + @"</Value></Eq></Where></Query>
             <ViewFields>
                       <FieldRef Name='Title' />
                       <FieldRef Name='Position' />
                    </ViewFields>
                    <QueryOptions /></View>";

            var sitehr = SiteUrl.Replace("/bo", "/hr");
            var infoAssetHolder = SPConnector.GetList("Professional Master", sitehr, caml);
            foreach (var info in infoAssetHolder)
            {
                viewModel.Professional.Value = (listItem["name"] as FieldLookupValue).LookupId.ToString();
                viewModel.Professional.Text = info["Title"] + "-" + (info["Position"] as FieldLookupValue).LookupValue;
            }


            //viewModel.AccpMemo.Value = Convert.ToString(listItem["acceptancememono"]);
           
            viewModel.ContactNo = Convert.ToString(listItem["professionalmobilephonenr"]);
            if (Convert.ToDateTime(listItem["loandate"]) == DateTime.MinValue)
            {
                viewModel.LoanDate = null;
            }
            else
            {
                viewModel.LoanDate = Convert.ToDateTime(listItem["loandate"]);
            }
          
            viewModel.ID = ID;

            var caml1 = @"<View><Query>
                        <Where>
                           <Eq>
                               <FieldRef Name='assetloanandreturn' />
                                <Value Type='Lookup'>"+ ID + @"</Value>
                           </Eq>
                        </Where>
                    </Query>
                 <ViewFields>
                       <FieldRef Name='assetsubasset' />
                    </ViewFields>
                    <QueryOptions /></View>";

            //var caml1 = @"<View><Query>
            //           <Where>
            //              <Eq>
            //                 <FieldRef Name='assetassignment' />
            //                 <Value Type='Lookup'>" + ID + @"</Value>
            //              </Eq>
            //           </Where>
            //        </Query>
            //        <ViewFields>
            //           <FieldRef Name='assetsubasset' />
            //        </ViewFields>
            //        <QueryOptions /></View>";

            var getDetails = SPConnector.GetList(SP_ASSLNRDetails_LIST_NAME, _siteUrl, caml1);
            var combine = "";
            foreach (var det in getDetails)
            {
                if (combine == "")
                {
                    combine = (det["assetsubasset"] as FieldLookupValue).LookupValue;
                }
                else
                {
                    if (!combine.Contains((det["assetsubasset"] as FieldLookupValue).LookupValue))
                    {
                        combine = combine + ", " + (det["assetsubasset"] as FieldLookupValue).LookupValue;
                    }
                }
            }
            //viewModel.AssetIDs = combine;
            viewModel.CancelURL = _siteUrl + UrlResource.AssetLoanAndReturn;

            return viewModel;
        }

        public IEnumerable<AssetLoanAndReturnItemVM> GetDetails(int? headerID)
        {
           
            var caml = @"<View><Query><Where><Eq><FieldRef Name='assetloanandreturn' /><Value Type='Lookup'>" + headerID.ToString() + "</Value></Eq></Where></Query></View>";
            var details = new List<AssetLoanAndReturnItemVM>();

            foreach (var item in SPConnector.GetList(SP_ASSLNRDetails_LIST_NAME, _siteUrl, caml))
            {
                details.Add(ConvertToDetails(item));
            }

            return details;
        }

        //private AssetLoanAndReturnItemVM ConvertToDetails(ListItem item)
        //{
        //    var ListAssetSubAsset = SPConnector.GetListItem("Asset Master", (item["Asset_x002d_Sub_x0020_Asset"] as FieldLookupValue).LookupId, _siteUrl);
        //    AjaxComboBoxVM _assetSubAsset = new AjaxComboBoxVM();
        //    _assetSubAsset.Value = (item["Asset_x002d_Sub_x0020_Asset"] as FieldLookupValue).LookupId;
        //    _assetSubAsset.Text = Convert.ToString(ListAssetSubAsset["AssetID"]) + " - " + Convert.ToString(ListAssetSubAsset["Title"]);
        //    return new AssetLoanAndReturnItemVM
        //    {
        //        ID = Convert.ToInt32(item["ID"]),
        //        AssetSubAsset = AssetLoanAndReturnItemVM.GetAssetSubAssetDefaultValue(_assetSubAsset)
        //    };

        //}

        private AssetLoanAndReturnItemVM ConvertToDetails(ListItem item)
        {
            var ListAssetSubAsset = SPConnector.GetListItem("Asset Master", (item["assetsubasset"] as FieldLookupValue).LookupId, _siteUrl);

            AjaxComboBoxVM _assetSubAsset = new AjaxComboBoxVM();
            _assetSubAsset.Value = (item["assetsubasset"] as FieldLookupValue).LookupId;
            _assetSubAsset.Text = Convert.ToString(ListAssetSubAsset["AssetID"]) + " - " + Convert.ToString(ListAssetSubAsset["Title"]);

          

            return new AssetLoanAndReturnItemVM
            {
                ID = Convert.ToInt32(item["ID"]),
               EstReturnDate = Convert.ToDateTime(item["estreturndate"]),
               ReturnDate = Convert.ToDateTime(item["returndate"]),
                //description = Convert.ToString(ListAssetSubAsset["Title"]),
                //quantity = 1,
                AssetSubAsset = AssetLoanAndReturnItemVM.GetAssetSubAssetDefaultValue(_assetSubAsset),
                //Province = AssetLoanAndReturnItemVM.GetProvinceDefaultValue(_province),
                Status = Convert.ToString(item["status"])
            };
        }

        public IEnumerable<AssetMasterVM> GetAssetSubAsset()
            {
            
            var models = new List<AssetMasterVM>();

            foreach (var item in SPConnector.GetList("Asset Master", _siteUrl))
            {
                models.Add(ConvertToModelAssetSubAsset(item));
            }

            return models;
        }

        

        private AssetMasterVM ConvertToModelAssetSubAsset(ListItem item)
        {
            var viewModel = new AssetMasterVM();

            viewModel.ID = Convert.ToInt32(item["ID"]);
            viewModel.AssetNoAssetDesc.Value = Convert.ToString(item["AssetID"]);
            viewModel.AssetDesc = Convert.ToString(item["Title"]);
            return viewModel;
        }

        public IEnumerable<AssetMasterVM> GetAssetLoanAndReturn()
        {
            throw new NotImplementedException();
        }

        public void CreateAssetLoanAndReturnItem(int? headerID, IEnumerable<AssetLoanAndReturnItemVM> AssetLoanAndReturnItem)
        {
            throw new NotImplementedException();
        }

        public ProfessionalsVM GetProfessionalInfo(int? ID, string SiteUrl)
        {
            var siteHr = SiteUrl.Replace("/bo", "/hr");
            var list = SPConnector.GetListItem(SP_PROFMASTER_LIST_NAME, ID, siteHr);
            var viewmodel = new ProfessionalsVM();
            viewmodel.ID = Convert.ToInt32(ID);
            viewmodel.ProfessionalName = Convert.ToString(list["Title"]);
            viewmodel.ProjectName = Convert.ToString(list["Project_x002f_Unit"]);
            viewmodel.ContactNo = Convert.ToString(list["mobilephonenr"]);

            return viewmodel;
        }

        //Dictionary<int, string> IAssetLoanAndReturnService.getListIDOfList(string listName, string key, string value, string SiteUrl)
        //{
        //    var caml = @"<View><Query />
        //                <ViewFields>
        //                   <FieldRef Name='" + key + @"' />
        //                   <FieldRef Name='" + value + @"' />
        //                </ViewFields>
        //                <QueryOptions /></View>";

        //    var list = SPConnector.GetList(listName, SiteUrl, caml);
        //    Dictionary<int, string> ids = new Dictionary<int, string>();
        //    if (list.Count > 0)
        //    {
        //        foreach (var l in list)
        //        {
        //            ids.Add(Convert.ToInt32(l[key]), Convert.ToString(l[value]));
        //        }
        //    }

        //    return ids;
        //}

        public int? MassUploadHeaderDetail(string ListName, DataTable CSVDataTable, string SiteUrl = null)
        {
            var rowTotal = CSVDataTable.Rows.Count;
            var columnTotal = CSVDataTable.Columns.Count;
            var columnTypes = new Type[columnTotal];
            var columnTechnicalNames = new string[columnTotal];

            // After Column Name, the first row should be Column Type
            for (int i = 0; i < columnTotal; i++)
            {
                //format header MUST be technicalname:type or technicalname_lookup:type technicalname_skip:type
                try
                {
                    columnTechnicalNames[i] = CSVDataTable.Columns[i].ColumnName;
                    columnTypes[i] = CSVDataTable.Columns[i].DataType;
                }
                catch (Exception e)
                {
                    logger.Error(e);
                    throw e;
                }
            }

            var updatedValues = new Dictionary<string, object>();
            // Start from 1 since 0 is header 
            for (int i = 0; i < rowTotal; i++)
            {
                for (int j = 0; j < columnTotal; j++)
                {
                    if (isLookup(columnTechnicalNames[j]))
                    {
                        FormatUtil.GenerateUpdatedValueFromGivenDataTable(ref updatedValues, columnTypes[j],
                            columnTechnicalNames[j], CSVDataTable.Rows[i].ItemArray[j], lookup: true, skip: false);
                    }
                    else if (isSkipped(columnTechnicalNames[j]))
                    {
                        FormatUtil.GenerateUpdatedValueFromGivenDataTable(ref updatedValues, columnTypes[j],
                           columnTechnicalNames[j], CSVDataTable.Rows[i].ItemArray[j], lookup: false, skip: true);
                    }
                    else
                    {
                        FormatUtil.GenerateUpdatedValueFromGivenDataTable(ref updatedValues, columnTypes[j],
                          columnTechnicalNames[j], CSVDataTable.Rows[i].ItemArray[j], lookup: false, skip: false);
                    }
                }
                try
                {
                    SPConnector.AddListItem(ListName, updatedValues, SiteUrl);
                }
                catch (Exception e)
                {
                    logger.Error(string.Format("{0} at ID: {1}", e.Message, i + 1));
                    throw new Exception(string.Format("An error occured at ID: {0}. Therefore, data on ID: {0} and afterwards have not been submitted.", i + 1));

                }
                updatedValues = new Dictionary<string, object>();
            }
            return SPConnector.GetLatestListItemID(ListName, SiteUrl);
        }

        private bool isSkipped(string columnName)
        {
            return columnName.Contains("_")
                && string.Compare(columnName.Split('_')[1], "skip", StringComparison.OrdinalIgnoreCase) == 0;
        }

        private bool isLookup(string columnName)
        {
            return columnName.Contains("_")
               && string.Compare(columnName.Split('_')[1], "lookup", StringComparison.OrdinalIgnoreCase) == 0;
        }

        public void RollbackParentChildrenUpload(string listNameHeader, int? latestIDHeader, string siteUrl)
        {
            SPConnector.DeleteListItem(listNameHeader, latestIDHeader, siteUrl);
        }

        public IEnumerable<AssetLoanAndReturnVM> GetAssetLoanAndReturns()
        {
            throw new NotImplementedException();
        }

        public bool CreateAssetLoanAndReturn(AssetLoanAndReturnVM assetLoanAndReturn)
        {
            throw new NotImplementedException();
        }

        public bool UpdateAssetLoanAndReturn(AssetLoanAndReturnVM assetLoanAndReturn)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AssetLoanAndReturnItemVM> GetAssetTransfers()
        {
            throw new NotImplementedException();
        }

        public int? CreateHeader(AssetLoanAndReturnHeaderVM viewmodel, string SiteUrl, string mode = null)
        {
            viewmodel.CancelURL = _siteUrl + UrlResource.AssetLoanAndReturn;
            var columnValues = new Dictionary<string, object>();
            //columnValues.add
            columnValues.Add("transactiontype", "Asset Loan And Return");
            if (viewmodel.LoanDate.HasValue)
            {
                columnValues.Add("loandate", viewmodel.LoanDate);
            }
            else
            {
                columnValues.Add("loandate", null);
            }

            if (viewmodel.Professional.Value == null)
            {
                return 0;
            }
            else
            {
                if (mode == null)
                {
                    var breaks = viewmodel.Professional.Value.Split('-');
                    var getInfo = GetProfMasterInfo(breaks[0], SiteUrl);
                    if (getInfo != null)
                    {
                        //  columnValues.Add("Professional0", new FieldLookupValue { LookupId = Convert.ToInt32(getInfo.ID) });
                        columnValues.Add("name", new FieldLookupValue { LookupId = Convert.ToInt32(getInfo.ID) });
                        columnValues.Add("professionalposition", breaks[1]);
                        columnValues.Add("professionalname", breaks[0]);
                        columnValues.Add("projectunit", getInfo.CurrentPosition.Text);
                        columnValues.Add("professionalmobilephonenr", getInfo.MobileNumberOne);
                        columnValues.Add("Purpose", viewmodel.Purpose);
                    }
                }
                else
                {
                    _siteUrl = SiteUrl;
                    var getInfo = GetProfMasterInfo(viewmodel.Professional.Value, _siteUrl);
                    if (getInfo != null)
                    {
                       // columnValues.Add("Professional0", new FieldLookupValue { LookupId = Convert.ToInt32(getInfo.ID) });
                        columnValues.Add("professionalposition", getInfo.Position);
                        columnValues.Add("professionalname",getInfo.FirstMiddleName);
                        columnValues.Add("projectunit", getInfo.CurrentPosition.Text);
                        columnValues.Add("professionalmobilephonenr", getInfo.MobileNumberOne);
                        columnValues.Add("Purpose", viewmodel.Purpose);
                    }
                }

            }

            try
            {
                SPConnector.AddListItem(SP_ASSLNR_LIST_NAME, columnValues, _siteUrl);

                //var id = SPConnector.GetLatestListItemID("Asset Disposal", _siteUrl);
                //var info = SPConnector.GetListItem("Asset Disposal", id, _siteUrl);

                //var loandate = 
                //if (Convert.ToBoolean(info["Attachments"]) == false)
                //{
                //    SPConnector.DeleteListItem("Asset Disposal", id, _siteUrl);
                //    return 0;
                //}


            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }
            var entitiy = new AssetLoanAndReturnHeaderVM();
            entitiy = viewmodel;
            return SPConnector.GetLatestListItemID(SP_ASSLNR_LIST_NAME, _siteUrl);


            //var columnValues = new Dictionary<string, object>();

            //string[] profesional = header.Professional.Value.Split('-');
            //var breaks = header.Professional.Value.Split('-');
            //var getInfo = GetProfMasterInfo(breaks[0], SiteUrl);
            ////columnValues.Add("acceptancememono", memo[1]);


            //columnValues.Add("transactiontype", header.TransactionType);

            //columnValues.Add("professional", new FieldLookupValue { LookupId = Convert.ToInt32(getInfo.ID) });
            //columnValues.Add("professionalposition", breaks[1]);
            //columnValues.Add("professionalname", breaks[0]);
            //columnValues.Add("projectunit", getInfo.CurrentPosition.Text);
            //columnValues.Add("contactnumber", getInfo.MobileNumberOne);

            //// columnValues.Add("professionalname", new FieldLookupValue { LookupId = Convert.ToInt32(Convert.ToInt32(profesional[0])) });
            ////columnValues.Add("professionalposition", new FieldLookupValue { LookupId = Convert.ToInt32(Convert.ToInt32(profesional[1])) });

            //columnValues.Add("projectunit", header.Project);
            //// columnValues.Add("Professional", new FieldLookupValue { LookupId = Convert.ToInt32(viewmodel.Project) });
            //columnValues.Add("professionalmobilephonenr", header.ContactNo);
            //columnValues.Add("loandate", header.LoanDate);
            //columnValues.Add("Purpose", header.Purpose);
            //try
            //{
            //    SPConnector.AddListItem(SP_ASSLNR_LIST_NAME, columnValues, _siteUrl);
            //}
            //catch (Exception e)
            //{
            //    logger.Error(e.Message);
            //}

            //return SPConnector.GetLatestListItemID(SP_ASSLNR_LIST_NAME, _siteUrl);

        }

        public bool CreateAssetTransfer(AssetLoanAndReturnItemVM assetTransfer)
        {
            throw new NotImplementedException();
        }

        public bool UpdateAssetTransfer(AssetLoanAndReturnItemVM assetTransfer)
        {
            throw new NotImplementedException();
        }

        public int? CreateHeader(AssetLoanAndReturnHeaderVM header, string SiteUrl)
        {

            var columnValues = new Dictionary<string, object>();

            string[] profesional = header.Professional.Value.Split('-');
            var breaks = header.Professional.Value.Split('-');
            var getInfo = GetProfMasterInfo(breaks[0], SiteUrl);
            //columnValues.Add("acceptancememono", memo[1]);


            columnValues.Add("transactiontype", header.TransactionType);

            columnValues.Add("professional", new FieldLookupValue { LookupId = Convert.ToInt32(getInfo.ID) });
            columnValues.Add("professionalposition", breaks[1]);
            columnValues.Add("professionalname", breaks[0]);
            columnValues.Add("projectunit", getInfo.CurrentPosition.Text);
            columnValues.Add("contactnumber", getInfo.MobileNumberOne);

           // columnValues.Add("professionalname", new FieldLookupValue { LookupId = Convert.ToInt32(Convert.ToInt32(profesional[0])) });
            //columnValues.Add("professionalposition", new FieldLookupValue { LookupId = Convert.ToInt32(Convert.ToInt32(profesional[1])) });

            columnValues.Add("projectunit", header.Project);
            // columnValues.Add("Professional", new FieldLookupValue { LookupId = Convert.ToInt32(viewmodel.Project) });
            columnValues.Add("professionalmobilephonenr", header.ContactNo);
            columnValues.Add("loandate", header.LoanDate);
            columnValues.Add("Purpose", header.Purpose);
            try
            {
                SPConnector.AddListItem(SP_ASSLNR_LIST_NAME, columnValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }

            return SPConnector.GetLatestListItemID(SP_ASSLNR_LIST_NAME, _siteUrl);
        }

        public int? CreateDetails(int? headerID, AssetLoanAndReturnItemVM item, string SiteUrl = null)
        {
            var updatedValues = new Dictionary<string, object>();
            updatedValues.Add("assetloanandreturn", new FieldLookupValue { LookupId = Convert.ToInt32(headerID) });
            updatedValues.Add("assetsubasset", new FieldLookupValue { LookupId = Convert.ToInt32(item.AssetSubAsset.Value.Value) });
            updatedValues.Add("estreturndate", item.EstReturnDate);
            updatedValues.Add("returndate", item.ReturnDate);
            updatedValues.Add("status", "LOAN");
            try
            {
                SPConnector.AddListItem(SP_ASSLNRDetails_LIST_NAME, updatedValues, SiteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e);
                throw new Exception(ErrorResource.SPInsertError);
            }

            return SPConnector.GetLatestListItemID(SP_ASSLNRDetails_LIST_NAME, SiteUrl);
        }

        public AssetLoanAndReturnHeaderVM GetPopulatedModel(string SiteUrl)
        {
            var model = new AssetLoanAndReturnHeaderVM();
            model.CancelURL = _siteUrl + UrlResource.AssetLoanAndReturn;
            model.Professional.Choices = GetFromListHR("Professional Master", "Title", "Position", SiteUrl);
            model.TransactionType = Convert.ToString("Asset Loan And Return");
            model.CancelURL = _siteUrl + UrlResource.AssetLoanAndReturn;

            return model;
        }

        public ProfessionalDataVM GetProfMasterInfo(string fullname, string SiteUrl)
        {
            var caml = @"<View><Query>
                       <Where>
                          <Eq>
                             <FieldRef Name='Title' />
                             <Value Type='Text'>" + fullname + @"</Value>
                          </Eq>
                       </Where>
                    </Query>
                    <ViewFields>
                       <FieldRef Name='Title' />
                        <FieldRef Name='ID' />   
                       <FieldRef Name='Position' />
                       <FieldRef Name='Project_x002f_Unit' />
                       <FieldRef Name='mobilephonenr' />
                    </ViewFields>
                    <QueryOptions /></View>";
            var siteHr = SiteUrl.Replace("/bo", "/hr");
            var list = SPConnector.GetList("Professional Master", siteHr, caml);
            var viewmodel = new ProfessionalDataVM();
            foreach (var item in list)
            {
                viewmodel.ID = Convert.ToInt32(item["ID"]);
                viewmodel.CurrentPosition.Text = Convert.ToString(item["Project_x002f_Unit"]);
                viewmodel.MobileNumberOne = Convert.ToString(item["mobilephonenr"]);
                if ((item["Position"] as FieldLookupValue) != null)
                {
                    viewmodel.Position = (item["Position"] as FieldLookupValue).LookupValue;
                }
            }

            return viewmodel;
        }

        private IEnumerable<string> GetFromListHR(string listname, string f1, string f2, string siteUrl)
        {
            var siteHr = siteUrl.Replace("/bo", "/hr");
            List<string> _choices = new List<string>();
            var listItems = SPConnector.GetList(listname, siteHr);
            foreach (var item in listItems)
            {
                if (f2 != null)
                {
                    var position = "";
                    if ((item["Position"] as FieldLookupValue) != null)
                    {
                        position = (item["Position"] as FieldLookupValue).LookupValue;
                    }
                    _choices.Add(item[f1] + "-" + position);
                }
                else
                {
                    _choices.Add(item[f1].ToString());
                }
                //var listProfMasBO = SPConnector.GetList(listname, siteUrl);
                //foreach(var pmbo in listProfMasBO)
                //{

                //}
            }
            return _choices.ToArray();
        }

        //public int? CreateDetails(int? headerID, AssetLoanAndReturnItemVM item, string SiteUrl = null)
        //{
        //    var updatedValues = new Dictionary<string, object>();
        //    updatedValues.Add("assetloanandreturn", new FieldLookupValue { LookupId = Convert.ToInt32(headerID) });
        //    updatedValues.Add("assetsubasset", new FieldLookupValue { LookupId = Convert.ToInt32(item.AssetSubAsset.Value.Value) });
        //    updatedValues.Add("estreturndate", item.EstReturnDate);
        //    updatedValues.Add("returndate", item.ReturnDate);
        //    updatedValues.Add("status", "LOAN");
        //    try
        //    {
        //        SPConnector.AddListItem(SP_ASSLNRDetails_LIST_NAME, updatedValues, SiteUrl);
        //    }
        //    catch (Exception e)
        //    {
        //        logger.Error(e);
        //        throw new Exception(ErrorResource.SPInsertError);
        //    }

        //    return SPConnector.GetLatestListItemID(SP_ASSLNRDetails_LIST_NAME, SiteUrl);
        //}

        //AssetLoanAndReturnHeaderVM IAssetLoanAndReturnService.GetPopulatedModel(int? id)
        //{
        //    var model = new AssetLoanAndReturnHeaderVM();
        //    model.TransactionType = Convert.ToString("Asset Loan Return");
        //  //  model.Professional.Choices = GetChoicesFromList(SP_PROFMASTER_LIST_NAME, "ID", "Title", "Position");

        //    return model;
        //}

        //private IEnumerable<string> GetChoicesFromList(string listname, string v1, string v2 = null, string v3 = null)
        //{
        //    List<string> _choices = new List<string>();

        //    var siteHr = _siteUrl.Replace("/bo", "/hr");
        //    var listItems = SPConnector.GetList(listname, siteHr);
        //    foreach (var item in listItems)
        //    {
        //        if (v3 != null)
        //        {
        //            _choices.Add(item[v1] + "-" + item[v2].ToString() + "-" + item[v3].ToString());
        //        }
        //        else
        //        {
        //            _choices.Add(item[v1].ToString());
        //        }
        //    }
        //    return _choices.ToArray();

        //}

        //int IAssetLoanAndReturnService.CreateHeader(AssetLoanAndReturnHeaderVM viewmodel)
        //{
        //    var columnValues = new Dictionary<string, object>();
        //    //columnValues.add
        //    columnValues.Add("Title", viewmodel.TransactionType);
        //    string[] propesionalName = viewmodel.Professional.Value.Split;
        //    //columnValues.Add("Acceptance_x0020_Memo_x0020_No", memo[1]);

        //    // columnValues.Add("Professional", new FieldLookupValue { LookupId = Convert.ToInt32(propesionalName[0]) });

        // var siteHr = _siteUrl.Replace("/bo", "/hr");
        //var memoinfo = SPConnector.GetListItem(SP_ASSLNR_LIST_NAME, Convert.ToInt32(propesionalName[0]), siteHr);
        //columnValues.Add("Project_x002f_Unit", memoinfo["Project_x002f_Unit"]);
        //columnValues.Add("Contact_x0020_No", memoinfo["mobilephonenr"]);
        //columnValues.Add("professional", new FieldLookupValue { LookupId = Convert.ToInt32(viewmodel.Professional.Value) });
        //    columnValues.Add("professionalprojectunit", viewmodel.Project);
        //   // columnValues.Add("Professional", new FieldLookupValue { LookupId = Convert.ToInt32(viewmodel.Project) });
        //    columnValues.Add("professionalmobilephonenr", viewmodel.ContactNo);
        //    columnValues.Add("Loan_x0020_Date", viewmodel.LoanDate);
        //    columnValues.Add("Purpose", viewmodel.Purpose);

        //    try
        //    {
        //        SPConnector.AddListItem(SP_ASSLNR_LIST_NAME, columnValues, _siteUrl);
        //    }
        //    catch (Exception e)
        //    {
        //        logger.Error(e.Message);
        //    }
        //    var entitiy = new AssetLoanAndReturnHeaderVM();
        //    entitiy = viewmodel;
        //    return SPConnector.GetLatestListItemID(SP_ASSLNR_LIST_NAME, _siteUrl);
        //}

        //public int? CreateHeader(AssetLoanAndReturnHeaderVM viewmodel)
        //{

        //}
    }
}
