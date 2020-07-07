using System;
using System.Collections.Generic;
using System.Text;

namespace PdfToCsv.Application.Common.Models
{
    public class PdfOutput
    {
        public string Name { get; set; }
        public string Date { get; set; }
        public string StartBalance { get; set; }
        public string FinalBalance { get; set; }
        public string Period { get; set; }
        public List<TableData> Data { get; set; }
    }

    public class TableData
    {
        public string TransactionDate { get; set; }
        public string ProccessDate { get; set; }
        public string TransactionData { get; set; }
        public string ForigenAmount { get; set; }
        public string Amount { get; set; }
        public string ForigenDetail { get; set; }
        public string DataDetail { get; set; }
    }
}
