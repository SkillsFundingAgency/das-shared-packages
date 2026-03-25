using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SFA.DAS.Apim.Shared.Infrastructure.Services;

namespace SFA.DAS.Apim.Shared.UnitTests.Infrastructure.Services
{
    public class WhenUsingTheCacheStorageService
    {
        [Test, MoqAutoData]
        public async Task Then_The_Item_Is_Saved_To_The_Cache_Using_Key_And_ConfigName_And_Expiry_Passed(
            string keyName,
            int expiryInHours,
            string appName,
            TestObject test,
            [Frozen] Mock<IDistributedCache> distributedCache,
            [Frozen] Mock<IConfiguration> configuration,
            CacheStorageService service)
        {
            //Arrange
            configuration.SetupGet(x => x[It.Is<string>(s => s.Equals("ConfigNames"))]).Returns(appName);
            
            //Act
            await service.SaveToCache(keyName, test, expiryInHours);
            
            //Assert
            distributedCache.Verify(x=>
                x.SetAsync(
                    $"{appName}_{keyName}",
                    It.Is<byte[]>(c=>Encoding.UTF8.GetString(c).Equals(JsonSerializer.Serialize(test,new JsonSerializerOptions()))), 
                    It.Is<DistributedCacheEntryOptions>(c
                        => c.AbsoluteExpirationRelativeToNow.Value.Hours == TimeSpan.FromHours(expiryInHours).Hours),
                    It.IsAny<CancellationToken>()), 
                Times.Once);
        }
        
        [Test, MoqAutoData]
        public async Task Then_The_Item_Is_Saved_To_The_Cache_Using_Key_And_First_ConfigName_And_Expiry_Passed(
            string keyName,
            int expiryInHours,
            string appName,
            string appName2,
            TestObject test,
            [Frozen] Mock<IDistributedCache> distributedCache,
            [Frozen] Mock<IConfiguration> configuration,
            CacheStorageService service)
        {
            //Arrange
            configuration.SetupGet(x => x[It.Is<string>(s => s.Equals("ConfigNames"))]).Returns($"{appName},{appName2}");
            
            //Act
            await service.SaveToCache(keyName, test, expiryInHours);
            
            //Assert
            distributedCache.Verify(x=>
                    x.SetAsync(
                        $"{appName}_{keyName}",
                        It.Is<byte[]>(c=>Encoding.UTF8.GetString(c).Equals(JsonSerializer.Serialize(test,new JsonSerializerOptions()))), 
                        It.Is<DistributedCacheEntryOptions>(c
                            => c.AbsoluteExpirationRelativeToNow.Value.Hours == TimeSpan.FromHours(expiryInHours).Hours),
                        It.IsAny<CancellationToken>()), 
                Times.Once);
        }

        [Test, MoqAutoData]
        public async Task Then_The_Item_Is_Retrieved_From_The_Cache_By_Key(
            string keyName,
            int expiryInHours,
            TestObject test,
            string appName,
            [Frozen] Mock<IDistributedCache> distributedCache,
            [Frozen] Mock<IConfiguration> configuration,
            CacheStorageService service)
        {
            //Arrange
            configuration.SetupGet(x => x[It.Is<string>(s => s.Equals("ConfigNames"))]).Returns(appName);
            distributedCache.Setup(x => x.GetAsync($"{appName}_{keyName}", It.IsAny<CancellationToken>()))
                .ReturnsAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(test,new JsonSerializerOptions())));

            //Act
            var item = await service.RetrieveFromCache<TestObject>(keyName);
            
