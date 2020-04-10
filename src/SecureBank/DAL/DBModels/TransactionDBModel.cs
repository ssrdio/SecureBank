using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.DAL.DBModels
{
    public class TransactionDBModel
    {
        [Key]
        public int Id { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public DateTime TransactionDateTime { get; set; }
        public string Reason { get; set; }
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public double Amount { get; set; }
        public string Reference { get; set; }
    }
}
