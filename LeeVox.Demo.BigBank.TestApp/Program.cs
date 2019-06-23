using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using RestSharp;
using ColorfulConsole = Colorful.Console;

namespace LeeVox.Demo.BigBank.TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            IRestResponse response;
            string token;

            var restClient = new RestClient("https://localhost:5001");
            // ignore dev certificate on localhost
            restClient.RemoteCertificateValidationCallback = 
                (sender, certificate, chain, sslPolicyErrors) => true;


            Step("\r\nTest fail login.");
            response = POST(restClient, "api/user/login", new {
                email = "admin@big.bank",
                password = "wrong-password"
            });


            Step("\r\nLogin to admin@big.bank");
            response = POST(restClient, "api/user/login", new {
                email = "admin@big.bank",
                password = "T0p$ecret"
            });


            dynamic content = JsonConvert.DeserializeObject(response.Content);
            token = content.token;


            Step("\r\nInsert exchange rates.");
            response = PUT(restClient, "api/exchange-rate", new []{
                new {
                    time = "2010-01-01T00:00:00.000Z",
                    from = "usd",
                    to = "vnd",
                    rate = "20000"
                }, new {
                    time = "2019-06-01T00:00:00.000Z",
                    from = "usd",
                    to = "vnd",
                    rate = "22000"
                }, new {
                    time = "2019-06-01T00:00:00.000Z",
                    from = "vnd",
                    to = "eur",
                    rate = "0.000037261"
                }, new {
                    time = "2019-01-01T00:00:00.000Z",
                    from = "usd",
                    to = "eur",
                    rate = "0.877000000123"
                },
            }, new Dictionary<string, string>() {
                {"Authorization", $"Bearer {token}"}
            });


            Step("\r\nCreate user bill.gate@microsoft.com");
            response = PUT(restClient, "api/user", new {
                first_name = "Bill",
                last_name = "Gate",
                email = "bill.gate@microsoft.com",
                password = "P@ssw0rd",

                account_name = "USD_Gate_001",
				account_currency = "USD"
            }, new Dictionary<string, string>() {
                {"Authorization", $"Bearer {token}"}
            });


            Step("\r\nCreate user steve.jobs@apple.com without bank account");
            response = PUT(restClient, "api/user", new {
                firstName = "Steve",
                lastName = "Jobs",
                email = "steve.jobs@apple.com",
                password = "P@ssw0rd"
            }, new Dictionary<string, string>() {
                {"Authorization", $"Bearer {token}"}
            });


            Step("\r\nCreate user dat.le@leevox.com");
            response = PUT(restClient, "api/user", new {
                FirstName = "Dat",
                LastName = "Le",
                Email = "dat.le@leevox.com",
                Password = "P@ssw0rd",

                accountName = "VND_Dat_999",
				accountCurrency = "VND"
            }, new Dictionary<string, string>() {
                {"Authorization", $"Bearer {token}"}
            });


            Step("\r\nRegister bank account for user steve.jobs@apple.com");
            response = PUT(restClient, "api/user/register-bank-account", new {
                email = "steve.jobs@apple.com",
				account = "EUR_Jobs_001",
				currency = "EUR"
            }, new Dictionary<string, string>() {
                {"Authorization", $"Bearer {token}"}
            });


            Step("\r\nRegister 2nd account for user dat.le@leevox.com");
            response = PUT(restClient, "api/user/register-bank-account", new {
                email = "dat.le@leevox.com",
				account = "USD_Dat_001",
				currency = "USD"
            }, new Dictionary<string, string>() {
                {"Authorization", $"Bearer {token}"}
            });


            Step("\r\nDeposit $1,000,000 to USD_Gate_001");
            response = POST(restClient, "api/bank-account/deposit/USD_Gate_001", new {
                currency = "USD",
                amount = 1_000_000m,
                message = "Deposit $1 000 000"
            }, new Dictionary<string, string>() {
                {"Authorization", $"Bearer {token}"}
            });


            Step("\r\nDeposit SGD 987.65 to EUR_Jobs_001");
            response = POST(restClient, "api/bank-account/deposit/EUR_Jobs_001", new {
                currency = "SGD",
                amount = 987.65m,
                message = "Deposit SGD 987.65"
            }, new Dictionary<string, string>() {
                {"Authorization", $"Bearer {token}"}
            });


            Step("\r\nDeposit $123.45 to VND_Dat_999");
            response = POST(restClient, "api/bank-account/deposit/VND_Dat_999", new {
                currency = "USD",
                amount = 123.45m,
                message = "Deposit $123.45"
            }, new Dictionary<string, string>() {
                {"Authorization", $"Bearer {token}"}
            });


            Step("\r\nLogout admin@big.bank");
            response = POST(restClient, "api/user/logout", new {
                email = "admin@big.bank"
            }, new Dictionary<string, string>() {
                {"Authorization", $"Bearer {token}"}
            });


            Step("\r\nShould not call authenticated API any more.");
            response = PUT(restClient, "api/user", new {
                first_name = "Shoud",
                last_name = "Not",
                email = "be_inserted@database.db",
                password = "Should not be inserted to database."
            }, new Dictionary<string, string>() {
                {"Authorization", $"Bearer {token}"}
            });


            Step("\r\nLogin as bill.gate@microsoft.com");
            response = POST(restClient, "api/user/login", new {
                email = "bill.gate@microsoft.com",
                password = "P@ssw0rd"
            });


            content = JsonConvert.DeserializeObject(response.Content);
            token = content.token;


            Step("\r\nShould not able to deposit money");
            response = POST(restClient, "api/bank-account/deposit/USD_Gate_001", new {
                currency = "EUR",
                amount = 987.65m,
                message = "Deposit EUR 987.65"
            }, new Dictionary<string, string>() {
                {"Authorization", $"Bearer {token}"}
            });


            Step("\r\nCheck balance of all bank accounts.");
            response = GET(restClient, "api/bank-account/check-balance", new {

            }, new Dictionary<string, string>() {
                {"Authorization", $"Bearer {token}"}
            });


            Step("\r\nTransfer EUR 500 from USD_Gate_001 to VND_Dat_999 .");
            response = POST(restClient, "api/bank-account/transfer/USD_Gate_001", new {
                to = "VND_Dat_999",
                currency = "EUR",
                money = 500m,
                message = "transfer EUR 500 from USD_Gate_001 to VND_Dat_999."
            }, new Dictionary<string, string>() {
                {"Authorization", $"Bearer {token}"}
            });


            Step("\r\nCheck balance of all bank accounts again.");
            response = GET(restClient, "api/bank-account/check-balance", new {

            }, new Dictionary<string, string>() {
                {"Authorization", $"Bearer {token}"}
            });


            Step("\r\nLogin as dat.le@leevox.com");
            response = POST(restClient, "api/user/login", new {
                email = "dat.le@leevox.com",
                password = "P@ssw0rd"
            });


            content = JsonConvert.DeserializeObject(response.Content);
            token = content.token;


            Step("\r\nCheck balance of all bank accounts.");
            response = GET(restClient, "api/bank-account/check-balance", new {
            }, new Dictionary<string, string>() {
                {"Authorization", $"Bearer {token}"}
            });


            Step("\r\nWithdraw VND 900,000,000 from VND_Dat_999.");
            response = POST(restClient, "api/bank-account/withdraw/VND_Dat_999", new {
                currency = "VND",
                money = 900_000_000m
            }, new Dictionary<string, string>() {
                {"Authorization", $"Bearer {token}"}
            });


            Step("\r\nWithdraw again, VND 900,000 instead of EUR 9,000,000.");
            response = POST(restClient, "api/bank-account/withdraw/VND_Dat_999", new {
                currency = "VND",
                money = 900_000m,
                message = "cash withdraw from ATM"
            }, new Dictionary<string, string>() {
                {"Authorization", $"Bearer {token}"}
            });


            Step("\r\nCheck balance of a specified account.");
            response = GET(restClient, "api/bank-account/check-balance/VND_Dat_999", new {

            }, new Dictionary<string, string>() {
                {"Authorization", $"Bearer {token}"}
            });


            Step("\r\nQuery transactions.");
            response = POST(restClient, "api/bank-account/query/VND_Dat_999", new {
                from = DateTime.Now.AddDays(-2)
            }, new Dictionary<string, string>() {
                {"Authorization", $"Bearer {token}"}
            });


            Step("\r\nFinished.");
            //Console.ReadLine();
        }

        static void Step(string message)
        {
            ColorfulConsole.WriteLine(message, Color.DarkCyan);
        }

        static IRestResponse GET(RestClient client, string url, object body = null, IDictionary<string, string> headers = null)
            => Request(client, url, Method.GET, body, headers);
        static IRestResponse POST(RestClient client, string url, object body = null, IDictionary<string, string> headers = null)
            => Request(client, url, Method.POST, body, headers);
        static IRestResponse PUT(RestClient client, string url, object body = null, IDictionary<string, string> headers = null)
            => Request(client, url, Method.PUT, body, headers);
        static IRestResponse DELETE(RestClient client, string url, object body = null, IDictionary<string, string> headers = null)
            => Request(client, url, Method.DELETE, body, headers);
        static IRestResponse Request(RestClient client, string url, Method method, object body = null, IDictionary<string, string> headers = null)
        {
            var request = new RestRequest(url, method);
            if (headers != null && headers.Any())
            {
                foreach (var header in headers)
                {
                    request.AddHeader(header.Key, header.Value);
                }
            }
            if (body != null)
            {
                request.AddJsonBody(body);
            }
            var watch = Stopwatch.StartNew();
            var response = client.Execute(request);
            watch.Stop();

            var success = response.StatusCode == HttpStatusCode.OK;
            Console.Write("Result: ");
            ColorfulConsole.WriteLine($"{(int)response.StatusCode} {response.StatusCode}", success ? Color.Green : Color.Red);
            Console.WriteLine($"Elapsed Time: {watch.ElapsedMilliseconds/1000d} sec");
            Console.WriteLine($"Content:\r\n{response.Content}");
            return response;
        }
    }
}
