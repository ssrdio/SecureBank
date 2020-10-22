using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SecureBank.Ctf.CTFd;
using SecureBank.Ctf.Models;
using SecureBank.Helpers;
using SecureBank.Helpers.Authorization;
using SecureBank.Interfaces;
using System;
using System.Collections.Generic;
using SecureBank.Ctf.Authorization;
using SecureBank.Ctf.Services;
using Microsoft.Extensions.Configuration;
using SecureBank.Models;

namespace SecureBank.Ctf
{
    public static class CtfExtensions
    {
        public static CtfOptions ConfigureWithoutCtf(this IServiceCollection services, IConfiguration configuration)
        {
            
            services.Configure<CtfOptions>(ctfOptions =>
            {
                ctfOptions.CtfChallanges = new List<CtfChallangeModel>();
                ctfOptions.CtfChallengeOptions = new CtfChallengeOptions();
            });
            return new CtfOptions(
             ctfChallanges: new List<CtfChallangeModel>(),
             ctfChallengeOptions: new CtfChallengeOptions());
        }

        public static CtfOptions ConfigureCtf(this IServiceCollection services, IConfiguration configuration)
        {
            AppSettings appSettings = configuration.GetSection("AppSettings").Get<AppSettings>();

            if (string.IsNullOrEmpty(appSettings.Ctf.Seed))
            {
                throw new Exception("Seed can not be null");
            }

            List<CtfChallangeModel> ctfChallenges = GetChallanges(appSettings.Ctf.Challenges, appSettings.Ctf);

            services.Configure<CtfOptions>(ctfOptions =>
            {
                ctfOptions.CtfChallanges = ctfChallenges;
                ctfOptions.CtfChallengeOptions = appSettings.Ctf.Challenges;
            });

            services.AddScoped<IAdminBL, CtfAdminBL>();
            services.AddScoped<IAdminStoreBL, CtfAdminStoreBL>();
            services.AddScoped<IUserBL, CtfUserBL>();
            services.AddScoped<ITransactionBL, CtfTransactionBL>();
            services.AddScoped<IStoreBL, CtfStoreBL>();
            services.AddScoped<IAuthBL, CtfAuthBL>();
            services.AddScoped<IUploadFileBL, CtfUploadFileBL>();

            services.AddScoped<IPortalSearchBL, CtfPortalSearchBL>();

            services.AddScoped<IAuthorizeService, CtfAuthorizeService>();

            return new CtfOptions(
                ctfChallanges: ctfChallenges,
                ctfChallengeOptions: appSettings.Ctf.Challenges);
        }

