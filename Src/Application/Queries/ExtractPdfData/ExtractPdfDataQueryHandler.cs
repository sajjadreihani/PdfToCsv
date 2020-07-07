using MediatR;
using PdfToCsv.Application.Common.Interfaces;
using PdfToCsv.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PdfToCsv.Application.Queries.ExtractPdfData
{
    public class ExtractPdfDataQueryHandler : IRequestHandler<ExtractPdfDataQuery,PdfOutput>
    {
        private readonly IPdfService pdfService;

        public ExtractPdfDataQueryHandler(IPdfService pdfService)
        {
            this.pdfService = pdfService;
        }

        public async Task<PdfOutput> Handle(ExtractPdfDataQuery request, CancellationToken cancellationToken)
        {
            return pdfService.ExtractPdf(request.PdfFile);

            //return Unit.Value;
        }
    }
}
