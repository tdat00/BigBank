using System;

namespace LeeVox.Demo.BigBank.Model
{
    public class Transaction : BaseEntity, IEntity
    {
        public DateTime DateTimeUtc {get; set;}

        public int AccountId {get; set;}
        public BankAccount Account {get; set;}
        
        public string Message {get; set;}
    }
}
