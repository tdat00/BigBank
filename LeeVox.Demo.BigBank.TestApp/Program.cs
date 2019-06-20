using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using RestSharp;

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


            Console.WriteLine("\r\nCall API without authentication.");
            response = PUT(restClient, "api/exchange-rate", new {
                time = new DateTime(2019, 01, 01),
                from = "usd",
                to = "vnd",
                rate = "20000"
            });


            Console.WriteLine("\r\nTest fail login.");
            response = POST(restClient, "api/user/login", new {
                email = "admin@big.bank",
                password = "wrong-password"
            });


            Console.WriteLine("\r\nTest success login.");
            response = POST(restClient, "api/user/login", new {
                email = "admin@big.bank",
                password = "T0p$ecret"
            });


            dynamic content = JsonConvert.DeserializeObject(response.Content);
            token = content.token;


            Console.WriteLine("\r\nInsert exchange rates.");
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
                    from = "eur",
                    to = "vnd",
                    rate = "26000"
                } 
            }, new Dictionary<string, string>() {
                {"Authorization", $"Bearer {token}"}
            });


            Console.WriteLine("\r\nCreate 1st user.");
            response = PUT(restClient, "api/user", new {
                first_name = "Bill",
                last_name = "Gate",
                email = "bill.gate@microsoft.com",
                password = "P@ssw0rd",

                account_number = "USD_Gate_001",
				account_currency = "USD"
            }, new Dictionary<string, string>() {
                {"Authorization", $"Bearer {token}"}
            });


            Console.WriteLine("\r\nCreate 2nd user without bank account.");
            response = PUT(restClient, "api/user", new {
                firstName = "Steve",
                lastName = "Jobs",
                email = "steve.jobs@apple.com",
                password = "P@ssw0rd"
            }, new Dictionary<string, string>() {
                {"Authorization", $"Bearer {token}"}
            });


            Console.WriteLine("\r\nCreate 3rd user.");
            response = PUT(restClient, "api/user", new {
                FirstName = "Dat",
                LastName = "Le",
                Email = "dat.le@leevox.com",
                Password = "P@ssw0rd",

                accountNumber = "VND_Dat_999",
				accountCurrency = "VND"
            }, new Dictionary<string, string>() {
                {"Authorization", $"Bearer {token}"}
            });


            Console.WriteLine("\r\nRegister bank account for 2nd user.");
            response = PUT(restClient, "api/user/register-bank-account", new {
                email = "steve.jobs@apple.com",
				account = "EUR_Jobs_001",
				currency = "EUR"
            }, new Dictionary<string, string>() {
                {"Authorization", $"Bearer {token}"}
            });


            Console.WriteLine("\r\nLogout.");
            response = POST(restClient, "api/user/logout", new {
                email = "admin@big.bank"
            }, new Dictionary<string, string>() {
                {"Authorization", $"Bearer {token}"}
            });


            Console.WriteLine("\r\nShould not call authenticated API any more.");
            response = PUT(restClient, "api/user", new {
                first_name = "Shoud",
                last_name = "Not",
                email = "be_inserted@database.db",
                password = "Should not be inserted to database."
            }, new Dictionary<string, string>() {
                {"Authorization", $"Bearer {token}"}
            });


            Console.WriteLine("\r\nFinished.");
            //Console.ReadLine();
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
            var response = client.Execute(request);
            Console.WriteLine($"Return Code: {response.StatusCode}, Content: {response.Content}");
            return response;
        }
    }
}
