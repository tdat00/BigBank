using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace LeeVox.Demo.BigBank.Model
{
    public class BankAccount : BaseEntity, IEntity
    {
        [Required]
        [StringLength(20, MinimumLength = 10)]
        //TODO: make unique index
        public string AccountNumber {get; set;}

        [IgnoreDataMember]
        public int CurrencyId {get; set;}
        public virtual Currency Currency {get; set;}
        [IgnoreDataMember]
        public int UserId {get; set;}
        [IgnoreDataMember]
        public virtual User User {get; set;}

        public decimal Balance {get; set;}
    }
}
