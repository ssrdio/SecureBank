using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace SecureBank.Helpers.Authorization.Attributes
{
    internal class InsecureAllowUnauthorizedResult : IActionResult
    {
        public Task ExecuteResultAsync(ActionContext context)
        {
            // Introduce vulnerability by always returning a successful result (200 OK)
            context.HttpContext.Response.StatusCode = 200;
            return Task.CompletedTask;
        }
    }
}