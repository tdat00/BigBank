using System;

namespace LeeVox.Demo.BigBank.Model
{
    public class Transaction : BaseEntity, IEntity
    {
        public DateTime DateTimeUtc {get; set;}
        public BankAccount ToAccount {get; set;}
        public Currency Currency {get; set;}
        public decimal Amount {get; set;}
        public string Message {get; set;}
    }
}
