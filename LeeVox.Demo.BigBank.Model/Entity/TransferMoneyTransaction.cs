using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;

namespace LeeVox.Demo.BigBank.Model
{
    public class TransferMoneyTransaction : Transaction
    {
        public int SourceAccountId {get; set;}
        public BankAccount SourceAccount {get; set;}

        public int CurrencyId {get; set;}
        public Currency Currency {get; set;}

        public decimal Money {get; set;}
    }
}
