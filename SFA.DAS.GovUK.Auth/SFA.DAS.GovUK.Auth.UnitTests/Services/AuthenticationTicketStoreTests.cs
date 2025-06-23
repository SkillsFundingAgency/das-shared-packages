using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Moq;
using SFA.DAS.GovUK.Auth.Configuration;
using SFA.DAS.GovUK.Auth.Services;

namespace SFA.DAS.GovUK.Auth.UnitTests.Services
{
    public class AuthenticationTicketStoreTests
    {
        private Mock<IDistributedCache> _distributedCache = null!;
        private Mock<IOptions<GovUkOidcConfiguration>> _config = null!;
        private AuthenticationTicketStore _store = null!;

        [SetUp]
        public void SetUp()
        {
            _distributedCache = new Mock<IDistributedCache>();
            _config = new Mock<IOptions<GovUkOidcConfiguration>>();
            _config.Setup(x => x.Value).Returns(new GovUkOidcConfiguration
            {
                LoginSlidingExpiryTimeOutInMinutes = 30
            });

            _store = new AuthenticationTicketStore(_distributedCache.Object, _config.Object);
        }

        [Test, AutoData]
        public async Task Then_The_Ticket_Is_Added_To_The_Store_With_SessionId_Set(AuthenticationTicket ticket)
        {
            var key = await _store.StoreAsync(ticket);

            Guid.TryParse(key, out _).Should().BeTrue();
            ticket.Properties.Items[AuthenticationTicketStore.SessionId].Should().Be(key);

            _distributedCache.Verify(x => x.SetAsync(
                key,
                It.Is<byte[]>(c => TicketSerializer.Default.Deserialize(c)!.AuthenticationScheme == ticket.AuthenticationScheme),
                It.Is<DistributedCacheEntryOptions>(c => c.SlidingExpiration!.Value.Minutes == 30),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test, AutoData]
        public async Task Then_The_Ticket_Is_Retrieved_And_Has_SessionId_Set(AuthenticationTicket ticket, string key)
        {
            var data = TicketSerializer.Default.Serialize(ticket);
            _distributedCache.Setup(x => x.GetAsync(key, It.IsAny<CancellationToken>()))
                .ReturnsAsync(data);

            var result = await _store.RetrieveAsync(key);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(ticket, options => options.Excluding(t => t.Properties.Items));
            result!.Properties.Items.Should().ContainKey(AuthenticationTicketStore.SessionId).WhoseValue.Should().Be(key);
        }

        [Test, AutoData]
        public async Task Then_Null_Is_Returned_If_The_Key_Does_Not_Exist(string key)
        {
            _distributedCache.Setup(x => x.GetAsync(key, It.IsAny<CancellationToken>()))
                .ReturnsAsync((byte[])null!);

            var result = await _store.RetrieveAsync(key);

            result.Should().BeNull();
        }

        [Test, AutoData]
        public async Task Then_The_Ticket_Is_Rewritten_With_SessionId(AuthenticationTicket ticket, string key)
        {
            await _store.RenewAsync(key, ticket);

            ticket.Properties.Items.Should().ContainKey(AuthenticationTicketStore.SessionId).WhoseValue.Should().Be(key);

            _distributedCache.Verify(x => x.SetAsync(
                key,
                It.Is<byte[]>(c => TicketSerializer.Default.Deserialize(c)!.AuthenticationScheme == ticket.AuthenticationScheme),
                It.Is<DistributedCacheEntryOptions>(c => c.SlidingExpiration!.Value.Minutes == 30),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test, AutoData]
        public async Task Then_The_Key_Is_Removed(string key)
        {
            await _store.RemoveAsync(key);

            _distributedCache.Verify(x => x.RemoveAsync(key, CancellationToken.None), Times.Once);
        }
    }
}
