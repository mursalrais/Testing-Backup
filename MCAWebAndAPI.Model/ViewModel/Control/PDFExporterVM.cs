using System.Collections.Generic;
using System.Web;
using System;
using MCAWebAndAPI.Model.ViewModel.Control;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MCAWebAndAPI.Model.ViewModel.Control
{
    public class PDFExporterVM
    {
        public string Url { get; set; }
        public int? WebPageWidth { get; set; }
        public int? WebPageHeight { get; set; }

        

        ComboBoxVM _pdfPageSize = new ComboBoxVM() ;        
        ComboBoxVM _PdfPageOrientation = new ComboBoxVM() ;

        [UIHint("ComboBox")]
        public ComboBoxVM PdfPageSize
        {
            get
            {
                return _pdfPageSize;
            }

            set
            {
                _pdfPageSize = value;
            }
        }
                
        [UIHint("ComboBox")]
        public ComboBoxVM PdfPageOrientation
        {
            get
            {
                return _PdfPageOrientation;
            }

            set
            {
                _PdfPageOrientation = value;
            }
        }
    }
}
