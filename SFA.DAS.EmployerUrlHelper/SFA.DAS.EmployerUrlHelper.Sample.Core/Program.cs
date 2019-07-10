using System;
using SFA.DAS.EmployerUrlHelper.Sample.Lib;

namespace SFA.DAS.EmployerUrl.Sample.Core
{
    class Program
    {
        static void Main(string[] args)
        {
            var testLinks = new TestLinks(Console.WriteLine, () => Console.ReadKey(true).Key);
            testLinks.ShowAllLinks("https://localhost");
            testLinks.WaitForKey(ConsoleKey.Escape);
        }
    }
}
