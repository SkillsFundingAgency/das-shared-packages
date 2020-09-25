namespace SFA.DAS.HashingService.Console
{
    using System;

    public class Program
    {
        private static HashingService _service;

        static void Main(string[] args)
        {
            const string title = "SFA.DAS.HashingService.Console v0.1";
            Console.Title = title;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.WriteLine(title);
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("Enter 'hashstring' setting:");
            var hashstring = Console.ReadLine();
            if (hashstring == "") hashstring = "TEST: Dummy hash code London is a city in UK";

            Console.WriteLine("Enter 'allowed characters' setting:");
            var allowedChars = Console.ReadLine();
            if (allowedChars == "") allowedChars = "12345QWERTYUIOPNDGHAK";

            _service = new HashingService(allowedChars, hashstring);

            PrintMenu();
        }

        private static void PrintMenu()
        {
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine(@"Using numeric keys select one of the following options:");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("1. HashValue(long id)");
            Console.WriteLine("2. HashValue(Guid id)");
            Console.WriteLine("3. HashValue(string id)");
            Console.WriteLine("4. DecodeValueToLong(string id)");
            Console.WriteLine("5. DecodeValueToGuid(string id)");
            Console.WriteLine("6. DecodeValueToString(string id)");
            Console.ResetColor();

            var selectedMenuItem = ReadSelectedMenuItem();

            Console.WriteLine();
            Console.WriteLine("Enter input value:");
            var input = Console.ReadLine();
            var result = PerformAction(selectedMenuItem, input);
            Console.WriteLine("Result: " + result);

            Console.WriteLine("Press [space] to return to the menu or any other key to exit.");
            if (Console.ReadKey().Key == ConsoleKey.Spacebar)
            {
                PrintMenu();
            }
        }

        private static string PerformAction(in int selected, string input)
        {
            var result = selected switch
            {
                1 => _service.HashValue(Convert.ToInt64(input)),
                2 => _service.HashValue(Guid.Parse(input)),
                3 => _service.HashValue(input),
                4 => _service.DecodeValue(input).ToString(),
                5 => _service.DecodeValueToGuid(input).ToString(),
                6 => _service.DecodeValueToString(input),
                _ => null
            };

            return result;
        }


        private static int ReadSelectedMenuItem()
        {
            Console.Write("Option: ");
            int.TryParse(Console.ReadKey().KeyChar.ToString(), out var key);
            if (key >= 1 && key <= 6) return key;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("⚠ this option is not valid (sigh), please try again.");
            Console.ResetColor();
            return ReadSelectedMenuItem();

        }

    }
}
