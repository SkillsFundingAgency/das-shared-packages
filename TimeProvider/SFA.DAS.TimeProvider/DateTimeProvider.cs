using System;

namespace SFA.DAS.TimeProvider
{
    public abstract class DateTimeProvider
    {
        private static DateTimeProvider current;

        static DateTimeProvider()
        {
            current = new DefaultTimeProvider();
        }

        public static DateTimeProvider Current
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