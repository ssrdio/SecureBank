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
using System.Linq;
using System.Threading.Tasks;
using SecureBank.Ctf.Authorization;
using SecureBank.Ctf.Services;
using Microsoft.Extensions.Configuration;
using SecureBank.Models;

namespace SecureBank.Ctf
{
    public static class CtfExtensions
    {
        private const string CTF_FLAG_FORMAT = "ctf{{{0}}}";

        private const bool USE_REAL_CHALLENGE_NAME = true;

        public static void ConfigureCtf(this IServiceCollection services, IConfiguration configuration)
        {
            AppSettings appSettings = configuration.GetSection("AppSettings").Get<AppSettings>();

            string ctfSeed = appSettings.Ctf.Seed;
            if (string.IsNullOrEmpty(ctfSeed))
            {
                ctfSeed = "icEYG435oN";
            }

            services.Configure<CtfOptions>(ctfOptions =>
            {
                ctfOptions.CtfChallanges = GetChallanges(ctfSeed, !string.IsNullOrEmpty(appSettings.LegalURL));
            });

            services.AddScoped<IAdminBL, CtfAdminBL>();
            services.AddScoped<IAdminStoreBL, CtfAdminStoreBL>();
            services.AddScoped<IUserBL, CtfUserBL>();
            services.AddScoped<ITransactionBL, CtfTransactionBL>();
            services.AddScoped<IStoreBL, CtfStoreBL>();
            services.AddScoped<IAuthBL, CtfAuthBL>();
            services.AddScoped<IUploadFileBL, CtfUploadFileBL>();

            services.AddScoped<IAuthorizeService, CtfAuthorizeService>();
        }

