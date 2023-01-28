using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Ctf.Models
{
    public class CtfChallengeModel
    {
        public string Title { get; set; }
        public CtfChallengeTypes Type { get; set; }

        public string Flag { get; set; }
        public string FlagKey { get; set; }

        public CtfChallengeCategories Category { get; set; }

        public CtfChallengeModel(string title, CtfChallengeTypes type, string flag, string flagKey, CtfChallengeCategories category)
        {
            Title = title;
            Type = type;
            Flag = flag;
            FlagKey = flagKey;
            Category = category;
        }
    }

    public enum CtfChallengeTypes
    {
        SqlInjection = 1,

        WeakPassword = 10,

        SensitiveDataExposure = 20,
        PathTraversal = 21,
        Enumeration = 22,

        XxeInjection = 30,

        MissingAuthentication = 40,
        RegistrationRoleSet = 41,
        ChangeRoleInCookie = 42,
        UnconfirmedLogin = 43,

        ExceptionHandling = 50,

        Xss = 60,

        InvalidModel = 100,
        UnknownGeneration = 101,
        HiddenPage = 102,

        InvalidRedirect = 103,
        DirectoryBrowsing = 104,
        Swagger = 105,
        HiddenComment = 106,
        Base2048Content = 107,

        SimultaneousRequest = 121,
        reDOS = 122,
        FreeCredit = 123,
        InvalidStoreModel = 124,
    }

    public static class CtfChallengeTypesExtensions
    {
        public static string ToChallengeNumber(this CtfChallengeTypes ctfChallengeType)
        {
            return ((int)ctfChallengeType).ToString("D3");
        }
    }

    public enum CtfChallengeCategories
    {
        Injection = 1,
        BrokenAuthentication = 2,
        SensitiveDataExposure = 3,
        XXE = 4,
        BrokenAccessControl = 5,
        SecurityMisconfiguration = 6,
        XSS = 7,
        InsecureDeserialization = 8,
        UsingComponentsWithKnownVulnerabilities = 9,
        InsufficientLoggingMonitoring = 10,
        Miscellaneous = 11,
    }
}
