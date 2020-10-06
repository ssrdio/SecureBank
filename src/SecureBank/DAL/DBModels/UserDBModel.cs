using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.DAL.DBModels
{
    public class UserDBModel
    {
        [Key]
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Password { get; set; }
        public int Role { get; set; }
        public bool Confirmed { get; set; }
        public string RecoveryGuid { get; set; }
    }
}
