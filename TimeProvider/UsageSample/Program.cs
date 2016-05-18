using System;
using SFA.DAS.TimeProvider;

namespace UsageSample
{
    class Program
    {
        static void Main(string[] args)
        {
            WriteCurrentDateTime();
            SetDateTime(DateTime.UtcNow.AddDays(-30));
            Reset();
            WriteCurrentDateTime();
            SetDateTime(DateTime.UtcNow.AddDays(30));
            Reset();

            Console.ReadLine();
        }

        private static void WriteCurrentDateTime()
        {
            Console.WriteLine("The current DateTime is: {0}", DateTimeProvider.Current.UtcNow);
        }

        private static void SetDateTime(DateTime newDateTime)
        {
            DateTimeProvider.Current = new FakeTimeProvider(newDateTime);
            Console.WriteLine("Changed TimeProvider to {0}", DateTimeProvider.Current);
            WriteCurrentDateTime();
        }

        private static void Reset()
        {
            DateTimeProvider.ResetToDefault();
            Console.WriteLine("Resetting TimeProvider to {0}", DateTimeProvider.Current);
        }
    }
}
