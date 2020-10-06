using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using SecureBank.Ctf.Models;
using SecureBank.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

namespace SecureBank.Ctf.Services
{
    public class CtfHomeBL : HomeBL
    {
        private const string ANDROIDAPP_PATH = "SecureFiles/SecureBank.apk";

        private readonly IWebHostEnvironment _webHostEnvironment;


        private readonly CtfOptions _ctfOptions;

        private readonly ILogger<CtfHomeBL> _logger;

        public CtfHomeBL(IWebHostEnvironment webHostEnvironment, IOptions<CtfOptions> ctfOptions, ILogger<CtfHomeBL> logger)
        {
            _webHostEnvironment = webHostEnvironment;
            _ctfOptions = ctfOptions.Value;
            _logger = logger;
        }

        public override void Index()
        {
            base.Index();
        }

        public async override Task<IActionResult> DownloadAndroidApp()
        {
            if(!_ctfOptions.CtfChallengeOptions.AndroidApp)
            {
                return await base.DownloadAndroidApp();
            }

            string path = Path.Combine(_webHostEnvironment.ContentRootPath, ANDROIDAPP_PATH);

            try
            {
                if(File.Exists(path))
                {
                    byte[] buffer = await File.ReadAllBytesAsync(path);

                    FileContentResult fileContentResult = new FileContentResult(buffer, MediaTypeNames.Application.Octet);
                    fileContentResult.FileDownloadName = "SecureBank.apk";

                    return fileContentResult;
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Error getting android app");
            }

            return new BadRequestObjectResult("faild_to_get_android_app");
        }
    }
}
