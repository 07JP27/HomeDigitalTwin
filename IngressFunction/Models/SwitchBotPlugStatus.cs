using System.Text.Json.Serialization;

namespace HomeDigitalTwinIngressFunction.Models
{
    public class SwitchBotPlugStatus : SwitchBotDeviceStatusBase
{
    [JsonPropertyName("power")]
    public string Power	{ get; set; }

    public bool PowerOn {
        get {
            return this.Power == "ON";
        }
    }
}
    }