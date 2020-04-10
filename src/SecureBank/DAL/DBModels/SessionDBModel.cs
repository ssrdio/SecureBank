using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.DAL.DBModels
{
    public class SessionDBModel
    {
        [Key]
        public int Id { get; set; }
        public string SessionId { get; set; }
        public DateTime ExpirationDateTime { get; set; }
    }
}
