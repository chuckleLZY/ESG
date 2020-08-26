using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace esg.Models
{
    public class Data
    {
        public string ESG_Id { get; set; }
        public int Report_Month { get; set; }
        //public string Data_f { get; set; }//定性指标
        //public double Data_t { get; set; }//定量指标
        public dynamic TData { get; set; }
        //public int Status { get; set; }
    }
    public class DataQualitative
    {
        public string EsgId { get; set; }
        public int ReportId { get; set; }
        public string Data { get; set; }
        public int Type { get; set; }
    }

    public class DataQuantitative
    {
        public string EsgId { get; set; }
        public int ReportId { get; set; }
        public double Data { get; set; }
        public int Type { get; set; }
    }
}
