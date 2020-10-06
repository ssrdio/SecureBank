using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Models.Auth
{
    public class UserModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public string Name { get; set; }
        public string Surname { get; set; }
        public int UserRight { get; set; }

        public string Cookie { get; set; }
        public string Status { get; set; }
        public string SiteAction { get; set; }
        public string Token { get; set; }

    }
}
