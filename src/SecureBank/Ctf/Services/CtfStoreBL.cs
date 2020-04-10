using SecureBank.Ctf;
using SecureBank.DAL.DAO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SecureBank.Ctf.Models;
using SecureBank.Interfaces;
using SecureBank.Models.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SecureBank.Services;

namespace SecureBank.Ctf.Services
{
    public class CtfStoreBL : StoreBL
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CtfOptions _ctfOptions;

        public CtfStoreBL(ITransactionDAO transactionDAO, StoreAPICalls storeAPICalls, IHttpContextAccessor httpContextAccessor, IOptions<CtfOptions> ctfOptions)
            : base(transactionDAO, storeAPICalls)
        {
            _httpContextAccessor = httpContextAccessor;
            _ctfOptions = ctfOptions.Value;
        }

        public async override Task<bool> BuyProduct(BuyProductReq buyProductReq, string userName)
        {
            List<StoreItem> storeItems = await _storeAPICalls.GetStoreItemsAsync();
            if (storeItems == null)
            {
                return await base.BuyProduct(buyProductReq, userName);
            }

            CtfChallangeModel invalidModelChallange = _ctfOptions.CtfChallanges
                .Where(x => x.Type == CtfChallengeTypes.InvalidModel)
                .Single();

            StoreItem storeItem = storeItems
                .Where(x => x.Id == buyProductReq.Id)
                .SingleOrDefault();
            if (storeItem == null)
            {
                _httpContextAccessor.HttpContext.Response.Headers.Add(invalidModelChallange.FlagKey, invalidModelChallange.Flag);
            }
            else
            {
                if (storeItem.Price != buyProductReq.Price)
                {
                    _httpContextAccessor.HttpContext.Response.Headers.Add(invalidModelChallange.FlagKey, invalidModelChallange.Flag);
                }
            }

            return await base.BuyProduct(buyProductReq, userName);
        }
    }
}
