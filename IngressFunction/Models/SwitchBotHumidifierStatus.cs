public class SwitchBotHumidifierStatus : SwitchBotDeviceStatusBase
{
    public string Power	{ get; set; }
    public int NebulizationEfficiency { get; set; }
    public bool LackWater { get; set; }
    public bool PowerOn {
        get {
            return Power == "ON";
        }
    }
}