using Microsoft.Extensions.DependencyInjection;
using PdfToCsv.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PdfToCsv.Infrastructures
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddTransient<IPdfService, PdfService>();
            return services;
        }
    }
}
