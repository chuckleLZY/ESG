using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace esg.Models
{
    public class Report
    {
        public int ReportId { get; set; }
        //public int UserId { get; set; }
        //public int ReceiveId { get; set; }
        public int CompanyId { get; set; }
        public string ReportName { get; set; }
        public int Status { get; set; }
        public int Report_Year { get; set; }
    }

    public class ReportForm
    {
        public int ReportId { get; set; }
        public int CompanyId { get; set; }
        public string ReportName { get; set; }
        public int Status { get; set; }
        public int Report_Year { get; set; }
        public string CompanyName { get; set; }
    }
}
