# BigBank
Simple ASP.Net Core Boilerplate.

## What's in this project
Simulate simple banking transations:

* Create new user with banking account(s).
* Deposit/Withdraw/Transfer money from a banking account in any currency. Exchange rate is automatically calculated.
* Check transaction history.

## Technologies

* ASP.Net Core WebApi.
* Entity Framework Core with [InMemory Db](https://docs.microsoft.com/en-us/ef/core/providers/in-memory/) for testing.
* Json Web Token for authentication/authorization. Supporting invalidate token when sign out.
* Slow hash passwords with random salt ([PBKDF2 standard](https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/consumer-apis/password-hashing?view=aspnetcore-2.2)).

## API endpoints
See detail in [WebApi ReadMe](LeeVox.Demo.BigBank.WebApi/README.md)

## How to run

### Prerequisites
* .Net Core SDK 2.2 or newer: https://dotnet.microsoft.com/download

### Run WebApi project
``` bash
cd LeeVox.Demo.BigBank.WebApi
dotnet run
```

### Run test scripts
``` bash
cd LeeVox.Demo.BigBank.TestApp
dotnet run
```