        private static List<CtfChallangeModel> GetChallanges(CtfChallengeOptions ctfChallengeOptions, CtfConfig ctfConfig)
        {
            RandomStringGenerator stringGenerator = new RandomStringGenerator(ctfConfig.Seed);

            List<CtfChallangeModel> ctfChallanges = new List<CtfChallangeModel>();

            #region Injection

            string flag = null;

            flag = string.Format(ctfConfig.FlagFormat, stringGenerator.Generate(), CtfChallengeTypes.SqlInjection.ToChallengeNumber());
            if (ctfChallengeOptions.SqlInjection)
            {
                CtfChallangeModel sqlInjection = new CtfChallangeModel(
                    title: ctfConfig.UseRealChallengeName ? "SQL Injection" : $"Challenge {CtfChallengeTypes.SqlInjection.ToChallengeNumber()}",
                    type: CtfChallengeTypes.SqlInjection,
                    flag: flag,
                    flagKey: ctfConfig.UseRealChallengeName ? "sql_injection" : $"challenge_{CtfChallengeTypes.SqlInjection.ToChallengeNumber()}",
                    category: CtfChallangeCategories.Injection);
                ctfChallanges.Add(sqlInjection);
            }

            #endregion
            #region BrokenAuthentication

            flag = string.Format(ctfConfig.FlagFormat, stringGenerator.Generate(), CtfChallengeTypes.WeakPassword.ToChallengeNumber());
            if (ctfChallengeOptions.WeakPassword)
            {
                CtfChallangeModel weakPassword = new CtfChallangeModel(
                    title: ctfConfig.UseRealChallengeName ? "Weak Password" : $"Challenge {CtfChallengeTypes.WeakPassword.ToChallengeNumber()}",
                    type: CtfChallengeTypes.WeakPassword,
                    flag: flag,
                    flagKey: ctfConfig.UseRealChallengeName ? "weak_password" : $"challenge_{CtfChallengeTypes.WeakPassword.ToChallengeNumber()}",
                    category: CtfChallangeCategories.BrokenAuthentication);
                ctfChallanges.Add(weakPassword);
            }

            #endregion
            #region SensitiveDataExposure

            flag = string.Format(ctfConfig.FlagFormat, stringGenerator.Generate(), CtfChallengeTypes.SensitiveDataExposure.ToChallengeNumber());
            if (ctfChallengeOptions.SensitiveDataExposureBalance || ctfChallengeOptions.SensitiveDataExposureProfileImage || ctfChallengeOptions.SensitiveDataExposureStore)
            {
                CtfChallangeModel sensitiveDataExposure = new CtfChallangeModel(
                    title: ctfConfig.UseRealChallengeName ? "Sensitive Data Exposure" : $"Challenge {CtfChallengeTypes.SensitiveDataExposure.ToChallengeNumber()}",
                    type: CtfChallengeTypes.SensitiveDataExposure,
                    flag: flag,
                    flagKey: ctfConfig.UseRealChallengeName ? "sensitive_data_exposure" : $"challenge_{CtfChallengeTypes.SensitiveDataExposure.ToChallengeNumber()}",
                    category: CtfChallangeCategories.SensitiveDataExposure);
                ctfChallanges.Add(sensitiveDataExposure);
            }

            flag = string.Format(ctfConfig.FlagFormat, stringGenerator.Generate(), CtfChallengeTypes.PathTraversal.ToChallengeNumber());
            if (ctfChallengeOptions.PathTraversal)
            {
                CtfChallangeModel pathTraversal = new CtfChallangeModel(
                    title: ctfConfig.UseRealChallengeName ? "Path Traversal" : $"Challenge {CtfChallengeTypes.PathTraversal.ToChallengeNumber()}",
                    type: CtfChallengeTypes.PathTraversal,
                    flag: flag,
                    flagKey: ctfConfig.UseRealChallengeName ? "path_traversal" : $"challenge_{CtfChallengeTypes.PathTraversal.ToChallengeNumber()}",
                    category: CtfChallangeCategories.SensitiveDataExposure);
                ctfChallanges.Add(pathTraversal);
            }

            flag = string.Format(ctfConfig.FlagFormat, stringGenerator.Generate(), CtfChallengeTypes.Enumeration.ToChallengeNumber());
            if (ctfChallengeOptions.Enumeration)
            {
                CtfChallangeModel enumeration = new CtfChallangeModel(
                    title: ctfConfig.UseRealChallengeName ? "Enumeration" : $"Challenge {CtfChallengeTypes.Enumeration.ToChallengeNumber()}",
                    type: CtfChallengeTypes.Enumeration,
                    flag: flag,
                    flagKey: ctfConfig.UseRealChallengeName ? "enumeration" : $"challenge_{CtfChallengeTypes.Enumeration.ToChallengeNumber()}",
                    category: CtfChallangeCategories.SensitiveDataExposure);
                ctfChallanges.Add(enumeration);
            }

            #endregion
            #region XXE

            flag = string.Format(ctfConfig.FlagFormat, stringGenerator.Generate(), CtfChallengeTypes.XxeInjection.ToChallengeNumber());
            if (ctfChallengeOptions.XxeInjection)
            {
                CtfChallangeModel xxeInjection = new CtfChallangeModel(
                    title: ctfConfig.UseRealChallengeName ? "XXE Injection" : $"Challenge {CtfChallengeTypes.XxeInjection.ToChallengeNumber()}",
                    type: CtfChallengeTypes.XxeInjection,
                    flag: flag,
                    flagKey: ctfConfig.UseRealChallengeName ? "xxe_injection" : $"challenge_{CtfChallengeTypes.XxeInjection.ToChallengeNumber()}",
                    category: CtfChallangeCategories.XXE);
                ctfChallanges.Add(xxeInjection);
            }

            #endregion
            #region BrokenAccesControl

            flag = string.Format(ctfConfig.FlagFormat, stringGenerator.Generate(), CtfChallengeTypes.RegistrationRoleSet.ToChallengeNumber());
            if (ctfChallengeOptions.RegistrationRoleSet)
            {
                CtfChallangeModel registrationRoleSet = new CtfChallangeModel(
                    title: ctfConfig.UseRealChallengeName ? "Registration role set" : $"Challenge {CtfChallengeTypes.RegistrationRoleSet.ToChallengeNumber()}",
                    type: CtfChallengeTypes.RegistrationRoleSet,
                    flag: flag,
                    flagKey: ctfConfig.UseRealChallengeName ? "registration_role_set" : $"challenge_{CtfChallengeTypes.RegistrationRoleSet.ToChallengeNumber()}",
                    category: CtfChallangeCategories.BrokenAccesControl);
                ctfChallanges.Add(registrationRoleSet);
            }

            flag = string.Format(ctfConfig.FlagFormat, stringGenerator.Generate(), CtfChallengeTypes.MissingAuthentication.ToChallengeNumber());
            if (ctfChallengeOptions.MissingAuthentication)
            {
                CtfChallangeModel missingAuth = new CtfChallangeModel(
                    title: ctfConfig.UseRealChallengeName ? "Missing Authentication" : $"Challenge {CtfChallengeTypes.MissingAuthentication.ToChallengeNumber()}",
                    type: CtfChallengeTypes.MissingAuthentication,
                    flag: flag,
                    flagKey: ctfConfig.UseRealChallengeName ? "missing_authentication" : $"challenge_{CtfChallengeTypes.MissingAuthentication.ToChallengeNumber()}",
                    category: CtfChallangeCategories.BrokenAccesControl);
                ctfChallanges.Add(missingAuth);
            }

            flag = string.Format(ctfConfig.FlagFormat, stringGenerator.Generate(), CtfChallengeTypes.ChangeRoleInCookie.ToChallengeNumber());
            if (ctfChallengeOptions.ChangeRoleInCookie)
            {
                CtfChallangeModel changeRoleInCookie = new CtfChallangeModel(
                    title: ctfConfig.UseRealChallengeName ? "Change Role" : $"Challenge {CtfChallengeTypes.ChangeRoleInCookie.ToChallengeNumber()}",
                    type: CtfChallengeTypes.ChangeRoleInCookie,
                    flag: flag,
                    flagKey: ctfConfig.UseRealChallengeName ? "change_role" : $"challenge_{CtfChallengeTypes.ChangeRoleInCookie.ToChallengeNumber()}",
                    category: CtfChallangeCategories.BrokenAccesControl);
                ctfChallanges.Add(changeRoleInCookie);
            }

            flag = string.Format(ctfConfig.FlagFormat, stringGenerator.Generate(), CtfChallengeTypes.UnconfirmedLogin.ToChallengeNumber());
            if (ctfChallengeOptions.UnconfirmedLogin)
            {
                CtfChallangeModel unconfirmedLogin = new CtfChallangeModel(
                    title: ctfConfig.UseRealChallengeName ? "Unconfirmed Login" : $"challenge_{CtfChallengeTypes.UnconfirmedLogin.ToChallengeNumber()}",
                    type: CtfChallengeTypes.UnconfirmedLogin,
                    flag: flag,
                    flagKey: ctfConfig.UseRealChallengeName ? "Unconfirmed Login" : $"challenge_{CtfChallengeTypes.UnconfirmedLogin.ToChallengeNumber()}",
                    category: CtfChallangeCategories.Miscellaneous);
                ctfChallanges.Add(unconfirmedLogin);
            }

            #endregion
            #region Security Misconfiguration

            flag = string.Format(ctfConfig.FlagFormat, stringGenerator.Generate(), CtfChallengeTypes.ExceptionHandling.ToChallengeNumber());
            if (ctfChallengeOptions.ExceptionHandlingTransactionCreate)
            {
                CtfChallangeModel exceptionHandlingMisconfiguration = new CtfChallangeModel(
                    title: ctfConfig.UseRealChallengeName ? "Exception Handling" : $"Challenge {CtfChallengeTypes.ExceptionHandling.ToChallengeNumber()}",
                    type: CtfChallengeTypes.ExceptionHandling,
                    flag: flag,
                    flagKey: ctfConfig.UseRealChallengeName ? "exception_handling" : $"challenge_{CtfChallengeTypes.ExceptionHandling.ToChallengeNumber()}",
                    category: CtfChallangeCategories.SecurityMisconfiguration);
                ctfChallanges.Add(exceptionHandlingMisconfiguration);
            }

            #endregion
            #region XSS

            flag = string.Format(ctfConfig.FlagFormat, stringGenerator.Generate(), CtfChallengeTypes.Xss.ToChallengeNumber());
            if (ctfChallengeOptions.TableXss || ctfChallengeOptions.PortalSearchXss)
            {
                CtfChallangeModel xxs = new CtfChallangeModel(
                    title: ctfConfig.UseRealChallengeName ? "XXS" : $"Challenge {CtfChallengeTypes.Xss.ToChallengeNumber()}",
                    type: CtfChallengeTypes.Xss,
                    flag: flag,
                    flagKey: ctfConfig.UseRealChallengeName ? "xss" : $"challenge_{CtfChallengeTypes.Xss.ToChallengeNumber()}",
                    category: CtfChallangeCategories.XSS);
                ctfChallanges.Add(xxs);
            }

            #endregion
            #region Miscellaneous

            flag = string.Format(ctfConfig.FlagFormat, stringGenerator.Generate(), CtfChallengeTypes.InvalidModel.ToChallengeNumber());
            if (ctfChallengeOptions.InvalidModelStore || ctfChallengeOptions.InvalidModelTransaction)
            {
                CtfChallangeModel invalidModel = new CtfChallangeModel(
                    title: ctfConfig.UseRealChallengeName ? "Invalid Model" : $"Challenge {CtfChallengeTypes.InvalidModel.ToChallengeNumber()}",
                    type: CtfChallengeTypes.InvalidModel,
                    flag: flag,
                    flagKey: ctfConfig.UseRealChallengeName ? "invalid_model" : $"challenge_{CtfChallengeTypes.InvalidModel.ToChallengeNumber()}",
                    category: CtfChallangeCategories.Miscellaneous);
                ctfChallanges.Add(invalidModel);
            }

            flag = string.Format(ctfConfig.FlagFormat, stringGenerator.Generate(), CtfChallengeTypes.UnknownGeneration.ToChallengeNumber());
            if (ctfChallengeOptions.UnknownGeneration)
            {
                CtfChallangeModel unknown = new CtfChallangeModel(
                    title: ctfConfig.UseRealChallengeName ? "Unknown Generation" : $"Challenge {CtfChallengeTypes.UnknownGeneration.ToChallengeNumber()}",
                    type: CtfChallengeTypes.UnknownGeneration,
                    flag: flag,
                    flagKey: ctfConfig.UseRealChallengeName ? "unknown_generation" : $"challenge_{CtfChallengeTypes.UnknownGeneration.ToChallengeNumber()}",
                    category: CtfChallangeCategories.Miscellaneous);
                ctfChallanges.Add(unknown);
            }

            flag = string.Format(ctfConfig.FlagFormat, stringGenerator.Generate(), CtfChallengeTypes.HiddenPage.ToChallengeNumber());
            if (ctfChallengeOptions.HiddenPageLoginAdmin || ctfChallengeOptions.HiddenPageRegisterAdmin)
            {
                CtfChallangeModel hiddenPage = new CtfChallangeModel(
                    title: ctfConfig.UseRealChallengeName ? "Hidden Page" : $"Challenge {CtfChallengeTypes.HiddenPage.ToChallengeNumber()}",
                    type: CtfChallengeTypes.HiddenPage,
                    flag: flag,
                    flagKey: ctfConfig.UseRealChallengeName ? "hidden_page" : $"challenge_{CtfChallengeTypes.HiddenPage.ToChallengeNumber()}",
                    category: CtfChallangeCategories.Miscellaneous);
                ctfChallanges.Add(hiddenPage);
            }

            flag = string.Format(ctfConfig.FlagFormat, stringGenerator.Generate(), CtfChallengeTypes.Base2048Content.ToChallengeNumber());
            if (ctfChallengeOptions.Base2048Content)
            {
                string fullFlag = $"Quisque non pulvinar libero, eget malesuada nisi. Ut molestie id arcu a scelerisque. Mauris bibendum sapien elit. {flag} " +
                                    $"Etiam condimentum consectetur nulla vitae rutrum. Vivamus condimentum egestas mauris, sed malesuada neque. Aenean in fermentum orci." +
                                    $" Donec quis dolor vitae libero sagittis sagittis.";

                CtfChallangeModel base2048Content = new CtfChallangeModel(
                   title: ctfConfig.UseRealChallengeName ? "Base2048" : $"Challenge {CtfChallengeTypes.Base2048Content.ToChallengeNumber()}",
                   type: CtfChallengeTypes.Base2048Content,
                   flag: flag,
                   flagKey: new Base2048().Encode(fullFlag),
                   category: CtfChallangeCategories.Miscellaneous);
                ctfChallanges.Add(base2048Content);
            }

            flag = string.Format(ctfConfig.FlagFormat, stringGenerator.Generate(), CtfChallengeTypes.InvalidRedirect.ToChallengeNumber());
            if (ctfChallengeOptions.InvalidRedirect)
            {
                CtfChallangeModel invalidRedirect = new CtfChallangeModel(
                    title: ctfConfig.UseRealChallengeName ? "Invalid Redirect" : $"Challenge {CtfChallengeTypes.InvalidRedirect.ToChallengeNumber()}",
                    type: CtfChallengeTypes.InvalidRedirect,
                    flag: flag,
                    flagKey: ctfConfig.UseRealChallengeName ? "invalid_redirect" : $"challenge_{CtfChallengeTypes.InvalidRedirect.ToChallengeNumber()}",
                    category: CtfChallangeCategories.Miscellaneous);
                ctfChallanges.Add(invalidRedirect);
            }

            flag = string.Format(ctfConfig.FlagFormat, stringGenerator.Generate(), CtfChallengeTypes.Swagger.ToChallengeNumber());
            if (ctfChallengeOptions.Swagger)
            {
                CtfChallangeModel swagger = new CtfChallangeModel(
                    title: ctfConfig.UseRealChallengeName ? "Swagger" : $"Challenge {CtfChallengeTypes.Swagger.ToChallengeNumber()}",
                    type: CtfChallengeTypes.Swagger,
                    flag: flag,
                    flagKey: ctfConfig.UseRealChallengeName ? "swagger" : $"challenge_{CtfChallengeTypes.Swagger.ToChallengeNumber()}",
                    category: CtfChallangeCategories.Miscellaneous);
                ctfChallanges.Add(swagger);
            }

            flag = string.Format(ctfConfig.FlagFormat, stringGenerator.Generate(), CtfChallengeTypes.DirectoryBrowsing.ToChallengeNumber());
            if (ctfChallengeOptions.DirectoryBrowsing)
            {
                CtfChallangeModel ftp = new CtfChallangeModel(
                    title: ctfConfig.UseRealChallengeName ? "DirectoryBrowsing" : $"challenge_{CtfChallengeTypes.DirectoryBrowsing.ToChallengeNumber()}",
                    type: CtfChallengeTypes.DirectoryBrowsing,
                    flag: string.IsNullOrEmpty(ctfConfig.FtpFlag) ? flag : ctfConfig.FtpFlag,
                    flagKey: ctfConfig.UseRealChallengeName ? "DirectoryBrowsing" : $"challenge_{CtfChallengeTypes.DirectoryBrowsing.ToChallengeNumber()}",
                    category: CtfChallangeCategories.Miscellaneous);
                ctfChallanges.Add(ftp);
            }

            flag = string.Format(ctfConfig.FlagFormat, stringGenerator.Generate(), CtfChallengeTypes.SimultaneousRequest.ToChallengeNumber());
            if (ctfChallengeOptions.SimultaneousRequest)
            {
                CtfChallangeModel simultaneousRequest = new CtfChallangeModel(
                    title: ctfConfig.UseRealChallengeName ? "SimultaneousRequest" : $"challenge_{CtfChallengeTypes.SimultaneousRequest.ToChallengeNumber()}",
                    type: CtfChallengeTypes.SimultaneousRequest,
                    flag: flag,
                    flagKey: ctfConfig.UseRealChallengeName ? "SimultaneousRequest" : $"challenge_{CtfChallengeTypes.SimultaneousRequest.ToChallengeNumber()}",
                    category: CtfChallangeCategories.Miscellaneous);
                ctfChallanges.Add(simultaneousRequest);
            }

            flag = string.Format(ctfConfig.FlagFormat, stringGenerator.Generate(), CtfChallengeTypes.reDOS.ToChallengeNumber());
            if (ctfChallengeOptions.reDOS)
            {
                CtfChallangeModel reDos = new CtfChallangeModel(
                    title: ctfConfig.UseRealChallengeName ? "reDOS" : $"challenge_{CtfChallengeTypes.reDOS.ToChallengeNumber()}",
                    type: CtfChallengeTypes.reDOS,
                    flag: flag,
                    flagKey: ctfConfig.UseRealChallengeName ? "reDOS" : $"challenge_{CtfChallengeTypes.reDOS.ToChallengeNumber()}",
                    category: CtfChallangeCategories.Miscellaneous);
                ctfChallanges.Add(reDos);
            }

            flag = string.Format(ctfConfig.FlagFormat, stringGenerator.Generate(), CtfChallengeTypes.FreeCredit.ToChallengeNumber());
            if (ctfChallengeOptions.FreeCredit)
            {
                CtfChallangeModel reDos = new CtfChallangeModel(
                    title: ctfConfig.UseRealChallengeName ? "FreeCredit" : $"challenge_{CtfChallengeTypes.FreeCredit.ToChallengeNumber()}",
                    type: CtfChallengeTypes.FreeCredit,
                    flag: flag,
                    flagKey: ctfConfig.UseRealChallengeName ? "FreeCredit" : $"challenge_{CtfChallengeTypes.FreeCredit.ToChallengeNumber()}",
                    category: CtfChallangeCategories.Miscellaneous);
                ctfChallanges.Add(reDos);
            }
            #endregion

            return ctfChallanges;
        }

        public static void GenerateCtfdExport(this IApplicationBuilder app, string path)
        {
            using IServiceScope scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            IServiceProvider services = scope.ServiceProvider;

            IOptions<CtfOptions> ctfOptions = services.GetRequiredService<IOptions<CtfOptions>>();

            CTFdExportService cTFdExportService = new CTFdExportService();

            cTFdExportService.Export(path, ctfOptions.Value.CtfChallanges).Wait();
        }
    }
}
