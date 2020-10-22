using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger _logger = LogManager.GetLogger("global_exception");

        public readonly string[] ALLOWD_INTERNAL_SERVER_ERRORS = { "/api/Transaction/Create" };

        public void OnException(ExceptionContext context)
        {
            if(ALLOWD_INTERNAL_SERVER_ERRORS.Any(x => x == context.HttpContext.Request.Path))
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
