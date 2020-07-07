using MediatR;
using PdfToCsv.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PdfToCsv.Application.Queries.ExportToCSV
{
    public class ExportCSVQuery : IRequest<Stream>
    {
        public PdfOutput Input { get; set; }
    }
}
