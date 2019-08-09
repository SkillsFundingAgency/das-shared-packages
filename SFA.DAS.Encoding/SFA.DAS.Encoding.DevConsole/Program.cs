using System;
using Mono.Options;
using SFA.DAS.Encoding.DevConsole.Interfaces;
using StructureMap;

namespace SFA.DAS.Encoding.DevConsole
{
    class Program
    {
        private static IContainer _container;

        private static void InitialiseContainer()
        {
            try
            {
                _container = new Container(expression =>
                {
                    expression.Scan(scanner =>
                    {
                        scanner.TheCallingAssembly();
                        scanner.WithDefaultConventions();
                        scanner.LookForRegistries();
                    });
                });
            }
            catch (Exception)
            {
                Console.WriteLine("Error initialising structure map container");
                throw;
            }
        }

        static void Main(string[] args)
        {
            InitialiseContainer();

            var arguments = new Arguments();
            var optionSet = BuildOptionSet(arguments);

            try {
                optionSet.Parse(args);
            }
            catch (OptionException e) {
                Console.WriteLine("error: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try '--help' for more information.");
                return;
            }

            if (arguments.ShowHelp) {
                ShowHelp(optionSet);
                return;
            }

            _container.GetInstance<IEncodingServiceFacade>().RunEncodingService(arguments);
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
