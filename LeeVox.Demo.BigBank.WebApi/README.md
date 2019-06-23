# WebApi Endpoints
Simple ASP.Net Core Boilerplate.

## Users

### Login
* Syntax:
  ```
  POST ~/api/user/login
  {
    "email": "<email>",
    "password": "<password>"
  }
  ```
* Response contains a token. Add this token to '___Authorization___' header when calling other APIs.
* Sample:
  ```
  POST ~/api/user/login
  {
    "email": "some.one@email.domain",
    "password": "T0p$ecrEt!"
  }
  ```

### Logout
* Syntax:
  ```
  POST ~/api/user/logout
  Authorization: Bearer <LOGIN_TOKEN>
  ```

### Create new user
* Syntax:
  ```
  PUT ~/api/user
  Authorization: Bearer <LOGIN_TOKEN>
  {
    "email": "<email>",
    "password": "<password>",
    "role": "<Admin, BankOfficer, Customer>",
    "first_name": "<first_name>",
    "last_name": "<last_name>",

    "account_name": "<bank_account_name>",
    "account_currency": "<bank_account_currency_name>"
  }
  ```
* For ___Admin___ and ___BankOfficer___ roles.
* __role__ is optional. Will be __Customer__ if missing.
* __account_name__ and __account_currency__ are optional. Will not create associated bank account if missing.
* Sample:
  ```
  PUT ~/api/user
  Authorization: Bearer <LOGIN_TOKEN>
  {
    "email": "super.user@big.bank",
    "password": "T0p$ecrEt!",
    "role": "Admin, BankOfficer, Customer",
    "first_name": "Super",
    "last_name": "User",

    "account_name": "USD_SuperUser_001",
    "account_currency": "USD"
  }
  ```

### Register new bank account
* For __BankOfficer__ role only.
* If __user_id__ is provided, __email__ will be ignored.
* Sample:
  ```
  PUT ~/api/user/register-bank-account
  Authorization: Bearer <LOGIN_TOKEN>
  {
    "user_id": 123,
    "email": "some.one@email.domain",
    "currency": "VND",
    "account": "VND_SomeOne_001"
  }
  ```


## ExchangeRates

### Add exchange rate(s)
* Sample:
  ```
  PUT ~/api/exchange-rate
  Authorization: Bearer <LOGIN_TOKEN>
  [
    {
      "time": "2011-01-01T00:00:00.000Z",
      "from": "USD",
      "to": "VND",
      "rate": 20000
    },
    {
      "time": "2019-01-01T00:00:00.000Z",
      "from": "USD",
      "to": "VND",
      "rate": 22222
    },
    {
      "time": "2019-01-01T01:23:45.678Z",
      "from": "USD",
      "to": "EUR",
      "rate": 0.8775
    },
  ]
  ```


## BankAccounts

### Check balance
* Sample:
  ```
  GET ~/api/bank-account/check-balance/<bank_account_name>
  Authorization: Bearer <LOGIN_TOKEN>
  ```
* If bank account name is missing, will check balance of all accounts of current login user.
* ___BankOfficer___ can check all banking accounts (included account of other users).
* Sample:
  ```
  GET ~/api/bank-account/check-balance/USD_SoneOne_001
  Authorization: Bearer <LOGIN_TOKEN>
  ```

### Query transaction history
* Syntax:
    ```
  POST ~/api/bank-account/query/<bank_account>
  Authorization: Bearer <LOGIN_TOKEN>
  {
    "from": "<from_time_utc>",
    "to": "<to_time_utc>"
  }
  ```
* __from__ is optional. Default is 3 months ago.
* __to__ is optional. Default is end of today.
* ___BankOfficer___ can check all banking accounts (included account of other users).
* Sample:   
  ```
  POST ~/api/bank-account/query/USD_SomeOne_001
  Authorization: Bearer <LOGIN_TOKEN>
  {
    "from": "2019-01-01T00:00:00.000Z",
    "to": "2019-01-31T23:59:59.999Z"
  }
  ```

### Deposit money
* Syntax:
  ```
  POST ~/api/bank-account/deposit/<bank_account_name>
  Authorization: Bearer <LOGIN_TOKEN>
  {
    "currency": "<currency_name>",
    "money": <amount_of_money>,
    "message": "<transaction_message>"
  }
  ```
* For ___BankOfficer___ role only.
* If transaction currency is different to bank account currency, an exchange rate will be applied automatically.
* Sample:
  ```
  POST ~/api/bank-account/deposit/EUR_SomeOne_001
  Authorization: Bearer <LOGIN_TOKEN>
  {
    "currency": "USD",
    "money": 123,
    "message": "Deposit $123 to EUR_SomeOne_001."
  }
  ```

### Withdraw money
* Syntax:
  ```
  POST ~/api/bank-account/withdraw/<bank_account_name>
  Authorization: Bearer <LOGIN_TOKEN>
  {
    "currency": "<currency_name>",
    "money": <amount_of_money>,
    "message": "<transaction_message>"
  }
  ```
* For ___Customer___ role only.
* If transaction currency is different to bank account currency, an exchange rate will be applied automatically.
* Sample:
  ```
  POST ~/api/bank-account/withdraw/USD_SomeOne_001
  Authorization: Bearer <LOGIN_TOKEN>
  {
    "currency": "VND",
    "money": 100000,
    "message": "Cash withdraw 100000 VND from USD_SomeOne_001."
  }
  ```

### Transfer money
* Syntax:
  ```
  POST ~/api/bank-account/transfer/<bank_account_name>
  Authorization: Bearer <LOGIN_TOKEN>
  {
    "to": "<target_bank_account_name>
    "currency": "<currency_name>",
    "money": <amount_of_money>,
    "message": "<transaction_message>"
  }
  ```
* For ___Customer___ role only.
* If transaction currency is different to bank account currency or target bank account currency, exchange rate(s) will be applied automatically.
* Sample:
  ```
  POST ~/api/bank-account/transfer/VND_SomeOne_001
  Authorization: Bearer <LOGIN_TOKEN>
  {
    "to": "EUR_SomeOne_001"
    "currency": "USD",
    "money": 987,
    "message": "Transfer $987 from VND_SomeOne_001 to EUR_SomeOne_001."
  }
  ```
