using System;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Configuration.FileStorage;

namespace GettingConfiguration
{
    class Program
    {
        static void Main(string[] args)
        {
            IConfigurationRepository repo = null;
            var exit = false;
            var defaultColor = Console.ForegroundColor;

            // Fluff - Get what persistence implementation to use
            while (repo == null && !exit)
            {
                Console.WriteLine("Where is your configuration stored:");
                Console.WriteLine("    1. Azure Table Storage");
                Console.WriteLine("    2. File Storage");
                Console.WriteLine("    3. None of the above. Exit");
                Console.Write("Please select (1 - 3): ");
                var input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        repo = new AzureTableStorageConfigurationRepository("UseDevelopmentStorage=true;");
                        break;
                    case "2":
                        repo = new FileStorageConfigurationRepository();
                        break;
                    case "3":
                        exit = true;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid input. Please enter an number between 1 and 2");
                        Console.ForegroundColor = defaultColor;
                        Console.WriteLine();
                        break;
                }
            }
            Console.WriteLine();

            // Create a new configuration service
            var configurationService = new ConfigurationService(repo,
                new ConfigurationOptions("GettingConfiguration", "DEV", "1.0"));

            // Get the configuration
            var configTask = configurationService.GetAsync<SampleConfiguration>();
            configTask.Wait();
            var config = configTask.Result;

            // Ouput result
            if (config != null)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Read {config.ServiceName} as configured service name");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Did not find configuration");
            }
            Console.ForegroundColor = defaultColor;

            Console.WriteLine();
            Console.Write("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
