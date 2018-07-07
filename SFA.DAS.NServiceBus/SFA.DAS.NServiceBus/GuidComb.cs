using System;

namespace SFA.DAS.NServiceBus
{
    /// <summary>
    /// Based on the GuidCombGenerator implementation in NHibernate
    /// https://github.com/nhibernate/nhibernate-core/blob/4.0.4.GA/src/NHibernate/Id/GuidCombGenerator.cs
    /// </summary>
    public static class GuidComb
    {
        private static readonly long BaseDateTicks = new DateTime(1900, 1, 1).Ticks;

        public static Guid NewGuidComb()
        {
            var guidArray = Guid.NewGuid().ToByteArray();
            var now = DateTime.UtcNow;
            var days = new TimeSpan(now.Ticks - BaseDateTicks);
            var timeOfDay = now.TimeOfDay;
            var daysArray = BitConverter.GetBytes(days.Days);
            var millisecondArray = BitConverter.GetBytes((long)(timeOfDay.TotalMilliseconds / 3.333333));

            Array.Reverse(daysArray);
            Array.Reverse(millisecondArray);
            Array.Copy(daysArray, daysArray.Length - 2, guidArray, guidArray.Length - 6, 2);
            Array.Copy(millisecondArray, millisecondArray.Length - 4, guidArray, guidArray.Length - 4, 4);

            return new Guid(guidArray);
        }
    }
}