using System;
using Azure;
using System.Net.Http;
using System.Threading.Tasks;
using Azure.DigitalTwins.Core;
using Azure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace HomeDigitalTwinIngressFunction
{
    public class IngressToAdt
    {
        private static readonly string adtInstanceUrl = Environment.GetEnvironmentVariable("ADT_SERVICE_URL");
        private static readonly HttpClient httpClient = HttpClientFactory.Create();

        //[FunctionName("IngressToAdt")]
        public void Run([TimerTrigger("*/10 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
