using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using NLog;

namespace MCAWebAndAPI.Service.Finance
{
    /// <summary>
    /// FIN14: Petty Cash Replenishment
    /// </summary>

    public class PettyCashReplenishmentService : IPettyCashReplenishmentService
    {
        private const string ListName = "Petty Cash Replenishment";
        private const string ListName_Document = "Petty Cash Replenishment Document";

        private const string FieldName_Date = "Date";
        private const string FieldName_Currency = "Currency";
        private const string FieldName_Amount = "Amount";
        private const string FieldName_Remarks = "Remarks";

        string siteUrl = null;
        static Logger logger = LogManager.GetCurrentClassLogger();

        public int Save(PettyCashReplenishmentVM viewModel)
        {
            var willCreate = viewModel.ID == null;
            var updatedValue = new Dictionary<string, object>();

            updatedValue.Add(FieldName_Date, viewModel.Date);
            updatedValue.Add(FieldName_Currency, viewModel.Currency.Value);
            updatedValue.Add(FieldName_Amount, viewModel.Amount);
            updatedValue.Add(FieldName_Remarks, viewModel.Remarks);
           
            try
            {
                if (willCreate)
                    SPConnector.AddListItem(ListName, updatedValue, siteUrl);
                else
                    SPConnector.UpdateListItem(ListName, viewModel.ID, updatedValue, siteUrl);
            }
            catch (ServerException e)
            {
                var errMsg = e.Message + Environment.NewLine + e.ServerErrorValue;
                logger.Error(errMsg);

#if DEBUG
                throw new Exception(errMsg);
#else
                 throw new Exception(ErrorResource.SPInsertError);
#endif
            }
            catch (Exception e)
            {
                logger.Error(e.Message);

#if DEBUG
                throw new Exception(e.Message);
#else
                 throw new Exception(ErrorResource.SPInsertError);
#endif
            }


            return SPConnector.GetLatestListItemID(ListName, siteUrl);
        }


        public async Task SaveAttachmentAsync(int? ID, string reference, IEnumerable<HttpPostedFileBase> documents)
        {
            SaveAttachment(ID, reference, documents);
        }

        private void SaveAttachment(int? ID, string reference, IEnumerable<HttpPostedFileBase> attachment)
        {
            if (ID != null)
            {
                foreach (var doc in attachment)
                {
                    if (doc != null)
                    {
                        var updateValue = new Dictionary<string, object>();
                        updateValue.Add(ListName, new FieldLookupValue { LookupId = Convert.ToInt32(ID) });

                        try
                        {
                            SPConnector.UploadDocument(ListName_Document, updateValue, doc.FileName, doc.InputStream, siteUrl);
                        }
                        catch (Exception e)
                        {
                            logger.Error(e.Message);
                            throw e;
                        }
                    }
                }
            }
        }

        public void SetSiteUrl(string siteUrl)
        {
            this.siteUrl = siteUrl;
        }

        public PettyCashReplenishmentVM Get(int? ID)
        {
            var viewModel = new PettyCashReplenishmentVM();

            if (ID != null)
            {
                var listItem = SPConnector.GetListItem(ListName, ID, siteUrl);
                viewModel = ConvertToVM(listItem);
            }

            return viewModel;

        }


        private PettyCashReplenishmentVM ConvertToVM(ListItem listItem)
        {
            PettyCashReplenishmentVM viewModel = new PettyCashReplenishmentVM();

            viewModel.Date= Convert.ToDateTime(listItem[FieldName_Date]);
            viewModel.Currency.Value = Convert.ToString(listItem[FieldName_Currency]);
            viewModel.Amount = Convert.ToDecimal(listItem[FieldName_Amount]);
            viewModel.Remarks = Convert.ToString(listItem[FieldName_Remarks]);
            
            return viewModel;
        }


    }
}
