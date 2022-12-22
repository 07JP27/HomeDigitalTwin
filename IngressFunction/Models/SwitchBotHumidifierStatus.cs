using System.Text.Json.Serialization;
namespace HomeDigitalTwinIngressFunction.Models
{
    public class SwitchBotHumidifierStatus : SwitchBotDeviceStatusBase
    {
        [JsonPropertyName("power")]
        public string Power { get; set; }
        [JsonPropertyName("nebulizationEfficiency")]
        public int NebulizationEfficiency { get; set; }
        [JsonPropertyName("lackWater")]
        public bool LackWater { get; set; }
        public bool PowerOn
        {
            get
            {
                return Power == "ON";
            }
        }
    }
}