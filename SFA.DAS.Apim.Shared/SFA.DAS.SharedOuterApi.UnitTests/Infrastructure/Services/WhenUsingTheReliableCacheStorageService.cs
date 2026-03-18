using System;
using System.Net;
using AutoFixture.NUnit4;
using SFA.DAS.SharedOuterApi.Infrastructure.Services;
using SFA.DAS.SharedOuterApi.Interfaces;
using SFA.DAS.SharedOuterApi.Models;

namespace SFA.DAS.SharedOuterApi.UnitTests.Infrastructure.Services
{
    public class WhenUsingTheReliableCacheStorageService
    {
        [Test, MoqAutoData]
        public async Task Then_The_Request_Is_Handled_And_Two_Items_Created(
            string cacheKey,
            TestRequest request,
            TestData requestData,
            [Frozen] Mock<IGetApiClient<IApiConfiguration>> getApiClient,
            [Frozen] Mock<ICacheStorageService> cacheStorageService,
            ReliableCacheStorageService<IApiConfiguration> reliableCacheStorageService)
        {
            //Arrange
            var response = new ApiResponse<TestData>(requestData, HttpStatusCode.OK, "");
            cacheStorageService.Setup(x => x.RetrieveFromCache<TestData>(It.IsAny<string>())).ReturnsAsync((TestData)null);
            getApiClient.Setup(x => x.GetWithResponseCode<TestData>(It.Is<IGetApiRequest>(c => c.GetUrl.Equals(request.GetUrl))))
                .ReturnsAsync(response);
            
            //Act
            var actual = await reliableCacheStorageService.GetData<TestData>(request, cacheKey, _ => true);
            
            //Assert
            actual.Should().BeEquivalentTo(requestData);
            cacheStorageService.Verify(x=>x.SaveToCache(cacheKey, requestData, TimeSpan.FromMinutes(5), null), Times.Once);
            cacheStorageService.Verify(x=>x.SaveToCache($"{cacheKey}_extended", requestData, TimeSpan.FromDays(180), null), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task Then_If_The_Short_Cache_Is_Available_The_Request_Is_Not_Called(
            string cacheKey,
            TestRequest request,
            TestData requestData,
            [Frozen] Mock<IGetApiClient<IApiConfiguration>> getApiClient,
            [Frozen] Mock<ICacheStorageService> cacheStorageService,
            ReliableCacheStorageService<IApiConfiguration> reliableCacheStorageService)
        {
            //Arrange
            cacheStorageService.Setup(x => x.RetrieveFromCache<TestData>(cacheKey)).ReturnsAsync(requestData);
            
            //Act
            var actual = await reliableCacheStorageService.GetData<TestData>(request, cacheKey, _ => true);
            
            //Assert
            actual.Should().BeEquivalentTo(requestData);
            getApiClient.Verify(x => x.GetWithResponseCode<TestData>(It.IsAny<IGetApiRequest>()), Times.Never);
        }

        [Test, MoqAutoData]
        public async Task Then_If_The_Request_Is_Not_Found_Then_Null_Returned(
            string cacheKey,
            string errorContent,
            TestRequest request,
            TestData requestData,
            [Frozen] Mock<IGetApiClient<IApiConfiguration>> getApiClient,
            [Frozen] Mock<ICacheStorageService> cacheStorageService,
            ReliableCacheStorageService<IApiConfiguration> reliableCacheStorageService)
        {
            var response = new ApiResponse<TestData>(null, HttpStatusCode.NotFound, errorContent);
            cacheStorageService.Setup(x => x.RetrieveFromCache<TestData>(It.IsAny<string>())).ReturnsAsync((TestData)null);
            getApiClient
                .Setup(
                    x => x.GetWithResponseCode<TestData>(It.Is<IGetApiRequest>(c => c.GetUrl.Equals(request.GetUrl))))
                .ReturnsAsync(response);
            
            //Act
            var actual = await reliableCacheStorageService.GetData<TestData>(request, cacheKey, _ => true);
            
            //Assert
            actual.Should().BeNull();
            cacheStorageService.Verify(x=>x.SaveToCache(It.IsAny<string>(), It.IsAny<TestData>(), It.IsAny<TimeSpan>(), null), Times.Never);
        }

        [Test, MoqAutoData]
        public async Task Then_If_The_RequestCheck_Returns_False_Then_The_Cache_Is_Not_Updated(
            string cacheKey,
            TestRequest request,
            TestData requestData,
            [Frozen] Mock<IGetApiClient<IApiConfiguration>> getApiClient,
            [Frozen] Mock<ICacheStorageService> cacheStorageService,
            ReliableCacheStorageService<IApiConfiguration> reliableCacheStorageService)
        {
            var response = new ApiResponse<TestData>(requestData, HttpStatusCode.OK, "");
            cacheStorageService.Setup(x => x.RetrieveFromCache<TestData>(It.IsAny<string>())).ReturnsAsync((TestData)null);
            getApiClient
                .Setup(
                    x => x.GetWithResponseCode<TestData>(It.Is<IGetApiRequest>(c => c.GetUrl.Equals(request.GetUrl))))
                .ReturnsAsync(response);
            
            //Act
            var actual = await reliableCacheStorageService.GetData<TestData>(request, cacheKey, _ => false);
            
            //Assert
            actual.Should().BeNull();
            cacheStorageService.Verify(x=>x.SaveToCache(It.IsAny<string>(), It.IsAny<TestData>(), It.IsAny<TimeSpan>(), null), Times.Never);
        }

        [Test, MoqAutoData]
        public async Task Then_If_The_Request_Is_Not_Successful_Then_The_Short_Cache_Is_Updated_From_Long_Cache(
            string cacheKey,
            string errorContent,
            TestRequest request,
            TestData requestData,
            TestData cacheData,
            [Frozen] Mock<IGetApiClient<IApiConfiguration>> getApiClient,
            [Frozen] Mock<ICacheStorageService> cacheStorageService,
            ReliableCacheStorageService<IApiConfiguration> reliableCacheStorageService)
        {
            //Arrange
            var response = new ApiResponse<TestData>(requestData, HttpStatusCode.TooManyRequests, errorContent);
            cacheStorageService.Setup(x => x.RetrieveFromCache<TestData>($"{cacheKey}_extended")).ReturnsAsync(cacheData);
            cacheStorageService.Setup(x => x.RetrieveFromCache<TestData>($"{cacheKey}")).ReturnsAsync((TestData)null);
            getApiClient
                .Setup(
                    x => x.GetWithResponseCode<TestData>(It.Is<IGetApiRequest>(c => c.GetUrl.Equals(request.GetUrl))))
                .ReturnsAsync(response);
            
            //Act
            var actual = await reliableCacheStorageService.GetData<TestData>(request, cacheKey, _ => true);
            
            //Assert
            actual.Should().BeEquivalentTo(cacheData);
            cacheStorageService.Verify(x=>x.SaveToCache(cacheKey, cacheData, TimeSpan.FromMinutes(5), null), Times.Once);
            cacheStorageService.Verify(x=>x.SaveToCache($"{cacheKey}_extended", It.IsAny<TestData>(), TimeSpan.FromDays(180), null), Times.Never);
        }

        public class TestRequest : IGetApiRequest
        {
            public string GetUrl => "test-url";
        }
        
        public class TestData
        {
            public int TestValue { get; set; }
            public string TestString { get; set; }
        }
    }
}