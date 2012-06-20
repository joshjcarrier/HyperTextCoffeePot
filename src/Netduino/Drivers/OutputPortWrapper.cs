namespace CoffeePotProtocol.Drivers
{
    using Core.Drivers;
    using Microsoft.SPOT.Hardware;

    public class OutputPortWrapper : IOutputPort
    {
        private readonly OutputPort outputPort;

        public OutputPortWrapper(OutputPort outputPort)
        {
            this.outputPort = outputPort;
        }

        public void Write(bool state)
        {
            this.outputPort.Write(state);
        }
    }
}
