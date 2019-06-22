using System.Runtime.Serialization;

namespace LeeVox.Demo.BigBank.Model
{
    public class WithdrawMoneyTransaction : Transaction
    {
        [IgnoreDataMember]
        public virtual Currency Currency {get; set;}
        public int CurrencyId {get; set;}

        public decimal Money {get; set;}
    }
}
