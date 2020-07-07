using Microsoft.AspNetCore.Http;
using PdfToCsv.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PdfToCsv.Application.Common.Interfaces
{
    public interface IPdfService
    {
        PdfOutput ExtractPdf(string pdfFile);
        Stream ExportToCSV(PdfOutput input);
    }
}
