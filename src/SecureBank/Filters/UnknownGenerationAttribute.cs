using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SecureBank.Ctf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class UnknownGenerationAttribute : Attribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            CtfOptions ctfOptions =
                context.HttpContext.RequestServices
                .GetRequiredService<IOptions<CtfOptions>>().Value;

            if (ctfOptions.CtfChallengeOptions.UnknownGeneration)
            {
                CtfChallengeModel unknowChallenge = ctfOptions.CtfChallenges
                    .Where(x => x.Type == CtfChallengeTypes.UnknownGeneration)
                    .Single();

                context.HttpContext.Response.Headers
                    .Add(unknowChallenge.FlagKey, unknowChallenge.Flag);
            }
            else if (ctfOptions.IsCtfEnabled)
            {
                context.Result = new NotFoundResult();
            }
        }
    }
}
