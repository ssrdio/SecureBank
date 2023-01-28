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
using Microsoft.Extensions.Caching.Memory;
using SecureBank.Ctf.CTFd.Models;
using System.Diagnostics;
using SecureBank.Helpers;
using SecureBank.Models.Transaction;

namespace SecureBank.Ctf.Services
{
    public class CtfStoreBL : StoreBL
    {
        /// <summary>
        /// username
        /// productId
        /// </summary>
        private const string SIMULTANEOUS_REQUESTS_KEY = "simultaneous_{0}_{1}";

        private readonly TimeSpan SIMULTANEOUS_REQUESTS_WAIT_FOR = new TimeSpan(0, 0, 1);
        private readonly TimeSpan LOCK_BUY_REQUEST_FOR = new TimeSpan(0, 0, 1);

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CtfOptions _ctfOptions;
        private readonly IMemoryCache _memoryCache;

        public CtfStoreBL(
            ITransactionDAO transactionDAO,
            StoreAPICalls storeAPICalls,
            IHttpContextAccessor httpContextAccessor,
            IOptions<CtfOptions> ctfOptions,
            IMemoryCache memoryCache)
            : base(transactionDAO, storeAPICalls)
        {
            _httpContextAccessor = httpContextAccessor;
            _memoryCache = memoryCache;

            _ctfOptions = ctfOptions.Value;
        }

        public async override Task<bool> BuyProduct(BuyProductReq buyProductReq, string userName)
        {
            string cacheKey = string.Format(SIMULTANEOUS_REQUESTS_KEY, userName, buyProductReq.Id);
            if (_memoryCache.TryGetValue(cacheKey, out string value))
            {
                if (!_ctfOptions.CtfChallengeOptions.SimultaneousRequest)
                {
                    return false;
                }
            }

            _memoryCache.Set(cacheKey, string.Empty, LOCK_BUY_REQUEST_FOR);

            bool result = await base.BuyProduct(buyProductReq, userName);

            if (_ctfOptions.CtfChallengeOptions.InvalidModelStore)
            {
                List<StoreItem> storeItems = await _storeAPICalls.GetStoreItemsAsync();
                if (storeItems != null)
                {
                    CtfChallengeModel invalidModelChallenge = _ctfOptions.CtfChallenges
                        .Where(x => x.Type == CtfChallengeTypes.InvalidStoreModel)
                        .Single();

                    StoreItem storeItem = storeItems
                        .Where(x => x.Id == buyProductReq.Id)
                        .SingleOrDefault();
                    if (storeItem != null)
                    {
                        if (storeItem.Price != buyProductReq.Price)
                        {
                            _httpContextAccessor.HttpContext.Response.Headers.Add(invalidModelChallenge.FlagKey, invalidModelChallenge.Flag);
                        }
                    }
                }
            }

            _memoryCache.Remove(cacheKey);

            return result;
        }

        protected override async Task<bool> Pay(BuyProductReq buyProductReq, string userName)
        {
            double accountBalance = _transactionDAO.GetAccountBalance(userName);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            double amountToPay;

            amountToPay = buyProductReq.Price * buyProductReq.Quantity;


            List<StoreItem> storeItems = await _storeAPICalls.GetStoreItemsAsync();
            if (storeItems == null)
            {
                return false;
            }

            StoreItem storeItem = storeItems
                .Where(x => x.Id == buyProductReq.Id)
                .SingleOrDefault();
            if (storeItem == null)
            {
                return false;
            }
            if (_ctfOptions.CtfChallengeOptions.InvalidModelStore)
            {
                if (storeItem.Price != buyProductReq.Price)
                {
                    CtfChallengeModel invalidStoreModelRequest = _ctfOptions.CtfChallenges
                           .Where(x => x.Type == CtfChallengeTypes.InvalidStoreModel)
                           .SingleOrDefault();

                    _httpContextAccessor.HttpContext.Response.Headers.Add(invalidStoreModelRequest.FlagKey, invalidStoreModelRequest.Flag);
                }
            }
            else
            {
                amountToPay = storeItem.Price * buyProductReq.Quantity;
            }


            stopwatch.Stop();

            TimeSpan sleepFor = SIMULTANEOUS_REQUESTS_WAIT_FOR - stopwatch.Elapsed;
            if (sleepFor.TotalMilliseconds > 0)
            {
                await Task.Delay(sleepFor);
            }
            if (accountBalance < amountToPay)
            {
                return false;
            }


            double accountBalanceAfterWait = _transactionDAO.GetAccountBalance(userName);
            if (accountBalanceAfterWait < amountToPay)
            {
                if (_ctfOptions.CtfChallengeOptions.SimultaneousRequest)
                {
                    CtfChallengeModel simultaneousRequest = _ctfOptions.CtfChallenges
                        .Where(x => x.Type == CtfChallengeTypes.SimultaneousRequest)
                        .SingleOrDefault();

                    _httpContextAccessor.HttpContext.Response.Headers.Add(simultaneousRequest.FlagKey, simultaneousRequest.Flag);
                }
            }

            DepositRequest depositRequest = new DepositRequest
            {
                Amount = amountToPay,
                Reason = TRANSACTION_REASON,
                ReceiverId = TRANSACTION_RECIVER_ID,
                SenderId = userName
            };

            return _transactionDAO.Pay(depositRequest);
        }

        public override Task<List<PurcahseHistoryItemResp>> GetPurchaseHistory(string userName)
        {
            if (userName != _httpContextAccessor.HttpContext.GetUserName())
            {
                if (_ctfOptions.CtfChallengeOptions.SensitiveDataExposureStore)
                {
                    CtfChallengeModel sensitiveDataExposure = _ctfOptions.CtfChallenges
                        .Where(x => x.Type == CtfChallengeTypes.SensitiveDataExposure)
                        .Single();

                    _httpContextAccessor.HttpContext.Response.Headers.Add(sensitiveDataExposure.FlagKey, sensitiveDataExposure.Flag);
                }
                else
                {
                    userName = _httpContextAccessor.HttpContext.GetUserName();
                }
            }

            return base.GetPurchaseHistory(userName);
        }
    }
}
