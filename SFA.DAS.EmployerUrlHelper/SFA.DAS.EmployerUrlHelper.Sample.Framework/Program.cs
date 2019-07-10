using System;
using SFA.DAS.EmployerUrlHelper.Sample.Lib;

namespace SFA.DAS.EmployerUrlHelper.Sample.Framework
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
