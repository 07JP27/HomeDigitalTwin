using System.Text.Json.Serialization;

namespace HomeDigitalTwinIngressFunction.Models
{
    public class SwitchBotDeviceStatusBase
    {
        [JsonPropertyName("deviceId")]
        public string DeviceId	{ get; set; }
        [JsonPropertyName("deviceType")]
        public string DeviceType	{ get; set; }
        [JsonPropertyName("hubDeviceId")]
        public string HubDeviceId	{ get; set; }
    }
}