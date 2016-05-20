﻿using CsvHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using MCAWebAndAPI.Service.Utils;

namespace MCAWebAndAPI.Service.Converter
{
    public class CSVConverter
    {
        static readonly Lazy<CSVConverter> lazy = new Lazy<CSVConverter>(() => new CSVConverter());
        ICsvParser csvParser;

        public static CSVConverter Instance { get { return lazy.Value; } }

        CSVConverter()
        {

        }

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
            DataTable personList = new DataTable();

            using (var csv = new CsvReader(fileReader))
            {
                var tes = csv.Configuration.Delimiter;
                csv.Configuration.Delimiter = ";";
                csv.Read(); //Do a read so we can get the headers
                foreach (var header in csv.FieldHeaders)
                {
                    personList.Columns.Add(header);
                }

                do //Do-while instead of a while loop because we already did the first Read()
                {
                    var row = personList.NewRow();
                    foreach (DataColumn col in personList.Columns)
                    {
                        row[col.ColumnName] = csv.GetField(col.DataType, col.ColumnName);
                    }
                    personList.Rows.Add(row);
                }
                while (csv.Read());
                return personList;
            }
        }

        public bool MassUpload(string ListName, DataTable CSVDataTable, string SiteUrl = null)
        {
            var updatedValues = new Dictionary<string, object>();
            for (int i = 0; i < CSVDataTable.Rows.Count; i++)
            {
                for (int j = 0; j < CSVDataTable.Columns.Count; j++)
                {
                    updatedValues.Add(CSVDataTable.Columns[j].ColumnName, CSVDataTable.Rows[i].ItemArray[j]);
                }
                try
                {
                    SPConnector.AddListItem(ListName, updatedValues, SiteUrl);
                }
                catch (Exception e)
                {
                    //logger.Error(e.Message);
                    var tes = e.Message;
                    return false;
                }
                updatedValues = new Dictionary<string, object>();

            }
            //updatedValues.Add("Title", header.TransactionType);
            //updatedValues.Add("AssignmentDate", header.Date);
            //updatedValues.Add("HolderID", new FieldLookupValue { LookupId = Convert.ToInt32(header.AssetHolderFrom.Value) });
            //updatedValues.Add("HolderIDTo", new FieldLookupValue { LookupId = Convert.ToInt32(header.AssetHolderTo.Value) });



            //return SPConnector.GetInsertedItemID(SP_HEADER_LIST_NAME, _siteUrl);
            return true;
        }
    }
}
