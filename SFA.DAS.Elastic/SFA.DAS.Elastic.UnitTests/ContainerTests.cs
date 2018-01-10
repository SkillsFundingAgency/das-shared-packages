using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using StructureMap;

namespace SFA.DAS.Elastic.UnitTests
{
    public static class ContainerTests
    {
        private static int _initCallCount;
        private static int _disposeCallCount;

        public class When_getting_singleton_from_container_on_multiple_threads : Test
        {
            private IContainer _container;
            private IStub _stub1;
            private IStub _stub2;

            protected override void Given()
            {
                _container = new Container(c =>
                {
                    c.For<IStub>().Use<Stub>().Singleton();
                });
            }

            protected override void When()
            {
                Task.WaitAll(
                    Task.Run(async () =>
                    {
                        await Task.Yield();
                        _stub1 = _container.GetInstance<IStub>();
                    }),
                    Task.Run(async () =>
                    {
                        await Task.Yield();
                        _stub2 = _container.GetInstance<IStub>();
                    })
                );

                _container.Dispose();
            }

            [Test]
            [Ignore("This test failure is a bug in StructureMap 4.5.3 so I've used a lock instead in ElasticRegistry.GetElasticClientFactory().")]
            public void Then_should_initialize_one_instance()
            {
                Assert.That(_initCallCount, Is.EqualTo(1));
            }

            [Test]
            public void Then_should_return_one_instance()
            {
                Assert.That(_stub1, Is.SameAs(_stub2));
            }

            [Test]
            public void Then_should_dispose_one_instance()
            {
                Assert.That(_disposeCallCount, Is.EqualTo(1));
            }
        }

        private interface IStub : IDisposable
        {
        }

        private class Stub : IStub
        {
            public Stub()
            {
                Interlocked.Increment(ref _initCallCount);
            }

            public void Dispose()
            {
                Interlocked.Increment(ref _disposeCallCount);
            }
        }
    }
}