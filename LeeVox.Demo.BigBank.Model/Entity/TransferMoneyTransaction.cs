using System.Runtime.Serialization;

namespace LeeVox.Demo.BigBank.Model
{
    public class TransferMoneyTransaction : Transaction
    {
        [IgnoreDataMember]
        public virtual BankAccount SourceAccount {get; set;}
        public int SourceAccountId {get; set;}

        [IgnoreDataMember]
        public virtual Currency Currency {get; set;}
        public int CurrencyId {get; set;}
        

        public decimal Money {get; set;}
    }
}
