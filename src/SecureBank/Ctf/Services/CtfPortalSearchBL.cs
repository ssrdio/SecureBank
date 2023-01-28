using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SecureBank.Ctf.Models;
using SecureBank.Models.PortalSearch;
using SecureBank.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Ctf.Services
{
    public class CtfPortalSearchBL : PortalSearchBL
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly CtfOptions _ctfOptions;

        public CtfPortalSearchBL(IHttpContextAccessor httpContextAccessor, IOptions<CtfOptions> ctfOptions)
        {
            _httpContextAccessor = httpContextAccessor;

            _ctfOptions = ctfOptions.Value;
        }

        public override PortalSearchModel Search(string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                if (_ctfOptions.CtfChallengeOptions.PortalSearchXss)
                {
                    bool hasXss = CtfConstants.XXS_KEYVORDS.Any(x => search.ToUpper().Contains(x.ToUpper()));
                    if (hasXss)
                    {
                        CtfChallengeModel xssChallenge = _ctfOptions.CtfChallenges
                            .Where(x => x.Type == CtfChallengeTypes.Xss)
                            .Single();

                        _httpContextAccessor.HttpContext.Response.Headers.Add(xssChallenge.FlagKey, xssChallenge.Flag);
                    }
                }
            }

            return base.Search(search);
        }
    }
}
