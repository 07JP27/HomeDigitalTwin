using System.Text.Json.Serialization;

namespace HomeDigitalTwinIngressFunction.Models
{
    public class SwitchBotMeterStatus : SwitchBotDeviceStatusBase
    {
        [JsonPropertyName("temperature")]
        public float Temperature { get; set; }
        [JsonPropertyName("humidity")]
        public int Humidity { get; set; }
    }
}