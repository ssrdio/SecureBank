using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Authorization
{
    public class CookieConstants
    {
        public const string AUTHENTICATED_CALIM_TYPE = "authenticated";
        public const string USERNAME_CALIM_TYPE = "userName";
        public const string ROLE_CALIM_TYPE = "role";

        public const string NORMAL_ROLE_STRING = "normal";
        public const string ADMIN_ROLE_STRING = "admin";
    }
}
