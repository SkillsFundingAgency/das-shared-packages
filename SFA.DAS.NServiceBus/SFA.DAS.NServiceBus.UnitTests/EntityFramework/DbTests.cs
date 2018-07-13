using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.NServiceBus.EntityFramework;
using SFA.DAS.Testing;

namespace SFA.DAS.NServiceBus.UnitTests.EntityFramework
{
    [TestFixture]
    public class DbTests : FluentTest<DbTestsFixture>
    {
        [Test]
        public Task SaveChangesAsync_WhenSavingChanges_ThenShouldSaveChanges()
        {
            return RunAsync(f => f.SaveChangesAsync(), f => f.DbContext.Verify(d => d.SaveChangesAsync(), Times.Once()));
        }
    }

    public class DbTestsFixture : FluentTestFixture
    {
        public Mock<DbContextFake> DbContext { get; set; }
        public IDb Db { get; set; }

        public DbTestsFixture()
        {
            DbContext = new Mock<DbContextFake>();
            Db = new Db<DbContextFake>(new Lazy<DbContextFake>(() => DbContext.Object));
        }

        public Task SaveChangesAsync()
        {
            return Db.SaveChangesAsync();
        }
    }
}