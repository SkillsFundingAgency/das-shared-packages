using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Messaging.FileSystem;

namespace SFA.DAS.Messaging.UnitTests.FileSystem
{
    public class FileSystemMessageTests
    {
        private const string TestFilePath = "TestFile.json";
        private const string ExpectedData = "Some Data";

        [SetUp]
        public void Arrange()
        {
            var serialisedContent = JsonConvert.SerializeObject(new PrivateSetterTestClass(ExpectedData));

            File.WriteAllText(TestFilePath, serialisedContent);
        }

        [Test]
        public async Task ShouldDeserialisePrivateSetters()
        {
            //Act
            var result = await FileSystemMessage<PrivateSetterTestClass>.Lock(new FileInfo(TestFilePath));

            //Assert
            Assert.AreEqual(ExpectedData, result.Content.Data);
        }

        [TearDown]
        public void CleanUp()
        {
            File.Delete(TestFilePath);
            File.Delete($"{TestFilePath}.lck");
        }
        
        internal class PrivateSetterTestClass
        {
            public string Data { get; protected set; }

            public PrivateSetterTestClass()
            { }

            public PrivateSetterTestClass(string data)
            {
                Data = data;
            }
        }
    }
}
