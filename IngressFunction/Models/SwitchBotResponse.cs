using System;
using System.Text.Json.Serialization;

namespace HomeDigitalTwinIngressFunction.Models
{
    public class SwitchBotResponse<T> where T : SwitchBotDeviceStatusBase
    {
        [JsonPropertyName("statusCode")]
        public string StatusCode { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("body")]
        public T Body { get; set; }
    }
}
