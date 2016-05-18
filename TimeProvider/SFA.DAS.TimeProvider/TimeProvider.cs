using System;

namespace SFA.DAS.TimeProvider
{
    public abstract class TimeProvider
    {
        private static TimeProvider current;

        static TimeProvider()
        {
            current = new DefaultTimeProvider();
        }

        public static TimeProvider Current
        {
            get { return current; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value), "You must provide a valid TimeProvider");

                current = value;
            }
        }

        public abstract DateTime UtcNow { get; }

        public static void ResetToDefault()
        {
            current = new DefaultTimeProvider();
        }
    }
}