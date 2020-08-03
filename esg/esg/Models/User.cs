using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace esg.Models
{
    public class User
    {
        public int CompanyId { get; set; }
        public string UserAccount { get; set; }
        public string UserPassword { get; set; }
        public string Email { get; set; }
        public int Level { get; set; }
    }
}
