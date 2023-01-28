using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.DAL.DBModels
{
    public class UserDataTable
    {
        [Key]
        public string UserId { get; set; }
        public double AvailableFunds { get; set; }
    }
}
