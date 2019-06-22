using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace LeeVox.Demo.BigBank.Model
{
    public class BankAccount : BaseEntity, IEntity
    {
        [Required]
        [StringLength(20, MinimumLength = 10)]
        //TODO: make unique index
        public string AccountName {get; set;}

        [IgnoreDataMember]
        public virtual Currency Currency {get; set;}
        public int CurrencyId {get; set;}

        [IgnoreDataMember]
        public virtual User User {get; set;}
        public int UserId {get; set;}

        public decimal Balance {get; set;}
    }
}
