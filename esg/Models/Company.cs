using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace esg.Models
{
    public class Company
    {
        public int Level { get; set; }
        public string Name { get; set; }
        public int Parent { get; set; }
    }
    public class SubCompany
    {
        public int Com_id { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
    }
}
