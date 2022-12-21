public class SwitchBotPlugStatus : SwitchBotDeviceStatusBase
{
    public string Power	{ get; set; }

    public bool PowerOn {
        get {
            return this.Power == "ON";
        }
    }
}