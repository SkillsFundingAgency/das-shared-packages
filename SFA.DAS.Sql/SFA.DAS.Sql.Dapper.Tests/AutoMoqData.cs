using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.NUnit3;

namespace SFA.DAS.Sql.Dapper.Tests
{
    public class AutomoqData : AutoDataAttribute
    {
        public AutomoqData() : base(new Fixture().Customize(new AutoMoqCustomization()))
        {}
    }
}
