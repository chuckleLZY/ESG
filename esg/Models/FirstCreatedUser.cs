using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace esg.Models
{
    public class FirstCreatedUser//本公司已建立客户
    {
        public int ErrorCode { get; set; }
        public string CompanyName { get; set; }
        public List<string> AdminAccount { get; set; }
        public List<string> DataAccount { get; set; }
        public List<string> ChildCompanyName { get; set; }

        public FirstCreatedUser()
        {
            AdminAccount = new List<string>();
            DataAccount = new List<string>();
            ChildCompanyName = new List<string>();
        }
    }
}
