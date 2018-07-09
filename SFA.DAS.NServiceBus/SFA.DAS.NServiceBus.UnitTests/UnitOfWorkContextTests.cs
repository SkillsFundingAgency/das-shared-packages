using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Testing;

namespace SFA.DAS.NServiceBus.UnitTests
{
    [TestFixture]
    public class UnitOfWorkContextTests : FluentTest<UnitOfWorkContextTestsFixture>
    {
        [Test]
        public void Get_WhenGettingData_ThenShouldReturnData()
        {
            Run(f => f.SetData(), f => f.GetData(), (f, d) => d.Should().Be(f.Data));
        }

        [Test]
        public void GetEvents_WhenGettingEvents_ThenShouldReturnAllEvents()
        {
            Run(f => f.SetEvents(), f => f.GetEvents(), (f, r) => r.Should().HaveCount(4).And.Match<IEnumerable<Event>>(e =>
                e.ElementAt(0) is FooEvent && e.ElementAt(0).Created == DateTime.MinValue &&
                e.ElementAt(1) is FooEvent && e.ElementAt(1).Created == f.Now &&
                e.ElementAt(2) is BarEvent && e.ElementAt(2).Created == DateTime.MinValue &&
                e.ElementAt(3) is BarEvent && e.ElementAt(3).Created == f.Now));
        }

        [Test]
        public void GetEvents_WhenGettingEventsAddedOnSeparateThreadsForSameAsyncFlow_ThenShouldReturnAllEvents()
        {
            Run(f => f.SetEventsOnSeparateThreads(), f => f.GetEvents(), (f, r) => r.Should().HaveCount(4).And
                .Contain(e => e is FooEvent && e.Created == DateTime.MinValue).And
                .Contain(e => e is FooEvent && e.Created == f.Now).And
                .Contain(e => e is BarEvent && e.Created == DateTime.MinValue).And
                .Contain(e => e is BarEvent && e.Created == f.Now));
        }

        [Test]
        public void GetEvents_WhenGettingEventsAddedOnSeparateThreadsForDifferentAsyncFlows_ThenShouldReturnNoEvents()
        {
            Run(f => f.SetEventsOnSeparateAsyncFlow(), f => f.GetEvents(), (f, r) => r.Should().BeEmpty());
        }
    }

    public class UnitOfWorkContextTestsFixture : FluentTestFixture
    {
        public DateTime Now { get; set; }
        public object Data { get; set; }
        public List<Event> Events { get; set; }
        public IUnitOfWorkContext UnitOfWorkContextInstance { get; set; }


        public UnitOfWorkContextTestsFixture()
        {
            Data = new object();
            Events = new List<Event>();
            UnitOfWorkContextInstance = new UnitOfWorkContext();
        }

        public object GetData()
        {
            return UnitOfWorkContextInstance.Get<object>();
        }

        public IEnumerable<Event> GetEvents()
        {
            return UnitOfWorkContextInstance.GetEvents();
        }

        public UnitOfWorkContextTestsFixture SetData()
        {
            UnitOfWorkContextInstance.Set(Data);

            return this;
        }

        public UnitOfWorkContextTestsFixture SetEvents()
        {
            var fooEvent = new FooEvent { Created = Now };
            var barEvent = new BarEvent { Created = Now };

            Events.Add(fooEvent);
            Events.Add(barEvent);

            UnitOfWorkContextInstance.AddEvent(fooEvent);
            UnitOfWorkContextInstance.AddEvent<FooEvent>(e => e.Created = Now);
            UnitOfWorkContext.AddEvent(barEvent);
            UnitOfWorkContext.AddEvent<BarEvent>(e => e.Created = Now);

            Now = DateTime.UtcNow;

            return this;
        }

        public UnitOfWorkContextTestsFixture SetEventsOnSeparateThreads()
        {
            var fooEvent = new FooEvent { Created = Now };
            var barEvent = new BarEvent { Created = Now };

            Events.Add(fooEvent);
            Events.Add(barEvent);

            var tasks = new List<Task>
            {
                Task.Run(() =>
                {
                    UnitOfWorkContextInstance.AddEvent(fooEvent);
                    UnitOfWorkContextInstance.AddEvent<FooEvent>(e => e.Created = Now);
                }),
                Task.Run(() =>
                {
                    UnitOfWorkContext.AddEvent(barEvent);
                    UnitOfWorkContext.AddEvent<BarEvent>(e => e.Created = Now);
                })
            };

            Task.WhenAll(tasks).GetAwaiter().GetResult();

            Now = DateTime.UtcNow;

            return this;
        }

        public UnitOfWorkContextTestsFixture SetEventsOnSeparateAsyncFlow()
        {
            var fooEvent = new FooEvent { Created = Now };
            var barEvent = new BarEvent { Created = Now };

            Events.Add(fooEvent);
            Events.Add(barEvent);

            var tasks = new List<Task>
            {
                Task.Run(() =>
                {
                    IUnitOfWorkContext unitOfWorkContextInstance = new UnitOfWorkContext();

                    UnitOfWorkContextInstance.AddEvent(fooEvent);
                    unitOfWorkContextInstance.AddEvent<FooEvent>(e => e.Created = Now);
                    UnitOfWorkContext.AddEvent(barEvent);
                    UnitOfWorkContext.AddEvent<BarEvent>(e => e.Created = Now);
                }),
                Task.Run(() =>
                {
                    IUnitOfWorkContext unitOfWorkContextInstance = new UnitOfWorkContext();

                    UnitOfWorkContextInstance.AddEvent(fooEvent);
                    unitOfWorkContextInstance.AddEvent<FooEvent>(e => e.Created = Now);
                    UnitOfWorkContext.AddEvent(barEvent);
                    UnitOfWorkContext.AddEvent<BarEvent>(e => e.Created = Now);
                })
            };

            Task.WhenAll(tasks).GetAwaiter().GetResult();

            Now = DateTime.UtcNow;

            return this;
        }
    }
}