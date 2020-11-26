using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Ctf.Models
{
    public class CtfChallengeOptions
    {
        public bool SqlInjection { get; set; }
        public bool WeakPassword { get; set; }

        public bool SensitiveDataExposureStore { get; set; }
        public bool SensitiveDataExposureBalance { get; set; }
        public bool SensitiveDataExposureProfileImage { get; set; }

        public bool PathTraversal { get; set; }
        public bool Enumeration { get; set; }

        public bool XxeInjection { get; set; }

        public bool MissingAuthentication { get; set; }
        public bool RegistrationRoleSet { get; set; }
        public bool ChangeRoleInCookie { get; set; }
        public bool UnconfirmedLogin { get; set; }

        public bool ExceptionHandlingTransactionCreate { get; set; }
        public bool ExceptionHandlingTransactionUpload { get; set; }

        public bool TableXss { get; set; }
        public bool PortalSearchXss { get; set; }

        public bool InvalidModelStore { get; set; }
        public bool InvalidModelTransaction { get; set; }
        public bool UnknownGeneration { get; set; }
        public bool HiddenPageRegisterAdmin { get; set; }
        public bool HiddenPageLoginAdmin { get; set; }

        public bool InvalidRedirect { get; set; }
        public bool DirectoryBrowsing { get; set; }
        public bool Swagger { get; set; }
        public bool Base2048Content { get; set; }

        public bool SimultaneousRequest { get; set; }

        public bool reDOS { get; set; }

        public bool FreeCredit { get; set; }
    }
}
