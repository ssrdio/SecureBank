using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]/[action]", Order = 100)]
    [Produces("application/json")]
    [ProducesErrorResponseType(typeof(void))]
    public class ApiBaseController : ControllerBase
    {
    }
}
