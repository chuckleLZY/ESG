using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace esg.Models
{
    public class SecondCreatedUser
    {
        public string CompanyName { get; set; }
        public List<string> AdminAccount { get; set; }
        public List<string> DataAccount { get; set; }

        public SecondCreatedUser()
        {
            AdminAccount = new List<string>();
            DataAccount = new List<string>();
        }
    }
}
