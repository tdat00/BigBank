namespace LeeVox.Demo.BigBank.Model
{
    public class WithdrawMoneyTransaction : Transaction
    {
        public int CurrencyId {get; set;}
        public Currency Currency {get; set;}

        public decimal Money {get; set;}
    }
}
