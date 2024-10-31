using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Moq;
using SFA.DAS.GovUK.Auth.Configuration;
using SFA.DAS.GovUK.Auth.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.GovUK.Auth.UnitTests.Services
{
    public class AuthenticationTicketStoreTests
    {
        [Test, MoqAutoData]
        public async Task Then_The_Ticket_Is_Added_To_The_Store(
            AuthenticationTicket ticket,
            int expiryTime,
            [Frozen] Mock<IDistributedCache> distributedCache,
            [Frozen] Mock<IOptions<GovUkOidcConfiguration>> config,
            AuthenticationTicketStore authenticationTicketStore)
        {
            config.Object.Value.LoginSlidingExpiryTimeOutInMinutes = expiryTime;
            
            var result = await authenticationTicketStore.StoreAsync(ticket);
            
            Assert.That(Guid.TryParse(result, out var actualKey), Is.True);
            distributedCache.Verify(x=>x.SetAsync(
                actualKey.ToString(),
                It.Is<byte[]>(c=>TicketSerializer.Default.Deserialize(c)!.AuthenticationScheme == ticket.AuthenticationScheme), 
                It.Is<DistributedCacheEntryOptions>(c
                    => c!.SlidingExpiration!.Value.Minutes == TimeSpan.FromMinutes(expiryTime).Minutes),
                It.IsAny<CancellationToken>()
                ), Times.Once);
        }
    
        [Test, MoqAutoData]
        public async Task Then_The_Ticket_Is_Retrieved_By_Id_From_The_Store(
            AuthenticationTicket ticket,
            string key,
            [Frozen] Mock<IDistributedCache> distributedCache,
            [Frozen] Mock<IOptions<GovUkOidcConfiguration>> config,
            AuthenticationTicketStore authenticationTicketStore)
        {
            distributedCache.Setup(x => x.GetAsync(key, It.IsAny<CancellationToken>()))
                .ReturnsAsync(TicketSerializer.Default.Serialize(ticket));
            
            var result = await authenticationTicketStore.RetrieveAsync(key);

            result.Should().BeEquivalentTo(ticket);
        }
        
        [Test, MoqAutoData]
        public async Task Then_Null_Is_Returned_If_The_Key_Does_Not_Exist(
            AuthenticationTicket ticket,
            string key,
            [Frozen] Mock<IDistributedCache> distributedCache,
            [Frozen] Mock<IOptions<GovUkOidcConfiguration>> config,
            AuthenticationTicketStore authenticationTicketStore)
        {
            distributedCache.Setup(x => x.GetAsync(key, It.IsAny<CancellationToken>()))
                .ReturnsAsync((byte[]) null!);
            
            var result = await authenticationTicketStore.RetrieveAsync(key);

            result.Should().BeNull();
        }

        [Test, MoqAutoData]
        public async Task Then_The_Key_Is_Refreshed(
            AuthenticationTicket ticket,
            string key,
            [Frozen] Mock<IDistributedCache> distributedCache,
            AuthenticationTicketStore authenticationTicketStore)
        {
            await authenticationTicketStore.RenewAsync(key, ticket);
            
            distributedCache.Verify(x=>x.RefreshAsync(key, CancellationToken.None));
        }
        
        [Test, MoqAutoData]
        public async Task Then_The_Key_Is_Removed(
            string key,
            [Frozen] Mock<IDistributedCache> distributedCache,
            AuthenticationTicketStore authenticationTicketStore)
        {
            await authenticationTicketStore.RemoveAsync(key);
            
            distributedCache.Verify(x=>x.RemoveAsync(key, CancellationToken.None));
        }
    }    
}
