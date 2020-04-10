using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Models.User
{
    public class AdminUserInfoResp
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Username { get; set; }
        public int Role { get; set; }
    }
}
