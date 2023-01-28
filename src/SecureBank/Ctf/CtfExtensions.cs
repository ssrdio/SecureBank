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
                ctfOptions.CtfChallenges = new List<CtfChallengeModel>();
                ctfOptions.CtfChallengeOptions = new CtfChallengeOptions();
            });
            return new CtfOptions(
             ctfChallenges: new List<CtfChallengeModel>(),
             ctfChallengeOptions: new CtfChallengeOptions());
        }

        public static CtfOptions ConfigureCtf(this IServiceCollection services, IConfiguration configuration)
        {
            AppSettings appSettings = configuration.GetSection("AppSettings").Get<AppSettings>();

            if (string.IsNullOrEmpty(appSettings.Ctf.Seed))
            {
                throw new Exception("Seed can not be null");
            }

            List<CtfChallengeModel> ctfChallenges = GetChallenges(appSettings.Ctf.Challenges, appSettings.Ctf);

            services.Configure<CtfOptions>(ctfOptions =>
            {
                ctfOptions.CtfChallenges = ctfChallenges;
                ctfOptions.CtfChallengeOptions = appSettings.Ctf.Challenges;
                ctfOptions.IsCtfEnabled = true;
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
                ctfChallenges: ctfChallenges,
                ctfChallengeOptions: appSettings.Ctf.Challenges);
        }

        private static List<CtfChallengeModel> GetChallenges(CtfChallengeOptions ctfChallengeOptions, CtfConfig ctfConfig)
        {
            RandomStringGenerator stringGenerator = new RandomStringGenerator(ctfConfig.Seed);

            List<CtfChallengeModel> ctfChallenges = new List<CtfChallengeModel>();

            #region Injection

            string flag = null;

            flag = string.Format(ctfConfig.FlagFormat, stringGenerator.Generate(), CtfChallengeTypes.SqlInjection.ToChallengeNumber());
            if (ctfChallengeOptions.SqlInjection)
            {
                CtfChallengeModel sqlInjection = new CtfChallengeModel(
                    title: ctfConfig.UseRealChallengeName ? "SQL Injection" : $"Challenge {CtfChallengeTypes.SqlInjection.ToChallengeNumber()}",
                    type: CtfChallengeTypes.SqlInjection,
                    flag: flag,
                    flagKey: ctfConfig.UseRealChallengeName ? "sql_injection" : $"challenge_{CtfChallengeTypes.SqlInjection.ToChallengeNumber()}",
                    category: CtfChallengeCategories.Injection);
                ctfChallenges.Add(sqlInjection);
            }

            #endregion
            #region BrokenAuthentication

            flag = string.Format(ctfConfig.FlagFormat, stringGenerator.Generate(), CtfChallengeTypes.WeakPassword.ToChallengeNumber());
            if (ctfChallengeOptions.WeakPassword)
            {
                CtfChallengeModel weakPassword = new CtfChallengeModel(
                    title: ctfConfig.UseRealChallengeName ? "Weak Password" : $"Challenge {CtfChallengeTypes.WeakPassword.ToChallengeNumber()}",
                    type: CtfChallengeTypes.WeakPassword,
                    flag: flag,
                    flagKey: ctfConfig.UseRealChallengeName ? "weak_password" : $"challenge_{CtfChallengeTypes.WeakPassword.ToChallengeNumber()}",
                    category: CtfChallengeCategories.BrokenAuthentication);
                ctfChallenges.Add(weakPassword);
            }

            #endregion
            #region SensitiveDataExposure

            flag = string.Format(ctfConfig.FlagFormat, stringGenerator.Generate(), CtfChallengeTypes.SensitiveDataExposure.ToChallengeNumber());
            if (ctfChallengeOptions.SensitiveDataExposureBalance || ctfChallengeOptions.SensitiveDataExposureProfileImage || ctfChallengeOptions.SensitiveDataExposureStore)
            {
                CtfChallengeModel sensitiveDataExposure = new CtfChallengeModel(
                    title: ctfConfig.UseRealChallengeName ? "Sensitive Data Exposure" : $"Challenge {CtfChallengeTypes.SensitiveDataExposure.ToChallengeNumber()}",
                    type: CtfChallengeTypes.SensitiveDataExposure,
                    flag: flag,
                    flagKey: ctfConfig.UseRealChallengeName ? "sensitive_data_exposure" : $"challenge_{CtfChallengeTypes.SensitiveDataExposure.ToChallengeNumber()}",
                    category: CtfChallengeCategories.SensitiveDataExposure);
                ctfChallenges.Add(sensitiveDataExposure);
            }

            flag = string.Format(ctfConfig.FlagFormat, stringGenerator.Generate(), CtfChallengeTypes.PathTraversal.ToChallengeNumber());
            if (ctfChallengeOptions.PathTraversal)
            {
                CtfChallengeModel pathTraversal = new CtfChallengeModel(
                    title: ctfConfig.UseRealChallengeName ? "Path Traversal" : $"Challenge {CtfChallengeTypes.PathTraversal.ToChallengeNumber()}",
                    type: CtfChallengeTypes.PathTraversal,
                    flag: flag,
                    flagKey: ctfConfig.UseRealChallengeName ? "path_traversal" : $"challenge_{CtfChallengeTypes.PathTraversal.ToChallengeNumber()}",
                    category: CtfChallengeCategories.SensitiveDataExposure);
                ctfChallenges.Add(pathTraversal);
            }

            flag = string.Format(ctfConfig.FlagFormat, stringGenerator.Generate(), CtfChallengeTypes.Enumeration.ToChallengeNumber());
            if (ctfChallengeOptions.Enumeration)
            {
                CtfChallengeModel enumeration = new CtfChallengeModel(
                    title: ctfConfig.UseRealChallengeName ? "Enumeration" : $"Challenge {CtfChallengeTypes.Enumeration.ToChallengeNumber()}",
                    type: CtfChallengeTypes.Enumeration,
                    flag: flag,
                    flagKey: ctfConfig.UseRealChallengeName ? "enumeration" : $"challenge_{CtfChallengeTypes.Enumeration.ToChallengeNumber()}",
                    category: CtfChallengeCategories.SensitiveDataExposure);
                ctfChallenges.Add(enumeration);
            }

            #endregion
            #region XXE

            flag = string.Format(ctfConfig.FlagFormat, stringGenerator.Generate(), CtfChallengeTypes.XxeInjection.ToChallengeNumber());
            if (ctfChallengeOptions.XxeInjection)
            {
                CtfChallengeModel xxeInjection = new CtfChallengeModel(
                    title: ctfConfig.UseRealChallengeName ? "XXE Injection" : $"Challenge {CtfChallengeTypes.XxeInjection.ToChallengeNumber()}",
                    type: CtfChallengeTypes.XxeInjection,
                    flag: flag,
                    flagKey: ctfConfig.UseRealChallengeName ? "xxe_injection" : $"challenge_{CtfChallengeTypes.XxeInjection.ToChallengeNumber()}",
                    category: CtfChallengeCategories.XXE);
                ctfChallenges.Add(xxeInjection);
            }

            #endregion
            #region BrokenAccesControl

            flag = string.Format(ctfConfig.FlagFormat, stringGenerator.Generate(), CtfChallengeTypes.RegistrationRoleSet.ToChallengeNumber());
            if (ctfChallengeOptions.RegistrationRoleSet)
            {
                CtfChallengeModel registrationRoleSet = new CtfChallengeModel(
                    title: ctfConfig.UseRealChallengeName ? "Registration role set" : $"Challenge {CtfChallengeTypes.RegistrationRoleSet.ToChallengeNumber()}",
                    type: CtfChallengeTypes.RegistrationRoleSet,
                    flag: flag,
                    flagKey: ctfConfig.UseRealChallengeName ? "registration_role_set" : $"challenge_{CtfChallengeTypes.RegistrationRoleSet.ToChallengeNumber()}",
                    category: CtfChallengeCategories.BrokenAccessControl);
                ctfChallenges.Add(registrationRoleSet);
            }

            flag = string.Format(ctfConfig.FlagFormat, stringGenerator.Generate(), CtfChallengeTypes.MissingAuthentication.ToChallengeNumber());
            if (ctfChallengeOptions.MissingAuthentication)
            {
                CtfChallengeModel missingAuth = new CtfChallengeModel(
                    title: ctfConfig.UseRealChallengeName ? "Missing Authentication" : $"Challenge {CtfChallengeTypes.MissingAuthentication.ToChallengeNumber()}",
                    type: CtfChallengeTypes.MissingAuthentication,
                    flag: flag,
                    flagKey: ctfConfig.UseRealChallengeName ? "missing_authentication" : $"challenge_{CtfChallengeTypes.MissingAuthentication.ToChallengeNumber()}",
                    category: CtfChallengeCategories.BrokenAccessControl);
                ctfChallenges.Add(missingAuth);
            }

            flag = string.Format(ctfConfig.FlagFormat, stringGenerator.Generate(), CtfChallengeTypes.ChangeRoleInCookie.ToChallengeNumber());
            if (ctfChallengeOptions.ChangeRoleInCookie)
            {
                CtfChallengeModel changeRoleInCookie = new CtfChallengeModel(
                    title: ctfConfig.UseRealChallengeName ? "Change Role" : $"Challenge {CtfChallengeTypes.ChangeRoleInCookie.ToChallengeNumber()}",
                    type: CtfChallengeTypes.ChangeRoleInCookie,
                    flag: flag,
                    flagKey: ctfConfig.UseRealChallengeName ? "change_role" : $"challenge_{CtfChallengeTypes.ChangeRoleInCookie.ToChallengeNumber()}",
                    category: CtfChallengeCategories.BrokenAccessControl);
                ctfChallenges.Add(changeRoleInCookie);
            }

            flag = string.Format(ctfConfig.FlagFormat, stringGenerator.Generate(), CtfChallengeTypes.UnconfirmedLogin.ToChallengeNumber());
            if (ctfChallengeOptions.UnconfirmedLogin)
            {
                CtfChallengeModel unconfirmedLogin = new CtfChallengeModel(
                    title: ctfConfig.UseRealChallengeName ? "Unconfirmed Login" : $"challenge_{CtfChallengeTypes.UnconfirmedLogin.ToChallengeNumber()}",
                    type: CtfChallengeTypes.UnconfirmedLogin,
                    flag: flag,
                    flagKey: ctfConfig.UseRealChallengeName ? "Unconfirmed Login" : $"challenge_{CtfChallengeTypes.UnconfirmedLogin.ToChallengeNumber()}",
                    category: CtfChallengeCategories.Miscellaneous);
                ctfChallenges.Add(unconfirmedLogin);
            }

            #endregion
            #region Security Misconfiguration

            flag = string.Format(ctfConfig.FlagFormat, stringGenerator.Generate(), CtfChallengeTypes.ExceptionHandling.ToChallengeNumber());
            if (ctfChallengeOptions.ExceptionHandlingTransactionCreate)
            {
                CtfChallengeModel exceptionHandlingMisconfiguration = new CtfChallengeModel(
                    title: ctfConfig.UseRealChallengeName ? "Exception Handling" : $"Challenge {CtfChallengeTypes.ExceptionHandling.ToChallengeNumber()}",
                    type: CtfChallengeTypes.ExceptionHandling,
                    flag: flag,
                    flagKey: ctfConfig.UseRealChallengeName ? "exception_handling" : $"challenge_{CtfChallengeTypes.ExceptionHandling.ToChallengeNumber()}",
                    category: CtfChallengeCategories.SecurityMisconfiguration);
                ctfChallenges.Add(exceptionHandlingMisconfiguration);
            }

            #endregion
            #region XSS

            flag = string.Format(ctfConfig.FlagFormat, stringGenerator.Generate(), CtfChallengeTypes.Xss.ToChallengeNumber());
            if (ctfChallengeOptions.TableXss || ctfChallengeOptions.PortalSearchXss)
            {
                CtfChallengeModel xxs = new CtfChallengeModel(
                    title: ctfConfig.UseRealChallengeName ? "XXS" : $"Challenge {CtfChallengeTypes.Xss.ToChallengeNumber()}",
                    type: CtfChallengeTypes.Xss,
                    flag: flag,
                    flagKey: ctfConfig.UseRealChallengeName ? "xss" : $"challenge_{CtfChallengeTypes.Xss.ToChallengeNumber()}",
                    category: CtfChallengeCategories.XSS);
                ctfChallenges.Add(xxs);
            }

            #endregion
            #region Miscellaneous

            flag = string.Format(ctfConfig.FlagFormat, stringGenerator.Generate(), CtfChallengeTypes.InvalidModel.ToChallengeNumber());
            if (ctfChallengeOptions.InvalidModelStore || ctfChallengeOptions.InvalidModelTransaction)
            {
                CtfChallengeModel invalidModel = new CtfChallengeModel(
                    title: ctfConfig.UseRealChallengeName ? "Invalid Model" : $"Challenge {CtfChallengeTypes.InvalidModel.ToChallengeNumber()}",
                    type: CtfChallengeTypes.InvalidModel,
                    flag: flag,
                    flagKey: ctfConfig.UseRealChallengeName ? "invalid_model" : $"challenge_{CtfChallengeTypes.InvalidModel.ToChallengeNumber()}",
                    category: CtfChallengeCategories.Miscellaneous);
                ctfChallenges.Add(invalidModel);
            }

            flag = string.Format(ctfConfig.FlagFormat, stringGenerator.Generate(), CtfChallengeTypes.UnknownGeneration.ToChallengeNumber());
            if (ctfChallengeOptions.UnknownGeneration)
            {
                CtfChallengeModel unknown = new CtfChallengeModel(
                    title: ctfConfig.UseRealChallengeName ? "Unknown Generation" : $"Challenge {CtfChallengeTypes.UnknownGeneration.ToChallengeNumber()}",
                    type: CtfChallengeTypes.UnknownGeneration,
                    flag: flag,
                    flagKey: ctfConfig.UseRealChallengeName ? "unknown_generation" : $"challenge_{CtfChallengeTypes.UnknownGeneration.ToChallengeNumber()}",
                    category: CtfChallengeCategories.Miscellaneous);
                ctfChallenges.Add(unknown);
            }

            flag = string.Format(ctfConfig.FlagFormat, stringGenerator.Generate(), CtfChallengeTypes.HiddenPage.ToChallengeNumber());
            if (ctfChallengeOptions.HiddenPageLoginAdmin || ctfChallengeOptions.HiddenPageRegisterAdmin)
            {
                CtfChallengeModel hiddenPage = new CtfChallengeModel(
                    title: ctfConfig.UseRealChallengeName ? "Hidden Page" : $"Challenge {CtfChallengeTypes.HiddenPage.ToChallengeNumber()}",
                    type: CtfChallengeTypes.HiddenPage,
                    flag: flag,
                    flagKey: ctfConfig.UseRealChallengeName ? "hidden_page" : $"challenge_{CtfChallengeTypes.HiddenPage.ToChallengeNumber()}",
                    category: CtfChallengeCategories.Miscellaneous);
                ctfChallenges.Add(hiddenPage);
            }

            flag = string.Format(ctfConfig.FlagFormat, stringGenerator.Generate(), CtfChallengeTypes.Base2048Content.ToChallengeNumber());
            if (ctfChallengeOptions.Base2048Content)
            {
                string fullFlag = $"Quisque non pulvinar libero, eget malesuada nisi. Ut molestie id arcu a scelerisque. Mauris bibendum sapien elit. {flag} " +
                                    $"Etiam condimentum consectetur nulla vitae rutrum. Vivamus condimentum egestas mauris, sed malesuada neque. Aenean in fermentum orci." +
                                    $" Donec quis dolor vitae libero sagittis sagittis.";

                CtfChallengeModel base2048Content = new CtfChallengeModel(
                   title: ctfConfig.UseRealChallengeName ? "Base2048" : $"Challenge {CtfChallengeTypes.Base2048Content.ToChallengeNumber()}",
                   type: CtfChallengeTypes.Base2048Content,
                   flag: flag,
                   flagKey: new Base2048().Encode(fullFlag),
                   category: CtfChallengeCategories.Miscellaneous);
                ctfChallenges.Add(base2048Content);
            }

            flag = string.Format(ctfConfig.FlagFormat, stringGenerator.Generate(), CtfChallengeTypes.InvalidRedirect.ToChallengeNumber());
            if (ctfChallengeOptions.InvalidRedirect)
            {
                CtfChallengeModel invalidRedirect = new CtfChallengeModel(
                    title: ctfConfig.UseRealChallengeName ? "Invalid Redirect" : $"Challenge {CtfChallengeTypes.InvalidRedirect.ToChallengeNumber()}",
                    type: CtfChallengeTypes.InvalidRedirect,
                    flag: flag,
                    flagKey: ctfConfig.UseRealChallengeName ? "invalid_redirect" : $"challenge_{CtfChallengeTypes.InvalidRedirect.ToChallengeNumber()}",
                    category: CtfChallengeCategories.Miscellaneous);
                ctfChallenges.Add(invalidRedirect);
            }

            flag = string.Format(ctfConfig.FlagFormat, stringGenerator.Generate(), CtfChallengeTypes.Swagger.ToChallengeNumber());
            if (ctfChallengeOptions.Swagger)
            {
                CtfChallengeModel swagger = new CtfChallengeModel(
                    title: ctfConfig.UseRealChallengeName ? "Swagger" : $"Challenge {CtfChallengeTypes.Swagger.ToChallengeNumber()}",
                    type: CtfChallengeTypes.Swagger,
                    flag: flag,
                    flagKey: ctfConfig.UseRealChallengeName ? "swagger" : $"challenge_{CtfChallengeTypes.Swagger.ToChallengeNumber()}",
                    category: CtfChallengeCategories.Miscellaneous);
                ctfChallenges.Add(swagger);
            }

            flag = string.Format(ctfConfig.FlagFormat, stringGenerator.Generate(), CtfChallengeTypes.DirectoryBrowsing.ToChallengeNumber());
            if (ctfChallengeOptions.DirectoryBrowsing)
            {
                CtfChallengeModel ftp = new CtfChallengeModel(
                    title: ctfConfig.UseRealChallengeName ? "DirectoryBrowsing" : $"challenge_{CtfChallengeTypes.DirectoryBrowsing.ToChallengeNumber()}",
                    type: CtfChallengeTypes.DirectoryBrowsing,
                    flag: string.IsNullOrEmpty(ctfConfig.FtpFlag) ? flag : ctfConfig.FtpFlag,
                    flagKey: ctfConfig.UseRealChallengeName ? "DirectoryBrowsing" : $"challenge_{CtfChallengeTypes.DirectoryBrowsing.ToChallengeNumber()}",
                    category: CtfChallengeCategories.Miscellaneous);
                ctfChallenges.Add(ftp);
            }

            flag = string.Format(ctfConfig.FlagFormat, stringGenerator.Generate(), CtfChallengeTypes.SimultaneousRequest.ToChallengeNumber());
            if (ctfChallengeOptions.SimultaneousRequest)
            {
                CtfChallengeModel simultaneousRequest = new CtfChallengeModel(
                    title: ctfConfig.UseRealChallengeName ? "SimultaneousRequest" : $"challenge_{CtfChallengeTypes.SimultaneousRequest.ToChallengeNumber()}",
                    type: CtfChallengeTypes.SimultaneousRequest,
                    flag: flag,
                    flagKey: ctfConfig.UseRealChallengeName ? "SimultaneousRequest" : $"challenge_{CtfChallengeTypes.SimultaneousRequest.ToChallengeNumber()}",
                    category: CtfChallengeCategories.Miscellaneous);
                ctfChallenges.Add(simultaneousRequest);
            }

            flag = string.Format(ctfConfig.FlagFormat, stringGenerator.Generate(), CtfChallengeTypes.reDOS.ToChallengeNumber());
            if (ctfChallengeOptions.reDOS)
            {
                CtfChallengeModel reDos = new CtfChallengeModel(
                    title: ctfConfig.UseRealChallengeName ? "reDOS" : $"challenge_{CtfChallengeTypes.reDOS.ToChallengeNumber()}",
                    type: CtfChallengeTypes.reDOS,
                    flag: flag,
                    flagKey: ctfConfig.UseRealChallengeName ? "reDOS" : $"challenge_{CtfChallengeTypes.reDOS.ToChallengeNumber()}",
                    category: CtfChallengeCategories.Miscellaneous);
                ctfChallenges.Add(reDos);
            }

            flag = string.Format(ctfConfig.FlagFormat, stringGenerator.Generate(), CtfChallengeTypes.FreeCredit.ToChallengeNumber());
            if (ctfChallengeOptions.FreeCredit)
            {
                CtfChallengeModel reDos = new CtfChallengeModel(
                    title: ctfConfig.UseRealChallengeName ? "FreeCredit" : $"challenge_{CtfChallengeTypes.FreeCredit.ToChallengeNumber()}",
                    type: CtfChallengeTypes.FreeCredit,
                    flag: flag,
                    flagKey: ctfConfig.UseRealChallengeName ? "FreeCredit" : $"challenge_{CtfChallengeTypes.FreeCredit.ToChallengeNumber()}",
                    category: CtfChallengeCategories.Miscellaneous);
                ctfChallenges.Add(reDos);
            }
            #endregion

            return ctfChallenges;
        }

        public static void GenerateCtfdExport(this IApplicationBuilder app, string path)
        {
            using IServiceScope scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            IServiceProvider services = scope.ServiceProvider;

            IOptions<CtfOptions> ctfOptions = services.GetRequiredService<IOptions<CtfOptions>>();

            CTFdExportService cTFdExportService = new CTFdExportService();

            cTFdExportService.Export(path, ctfOptions.Value.CtfChallenges).Wait();
        }
    }
}
