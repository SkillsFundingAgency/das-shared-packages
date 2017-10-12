using System;
using System.IO;
using System.Linq;
using PublishReceiveSample.SyndicationSamples;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.AzureServiceBus;
using SFA.DAS.Messaging.AzureStorageQueue;
using SFA.DAS.Messaging.FileSystem;
using SFA.DAS.Messaging.Interfaces;
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

            IMessageSubscriber<SampleEvent> receiver = null;

            try
            {
                IMessagePublisher publisher;

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
            finally
            {
                receiver?.Dispose();
            }
        }

        private static void LoadSubsystem<T>(
            out IMessagePublisher publisher, 
            out IMessageSubscriber<T> subscriber) where T : new()
        {
            publisher = null;
            subscriber = null;

            var validSelection = false;
            while (!validSelection)
            {
                WriteColoredLine("What sub-system would you like to use:");
                WriteColoredLine("   1. File system");
                WriteColoredLine("   2. Azure Storage Queue");
                WriteColoredLine("   3. Azure Service Bus");
                WriteColoredLine("   4. Hal+Json");
                WriteColoredLine("   0. Quit");
                WriteColoredText("Enter selection: ");

                var input = Console.ReadLine().Trim();
                switch (input)
                {
                    case "1":
                        LoadFileSystem(out publisher, out subscriber);
                        validSelection = true;
                        break;
                    case "2":
                        LoadAzureStorageQueue(out publisher, out subscriber);
                        validSelection = true;
                        break;
                    case "3":
                        LoadAzureServiceBus(out publisher, out subscriber);
                        validSelection = true;
                        break;
                    case "4":
                        LoadHalJson(out publisher, out subscriber);
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
        private static void LoadFileSystem<T>(
            out IMessagePublisher publisher, 
            out IMessageSubscriber<T> subscriber) where T : new()
        {
            WriteColoredText("Storage dir (%temp%/[Guid]): ");
            var dir = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(dir))
            {
                dir = Path.Combine(Environment.GetEnvironmentVariable("TEMP"), Guid.NewGuid().ToString());
            }

            publisher = new FileSystemMessagePublisher(dir);

            var subscriberFactory = new FileSystemMessageSubscriberFactory(dir);
            subscriber = subscriberFactory.GetSubscriber<T>();
        }

        private static void LoadAzureStorageQueue<T>(
            out IMessagePublisher publisher, 
            out IMessageSubscriber<T> subscriber) where T : new()
        {
            WriteColoredText("Connection string (Blank for dev storage): ");
            var connectionString = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                connectionString = "UseDevelopmentStorage=true;";
            }

            publisher = new QueueMessagePublisher(connectionString);

            var subscriberFactory = new QueueMessageSubscriberFactory(connectionString);
            subscriber = subscriberFactory.GetSubscriber<T>();
        }

        private static void LoadAzureServiceBus<T>(
            out IMessagePublisher publisher, 
            out IMessageSubscriber<T> subscriber) where T : new()
        {
            WriteColoredText("Connection string: ");
            var connectionString = Console.ReadLine();

            WriteColoredText("Subscription name: ");
            var subscriptionName = Console.ReadLine();

            publisher = new TopicMessagePublisher(connectionString);

            var subscriberFactory = new TopicSubscriberFactory(connectionString, subscriptionName);
            subscriber = subscriberFactory.GetSubscriber<T>();
        }

        private static void LoadHalJson<T>(
            out IMessagePublisher publisher, 
            out IMessageSubscriber<T> subscriber) where T : new()
        {
            var connectionString = "server=.;database=scratchpad;trusted_connection=true;";
            var pubStoreSproc = "usp_StoreMessage";
            var pubReceiveSproc = "usp_GetPageOfMessages";
            var subGetSproc = "usp_GetLastMessageId";
            var subUpdateSproc = "usp_UpdateLastMessageId";

            //WriteColoredText("Connection string: ");
            //var connectionString = Console.ReadLine();

            //WriteColoredText("Publish store sproc: ");
            //var pubStoreSproc = Console.ReadLine();

            //WriteColoredText("Publish receive sproc: ");
            //var pubReceiveSproc = Console.ReadLine();

            //WriteColoredText("Receive get sproc: ");
            //var subGetSproc = Console.ReadLine();

            //WriteColoredText("Receive update sproc: ");
            //var subUpdateSproc = Console.ReadLine();

            publisher = new SyndicationMessagePublisher(new SqlServerMessageRepository(connectionString, pubStoreSproc, pubReceiveSproc, 10));

            var feedPositionRepo = new SqlServerFeedPositionRepository(connectionString, subGetSproc, subUpdateSproc);
            subscriber = new SyndicationPollingMessageReceiver<T>(
                new HalJsonMessageClient(feedPositionRepo, new HttpClientWrapper("http://localhost:16972/"), new MessageIdentifierFactory()),
                feedPositionRepo);
        }

        private static bool PerformAction(
             IMessagePublisher publisher,
             IMessageSubscriber<SampleEvent> receiver)
        {
            while (true)
            {
                WriteColoredLine("What would you like to do:");
                WriteColoredLine("   1. Publish message");
                WriteColoredLine("   2. Receive message");
                WriteColoredLine("   3. Receive batch of messages");
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
                    case "3":
                        ReceiveBatch(receiver);
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

        private static void Receive(IMessageSubscriber<SampleEvent> receiver)
        {
            var message = receiver.ReceiveAsAsync().Result;
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

        private static void ReceiveBatch(IMessageSubscriber<SampleEvent> receiver)
        {
            var messages = receiver.ReceiveBatchAsAsync(10).Result?.ToArray();
            if (messages == null || !messages.Any())
            {
                WriteColoredLine("No messages waiting for processing", DetailsColor);
            }
            else
            {
                foreach (var message in messages)
                {
                    message.CompleteAsync();
                    WriteColoredLine($"Received message with id {message.Content.Id} published at {message.Content.Timestamp}", DetailsColor);
                    System.Threading.Thread.Sleep(50);
                }
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
