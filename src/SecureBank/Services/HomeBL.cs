using Microsoft.AspNetCore.Mvc;
using SecureBank.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Services
{
    public class HomeBL : IHomeBL
    {
        public virtual Task<IActionResult> DownloadAndroidApp()
        {
            IActionResult result = new NotFoundResult();

            return Task.FromResult(result);
        }

        public virtual void Index()
        {
        }
    }
}
