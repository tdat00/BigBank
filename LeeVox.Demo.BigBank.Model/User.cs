using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

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
        [JsonIgnore]
        public string Name => $"{FirstName} {LastName}";

        [Required]
        [EmailAddress]
        //TODO: create unique index
        public string Email {get; set;}

        [NotMapped]
        [JsonIgnore]
        public string Password {get; set;}
        [JsonIgnore]
        public string PasswordSalt {get; set;}
        [JsonIgnore]
        public string PasswordHash {get; set;}

        //public virtual IQueryable<BankAccount> Accounts {get; set;}
    }
}
