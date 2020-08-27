using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace esg.Models
{
    public class DataDetail
    {
        public String ESG_Id { get; set; }
        public int Type { get; set; }
    }
    public class DataDetails
    {
        public int Report_Year { get; set; }
        public int ReportId { get; set; }
        public List<DataDetail> dataDetails { get; set; }
    }
}
