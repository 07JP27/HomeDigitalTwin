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
using System.Text;
using System.Security.Cryptography;
using HomeDigitalTwinIngressFunction.Models;

namespace HomeDigitalTwinIngressFunction
{
    public class SampleIngressFunction
    {
        private static readonly string AdtInstanceUrl = Environment.GetEnvironmentVariable("ADT_SERVICE_URL");
        private static readonly string SwitchbotToken = Environment.GetEnvironmentVariable("SWITCHBOT_TOKEN");
        private static readonly string SwitchbotSecret = Environment.GetEnvironmentVariable("SWITCHBOT_SECRET");
        private static readonly string Meter1Id = Environment.GetEnvironmentVariable("METER1_ID");
        private static readonly string Circulator1Id = Environment.GetEnvironmentVariable("CIRCULATOR1_ID");
        private static readonly string Humidifer1Id = Environment.GetEnvironmentVariable("HUMIDIFER1_ID");
        private static readonly HttpClient Client = HttpClientFactory.Create();

        [FunctionName("SampleIngress")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            if (AdtInstanceUrl == null) throw new ArgumentNullException("AdtInstanceUrl");
            if (SwitchbotToken == null) throw new ArgumentNullException("SwitchbotToken");
            if (SwitchbotSecret == null) throw new ArgumentNullException("SwitchbotSecret");
            if (Meter1Id == null) throw new ArgumentNullException("METER1_ID");
            if (Circulator1Id == null) throw new ArgumentNullException("PLUG1_ID");
            if (Humidifer1Id == null) throw new ArgumentNullException("HUMIDIFER_ID");

            try
            {
                var switchbotauth = AuthSwitchBot(SwitchbotToken, SwitchbotSecret);

                // Authenticate with Managed ID
                var cred = new DefaultAzureCredential();
                var client = new DigitalTwinsClient(new Uri(AdtInstanceUrl), cred);
                log.LogInformation($"ADT service client connection created.");
               
                // Update twin with device status
                var meterStatus = await GetSwitchBotDeviceStatus<SwitchBotMeterStatus>(switchbotauth,Meter1Id);
                var meterData = new Azure.JsonPatchDocument();
                meterData.AppendReplace("/Humidity", meterStatus.Body.Humidity);
                meterData.AppendReplace("/Temperature", meterStatus.Body.Temperature);
                await client.UpdateDigitalTwinAsync("Meter1", meterData);

                var circulatorStatus = await GetSwitchBotDeviceStatus<SwitchBotPlugStatus>(switchbotauth,Circulator1Id);
                var circulatorData = new Azure.JsonPatchDocument();
                circulatorData.AppendReplace("/PowerOn", circulatorStatus.Body.PowerOn);
                await client.UpdateDigitalTwinAsync("Circulator1", circulatorData);

                var humidifierStatus = await GetSwitchBotDeviceStatus<SwitchBotHumidifierStatus>(switchbotauth,Humidifer1Id);
                var humidifierData = new Azure.JsonPatchDocument();
                humidifierData.AppendReplace("/LackWater", humidifierStatus.Body.LackWater);
                humidifierData.AppendReplace("/PowerOn", humidifierStatus.Body.PowerOn);
                humidifierData.AppendReplace("/NebulizationEfficiency", humidifierStatus.Body.NebulizationEfficiency);
                await client.UpdateDigitalTwinAsync("Humidifier1", humidifierData);
            }
            catch (Exception ex)
            {
                log.LogError($"Error in ingest function: {ex.Message}");
                return new BadRequestObjectResult(ex.Message);
            }

            return new OkResult();
        }

        private SwitchBotAuthResult AuthSwitchBot(string token, string secret)
        {
            var res = new SwitchBotAuthResult();

            res.token = token;
            DateTime dt1970 = new DateTime(1970, 1, 1);
            DateTime current = DateTime.Now;
            TimeSpan span = current - dt1970;
            res.t = Convert.ToInt64(span.TotalMilliseconds).ToString();
            res.nonce = Guid.NewGuid().ToString();
            string data = token + res.t + res.nonce;
            Encoding utf8 = Encoding.UTF8;
            HMACSHA256 hmac = new HMACSHA256(utf8.GetBytes(secret));
            res.sign = Convert.ToBase64String(hmac.ComputeHash(utf8.GetBytes(data)));

            return res;
        }

        private async Task<SwitchBotResponse<T>> GetSwitchBotDeviceStatus<T>(SwitchBotAuthResult auth, string id) where T : SwitchBotDeviceStatusBase
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.switch-bot.com/v1.1/devices/{id}/status");
            request.Headers.Add("Authorization", auth.token);
            request.Headers.Add("sign", auth.sign);
            request.Headers.Add("t", auth.t);
            request.Headers.Add("nonce", auth.nonce);

            var response = await Client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var jsonStr = await response.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<SwitchBotResponse<T>>(jsonStr);
            return obj;
        }
    }
}

