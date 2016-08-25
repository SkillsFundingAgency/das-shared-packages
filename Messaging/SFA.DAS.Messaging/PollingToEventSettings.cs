namespace SFA.DAS.Messaging
{
    public class PollingToEventSettings
    {
        public PollingToEventSettings()
        {
            PollingInterval = 1000;
        }

        /// <summary>
        /// Number of milliseconds between polls.
        /// </summary>
        public int PollingInterval { get; set; }
    }
}
