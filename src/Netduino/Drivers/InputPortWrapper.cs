namespace CoffeePotProtocol.Drivers
{
    using Core.Drivers;
    using Microsoft.SPOT.Hardware;

    public class InputPortWrapper : IInputPort
    {
        private readonly InputPort inputPort;

        public InputPortWrapper(InputPort inputPort)
        {
            this.inputPort = inputPort;
        }

        public event NativeEventHandler OnInterrupt
        {
            add { this.inputPort.OnInterrupt += value; }
            remove { this.inputPort.OnInterrupt -= value; }
        }

        public bool Read()
        {
            return this.inputPort.Read();
        }
    }
}