using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeeVox.Demo.BigBank.WebApi.Controllers
{
    public interface IBankAccountController : IAuthenticatedController
    {
        ActionResult CheckBalance(string account);
        ActionResult DepositMoney(dynamic body);
        ActionResult TransferMoney(dynamic body);
        ActionResult QueryTransactions(dynamic body);
    }
}
