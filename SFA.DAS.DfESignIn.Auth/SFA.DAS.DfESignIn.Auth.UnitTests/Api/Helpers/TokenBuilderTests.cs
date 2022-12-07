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
        public void NoAlgorithmDefined_ExceptionThrown()
        {
            var fixture = new Fixture();

            var _tokenBuilder = new TokenBuilder(
                _mockTokenDataSerializer.Object,
                _mockTokenData.Object,
                _mockTokenEncoder.Object,
                _mockJsonWebAlgorithm.Object)
            {
                Algorithm = null,
                Issuer = fixture.Create<string>(),
                Audience = fixture.Create<string>(),
                SecretKey = fixture.Create<byte[]>()
            };

            var ex = Assert.Throws<ArgumentNullException>(() => _tokenBuilder.CreateToken());
            Assert.That(ex.Message, Is.EqualTo("Value cannot be null. (Parameter 'Algorithm')"));
        }

        [Test]
        public void NoIssuerDefined_ExceptionThrown()
        {
            var fixture = new Fixture();

            var _tokenBuilder = new TokenBuilder(
                _mockTokenDataSerializer.Object,
                _mockTokenData.Object,
                _mockTokenEncoder.Object,
                _mockJsonWebAlgorithm.Object)
            {
                Algorithm = fixture.Create<string>(),
                Issuer = null,
                Audience = fixture.Create<string>(),
                SecretKey = fixture.Create<byte[]>()
            };

            var ex = Assert.Throws<ArgumentNullException>(() => _tokenBuilder.CreateToken());
            Assert.That(ex.Message, Is.EqualTo("Value cannot be null. (Parameter 'Issuer')"));
        }

        [Test]
        public void NoAudienceDefined_ExceptionThrown()
        {
            var fixture = new Fixture();

            var _tokenBuilder = new TokenBuilder(
                _mockTokenDataSerializer.Object,
                _mockTokenData.Object,
                _mockTokenEncoder.Object,
                _mockJsonWebAlgorithm.Object)
            {
                Algorithm = fixture.Create<string>(),
                Issuer = fixture.Create<string>(),
                Audience = null,
                SecretKey = fixture.Create<byte[]>()
            };

            var ex = Assert.Throws<ArgumentNullException>(() => _tokenBuilder.CreateToken());
            Assert.That(ex.Message, Is.EqualTo("Value cannot be null. (Parameter 'Audience')"));
        }

        [Test]
        public void NoSecretKeyDefined_ExceptionThrown()
        {
            var fixture = new Fixture();

            var _tokenBuilder = new TokenBuilder(
                _mockTokenDataSerializer.Object,
                _mockTokenData.Object,
                _mockTokenEncoder.Object,
                _mockJsonWebAlgorithm.Object)
            {
                Algorithm = fixture.Create<string>(),
                Issuer = fixture.Create<string>(),
                Audience = fixture.Create<string>(),
                SecretKey = null
            };

            var ex = Assert.Throws<ArgumentNullException>(() => _tokenBuilder.CreateToken());
            Assert.That(ex.Message, Is.EqualTo("Value cannot be null. (Parameter 'SecretKey')"));
        }
    }
}