using System;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.AzureServiceBus;
using SFA.DAS.Messaging.FileSystem;

namespace PublishReceiveSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var messageSubSystem = GetMessageSubSystem();
            if (messageSubSystem == null)
            {
                return;
            }
            Console.WriteLine();

            var messageService = new MessagingService(messageSubSystem);
            var exit = false;

            while (!exit)
            {
                Console.WriteLine("What would you like to do?");
                Console.WriteLine("   1. Publish event");
                Console.WriteLine("   2. Receive event");
                Console.WriteLine("   3. Quit");
                Console.Write("Selection (1 - 3): ");
                var selection = int.Parse(Console.ReadLine().Trim());

                switch (selection)
                {
                    case 1:
                        PublishMessage(messageService);
                        break;
                    case 2:
                        ReceiveMessage(messageService);
                        break;
                    default:
                        exit = true;
                        break;
                }
                Console.WriteLine();
            }
        }

        private static IMessageSubSystem GetMessageSubSystem()
        {
            Console.WriteLine("Which message sub-system would you like to use?");
            Console.WriteLine("   1. File system");
            Console.WriteLine("   2. Azure ServiceBus");
            Console.WriteLine("   3. Quit");
            Console.Write("Selection (1 - 3): ");
            var selection = int.Parse(Console.ReadLine().Trim());

            switch (selection)
            {
                case 1:
                    return new FileSystemMessageSubSystem();
                case 2:
                    Console.Write("Connection string: ");
                    var connectionString = Console.ReadLine().Trim();

                    Console.Write("Queue name: ");
                    var queueName = Console.ReadLine().Trim();

                    return new AzureServiceBusMessageSubSystem(connectionString, queueName);
                default:
                    return null;
            }
        }
        private static void PublishMessage(MessagingService messageService)
        {
            var e = new SampleEvent { Timestamp = DateTimeOffset.Now, Id = Guid.NewGuid().ToString() };
            messageService.Publish(e).Wait();

            WriteColoredLine($"Published message at {e.Timestamp} with id {e.Id}", ConsoleColor.Yellow);
        }
        private static void ReceiveMessage(MessagingService messageService)
        {
            var receiveTask = messageService.Receive<SampleEvent>();
            receiveTask.Wait();
            var e = receiveTask.Result;
            if (e == null || e.Content == null)
            {
                WriteColoredLine("No messages in queue to receive", ConsoleColor.Red);
            }
            else
            {
                e.Complete();
                WriteColoredLine($"Published message at {e.Content.Timestamp} with id {e.Content.Id}", ConsoleColor.Yellow);
            }
        }
        private static void WriteColoredLine(string line, ConsoleColor color)
        {
            var c = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(line);
            Console.ForegroundColor = c;
        }
    }
}
