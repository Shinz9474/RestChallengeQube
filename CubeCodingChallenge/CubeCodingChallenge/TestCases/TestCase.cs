﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Reflection;
using CubeCodingChallenge.Variables;
using CubeCodingChallenge.ResponseClasses;
using System.Configuration;

namespace CubeCodingChallenge.TestCases
{
    class TestCase
    {        
        public static void GetListOfFiles()
        {
            String responseContent = "";
            String methodName = MethodBase.GetCurrentMethod().Name;
            
            try
            {
                ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

                RestClient client = new RestClient(@"https://ec2-13-127-159-5.ap-south-1.compute.amazonaws.com/sharebox");

                RestRequest request = new RestRequest(@"/api/files", Method.GET);

                request.AddParameter("token", Variables.CommonVariables.hosttokenID);

                IRestResponse response = client.Execute(request);

                responseContent = response.Content;

                if(response.StatusCode.ToString()=="OK")
                {
                    Console.WriteLine("Rest call success. The response code is: {0}", response.StatusCode.ToString());
                }
                else
                {
                    Console.WriteLine("Rest call was not successful. The response code is: {0}", response.StatusCode.ToString());
                }

                String getFileResponseContent = "{\"response\" :" + responseContent + "}";

                JObject jObject = JObject.Parse(getFileResponseContent);

                Console.WriteLine(jObject);

                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Include
                };

                CommonVariables.getFilesResponse= JsonConvert.DeserializeObject<GetFilesResponse>(getFileResponseContent);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Following exception occured when trying to make the {0} request: {1}", methodName, ex.StackTrace);
            }

        }
        public static void UploadFile()
        {
            String responseContent = "";
            String methodName = MethodBase.GetCurrentMethod().Name;
            CommonVariables.date = UtilityMethods.GetCurrentDate();
            
            String fileUploadJSON = "{" +
                                        "\"name\": \"Shinu \"," +
                                        "\"size\": \"40\"," +
                                        "\"token\": \"" + Variables.CommonVariables.hosttokenID + "\"," +
                                        "\"hash\": \"abc-" + CommonVariables.date + "\"" +
                                    "}";
            try
            {
                ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

                RestClient client = new RestClient(@"https://ec2-13-127-159-5.ap-south-1.compute.amazonaws.com/sharebox");

                RestRequest request = new RestRequest(@"/api/upload", Method.POST);
                
                request.AddParameter("application/json", fileUploadJSON, ParameterType.RequestBody);

                IRestResponse response = client.Execute(request);

                responseContent = response.Content;

                if (response.StatusCode.ToString().ToUpper() == "CREATED")
                {
                    Console.WriteLine("Rest call success. The response code is: {0}", response.StatusCode.ToString());
                }
                else
                {
                    Console.WriteLine("Rest call was not successful. The response code is: {0}", response.StatusCode.ToString());
                }

                JObject jObject = JObject.Parse(responseContent);

                Console.WriteLine(jObject);

                CommonVariables.createdFileID = jObject["fileId"].ToString();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Following exception occured when trying to make the {0} request: {1}", methodName, ex.StackTrace);
            }
        }

        public static void VerifyUploadedFile()
        {
            GetListOfFiles();

            var listOfFileID = (from file in CommonVariables.getFilesResponse.response
                                select file.fileId).ToList();

            if(listOfFileID.Contains(CommonVariables.createdFileID))
            {
                Console.WriteLine("The file, FileID: {0} exists in the server", CommonVariables.createdFileID);
            }

        }

        public static void DeleteSpecificFile()
        {
            {
                String responseContent = "";
                String methodName = MethodBase.GetCurrentMethod().Name;
                               
                try
                {
                    ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

                    RestClient client = new RestClient(@"https://ec2-13-127-159-5.ap-south-1.compute.amazonaws.com/sharebox");

                    RestRequest request = new RestRequest(@"/api/files", Method.DELETE);

                    request.AddParameter("token", Variables.CommonVariables.hosttokenID);
                    request.AddParameter("fileId", Variables.CommonVariables.createdFileID);

                    IRestResponse response = client.Execute(request);

                    responseContent = response.Content;

                    JObject jObject = JObject.Parse(responseContent);

                    Console.WriteLine(jObject);

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Following exception occured when trying to make the {0} request: {1}", methodName, ex.StackTrace);
                }
            }
        }

        public static void ShareFile()
        {
            UploadFile();

            String responseContent = "";
            String methodName = MethodBase.GetCurrentMethod().Name;

            try
            {
                ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

                RestClient client = new RestClient(@"https://ec2-13-127-159-5.ap-south-1.compute.amazonaws.com/sharebox");

                RestRequest request = new RestRequest(@"/api/files", Method.POST);

                request.AddParameter("token", Variables.CommonVariables.hosttokenID);
                request.AddParameter("fileId", Variables.CommonVariables.createdFileID);
                request.AddParameter("shareTo", "ShinuGuest");

                IRestResponse response = client.Execute(request);

                responseContent = response.Content;

                JObject jObject = JObject.Parse(responseContent);

                Console.WriteLine(jObject);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Following exception occured when trying to make the {0} request: {1}", methodName, ex.StackTrace);
            }
        }

        public static void VerifySharedFile()
        {
            String responseContent = "";
            String methodName = MethodBase.GetCurrentMethod().Name;
            String shouldAccept = ConfigurationManager.AppSettings.Get("shouldAccept");

            try
            {
                ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

                RestClient client = new RestClient(@"https://ec2-13-127-159-5.ap-south-1.compute.amazonaws.com/sharebox");

                RestRequest request = new RestRequest(@"/api/files", Method.PUT);
                                
                request.AddParameter("token", Variables.CommonVariables.guesttokenID);
                request.AddParameter("fileId", Variables.CommonVariables.createdFileID);
                request.AddParameter("isAccepted", shouldAccept);
                
                IRestResponse response = client.Execute(request);

                responseContent = response.Content;

                if(shouldAccept=="true")
                {
                    if(responseContent.Contains(""))
                    {

                    }
                    else
                    {
                        Console.WriteLine("The shared file is missing")
                    }
                }
                else if (shouldAccept == "false")
                {
                    if (responseContent.Contains(""))
                    {

                    }
                    else
                    {

                    }
                }

                JObject jObject = JObject.Parse(responseContent);

                Console.WriteLine(jObject);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Following exception occured when trying to make the {0} request: {1}", methodName, ex.StackTrace);
            }
        }
    }
    
}
