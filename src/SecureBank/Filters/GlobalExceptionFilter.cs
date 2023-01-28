using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SecureBank.Ctf.Models;

namespace SecureBank.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger _logger = LogManager.GetLogger("global_exception");

        public void OnException(ExceptionContext context)
        {
            CtfOptions ctfOptions = context.HttpContext.RequestServices
                .GetRequiredService<IOptions<CtfOptions>>().Value;

            List<string> allowdErrors = new List<string>();

            if (ctfOptions.IsCtfEnabled)
            {
                if (ctfOptions.CtfChallengeOptions.ExceptionHandlingTransactionCreate)
                {
                    allowdErrors.Add("/api/Transaction/Create");
                }

                if (ctfOptions.CtfChallengeOptions.ExceptionHandlingTransactionUpload)
                {
                    allowdErrors.Add("/Upload/UploadTransactions");
                }
            }
            else
            {
                allowdErrors.Add("/api/Transaction/Create");
                allowdErrors.Add("/Upload/UploadTransactions");
            }


            if (allowdErrors.Any(x => x == context.HttpContext.Request.Path))
            {
                //Allow developer page exception
                return;
            }

            _logger.Error(context.Exception, "There was an exception");

            ViewResult result = new ViewResult { ViewName = "Error" };

            context.Result = result;
        }
    }
}
