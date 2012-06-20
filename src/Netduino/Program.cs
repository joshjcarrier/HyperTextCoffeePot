namespace CoffeePotProtocol
{
    using Microsoft.SPOT.Hardware;
    using SecretLabs.NETMF.Hardware.NetduinoPlus;
    using CoffeePotProtocol.Drivers;
    using Core.Drivers;
    
    public class Program
    {
        public static Cpu.Pin PowerSensorPin = Pins.GPIO_PIN_D13;
        public static Cpu.Pin PowerTriggerPin = Pins.GPIO_PIN_D12;

        public static void Main()
        {
            // write your code here
            var powerTriggerPort = new OutputPort(PowerTriggerPin, false);
            var powerSensorPort = new InputPort(PowerSensorPin, false, Port.ResistorMode.Disabled);
            new BrewDriver(new OutputPortWrapper(powerTriggerPort), new InputPortWrapper(powerSensorPort));
        }
    }
}
