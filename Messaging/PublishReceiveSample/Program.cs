using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.Messaging;

namespace PublishReceiveSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var messageSubSystem = GetMessageSubSystem();
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
            }
        }

        private static IMessageSubSystem GetMessageSubSystem()
        {
            return new FileSystemMessageSubSystem();
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
            if (e == null)
            {
                WriteColoredLine("No messages in queue to receive", ConsoleColor.Red);
            }
            else
            {
                WriteColoredLine($"Published message at {e.Timestamp} with id {e.Id}", ConsoleColor.Yellow);
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
