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
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CtfAdminStoreBL(StoreAPICalls storeAPICalls, IOptions<CtfOptions> ctfOptions, IHttpContextAccessor httpContextAccessor) : base(storeAPICalls)
        {
            _ctfOptions = ctfOptions.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        public override async Task<DataTableResp<StoreItem>> GetStoreItems()
        {
            DataTableResp<StoreItem> storeItems = await base.GetStoreItems();
            if (_ctfOptions.CtfChallengeOptions.TableXss)
            {
                bool xss = storeItems.Data.Any(x => CtfConstants.XXS_KEYVORDS.Any(c => (x.Name?.Contains(c) ?? false) || (x.Description?.Contains(c) ?? false)));
                if (xss)
                {
                    CtfChallengeModel xxsChallenge = _ctfOptions.CtfChallenges
                        .Where(x => x.Type == CtfChallengeTypes.Xss)
                        .Single();

                    _httpContextAccessor.HttpContext.Response.Headers.Add(xxsChallenge.FlagKey, xxsChallenge.Flag);
                }
            }

            return storeItems;
        }
    }
}
