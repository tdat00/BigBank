using System.Runtime.Serialization;

namespace LeeVox.Demo.BigBank.Model
{
    public class DepositMoneyTransaction : Transaction
    {
        [IgnoreDataMember]
        public virtual User BankOfficer {get; set;}
        public int BankOfficerId {get; set;}

        [IgnoreDataMember]
        public virtual Currency Currency {get; set;}
        public int CurrencyId {get; set;}

        public decimal Money {get; set;}
    }
}
