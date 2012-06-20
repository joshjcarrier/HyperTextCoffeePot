namespace Core.Drivers
{
    #if MF_FRAMEWORK_VERSION_V4_1
    using Microsoft.SPOT.Hardware;    
    #else
    using System;
    using Core.Events;
    #endif

    public interface IInputPort
    {
        #if MF_FRAMEWORK_VERSION_V4_1
        event NativeEventHandler OnInterrupt;
        #else
        event EventHandler<InterruptEventArgs> OnInterrupt;
        #endif

        bool Read();
    }
}