        private static List<CtfChallangeModel> GetChallanges(string seed, bool includeFtp)
        {
            RandomStringGenerator stringGenerator = new RandomStringGenerator(seed);

            List<CtfChallangeModel> ctfChallanges = new List<CtfChallangeModel>();

            #region Injection

            CtfChallangeModel sqlInjection = new CtfChallangeModel(
                title: USE_REAL_CHALLENGE_NAME ? "SQL Injection" : $"Challenge {CtfChallengeTypes.SqlInjection.ToChallengeNumber()}",
                type: CtfChallengeTypes.SqlInjection,
                flag: string.Format(CTF_FLAG_FORMAT, stringGenerator.Generate()),
                flagKey: USE_REAL_CHALLENGE_NAME ? "sql_injection" : $"challenge_{CtfChallengeTypes.SqlInjection.ToChallengeNumber()}",
                category: CtfChallangeCategories.Injection);
            ctfChallanges.Add(sqlInjection);

            #endregion
            #region BrokenAuthentication

            CtfChallangeModel weakPassword = new CtfChallangeModel(
                title: USE_REAL_CHALLENGE_NAME ? "Weak Password" : $"Challenge {CtfChallengeTypes.WeakPassword.ToChallengeNumber()}",
                type: CtfChallengeTypes.WeakPassword,
                flag: string.Format(CTF_FLAG_FORMAT, stringGenerator.Generate()),
                flagKey: USE_REAL_CHALLENGE_NAME ? "weak_password" : $"challenge_{CtfChallengeTypes.WeakPassword.ToChallengeNumber()}",
                category: CtfChallangeCategories.BrokenAuthentication);
            ctfChallanges.Add(weakPassword);

            #endregion
            #region SensitiveDataExposure

            CtfChallangeModel sensitiveDataExposure = new CtfChallangeModel(
                title: USE_REAL_CHALLENGE_NAME ? "Sensitive Data Exposure" : $"Challenge {CtfChallengeTypes.SensitiveDataExposure.ToChallengeNumber()}",
                type: CtfChallengeTypes.SensitiveDataExposure,
                flag: string.Format(CTF_FLAG_FORMAT, stringGenerator.Generate()),
                flagKey: USE_REAL_CHALLENGE_NAME ? "sensitive_data_exposure" : $"challenge_{CtfChallengeTypes.SensitiveDataExposure.ToChallengeNumber()}",
                category: CtfChallangeCategories.SensitiveDataExposure);
            ctfChallanges.Add(sensitiveDataExposure);

            CtfChallangeModel pathTraversal = new CtfChallangeModel(
                title: USE_REAL_CHALLENGE_NAME ? "Path Traversal" : $"Challenge {CtfChallengeTypes.PathTraversal.ToChallengeNumber()}",
                type: CtfChallengeTypes.PathTraversal,
                flag: string.Format(CTF_FLAG_FORMAT, stringGenerator.Generate()),
                flagKey: USE_REAL_CHALLENGE_NAME ? "path_traversal" : $"challenge_{CtfChallengeTypes.PathTraversal.ToChallengeNumber()}",
                category: CtfChallangeCategories.SensitiveDataExposure);
            ctfChallanges.Add(pathTraversal);

            CtfChallangeModel enumeration = new CtfChallangeModel(
                title: USE_REAL_CHALLENGE_NAME ? "Enumeration" : $"Challenge {CtfChallengeTypes.Enumeration.ToChallengeNumber()}",
                type: CtfChallengeTypes.Enumeration,
                flag: string.Format(CTF_FLAG_FORMAT, stringGenerator.Generate()),
                flagKey: USE_REAL_CHALLENGE_NAME ? "enumeration" : $"challenge_{CtfChallengeTypes.Enumeration.ToChallengeNumber()}",
                category: CtfChallangeCategories.SensitiveDataExposure);
            ctfChallanges.Add(enumeration);

            #endregion
            #region XXE

            CtfChallangeModel xxeInjection = new CtfChallangeModel(
                title: USE_REAL_CHALLENGE_NAME ? "XXE Injection" : $"Challenge {CtfChallengeTypes.XxeInjection.ToChallengeNumber()}",
                type: CtfChallengeTypes.XxeInjection,
                flag: string.Format(CTF_FLAG_FORMAT, stringGenerator.Generate()),
                flagKey: USE_REAL_CHALLENGE_NAME ? "xxe_injection" : $"challenge_{CtfChallengeTypes.XxeInjection.ToChallengeNumber()}",
                category: CtfChallangeCategories.XXE);
            ctfChallanges.Add(xxeInjection);

            #endregion
            #region BrokenAccesControl

            CtfChallangeModel registrationRoleSet = new CtfChallangeModel(
                title: USE_REAL_CHALLENGE_NAME ? "Registration role set" : $"Challenge {CtfChallengeTypes.RegistrationRoleSet.ToChallengeNumber()}",
                type: CtfChallengeTypes.RegistrationRoleSet,
                flag: string.Format(CTF_FLAG_FORMAT, stringGenerator.Generate()),
                flagKey: USE_REAL_CHALLENGE_NAME ? "registration_role_set" : $"challenge_{CtfChallengeTypes.RegistrationRoleSet.ToChallengeNumber()}",
                category: CtfChallangeCategories.BrokenAccesControl);
            ctfChallanges.Add(registrationRoleSet);

            CtfChallangeModel missingAuth = new CtfChallangeModel(
                title: USE_REAL_CHALLENGE_NAME ? "Missing Authentication" : $"Challenge {CtfChallengeTypes.MissingAuthentication.ToChallengeNumber()}",
                type: CtfChallengeTypes.MissingAuthentication,
                flag: string.Format(CTF_FLAG_FORMAT, stringGenerator.Generate()),
                flagKey: USE_REAL_CHALLENGE_NAME ? "missing_authentication" : $"challenge_{CtfChallengeTypes.MissingAuthentication.ToChallengeNumber()}",
                category: CtfChallangeCategories.BrokenAccesControl);
            ctfChallanges.Add(missingAuth);

            CtfChallangeModel changeRoleInCookie = new CtfChallangeModel(
                title: USE_REAL_CHALLENGE_NAME ? "Change Role" : $"Challenge {CtfChallengeTypes.ChangeRoleInCookie.ToChallengeNumber()}",
                type: CtfChallengeTypes.ChangeRoleInCookie,
                flag: string.Format(CTF_FLAG_FORMAT, stringGenerator.Generate()),
                flagKey: USE_REAL_CHALLENGE_NAME ? "change_role" : $"challenge_{CtfChallengeTypes.ChangeRoleInCookie.ToChallengeNumber()}",
                category: CtfChallangeCategories.BrokenAccesControl);
            ctfChallanges.Add(changeRoleInCookie);

            #endregion
            #region Security Misconfiguration

            CtfChallangeModel exceptionHandlingMisconfiguration = new CtfChallangeModel(
                title: USE_REAL_CHALLENGE_NAME ? "Exception Handling" : $"Challenge {CtfChallengeTypes.ExcaptionHandling.ToChallengeNumber()}",
                type: CtfChallengeTypes.ExcaptionHandling,
                flag: string.Format(CTF_FLAG_FORMAT, stringGenerator.Generate()),
                flagKey: USE_REAL_CHALLENGE_NAME ? "exception_handling" : $"challenge_{CtfChallengeTypes.ExcaptionHandling.ToChallengeNumber()}",
                category: CtfChallangeCategories.SecurityMisconfiguration);
            ctfChallanges.Add(exceptionHandlingMisconfiguration);

            #endregion
            #region XSS

            CtfChallangeModel xxs = new CtfChallangeModel(
                title: USE_REAL_CHALLENGE_NAME ? "XXS" : $"Challenge {CtfChallengeTypes.Xss.ToChallengeNumber()}",
                type: CtfChallengeTypes.Xss,
                flag: string.Format(CTF_FLAG_FORMAT, stringGenerator.Generate()),
                flagKey: USE_REAL_CHALLENGE_NAME ? "xss" : $"challenge_{CtfChallengeTypes.Xss.ToChallengeNumber()}",
                category: CtfChallangeCategories.XSS);
            ctfChallanges.Add(xxs);

            #endregion
            #region Miscellaneous

            CtfChallangeModel invalidModel = new CtfChallangeModel(
                title: USE_REAL_CHALLENGE_NAME ? "Invalid Model" : $"Challenge {CtfChallengeTypes.InvalidModel.ToChallengeNumber()}",
                type: CtfChallengeTypes.InvalidModel,
                flag: string.Format(CTF_FLAG_FORMAT, stringGenerator.Generate()),
                flagKey: USE_REAL_CHALLENGE_NAME ? "invalid_model" : $"challenge_{CtfChallengeTypes.InvalidModel.ToChallengeNumber()}",
                category: CtfChallangeCategories.Miscellaneous);
            ctfChallanges.Add(invalidModel);

            CtfChallangeModel unknown = new CtfChallangeModel(
                title: USE_REAL_CHALLENGE_NAME ? "Unknown Generation" : $"Challenge {CtfChallengeTypes.UnknownGeneration.ToChallengeNumber()}",
                type: CtfChallengeTypes.UnknownGeneration,
                flag: string.Format(CTF_FLAG_FORMAT, stringGenerator.Generate()),
                flagKey: USE_REAL_CHALLENGE_NAME ? "unknown_generation" : $"challenge_{CtfChallengeTypes.UnknownGeneration.ToChallengeNumber()}",
                category: CtfChallangeCategories.Miscellaneous);
            ctfChallanges.Add(unknown);

            CtfChallangeModel hiddenPage = new CtfChallangeModel(
                title: USE_REAL_CHALLENGE_NAME ? "Hidden Page" : $"Challenge {CtfChallengeTypes.HiddenPage.ToChallengeNumber()}",
                type: CtfChallengeTypes.HiddenPage,
                flag: string.Format(CTF_FLAG_FORMAT, stringGenerator.Generate()),
                flagKey: USE_REAL_CHALLENGE_NAME ? "hidden_page" : $"challenge_{CtfChallengeTypes.HiddenPage.ToChallengeNumber()}",
                category: CtfChallangeCategories.Miscellaneous);
            ctfChallanges.Add(hiddenPage);

            CtfChallangeModel invalidRedirect = new CtfChallangeModel(
                title: USE_REAL_CHALLENGE_NAME ? "Invalid Redirect" : $"Challenge {CtfChallengeTypes.InvalidModel.ToChallengeNumber()}",
                type: CtfChallengeTypes.InvalidRedirect,
                flag: string.Format(CTF_FLAG_FORMAT, stringGenerator.Generate()),
                flagKey: USE_REAL_CHALLENGE_NAME ? "invalid_redirect" : $"challenge_{CtfChallengeTypes.InvalidModel.ToChallengeNumber()}",
                category: CtfChallangeCategories.Miscellaneous);
            ctfChallanges.Add(invalidRedirect);

            CtfChallangeModel swagger = new CtfChallangeModel(
                title: USE_REAL_CHALLENGE_NAME ? "Swagger" : $"Challenge {CtfChallengeTypes.InvalidModel.ToChallengeNumber()}",
                type: CtfChallengeTypes.Swagger,
                flag: string.Format(CTF_FLAG_FORMAT, stringGenerator.Generate()),
                flagKey: USE_REAL_CHALLENGE_NAME ? "swagger" : $"challenge_{CtfChallengeTypes.Swagger.ToChallengeNumber()}",
                category: CtfChallangeCategories.Miscellaneous);
            ctfChallanges.Add(swagger);

            if (includeFtp)
            {
                CtfChallangeModel ftp = new CtfChallangeModel(
                    title: USE_REAL_CHALLENGE_NAME ? "FTP" : $"challenge_{CtfChallengeTypes.Ftp.ToChallengeNumber()}",
                    type: CtfChallengeTypes.Ftp,
                    flag: "ctf{6HtZa6lAea}",
                    flagKey: USE_REAL_CHALLENGE_NAME ? "ftp" : $"challenge_{CtfChallengeTypes.Ftp.ToChallengeNumber()}",
                    category: CtfChallangeCategories.Miscellaneous);
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
