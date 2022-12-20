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
    public static class SampleIngressFunction
    {
        private static readonly string adtInstanceUrl = Environment.GetEnvironmentVariable("ADT_SERVICE_URL");
        private static readonly HttpClient httpClient = HttpClientFactory.Create();

        [FunctionName("SampleIngress")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                if (adtInstanceUrl == null) throw new ArgumentNullException("adtInstanceUrl");

                // Authenticate with Managed ID
                var cred = new DefaultAzureCredential();
                var client = new DigitalTwinsClient(new Uri(adtInstanceUrl), cred);
                log.LogInformation($"ADT service client connection created.");
               
                // <Update_twin_with_device_temperature>
                var updateTwinData = new Azure.JsonPatchDocument();
                updateTwinData.AppendReplace("/LackWater", true);
                updateTwinData.AppendReplace("/PowerOn", true);
                await client.UpdateDigitalTwinAsync("Humidifier1", updateTwinData);
                // </Update_twin_with_device_temperature>
            }
            catch (Exception ex)
            {
                log.LogError($"Error in ingest function: {ex.Message}");
                return new BadRequestObjectResult(ex.Message);
            }

            return new OkResult();
        }
    }
}

