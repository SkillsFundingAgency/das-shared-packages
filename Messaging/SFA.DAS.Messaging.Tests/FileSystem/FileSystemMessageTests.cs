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

        [TestCase(null)]
        [TestCase("")]
        [TestCase(".txt")]
        [TestCase(".lck")]
        public void Constructor_WithGuidFileName_ShouldTakeFileNameAsMessageId(string requiredFileExtension)
        {
            var fileNameWithoutExtension = Guid.NewGuid().ToString();
            var fileNameWithExtension = Path.ChangeExtension(fileNameWithoutExtension, requiredFileExtension);
            AssertTestFile(fileNameWithExtension, msg => Assert.AreEqual(fileNameWithoutExtension, msg.Id));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(".txt")]
        [TestCase(".lck")]
        public void Constructor_WithNonGuidFileName_ShouldHaveMessageIdSet(string requiredFileExtension)
        {
            var fileNameWithoutExtension = "abc";
            var fileNameWithExtension = Path.ChangeExtension(fileNameWithoutExtension, requiredFileExtension);
            AssertTestFile(fileNameWithExtension, msg => Assert.AreNotEqual(Guid.Empty, msg.Id));
        }

        private void AssertTestFile(string filename, Action<FileSystemMessage<object>> check)
        {
            var fullFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), filename);

            if (File.Exists(fullFileName))
            {
                File.Delete(fullFileName);
            }

            File.WriteAllText(fullFileName, "File used by unit test - can be deleted");
            try
            {
                var fileInfo = new FileInfo(fullFileName);
                var message = new FileSystemMessage<object>(fileInfo, null, null);
                check(message);
            }
            finally
            {
                File.Delete(fullFileName);
            }
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
