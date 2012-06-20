namespace Tests.Drivers
{
    using Core.Drivers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class BrewDriver_Tests
    {
        [TestMethod]
        public void When_initialized_and_not_heating_it_should_not_change_power_state()
        {
            var powerTriggerPortMock = new Mock<IOutputPort>();
            //powerTriggerPortMock.Setup(m => m.Write(It.IsAny<bool>())
            
            // returns false indicating not heating
            var powerSensorPortMock = new Mock<IInputPort>();
            powerSensorPortMock.Setup(m => m.Read())
                .Returns(false);

            new BrewDriver(powerTriggerPortMock.Object, powerSensorPortMock.Object);
        
            powerTriggerPortMock.Verify(m => m.Write(It.IsAny<bool>()), Times.Never());
        }
    }
}