            //Assert
            Assert.That(item, Is.Not.Null);
            item.Should().BeEquivalentTo(test);
        }

        [Test, MoqAutoData]
        public async Task Then_The_Item_Is_Retrieved_From_The_Cache_By_Key_For_Multiple_ConfigNames(
            string keyName,
            int expiryInHours,
            TestObject test,
            string appName,
            string appName2,
            [Frozen] Mock<IDistributedCache> distributedCache,
            [Frozen] Mock<IConfiguration> configuration,
            CacheStorageService service)
        {
            //Arrange
            configuration.SetupGet(x => x[It.Is<string>(s => s.Equals("ConfigNames"))]).Returns($"{appName},{appName2}");
            distributedCache.Setup(x => x.GetAsync($"{appName}_{keyName}", It.IsAny<CancellationToken>()))
                .ReturnsAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(test,new JsonSerializerOptions())));

            //Act
            var item = await service.RetrieveFromCache<TestObject>(keyName);
            
            //Assert
            Assert.That(item, Is.Not.Null);
            item.Should().BeEquivalentTo(test);
        }
        
        [Test, MoqAutoData]
        public async Task Then_If_The_Item_Does_Not_Exist_Default_Is_Returned(
            string keyName,
            string appName,
            [Frozen] Mock<IDistributedCache> distributedCache,
            [Frozen] Mock<IConfiguration> configuration,
            CacheStorageService service)
        {
            //Arrange
            configuration.SetupGet(x => x[It.Is<string>(s => s.Equals("ConfigNames"))]).Returns(appName);
            distributedCache.Setup(x => x.GetAsync($"{appName}_{keyName}", It.IsAny<CancellationToken>()))
                .ReturnsAsync((byte[]) null);

            //Act
            var item = await service.RetrieveFromCache<TestObject>(keyName);
            
            //Assert
            Assert.That(item, Is.Null);
        }

        [Test, MoqAutoData]
        public async Task And_List_Does_Not_Exist_Then_Default_Is_Returned(
            string keyName,
            string appName,
            [Frozen] Mock<IDistributedCache> distributedCache,
            [Frozen] Mock<IConfiguration> configuration,
            CacheStorageService service)
        {
            //Arrange
            configuration.SetupGet(x => x[It.Is<string>(s => s.Equals("ConfigNames"))]).Returns(appName);
            distributedCache.Setup(x => x.GetAsync($"{appName}_{keyName}", It.IsAny<CancellationToken>()))
                .ReturnsAsync((byte[]) null);

            //Act
            var item = await service.RetrieveFromCache<List<TestObject>>(keyName);
            
            //Assert
            Assert.That(item, Is.Null);
        }
        
        [Test, MoqAutoData]
        public async Task Then_The_Item_Is_Removed_From_The_Cache(
            string keyName,
            string appName,
            [Frozen] Mock<IDistributedCache> distributedCache,
            [Frozen] Mock<IConfiguration> configuration,
            CacheStorageService service)
        {
            //Arrange
            configuration.SetupGet(x => x[It.Is<string>(s => s.Equals("ConfigNames"))]).Returns(appName);
            
            //Act
            await service.DeleteFromCache(keyName);
            
            //Assert
            distributedCache.Verify(x=>x.RemoveAsync($"{appName}_{keyName}", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task Then_The_Key_Is_Added_To_The_Registry_If_Not_Already_Present(
            string registryName,
            string keyToAdd,
            string appName,
            TestObject item,
            [Frozen] Mock<IDistributedCache> distributedCache,
            [Frozen] Mock<IConfiguration> configuration,
            CacheStorageService service)
        {
            // Arrange
            configuration.SetupGet(x => x[It.Is<string>(s => s.Equals("ConfigNames"))]).Returns(appName);
            distributedCache.Setup(x => x.GetAsync($"{appName}_{registryName}", It.IsAny<CancellationToken>()))
                .ReturnsAsync((byte[])null);

            // Act
            await service.SaveToCache(keyToAdd, item, 1, registryName);

            // Assert
            distributedCache.Verify(x => x.SetAsync(
                $"{appName}_{registryName}",
                It.Is<byte[]>(c => Encoding.UTF8.GetString(c).Contains(keyToAdd)),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test, MoqAutoData]
        public async Task Then_The_Registry_Is_Retrieved_And_Deserialized(
            string registryName,
            string appName,
            List<string> expectedKeys,
            [Frozen] Mock<IDistributedCache> distributedCache,
            [Frozen] Mock<IConfiguration> configuration,
            CacheStorageService service)
        {
            // Arrange
            configuration.SetupGet(x => x[It.Is<string>(s => s.Equals("ConfigNames"))]).Returns(appName);
            var json = JsonSerializer.Serialize(expectedKeys);
            distributedCache.Setup(x => x.GetAsync($"{appName}_{registryName}", It.IsAny<CancellationToken>()))
                .ReturnsAsync(Encoding.UTF8.GetBytes(json));

            // Act
            var result = await service.GetCacheKeyRegistry(registryName);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EquivalentTo(expectedKeys));
        }

        [Test, MoqAutoData]
        public async Task Then_An_Empty_List_Is_Returned_If_Registry_Does_Not_Exist(
            string registryName,
            string appName,
            [Frozen] Mock<IDistributedCache> distributedCache,
            [Frozen] Mock<IConfiguration> configuration,
            CacheStorageService service)
        {
            // Arrange
            configuration.SetupGet(x => x[It.Is<string>(s => s.Equals("ConfigNames"))]).Returns(appName);
            distributedCache.Setup(x => x.GetAsync($"{appName}_{registryName}", It.IsAny<CancellationToken>()))
                .ReturnsAsync((byte[])null);

            // Act
            var result = await service.GetCacheKeyRegistry(registryName);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        }

        [Test, MoqAutoData]
        public async Task When_Adding_To_Cache_With_Registry_The_KeyName_Is_In_The_Registry(
            string registryName,
            string appName,
            string keyName,
            TestObject test,
            [Frozen] Mock<IConfiguration> configuration)
        {
            // Arrange
            var options = Options.Create(new MemoryDistributedCacheOptions());
            IDistributedCache cache = new MemoryDistributedCache(options);
            var service = new CacheStorageService(cache, configuration.Object);
            configuration.SetupGet(x => x[It.Is<string>(s => s.Equals("ConfigNames"))]).Returns(appName);

            // Act
            await service.SaveToCache<TestObject>(keyName, test, 1, registryName);
            var items = await service.GetCacheKeyRegistry(registryName);

            // Assert
            items.Should().HaveCount(1);
            items.Should().Contain($"{appName}_{keyName}");
        }

        public class TestObject
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}