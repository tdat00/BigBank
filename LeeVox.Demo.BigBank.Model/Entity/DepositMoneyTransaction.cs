namespace LeeVox.Demo.BigBank.Model
{
    public class DepositMoneyTransaction : Transaction
    {
        public int BankOfficerId {get; set;}
        public User BankOfficer {get; set;}

        public int CurrencyId {get; set;}
        public Currency Currency {get; set;}

        public decimal Money {get; set;}
    }
}
