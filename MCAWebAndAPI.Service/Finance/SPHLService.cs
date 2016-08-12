using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using MCAWebAndAPI.Model.ViewModel.Form.Finance.SPHL;
using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using NLog;

namespace MCAWebAndAPI.Service.Finance
{
    public class SPHLService : ISPHLService
    {
        static Logger logger = LogManager.GetCurrentClassLogger();

        private const string ListName = "SPHL";
        private const string ListName_Document = "SPHL Documents";
        private const string ListName_ID = "ID";
        private const string ListName_No = "Title";
        private const string ListName_Date = "Date";
        private const string ListName_Amount = "Amount";
        private const string ListName_REmarks = "Remarks";

        private string _siteUrl = string.Empty;

        public void SetSiteUrl(string siteUrl)
        {
            this._siteUrl = siteUrl;
        }

        public int? CreateSPHL(SPHLVM viewMOdel)
        {
           int? result = null;
           var columnValues = new Dictionary<string, object>
           {
               {ListName_No, viewMOdel.No},
               {ListName_Date, viewMOdel.Date},
               {ListName_Amount, viewMOdel.AmountIDR},
               {ListName_REmarks, viewMOdel.Remarks}
            };

            try
            {
                SPConnector.AddListItem(ListName, columnValues, _siteUrl);
                result = SPConnector.GetLatestListItemID(ListName, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);

                throw e;
            }

            return result;
        }

        public bool UpdateSPHL(SPHLVM viewMOdel)
        {
           bool result = false;
           var columnValues = new Dictionary<string, object>
           {
               {ListName_No, viewMOdel.No},
               {ListName_Date, viewMOdel.Date},
               {ListName_Amount, viewMOdel.AmountIDR},
               {ListName_REmarks, viewMOdel.Remarks}
            };

            try
            {
                SPConnector.UpdateListItem(ListName, viewMOdel.ID, columnValues, _siteUrl);
                result = true;
            }
            catch (Exception e)
            {
                logger.Error(e.Message);

                throw e;
            }

            return result;
        }

        public SPHLVM GetDataSPHL(int? ID=null)
        {
            SPHLVM viewModel = new SPHLVM();
            if (ID != null)
            {
                var model = SPConnector.GetListItem(ListName, ID, _siteUrl);
                viewModel = ConvertToSPHLVM(model);
                viewModel.DocumentUrl = GetDocumentUrl(viewModel.ID);
            }

            return viewModel;
        }

        public bool CheckExistingSPHLNo(string no)
        {
            var caml = @"<View><Query> <Where><Eq><FieldRef Name='"+ ListName_No + "' /><Value Type='Text'>" + no.ToString() + "</Value></Eq></Where></Query></View>";
            foreach (var item in SPConnector.GetList(ListName, _siteUrl, caml))
            {
                return false;
            }
            return true;
        }

        public async Task CreateSPHLAttachmentAsync(int? ID, string sphlNo, IEnumerable<HttpPostedFileBase> documents)
        {
            CreateSPHLAttachment(ID,sphlNo, documents);
        }

        private void CreateSPHLAttachment(int? ID, string sphlNo, IEnumerable<HttpPostedFileBase> attachment)
        {
            if (ID != null)
            {
                foreach (var doc in attachment)
                {
                    if (doc != null)
                    {
                        var filename = sphlNo + "_" + doc.FileName;
                        var updateValue = new Dictionary<string, object>();
                        updateValue.Add(ListName, new FieldLookupValue { LookupId = Convert.ToInt32(ID) });

                        try
                        {
                            SPConnector.UploadDocument(ListName_Document, updateValue, filename, doc.InputStream, _siteUrl);
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

        private SPHLVM ConvertToSPHLVM(ListItem listItem)
        {
            return new SPHLVM
            {
                ID = Convert.ToInt32(listItem[ListName_ID]),
                No = listItem[ListName_No].ToString(),
                Date = Convert.ToDateTime(listItem[ListName_Date].ToString()),
                AmountIDR = Convert.ToDecimal(listItem[ListName_Amount]),
                Remarks = listItem[ListName_REmarks] == null ? "" : listItem[ListName_REmarks].ToString()
            };
        }

        private string GetDocumentUrl(int? ID)
        {
            return string.Format(UrlResource.SPHLDocumentByID, _siteUrl, ID);
        }
    }
}
