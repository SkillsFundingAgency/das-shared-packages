using System;
using Mono.Options;
using SFA.DAS.AutoConfiguration;

namespace SFA.DAS.Encoding.DevConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var arguments = new Arguments();
            var p = BuildOptionSet(arguments);

            try {
                p.Parse(args);
            }
            catch (OptionException e) {
                Console.WriteLine("error: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try '--help' for more information.");
                return;
            }

            if (arguments.ShowHelp) {
                ShowHelp(p);
                return;
            }
            
            RunEncodingService(arguments);
        }

        private static void RunEncodingService(Arguments arguments)
        {
            var tableStorageConfigurationService = new TableStorageConfigurationService(
                new EnvironmentService(),
                new AzureTableStorageConnectionAdapter());
            var encodingConfig = tableStorageConfigurationService.Get<EncodingConfig>("SFA.DAS.Encoding");

            var encodingService = new EncodingService(encodingConfig);

            switch (arguments.ActionType)
            {
                case ActionType.Encode:
                    long.TryParse(arguments.Value, out var rawValue);
                    var encodedValue = encodingService.Encode(rawValue, arguments.EncodingType);
                    Console.WriteLine($"encoded value: {encodedValue}");
                    break;
                case ActionType.Decode:
                    var decodedValue = encodingService.Decode(arguments.Value, arguments.EncodingType);
                    Console.WriteLine($"decoded value: {decodedValue}");
                    break;
                default:
                    Console.WriteLine("Error: invalid action");
                    return;
            }
        }

        private static OptionSet BuildOptionSet(Arguments arguments)
        {
            var p = new OptionSet
            {
                {
                    "a|action=", "the {ActionType} to execute, either encode or decode.",
                    (ActionType v) => arguments.ActionType = v
                },
                {
                    "t|type=", "the {EncodingType} to use.",
                    (EncodingType v) => arguments.EncodingType = v
                },
                {
                    "v|value=", "the {Value} to operate on",
                    v => arguments.Value = v
                },
                {
                    "h|help", "show this message and exit",
                    v => arguments.ShowHelp = v != null
                },
            };
            return p;
        }

        static void ShowHelp (OptionSet p)
        {
            Console.WriteLine ("Usage: dotnet SFA.DAS.Encoding.DevConsole.dll [OPTIONS]");
            Console.WriteLine ();
            Console.WriteLine ("Options:");
            p.WriteOptionDescriptions (Console.Out);
        }
    }
}
