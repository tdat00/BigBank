using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace LeeVox.Demo.BigBank.Model
{
    [Flags]
    public enum UserRole
    {
        Customer,
        BankOfficer,
        Admin
    }
    public class User : BaseEntity, IEntity
    {
        [Required]
        [MaxLength(50)]
        public string FirstName {get; set;}

        [Required]
        [MaxLength(50)]
        public string LastName {get; set;}

        [NotMapped]
        [IgnoreDataMember]
        public string Name => $"{FirstName} {LastName}";

        [Required]
        [EmailAddress]
        //TODO: create unique index
        public string Email {get; set;}

        public UserRole Role {get; set;}

        [NotMapped]
        [IgnoreDataMember]
        public string Password {get; set;}
        [IgnoreDataMember]
        public string PasswordSalt {get; set;}
        [IgnoreDataMember]
        public string PasswordHash {get; set;}
    }
}
