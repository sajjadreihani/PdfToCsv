using MediatR;
using PdfToCsv.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PdfToCsv.Application.Queries.ExportToCSV
{
    public class ExportCSVQueryHandler : IRequestHandler<ExportCSVQuery, Stream>
    {
        private readonly IPdfService service;

        public ExportCSVQueryHandler(IPdfService service)
        {
            this.service = service;
        }

        public async Task<Stream> Handle(ExportCSVQuery request, CancellationToken cancellationToken)
        {
            return service.ExportToCSV(request.Input);
        }
    }
}
