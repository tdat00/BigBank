using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;

namespace LeeVox.Demo.BigBank.Model
{
    public class BankAccount : BaseEntity, IEntity
    {
        [Required]
        [StringLength(20, MinimumLength = 10)]
        //TODO: make unique index
        public string AccountNumber {get; set;}

        public virtual Currency Currency {get; set;}

        public virtual User User {get; set;}

        public virtual IQueryable<Transaction> Transactions {get; set;}
    }
}
