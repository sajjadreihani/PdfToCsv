using PdfToCsv.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Text.RegularExpressions;
using PdfToCsv.Application.Common.Models;
using System.Linq;
using Apitron.PDF.Kit;
using Microsoft.EntityFrameworkCore.Internal;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Runtime.Serialization;

namespace PdfToCsv.Infrastructures
{
    public class PdfService : IPdfService
    {
        private readonly IHostingEnvironment hostingEnvironment;

        public PdfService(IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }

        public PdfOutput ExtractPdf(string pdfFile)
        {
            string pdfPath = Path.Combine(hostingEnvironment.WebRootPath, pdfFile.Replace("wwwroot", "").Substring(1).Replace("/", "\\"));
            FileStream docStream = new FileStream(pdfPath, FileMode.Open, FileAccess.Read);
            PdfOutput output = new PdfOutput()
            {
                Data = new List<TableData>()
            };

            FixedDocument doc = new FixedDocument(docStream);

            string extractedTexts = doc.Pages[0].ExtractText(Apitron.PDF.Kit.Extraction.TextExtractionOptions.FormattedText);
            string[] rows = extractedTexts.Split("\r\n");

            for (int i = 0; i < rows.Length; i++)
            {
                if(rows[i].Contains("Kaartnummer")&&rows[i].Contains("Datum")&&rows[i].Contains("maandafrekening"))
                {
                    output.Name = Regex.Match(rows[i + 1], "([A-Z]+[ ]*)+").Value;
                    output.Date = Regex.Match(rows[i + 1], "(0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])[- /.]([0-9]{2})").Value;
                    i++;
                    continue;
                }
                if (rows[i].Contains("Vorigsaldo") && rows[i].Contains("Tebetalen"))
                {
                    var matches = Regex.Match(rows[i + 1], @"\d{1,3}(\.\d{3})*(,\d{2})");
                    output.StartBalance = matches.Value;

                    var tempMatch = matches;
                    while (matches.Success)
                    {
                        tempMatch = matches;
                        matches = matches.NextMatch();
                    }

                    output.FinalBalance = tempMatch.Value;
                    i++;
                    continue;
                }
                if(rows[i].Contains("Periode:"))
                {
                    output.Period = Regex.Match(rows[i], @"(\d+[a-z ]*)+").Value.Replace("tot", "tot ");
                    break;
                }

            }

            output.Data.AddRange(rows.Where(r => Regex.IsMatch(r, @"((\d{1,2})+[a-z]{3})") && Regex.IsMatch(r, @"\d{1,3}(\.\d{3})*(,\d{2})"))
            .Select((r) => new TableData
            {
                Amount = Regex.Match(r, @"\d{1,3}(\.\d{3})*(,\d{2})").Value,
                ForigenAmount = Regex.Match(r, @"\d{1,3}(\,\d{3})*([.]\d{2})").Value,
                ProccessDate = Regex.Match(r, @"((\d{1,2})+[a-z]{3})").NextMatch().Value,
                TransactionDate = Regex.Match(r, @"((\d{1,2})+[a-z]{3})").Value,
                TransactionData = Regex.Replace(Regex.Replace(Regex.Replace(r, @"\d{1,3}(\.\d{3})*(,\d{2})", ""), @"\d{1,3}(\,\d{3})*([.]\d{2})", ""), @"((\d{1,2})+[a-z]{3})", ""),
                ForigenDetail = rows.Length - 1 >= rows.IndexOf(r) + 2 ? Regex.Match(rows[rows.IndexOf(r) + 1].Replace(" ", ""), "^[A-Za-z]+").Value : "",
                DataDetail = rows.Length - 1 >= rows.IndexOf(r) + 2 ? Regex.Match(rows[rows.IndexOf(r) + 2].Replace(" ", ""), @"^[A-Z]{2,}[A-Za-z\d.+,]*").Value : ""

            }));
            for (int i = 1; i < doc.Pages.Count; i++)
            {

                //Extract text from first page.
                extractedTexts = doc.Pages[i].ExtractText(Apitron.PDF.Kit.Extraction.TextExtractionOptions.FormattedText);
                rows = extractedTexts.Split("\r\n");

                output.Data.AddRange(rows.Where(r => Regex.IsMatch(r, @"((\d{1,2})+[a-z]{3})") && Regex.IsMatch(r, @"\d{1,3}(\.\d{3})*(,\d{2})"))
                .Select((r) => new TableData
                {
                    Amount = Regex.Match(r, @"\d{1,3}(\.\d{3})*(,\d{2})").Value,
                    ForigenAmount = Regex.Match(r, @"\d{1,3}(\,\d{3})*([.]\d{2})").Value,
                    ProccessDate = Regex.Match(r, @"((\d{1,2})+[a-z]{3})").NextMatch().Value,
                    TransactionDate = Regex.Match(r, @"((\d{1,2})+[a-z]{3})").Value,
                    TransactionData = Regex.Replace(Regex.Replace(Regex.Replace(r, @"\d{1,3}(\.\d{3})*(,\d{2})",""), @"\d{1,3}(\,\d{3})*([.]\d{2})",""), @"((\d{1,2})+[a-z]{3})",""),
                    ForigenDetail = rows.Length - 1 >= rows.IndexOf(r) + 2 ? Regex.Match(rows[rows.IndexOf(r) + 1].Replace(" ",""), "^[A-Za-z]+").Value : "",
                    DataDetail = rows.Length - 1 >= rows.IndexOf(r) + 2? Regex.Match(rows[rows.IndexOf(r) + 2].Replace(" ",""), @"^[A-Z]{2,}[A-Za-z\d.+,]*").Value:""
                }));
            }

            doc.Dispose();

            docStream.Close();
            File.Delete(pdfPath);
            return output;
            
        }

        public Stream ExportToCSV(PdfOutput input)
        {
            var stream = new MemoryStream();
            using (var writeFile = new StreamWriter(stream, leaveOpen: true))
            {
                var configuration = new CsvConfiguration(new CultureInfo("de-DE"));
                input.Data.ForEach(d => d.ForigenAmount = d.ForigenAmount.Replace(".", ","));
                var csv = new CsvWriter(writeFile,configuration,true);
                csv.WriteRecords(input.Data);
                writeFile.Flush();
            }
            stream.Position = 0; //reset stream

            return stream;
        }
    }
}
