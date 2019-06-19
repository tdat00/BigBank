using System;
using Newtonsoft.Json;
using RestSharp;

namespace LeeVox.Demo.BigBank.TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            IRestRequest request;
            IRestResponse response;
            string token;

            var restClient = new RestClient("https://localhost:5001");
            // ignore dev certificate on localhost
            restClient.RemoteCertificateValidationCallback = 
                (sender, certificate, chain, sslPolicyErrors) => true;

            // Console.WriteLine("Test fail login.");
            // request = new RestRequest("api/user/login", Method.POST);
            // request.AddJsonBody(new {
            //     email = "admin@big.bank",
            //     password = "wrong-password"
            // });
            // response = restClient.Execute(request);
            // Console.WriteLine($"Return Code: {response.StatusCode}, Content: {response.Content}");

            Console.WriteLine("Test success login.");
            request = new RestRequest("api/user/login", Method.POST);
            request.AddJsonBody(new {
                email = "admin@big.bank",
                password = "T0p$ecret"
            });
            response = restClient.Execute(request);
            Console.WriteLine($"Return Code: {response.StatusCode}, Content: {response.Content}");
            dynamic content = JsonConvert.DeserializeObject(response.Content);
            token = content.token;

            // Console.WriteLine("Create 1st user.");
            // request = new RestRequest("api/user", Method.PUT);
            // request.AddJsonBody(new {
            //     first_name = "Bill",
            //     last_name = "Gate",
            //     email = "bill.gate@microsoft.com",
            //     password = "P@ssw0rd"
            // });
            // response = restClient.Execute(request);
            // Console.WriteLine($"Return Code: {response.StatusCode}, Content: {response.Content}");

            // Console.WriteLine("Create 2nd user.");
            // request = new RestRequest("api/user", Method.PUT);
            // request.AddJsonBody(new {
            //     first_name = "Steve",
            //     last_name = "Jobs",
            //     email = "steve.jobs@apple.com",
            //     password = "Sup3r$ecret"
            // });
            // response = restClient.Execute(request);
            // Console.WriteLine($"Return Code: {response.StatusCode}, Content: {response.Content}");

            // Console.WriteLine("Test fail login.");
            // request = new RestRequest("api/user/login", Method.POST);
            // request.AddJsonBody(new {
            //     email = "steve.jobs@apple.com",
            //     password = "wrong-password"
            // });
            // response = restClient.Execute(request);
            // Console.WriteLine($"Return Code: {response.StatusCode}, Content: {response.Content}");

            // Console.WriteLine("Test success login.");
            // request = new RestRequest("api/user/login", Method.POST);
            // request.AddJsonBody(new {
            //     email = "bill.gate@microsoft.com",
            //     password = "P@ssw0rd"
            // });
            // response = restClient.Execute(request);
            // Console.WriteLine($"Return Code: {response.StatusCode}, Content: {response.Content}");

            Console.WriteLine("Logout.");
            request = new RestRequest("api/user/logout", Method.POST);
            request.AddJsonBody(new {
                email = "admin@big.bank"
            });
            response = restClient.Execute(request);
            Console.WriteLine($"Return Code: {response.StatusCode}, Content: {response.Content}");

            Console.WriteLine("Finished.");
            //Console.ReadLine();
        }
    }
}
