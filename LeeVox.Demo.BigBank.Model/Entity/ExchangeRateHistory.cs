using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;

namespace LeeVox.Demo.BigBank.Model
{
    public class ExchangeRateHistory : BaseEntity, IEntity
    {
        //TODO: unique constrant
        public DateTime DateTimeUtc {get; set;}

        public virtual Currency FromCurrency {get; set;}
        public virtual Currency ToCurrency {get; set;}
        public decimal Rate {get; set;}
    }
}
