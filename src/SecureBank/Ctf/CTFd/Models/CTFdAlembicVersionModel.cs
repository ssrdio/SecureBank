using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Ctf.CTFd.Models
{
    public class CTFdAlembicVersionModel
    {
        public string VersionNum { get; set; }

        public CTFdAlembicVersionModel(string versionNum)
        {
            VersionNum = versionNum;
        }
    }
}
