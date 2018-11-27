using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Replacer.Models
{
    public class Reason
    {
        public Reason()
        {
            this.NameReason = "Not found!!!";
            this.NameRecommendation = "Not found!!!";
            this.WasUsed = false;
        }
        public string NameReason { get; set; }
        public string NameRecommendation { get; set; }
        public bool WasUsed { get; set; }
    }
}
