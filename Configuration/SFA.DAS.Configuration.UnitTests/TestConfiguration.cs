using System.Collections.Generic;

namespace SFA.DAS.Configuration.UnitTests
{
    public class TestConfiguration
    {
        public TestConfiguration()
        {
            Collection = new List<string>();
        }

        public int Number { get; set; }
        public decimal Amount { get; set; }
        public bool Toggle { get; set; }
        public string Text { get; set; }
        public List<string> Collection { get; set; }

        public static TestConfiguration GetDefault()
        {
            return new TestConfiguration
            {
                Number = 1234,
                Amount = 12.34m,
                Toggle = true,
                Text = "Some details",
                Collection = new List<string>(new[] {"a", "b", "c"})
            };
        }
    }
}
