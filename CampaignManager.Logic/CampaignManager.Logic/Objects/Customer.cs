using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampaignManager.Logic
{
    public class Customer
    {
        public int Id { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public string City { get; set; }
        public double Deposit { get; set; }
        public bool IsNew { get; set; }
    }
}
