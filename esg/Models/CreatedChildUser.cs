using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace esg.Models
{
    public class CreatedChildUser//子公司已建立客户
    {
        public List<string> AdminAccount { get; set; }
        public List<string> DataAccount { get; set; }
        public List<int> userID{get; set;}
        public CreatedChildUser()
        {
            AdminAccount = new List<string>();
            DataAccount = new List<string>();
            userID=new List<int>();
        }
    }
}
