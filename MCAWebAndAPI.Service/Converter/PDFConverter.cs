using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuesPechkin;

namespace MCAWebAndAPI.Service.Converter
{ 
    public sealed class PDFConverter
    {
        static IConverter converter;

        static readonly Lazy<PDFConverter> lazy = new Lazy<PDFConverter>(() => new PDFConverter());

        public static PDFConverter Instance { get { return lazy.Value; } }

        PDFConverter()
        {
            converter = new ThreadSafeConverter(
                                new RemotingToolset<PdfToolset>(
                                    new Win64EmbeddedDeployment(
                                        new TempFolderDeployment())));
        }

        byte[] Convert(HtmlToPdfDocument document)
        {
            byte[] result = converter.Convert(document);

            return result;
        }

        public byte[] ConvertFromHTML(string pageTitle, string stringHTML)
        {
            return ConvertFromHTML(pageTitle, stringHTML, string.Empty);
        }

        public byte[] ConvertFromHTML(string pageTitle, string stringHTML, string footer)
        {
            var document = new HtmlToPdfDocument
            {
                GlobalSettings = {
                    ProduceOutline = true,
                    DocumentTitle = pageTitle,
                    PaperSize = PaperKind.A4, // Implicit conversion to PechkinPaperSize
                    Margins =
                    {
                        All = 1.375,
                        Unit = Unit.Centimeters
                    }
                },
                Objects = {
                    new ObjectSettings
                    {
                        HtmlText = stringHTML,
                        FooterSettings = new TuesPechkin.FooterSettings { LeftText = footer, FontSize = 8},
                    }
                }
            };

            return Convert(document);
        }

        public byte[] ConvertFromURL(string pageTitle, string url)
        {
            var document = new HtmlToPdfDocument
            {
                GlobalSettings = {
                    ProduceOutline = true,
                    DocumentTitle = pageTitle,
                    PaperSize = PaperKind.A4, // Implicit conversion to PechkinPaperSize
                    Margins =
                    {
                        All = 1.375,
                        Unit = Unit.Centimeters
                    }
                },
                Objects = {
                    new ObjectSettings { PageUrl = url }
                }
            };

            return Convert(document);
        }

        public byte[] ConvertFromHTMLLandscape(string pageTitle, string stringHTML)
        {
            var document = new HtmlToPdfDocument
            {
                GlobalSettings = {
                    ProduceOutline = true,
                    DocumentTitle = pageTitle,
                    PaperSize = PaperKind.A4Rotated, // Implicit conversion to PechkinPaperSize
                    Margins =
                    { 
                        All = 0.3,
                        Unit = Unit.Centimeters
                    }
                },
                Objects = {
                    new ObjectSettings { HtmlText = stringHTML }
                }
            };

            return Convert(document);
        }
    }
}
