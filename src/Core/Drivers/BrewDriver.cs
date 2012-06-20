namespace Core.Drivers
{
    using System;
    using System.Threading;

    #if MF_FRAMEWORK_VERSION_V4_1
    using Microsoft.SPOT.Hardware;    
    #endif

    /// <summary>
    /// Controls and monitors basic brew functions of a coffee maker.
    /// </summary>
    /// <remarks>
    /// State for this model is this:
    /// State				| Timer Brew LED	| Power LED		| Pin 2
    /// OFF					| OFF				| OFF			| LOW
    /// BREW (on demand)	| OFF				| ON			| HIGH
    /// TIMER				| ON				| OFF			| LOW
    /// BREW (timer)		| ON				| ON			| HIGH
    /// 
    /// Each power button press state changes:
    /// Current State		| New state
    /// OFF					| BREW (on demand)
    /// BREW (on demand)	| TIMER
    /// TIMER				| OFF
    /// BREW (timer)		| OFF
    /// </remarks>
    public class BrewDriver
    {
        public int PowerStates = 3;
        public int BrewMillisecondsPerCup = 60000;
        private readonly IOutputPort powerTriggerPort;
        private readonly IInputPort powerSensorPort;
        private readonly Timer brewTimer;
        private readonly Timer powerOffTimer;
        private int brewDuration;
        private int keepHotDuration;

        /// <summary>
        /// Initializes a new instance of the BrewDriver class.
        /// </summary>
        /// <param name="powerTriggerPort">
        /// The power trigger digital output port.
        /// </param>
        /// <param name="powerSensorPort">
        /// The power sensor digital input port.
        /// </param>
        public BrewDriver(IOutputPort powerTriggerPort, IInputPort powerSensorPort)
        {
            this.brewDuration = 0;
            this.keepHotDuration = 0;
            this.powerTriggerPort = powerTriggerPort;
            this.powerSensorPort = powerSensorPort;

            #if MF_FRAMEWORK_VERSION_V4_1
            powerSensorPort.OnInterrupt += OnPowerSensorPortInterrupt;    
            #else
            powerSensorPort.OnInterrupt += (obj, args) => OnPowerSensorPortInterrupt(args.Data1, args.Data2, args.Time);
            #endif

            // set up brew timer without callback
            this.brewTimer = new Timer(OnBrewTimerCallback, null, -1, -1);
            this.powerOffTimer = new Timer(OnPowerOffTimerCallback, null, -1, -1);

            // check initial state and set power timer timeout if necessary
            if (IsHeating)
            {
                powerOffTimer.Change(this.keepHotDuration, -1);
            }
        }

        /// <summary>
        /// Gets a value indicating if the brewer is currently brewing.
        /// </summary>
        public bool IsBrewing { get; private set; }

        /// <summary>
        /// Gets a value indicating if the brewer is currently heating.
        /// </summary>
        public bool IsHeating
        {
            get { return this.powerSensorPort.Read(); }
        }

        /// <summary>
        /// Starts the brewing process.
        /// </summary>
        /// <param name="cups">
        /// The estimated number of cups being brewed. This affects the predicted brewing time.
        /// </param>
        /// <param name="keepHotAfterBrewDuration">
        /// The length of time to keep the brewer burner on after brewing is completed.
        /// </param>
        /// <returns>
        /// True if the brewing process successfully started, false if brewer is already on.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The brewer could not be started.
        /// </exception>
        public bool StartBrew(int cups, int keepHotAfterBrewDuration)
        {
            if (IsHeating)
            {
                // brewer already on
                return false;
            }

            this.brewDuration = cups * this.BrewMillisecondsPerCup;
            this.keepHotDuration = keepHotAfterBrewDuration;

            for (var attempt = 0; attempt < PowerStates; attempt++)
            {
                this.powerTriggerPort.Write(true);

                if (IsHeating)
                {
                    return true;
                }
            }

            throw new InvalidOperationException("The brewer could not be started.");
        }

        /// <summary>
        /// Stops the brewing/heating process.
        /// </summary>
        /// <returns>
        /// True if powered off, false if it was already off.
        /// </returns>
        public bool PowerOff()
        {
            if (!IsHeating)
            {
                // already stopped heating
                return false;
            }

            for (var attempt = 0; attempt < PowerStates; attempt++)
            {
                // pulse twice to skip over timer phase
                PulseHigh(this.powerTriggerPort);
                PulseHigh(this.powerTriggerPort);

                if (!IsHeating)
                {
                    return true;
                }
            }

            throw new InvalidOperationException("The brewer could not be stopped");
        }

        private static void PulseHigh(IOutputPort port)
        {
            port.Write(true);
            port.Write(false);
        }

        private void OnBrewTimerCallback(object state)
        {
            this.IsBrewing = false;

            // start power off timer
            this.powerOffTimer.Change(this.keepHotDuration, -1);
        }

        private void OnPowerOffTimerCallback(object state)
        {
            this.PowerOff();
        }

        private void OnPowerSensorPortInterrupt(uint data1, uint data2, DateTime time)
        {
            // disable power off timer
            powerOffTimer.Change(-1, -1);

            var isHeating = data2 == 1;
            brewTimer.Change(isHeating ? this.brewDuration : -1, -1);
        }
    }
}
