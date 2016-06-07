using CsvHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using MCAWebAndAPI.Service.Utils;
using NLog;

namespace MCAWebAndAPI.Service.Converter
{

    public class CSVConverter
    {
        static readonly Lazy<CSVConverter> lazy = new Lazy<CSVConverter>(() => new CSVConverter());
        ICsvParser csvParser;

        public static CSVConverter Instance { get { return lazy.Value; } }

        Logger logger = LogManager.GetCurrentClassLogger();

        public IEnumerable<string[]> ToStrings(StreamReader fileReader)
        {
            csvParser = new CsvParser(fileReader);
            CsvReader csvReader = new CsvReader(csvParser);
            string[] headers = { };
            List<string[]> rows = new List<string[]>();
            string[] row;
            while (csvReader.Read())
            {
                // Gets Headers if they exist
                if (csvReader.Configuration.HasHeaderRecord && !headers.Any())
                {
                    headers = csvReader.FieldHeaders;
                }
                row = new string[headers.Count()];
                for (int j = 0; j < headers.Count(); j++)
                {
                    row[j] = csvReader.GetField(j);
                }
                rows.Add(row);
            }

            return rows;
        }

        public DataTable ToDataTable(StreamReader fileReader)
        {
            DataTable dataTable = new DataTable();

            using (var csv = new CsvReader(fileReader))
            {
                csv.Configuration.TrimHeaders = true;
                csv.Configuration.Delimiter = ";";
                csv.Configuration.IgnoreHeaderWhiteSpace = true;
                csv.Read(); //Do a read so we can get the headers

                for (int i = 0; i < csv.FieldHeaders.Length; i++)
                {
                    string columnName, columnType = string.Empty;
                    try
                    {
                        columnName = csv.FieldHeaders[i].Split(':')[0];
                        columnType = csv.FieldHeaders[i].Split(':')[1];
                    }
                    catch (Exception e)
                    {
                        logger.Error(e);
                        throw e;
                    }

                    columnName = columnName.Trim();
                    var dataColumn = new DataColumn(columnName, Type.GetType(columnType));
                    dataTable.Columns.Add(dataColumn);

                    csv.FieldHeaders[i] = columnName;
                }

                var indexID = 0;
                do //Do-while instead of a while loop because we already did the first Read()
                {
                    var row = dataTable.NewRow();

                    for (int i = 0; i < dataTable.Columns.Count; i++)
                    {
                        var col = dataTable.Columns[i];

                        if (string.Compare(col.ColumnName, "ID", StringComparison.OrdinalIgnoreCase) == 0)
                            row[col.ColumnName] = indexID++;
                        else
                        {
                            try
                            {
                                row[col.ColumnName] = csv.GetField(col.DataType, i);
                            }
                            catch (Exception e)
                            {
                                row[col.ColumnName] = -1;
                            }
                        }
                    }

                    dataTable.Rows.Add(row);
                }
                while (csv.Read());
                return dataTable;
            }
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

        public bool MassUpload(string ListName, DataTable CSVDataTable, string SiteUrl = null)
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
            for (int i = 1; i < rowTotal; i++)
            {
                for (int j = 0; j < columnTotal; j++)
                {
                    if (isLookup(columnTechnicalNames[j]))
                    {
                        FormatUtil.GenerateUpdatedValueFromGivenDataTable(ref updatedValues, columnTypes[j],
                            columnTechnicalNames[j], CSVDataTable.Rows[i].ItemArray[j], lookup: true, skip: false);
                    }
                    else if (isSkipped(columnTechnicalNames[j])){
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
                    logger.Error(e);
                    throw e;
                }
                updatedValues = new Dictionary<string, object>();
            }
            
            return true;
        }
    }
}
