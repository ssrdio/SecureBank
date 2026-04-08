using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SecureBank.Ctf;
using SecureBank.Ctf.Models;
using SecureBank.Models;
using SecureBank.Models.Store;
using SecureBank.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Ctf.Services
{
    public class CtfAdminStoreBL : AdminStoreBL
    {
        private readonly CtfOptions _ctfOptions;

        public CtfAdminStoreBL(StoreAPICalls storeAPICalls, IOptions<CtfOptions> ctfOptions) : base(storeAPICalls)
        {
            _ctfOptions = ctfOptions.Value;
        }

        public override async Task<DataTableResp<StoreItem>> GetStoreItems(HttpContext httpContext)
        {
            DataTableResp<StoreItem> storeItems = await base.GetStoreItems(httpContext);
            if (_ctfOptions.CtfChallengeOptions.TableXss)
            {
                bool xss = storeItems.Data.Any(x => CtfConstants.XXS_KEYVORDS.Any(c => (x.Name?.Contains(c) ?? false) || (x.Description?.Contains(c) ?? false)));
                if (xss)
                {
                    CtfChallengeModel xxsChallenge = _ctfOptions.CtfChallenges
                        .Where(x => x.Type == CtfChallengeTypes.Xss)
                        .Single();

                    httpContext.Response.Headers.Add(xxsChallenge.FlagKey, xxsChallenge.Flag);
                }
            }

            return storeItems;
        }
    }
}
