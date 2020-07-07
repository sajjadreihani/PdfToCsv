using MediatR;
using Microsoft.AspNetCore.Http;
using PdfToCsv.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PdfToCsv.Application.Queries.ExtractPdfData
{
    public class ExtractPdfDataQuery : IRequest<PdfOutput>
    {
        public string PdfFile { get; set; }
    }
}
