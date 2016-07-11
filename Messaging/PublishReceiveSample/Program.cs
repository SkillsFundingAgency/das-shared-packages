using System;
using System.IO;
using PublishReceiveSample.SyndicationSamples;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.AzureServiceBus;
using SFA.DAS.Messaging.FileSystem;
using SFA.DAS.Messaging.Syndication;
using SFA.DAS.Messaging.Syndication.Hal.Json;
using SFA.DAS.Messaging.Syndication.Http;
using SFA.DAS.Messaging.Syndication.SqlServer;

namespace PublishReceiveSample
{
    class Program
    {
        private static ConsoleColor DefaultColor;
        private static readonly ConsoleColor DetailsColor = ConsoleColor.Yellow;
        private static readonly ConsoleColor ErrorColor = ConsoleColor.Red;


        static void Main(string[] args)
        {
            DefaultColor = Console.ForegroundColor;

            try
            {
                IMessagePublisher publisher;
                IPollingMessageReceiver receiver;
                LoadSubsystem(out publisher, out receiver);
                if (publisher == null)
                {
                    WriteColoredLine("Goodbye", DetailsColor);
                    System.Threading.Thread.Sleep(2000);
                    return;
                }

                WriteColoredLine("");
                while (PerformAction(publisher, receiver))
                {
                    WriteColoredLine("");
                }
            }
            catch (AggregateException ex)
            {
                WriteColoredLine(ex.InnerException.Message, ErrorColor);
                WriteColoredLine("Press any key to exit");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                WriteColoredLine(ex.Message, ErrorColor);
                WriteColoredLine("Press any key to exit");
                Console.ReadKey();
            }
        }

        private static void LoadSubsystem(out IMessagePublisher publisher, out IPollingMessageReceiver receiver)
        {
            publisher = null;
            receiver = null;

            var validSelection = false;
            while (!validSelection)
            {
                WriteColoredLine("What sub-system would you like to use:");
                WriteColoredLine("   1. File system");
                WriteColoredLine("   2. Azure Service Bus");
                WriteColoredLine("   3. Hal+Json");
                WriteColoredLine("   0. Quit");
                WriteColoredText("Enter selection: ");

                var input = Console.ReadLine().Trim();
                switch (input)
                {
                    case "1":
                        LoadFileSystem(out publisher, out receiver);
                        validSelection = true;
                        break;
                    case "2":
                        LoadAzureServiceBus(out publisher, out receiver);
                        validSelection = true;
                        break;
                    case "3":
                        LoadHalJson(out publisher, out receiver);
                        validSelection = true;
                        break;
                    case "0":
                        validSelection = true;
                        break;
                    default:
                        WriteColoredLine("Invalid selection! Enter 1, 2 or 0", ErrorColor);
                        WriteColoredLine("");
                        break;
                }
            }
        }
        private static void LoadFileSystem(out IMessagePublisher publisher, out IPollingMessageReceiver receiver)
        {
            WriteColoredText("Storage dir (%temp%/[Guid]): ");
            var dir = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(dir))
            {
                dir = Path.Combine(Environment.GetEnvironmentVariable("TEMP"), Guid.NewGuid().ToString());
            }

            var fs = new FileSystemMessageService(dir);
            publisher = fs;
            receiver = fs;
        }
        private static void LoadAzureServiceBus(out IMessagePublisher publisher, out IPollingMessageReceiver receiver)
        {
            WriteColoredText("Connection string: ");
            var connectionString = Console.ReadLine();

            WriteColoredText("Queue name: ");
            var queueName = Console.ReadLine();

            var asb = new AzureServiceBusMessageService(connectionString, queueName);
            publisher = asb;
            receiver = asb;
        }
        private static void LoadHalJson(out IMessagePublisher publisher, out IPollingMessageReceiver receiver)
        {
            WriteColoredText("Connection string: ");
            var connectionString = Console.ReadLine();

            WriteColoredText("Publish store sproc: ");
            var pubStoreSproc = Console.ReadLine();

            WriteColoredText("Publish receive sproc: ");
            var pubReceiveSproc = Console.ReadLine();

            WriteColoredText("Receive get sproc: ");
            var subGetSproc = Console.ReadLine();

            WriteColoredText("Receive update sproc: ");
            var subUpdateSproc = Console.ReadLine();

            publisher = new SyndicationMessagePublisher(new SqlServerMessageRepository(connectionString, pubStoreSproc, pubReceiveSproc));

            var feedPositionRepo = new SqlServerFeedPositionRepository(connectionString, subGetSproc, subUpdateSproc);
            receiver = new SyndicationPollingMessageReceiver(
                new HalJsonMessageClient(feedPositionRepo, new HttpClientWrapper("http://localhost:16972/"), new MessageIdentifierFactory()), 
                feedPositionRepo);
        }

        private static bool PerformAction(IMessagePublisher publisher, IPollingMessageReceiver receiver)
        {
            while (true)
            {
                WriteColoredLine("What would you like to do:");
                WriteColoredLine("   1. Publish message");
                WriteColoredLine("   2. Receive message");
                WriteColoredLine("   0. Quit");
                WriteColoredText("Enter selection: ");

                var input = Console.ReadLine().Trim();
                switch (input)
                {
                    case "1":
                        Publish(publisher);
                        return true;
                    case "2":
                        Receive(receiver);
                        return true;
                    case "0":
                        return false;
                    default:
                        WriteColoredLine("Invalid selection! Enter 1, 2 or 0", ErrorColor);
                        WriteColoredLine("");
                        break;
                }
            }
        }
        private static void Publish(IMessagePublisher publisher)
        {
            var message = new SampleEvent();
            publisher.PublishAsync(message).Wait();
            WriteColoredLine($"Published message with id {message.Id} at {message.Timestamp}", DetailsColor);
        }
        private static void Receive(IPollingMessageReceiver receiver)
        {
            var message = receiver.ReceiveAsAsync<SampleEvent>().Result;
            if (message == null)
            {
                WriteColoredLine("No messages waiting for processing", DetailsColor);
            }
            else
            {
                message.CompleteAsync();
                WriteColoredLine($"Received message with id {message.Content.Id} published at {message.Content.Timestamp}", DetailsColor);
            }
        }

        private static void WriteColoredLine(string line)
        {
            WriteColoredLine(line, DefaultColor);
        }
        private static void WriteColoredLine(string line, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(line);
            Console.ForegroundColor = DefaultColor;
        }

        private static void WriteColoredText(string line)
        {
            WriteColoredText(line, DefaultColor);
        }
        private static void WriteColoredText(string line, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(line);
            Console.ForegroundColor = DefaultColor;
        }
    }
}
