using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Models.PortalSearch
{
    public class PortalSearchModel
    {
        public string SearchString { get; set; }
        public List<string> Results { get; set; }
        public PortalSearchModel()
        {
            Results = new List<string>();
        }
    }
}
