using System;
using System.Runtime.Serialization;

namespace LeeVox.Demo.BigBank.Model
{
    public abstract class Transaction : BaseEntity, IEntity
    {
        public DateTime DateTimeUtc {get; set;}

        [IgnoreDataMember]
        public virtual BankAccount Account {get; set;}
        public int AccountId {get; set;}
        
        public string Message {get; set;}
    }
}
