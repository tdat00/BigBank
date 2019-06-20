using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

        public virtual IEnumerable<Transaction> Transaction {get; set;}
    }
}
