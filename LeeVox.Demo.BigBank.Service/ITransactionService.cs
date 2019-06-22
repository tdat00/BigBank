using System;
using System.Collections.Generic;
using LeeVox.Demo.BigBank.Model;

namespace LeeVox.Demo.BigBank.Service
{
    public interface ITransactionService : IService
    {
        (int transactionId, decimal exchangeRate, decimal delta, decimal newBalance) Deposit(User bankOfficer, string account, string currency, decimal money, string message);
        (int transactionId, decimal exchangeRate, decimal delta, decimal newBalance) Withdraw(string accountName, string currency, decimal money, string message);
        
        (
            int transactionId,
            decimal exchangeRateFromSourceAccount, decimal deltaFromSourceAccount, decimal newSourceAccountBalance,
            decimal exchangeRateToTargetAccount, decimal deltaToTargetAccount, decimal newTargetAccountBalance
        ) Transfer(string fromAccount, string toAccount, string currency, decimal money, string message);
        
        (DateTime from, DateTime to, IEnumerable<Transaction> transactions) QueryTransactions(string account, string fromDate, string toDate);
    }
}
