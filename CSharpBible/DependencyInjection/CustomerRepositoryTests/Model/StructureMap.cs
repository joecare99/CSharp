using CustomerRepository.Model;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructureMap;
using System;
using System.Linq;

namespace CustomerRepositoryTests.Model
{
    [TestClass]
    public class StructureMap
    {
        private Container container { get; set; }

        [TestInitialize]
        public void Init()
        {
            container = new Container(x =>
            {
                x.For<IClock>().Use<CStaticClock>();
                x.For<ILog>().Singleton().Use<CLog>();
                x.For<ICustomerRepository>().Use<CustomerRepository5>();
            });
        }

        [TestMethod]
        public void RepThrowsExOnInvGet()
        {
            var container = new Container(x =>
            {
                x.For<IClock>().Use<CStaticClock>();
                x.For<ILog>().Singleton().Use<CLog>();
                x.For<ICustomerRepository>().Use<CustomerRepository5>();
            });

            var log = container.GetInstance<ILog>();
                
            ICustomerRepository repository =
                container.GetInstance<ICustomerRepository>();

            Assert.ThrowsException<ArgumentException>(
                () => repository.Get(Guid.NewGuid())
            );

            log.Get().Should().HaveCount( 1 );
        }

        [TestMethod]
        public void RepThrowsExOnInvGet2()
        {
            var log = container.GetInstance<ILog>();
            var repository =
                container.GetInstance<ICustomerRepository>();

            Assert.ThrowsException<ArgumentException>(
                () => repository?.Get(Guid.NewGuid())
            );

            log?.Get().Should().HaveCount(1);
        }

        [TestMethod]
        public void RepThrows2ExOn2InvGet2()
        {
            var log = container.GetInstance<ILog>();
            var repository =
                container.GetInstance<ICustomerRepository>();

            Assert.ThrowsException<ArgumentException>(
                () => repository?.Get(Guid.NewGuid())
            );
            Assert.ThrowsException<ArgumentException>(
                () => repository?.Get(Guid.NewGuid())
            );

            log?.Get().Should().HaveCount(2);
        }

        [TestMethod]
        public void LogsTimeOnInvLogGet()
        {
            var log = container.GetInstance<ILog>();
            var clock = container.GetInstance<IClock>();
            var repository =
                container.GetInstance<ICustomerRepository>();

            Assert.ThrowsException<ArgumentException>(
                () => repository?.Get(Guid.NewGuid())
            );

            log.Get().First().Time
                 .Should().Be(clock.Now);
        }
    }
}
