using AutoFixture;
using Moq;
using SFA.DAS.DfESignIn.Auth.Api.Helpers;

namespace SFA.DAS.DfESignIn.Auth.UnitTests.Api.Helpers
{
    [TestFixture]
    public class TokenBuilderTests
    {
        private MockRepository _mockRepository;
        private Mock<ITokenDataSerializer> _mockTokenDataSerializer;
        private Mock<ITokenData> _mockTokenData;
        private Mock<ITokenEncoder> _mockTokenEncoder;
        private Mock<IJsonWebAlgorithm> _mockJsonWebAlgorithm;

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
        public void NoAlgorithmDefinedExceptionThrown()
        {
            var fixture = new Fixture();

            var _tokenBuilder = new TokenBuilder(
                _mockTokenDataSerializer.Object,
                _mockTokenData.Object,
                _mockTokenEncoder.Object,
                _mockJsonWebAlgorithm.Object);

            _tokenBuilder.Algorithm = null;
            _tokenBuilder.Issuer = fixture.Create<string>();
            _tokenBuilder.Audience = fixture.Create<string>();
            _tokenBuilder.SecretKey = fixture.Create<byte[]>();

            var ex = Assert.Throws<Exception>(() => _tokenBuilder.CreateToken());
            Assert.That(ex.Message, Is.EqualTo("Algorithm"));
        }

        [Test]
        public void NoIssuerDefinedExceptionThrown()
        {
            var fixture = new Fixture();

            var _tokenBuilder = new TokenBuilder(
                _mockTokenDataSerializer.Object,
                _mockTokenData.Object,
                _mockTokenEncoder.Object,
                _mockJsonWebAlgorithm.Object);

            _tokenBuilder.Algorithm = fixture.Create<string>();
            _tokenBuilder.Issuer = null;
            _tokenBuilder.Audience = fixture.Create<string>();
            _tokenBuilder.SecretKey = fixture.Create<byte[]>();

            var ex = Assert.Throws<Exception>(() => _tokenBuilder.CreateToken());
            Assert.That(ex.Message, Is.EqualTo("Issuer"));
        }

        [Test]
        public void NoAudienceDefinedExceptionThrown()
        {
            var fixture = new Fixture();

            var _tokenBuilder = new TokenBuilder(
                _mockTokenDataSerializer.Object,
                _mockTokenData.Object,
                _mockTokenEncoder.Object,
                _mockJsonWebAlgorithm.Object);

            _tokenBuilder.Algorithm = fixture.Create<string>();
            _tokenBuilder.Issuer = fixture.Create<string>();
            _tokenBuilder.Audience = null;
            _tokenBuilder.SecretKey = fixture.Create<byte[]>();

            var ex = Assert.Throws<Exception>(() => _tokenBuilder.CreateToken());
            Assert.That(ex.Message, Is.EqualTo("Audience"));
        }

        [Test]
        public void NoSecretKeyDefinedExceptionThrown()
        {
            var fixture = new Fixture();

            var _tokenBuilder = new TokenBuilder(
                _mockTokenDataSerializer.Object,
                _mockTokenData.Object,
                _mockTokenEncoder.Object,
                _mockJsonWebAlgorithm.Object);

            _tokenBuilder.Algorithm = fixture.Create<string>();
            _tokenBuilder.Issuer = fixture.Create<string>();
            _tokenBuilder.Audience = fixture.Create<string>();
            _tokenBuilder.SecretKey = null;

            var ex = Assert.Throws<Exception>(() => _tokenBuilder.CreateToken());
            Assert.That(ex.Message, Is.EqualTo("SecretKey"));
        }
    }
}