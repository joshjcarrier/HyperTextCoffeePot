namespace Core.Events
{
    using System;

    #if MF_FRAMEWORK_VERSION_V4_1
    using Microsoft.SPOT;   
    #endif

    public class InterruptEventArgs : EventArgs
    {
        public uint Data1 { get; set; }
        public uint Data2 { get; set; }
        public DateTime Time { get; set; }
    }
}
