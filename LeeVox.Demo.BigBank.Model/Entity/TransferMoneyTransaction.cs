using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;

namespace LeeVox.Demo.BigBank.Model
{
    public class TransferMoneyTransaction : Transaction
    {
        public BankAccount FromAccount {get; set;}
    }
}
