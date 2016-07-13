using System;

namespace PubSubSampleGui.SubSystems
{
    public class SampleMessageReceivedEventArgs : EventArgs
    {
        public SampleMessage Message { get; set; }
    }
}