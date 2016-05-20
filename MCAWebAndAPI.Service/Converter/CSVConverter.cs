using CsvHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

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
            csvParser = new CsvParser(fileReader);
            CsvReader csvReader = new CsvReader(csvParser);

            var dataTable = new DataTable();
            while (csvReader.Read())
            {
                var row = dataTable.NewRow();
                foreach (DataColumn column in dataTable.Columns)
                {
                    row[column.ColumnName] = csvReader.GetField(column.DataType, column.ColumnName);
                }
                dataTable.Rows.Add(row);
            }

            return dataTable;
        }
    }
}
