using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;

namespace LeeVox.Demo.BigBank.Model
{
    public class User : BaseEntity, IEntity
    {
        [Required]
        [MaxLength(50)]
        public string FirstName {get; set;}

        [Required]
        [MaxLength(50)]
        public string LastName {get; set;}

        [NotMapped]
        public string Name => $"{FirstName} {LastName}";

        [Required]
        [EmailAddress]
        //TODO: create unique index
        public string Email {get; set;}

        [NotMapped]
        [StringLength(50, MinimumLength = 8)]
        public string Password {get; set;}

        public string PasswordHash {get; set;}

        //public virtual IQueryable<BankAccount> Accounts {get; set;}
    }
}
