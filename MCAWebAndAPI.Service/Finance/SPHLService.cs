using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using MCAWebAndAPI.Model.ViewModel.Form.Finance.SPHL;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using NLog;

namespace MCAWebAndAPI.Service.Finance
{
    public class SPHLService : ISPHLService
    {
        static Logger logger = LogManager.GetCurrentClassLogger();

        private const string LIST_NAME = "SPHL";
        private const string DOC_LIST_NAME = "SPHL Documents";
        private const string FIELD_NAME_NO = "Title";
        private const string FIELD_NAME_DATE = "Date";
        private const string FIELD_NAME_AMOUNT = "Amount";
        private const string FIELD_NAME_REMARKS = "Remarks";

        private string _siteUrl = string.Empty;

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = siteUrl;
        }

        public int? CreateSPHL(SPHLVM sphl)
        {
           int? result = null;
           var columnValues = new Dictionary<string, object>
           {
               {FIELD_NAME_NO, sphl.No},
               {FIELD_NAME_DATE, sphl.Date},
               {FIELD_NAME_AMOUNT, sphl.AmountIDR},
               {FIELD_NAME_REMARKS, sphl.Remarks}
            };

            try
            {
                SPConnector.AddListItem(LIST_NAME, columnValues, _siteUrl);
                result = SPConnector.GetLatestListItemID(LIST_NAME, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }

            return result;
        }

        public async Task CreateSPHLAttachmentAsync(int? ID, IEnumerable<HttpPostedFileBase> documents)
        {
            CreateSPHLAttachment(ID, documents);
        }

        private void CreateSPHLAttachment(int? ID, IEnumerable<HttpPostedFileBase> attachment)
        {
            if (ID != null)
            {
                foreach (var doc in attachment)
                {
                    var updateValue = new Dictionary<string, object>();
                    updateValue.Add(LIST_NAME, new FieldLookupValue { LookupId = Convert.ToInt32(ID) });
                    try
                    {
                        SPConnector.UploadDocument(DOC_LIST_NAME, updateValue, doc.FileName, doc.InputStream, _siteUrl);
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
}
