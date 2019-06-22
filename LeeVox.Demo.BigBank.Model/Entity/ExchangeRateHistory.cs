using System;
using System.Runtime.Serialization;

namespace LeeVox.Demo.BigBank.Model
{
    public class ExchangeRateHistory : BaseEntity, IEntity
    {
        public DateTime DateTimeUtc {get; set;}

        [IgnoreDataMember]
        public virtual Currency FromCurrency {get; set;}
        public int FromCurrencyId {get; set;}

        [IgnoreDataMember]
        public virtual Currency ToCurrency {get; set;}
        public int ToCurrencyId {get; set;}

        
        public decimal Rate {get; set;}
    }
}
