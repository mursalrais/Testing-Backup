using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Control;
using NLog;
using SelectPdf;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client.Utilities;

namespace MCAWebAndAPI.Service.PDFExporter
{
    public class PDFExporterService : IPDFExporterService
    {
        string _siteUrl = null;
        static Logger logger = LogManager.GetCurrentClassLogger();

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = siteUrl;
        }

        public PDFExporterVM GetPDFExporter()
        {
            var viewModel = new PDFExporterVM();
            viewModel.WebPageWidth = 1024;

            viewModel.PdfPageSize.Choices = new string[] { "A4", "A5", "A6" };
            viewModel.PdfPageOrientation.Choices = new string[] { "Potrait", "Landscape" };

            return viewModel;
        }

        public bool CreatePDFExporter(PDFExporterVM PDFExporter)
        {
            string _url = PDFExporter.Url;
            string _pageSize = PDFExporter.PdfPageSize.Value;
            PdfPageSize pageSize = (PdfPageSize)Enum.Parse(typeof(PdfPageSize),
                _pageSize, true);

            string _pdfOrientation = PDFExporter.PdfPageOrientation.Value;
            PdfPageOrientation pdfOrientation =
                (PdfPageOrientation)Enum.Parse(typeof(PdfPageOrientation),
                _pdfOrientation, true);

            int webPageWidth = 1024;
            try
            {
                webPageWidth = Convert.ToInt32(PDFExporter.WebPageWidth.Value);
            }
            catch { }

            int webPageHeight = 0;
            try
            {
                webPageHeight = Convert.ToInt32(PDFExporter.WebPageHeight.Value);
            }
            catch { }



            // instantiate a html to pdf converter object
            HtmlToPdf converter = new HtmlToPdf();

            // set converter options
            converter.Options.PdfPageSize = pageSize;
            converter.Options.PdfPageOrientation = pdfOrientation;
            converter.Options.WebPageWidth = webPageWidth;
            converter.Options.WebPageHeight = webPageHeight;

            // create a new pdf document converting an url
            //PdfDocument doc = converter.ConvertUrl(_url);

            // save pdf document
            //doc.Save(Response, false, "Sample.pdf");
            //doc.Save(@"D:\Sample.pdf");

            SPConnector.sendEmail("antonious.tan@eceos.com","content","subject");


            // close pdf document
            //doc.Close();
            return true;
        }

      
    }
}
