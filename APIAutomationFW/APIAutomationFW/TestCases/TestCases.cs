using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using RestSharp;

namespace APIAutomationFW
{
    public class TestCases
    {
       
        public static void Main(String[] args)
        {
            TestGet1();
        }

       
        public static void TestGet1()
        {
            String server = @"https://reqres.in";
            try
            {
                //1. Create a client and server connection.
                RestClient client = new RestClient(server);

                //2. Create a request instance.
                RestRequest request = new RestRequest("/api/users", Method.GET);

                //3. Pass in my resource uri, method and params.
                request.AddParameter("page", "2");

                //4.Execute the request.
                IRestResponse response = client.Execute(request);

                //5. Get the reponse, response code, status ,etc etc
                Console.WriteLine("The response code: " + response.StatusCode);

                Console.WriteLine("The response body: " + response.Content);
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void TwitterAPI()
        {

        }

        public static void BasicAuthentication()
        {

        }
    }
}
