using AutoFixture;
using Moq;
using SFA.DAS.DfESignIn.Auth.Api.Helpers;

namespace SFA.DAS.DfESignIn.Auth.UnitTests.Api.Helpers
{
    [TestFixture]
    public class TokenDataExtensionsTests
    {
        private MockRepository _mockRepository;
        private Mock<ITokenDataSerializer> _mockTokenDataSerializer;
        private Mock<ITokenData> _mockTokenData;
        private Mock<ITokenEncoder> _mockTokenEncoder;
        private Mock<IJsonWebAlgorithm> _mockJsonWebAlgorithm;
        private TokenBuilder? _tokenBuilder;

        [SetUp]
        public void Arrange()
        {
            _mockRepository = new MockRepository(MockBehavior.Loose);
            _mockTokenDataSerializer = _mockRepository.Create<ITokenDataSerializer>();
            _mockTokenData = _mockRepository.Create<ITokenData>();
            _mockTokenEncoder = _mockRepository.Create<ITokenEncoder>();
            _mockJsonWebAlgorithm = _mockRepository.Create<IJsonWebAlgorithm>();
        }

        [Test]
        public void ForAudience_When_EmptyAudience_Given_ExceptionThrown()
        {
            // Arrange
            _tokenBuilder = GetTokenBuilder();

            // Sut
            var ex = Assert.Throws<ArgumentNullException>(() => _tokenBuilder.ForAudience(string.Empty));
            Assert.That(ex?.Message, Is.EqualTo("Value cannot be null. (Parameter 'audience')"));
        }

        [Test]
        public void UseAlgorithm_When_EmptyAlgorithm_Given_ExceptionThrown()
        {
            // Arrange
            _tokenBuilder = GetTokenBuilder();

            // Sut
            var ex = Assert.Throws<ArgumentNullException>(() => _tokenBuilder.UseAlgorithm(string.Empty));
            Assert.That(ex?.Message, Is.EqualTo("Value cannot be null. (Parameter 'algorithm')"));
        }

        [Test]
        public void WithSecretKey_When_EmptySecret_Given_ExceptionThrown()
        {
            // Arrange
            _tokenBuilder = GetTokenBuilder();

            // Sut
            var ex = Assert.Throws<ArgumentNullException>(() => _tokenBuilder.WithSecretKey(string.Empty));
            Assert.That(ex?.Message, Is.EqualTo("Value cannot be null. (Parameter 'secret')"));
        }

        [Test]
        public void Issuer_When_EmptyIssuer_Given_ExceptionThrown()
        {
            // Arrange
            _tokenBuilder = GetTokenBuilder();

            // Sut
            var ex = Assert.Throws<ArgumentNullException>(() => _tokenBuilder.Issuer(string.Empty));
            Assert.That(ex?.Message, Is.EqualTo("Value cannot be null. (Parameter 'issuer')"));
        }

        private TokenBuilder GetTokenBuilder()
        {
            var fixture = new Fixture();

            return new TokenBuilder(
                _mockTokenDataSerializer.Object,
                _mockTokenData.Object,
                _mockTokenEncoder.Object,
                _mockJsonWebAlgorithm.Object)
            {
                Algorithm = fixture.Create<string>(),
                Issuer = fixture.Create<string>(),
                Audience = fixture.Create<string>(),
                SecretKey = fixture.Create<byte[]>()
            };
        }
    }
}