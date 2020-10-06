using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Interfaces
{
    public interface IHomeBL
    {
        void Index();

        Task<IActionResult> DownloadAndroidApp();
    }
}
