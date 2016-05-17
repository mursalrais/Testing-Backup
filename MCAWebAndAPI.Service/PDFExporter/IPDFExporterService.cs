using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Control;

namespace MCAWebAndAPI.Service.PDFExporter
{
    public interface IPDFExporterService
    {
        void SetSiteUrl(string siteUrl);

        PDFExporterVM GetPDFExporter();

        bool CreatePDFExporter(PDFExporterVM PDFExporter);



    }
}
