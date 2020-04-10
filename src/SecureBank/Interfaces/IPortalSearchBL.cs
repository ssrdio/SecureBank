using SecureBank.Models.PortalSearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Interfaces
{
    public interface IPortalSearchBL
    {
        PortalSearchModel Search(string search);
    }
}
