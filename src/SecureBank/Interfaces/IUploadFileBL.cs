using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Interfaces
{
    public interface IUploadFileBL
    {
        string UploadFile(MemoryStream stream);
    }
}
