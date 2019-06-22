using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeeVox.Demo.BigBank.WebApi.Controllers
{
    public interface IBankAccountController : IAuthenticatedController
    {
        ActionResult CheckBalance(string account);
        ActionResult DepositMoney(string account, dynamic body);
        ActionResult TransferMoney(string account, dynamic body);
        ActionResult QueryTransactions(string account, dynamic body);
    }
}
