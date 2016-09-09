using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using NLog;

namespace MCAWebAndAPI.Service.Finance
{
    public class CSVErrorLogService : ICSVErrorLogService
    {
        static Logger logger = LogManager.GetCurrentClassLogger();

        private const string ListName = "Outstanding Advance CSV Upload Error";
        private const string FieldName_ID = "ID";
        private const string FieldName_Key = "Title";
        private const string FieldName_FileName = "FileName";
        private const string FieldName_FieldName = "FieldName";
        private const string FieldName_Value = "Value";
        private const string FieldName_ErrorDescription = "ErrorDescription";

        private string siteUrl = string.Empty;

        public CSVErrorLogService(string siteUrl)
        {
            this.siteUrl = siteUrl;
        }

        public void SetSiteUrl(string siteUrl)
        {
            this.siteUrl = siteUrl;
        }

        public CSVErrorLogVM Get(int? ID)
        {
            CSVErrorLogVM viewModel = new CSVErrorLogVM();
            if (ID != null)
            {
                var model = SPConnector.GetListItem(ListName, ID, siteUrl);
                viewModel = ConvertVM(model);
            }

            return viewModel;
        }

        public IEnumerable<CSVErrorLogVM> GetAll(string key)
        {
            var result = new List<CSVErrorLogVM>();

            var caml = CamlQueryUtil.Generate(FieldName_Key, "Text", key);


            foreach (var item in SPConnector.GetList(ListName, siteUrl, caml))
            {
                result.Add(
                    new CSVErrorLogVM
                    {
                        ID = Convert.ToInt32(item[FieldName_ID]),
                        Title = Convert.ToString(item[FieldName_Key]),
                        FileName = Convert.ToString(item[FieldName_FileName]),
                        FieldName = Convert.ToString(item[FieldName_FieldName]),
                        Value = Convert.ToString(item[FieldName_Value]),
                        ErrorDescription = Convert.ToString(item[FieldName_ErrorDescription])
                    }
                );
            }

            return result;
        }

        public List<CSVErrorLogVM> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Save(string key, IEnumerable<CSVErrorLogVM> viewModels)
        {
            int? result = null;

            var mastervalue = new Dictionary<string, Dictionary<string, object>>();
            int i = 0;

            foreach (var viewModel in viewModels)
            {
                var columnValues = new Dictionary<string, object>
                {
                    { FieldName_Key, key },
                    { FieldName_FileName, viewModel.FileName },
                    { FieldName_FieldName, viewModel.FieldName },
                    { FieldName_Value, viewModel.Value },
                    { FieldName_ErrorDescription, viewModel.ErrorDescription }
                };

                mastervalue.Add(i + ";Add", columnValues);
                i++;
            }

            try
            {
                SPConnector.AddListItemAsync(ListName, mastervalue, siteUrl);
                result = SPConnector.GetLatestListItemID(ListName, siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);

                throw e;
            }
        }

        private CSVErrorLogVM ConvertVM(ListItem listItem)
        {
            return new CSVErrorLogVM
            {
                ID = Convert.ToInt32(listItem[FieldName_ID]),
                Title = listItem[FieldName_Key].ToString(),
                FileName = listItem[FieldName_FileName].ToString(),
                FieldName = listItem[FieldName_FieldName].ToString(),
                Value = listItem[FieldName_Value].ToString(),
                ErrorDescription = listItem[FieldName_ErrorDescription].ToString()
            };
        }
    }
}