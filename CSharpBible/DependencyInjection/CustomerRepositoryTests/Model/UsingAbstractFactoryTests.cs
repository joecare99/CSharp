using CustomerRepository.Model;
using CustomerRepository.Model.Factory;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace CustomerRepositoryTests.Model
{
    [TestClass]
    public class UsingAbstractFactoryTests
    {
        [TestMethod]
        public void EmptyRepThrowsOnGet()
        {
            ILogFactory logFactory = new CLogFactory();
            IClockFactory clockFactory = new CStaticClockFactory();

            ICustomerRepository repository =
                 new CustomerRepository3(clockFactory, logFactory);

            Assert.ThrowsException<ArgumentException>(
                () => repository.Get(Guid.NewGuid())
            );
        }

        [TestMethod]
        public void RepLogsOnInvGet()
        {
            ILogFactory logFactory = new CLogFactory();
            IClockFactory clockFactory = new CStaticClockFactory();

            ICustomerRepository2 repository =
                 new CustomerRepository3(clockFactory, logFactory);

            Assert.ThrowsException<ArgumentException>(
                () => repository.Get(Guid.NewGuid())
            );

            repository.Log.Get().Should().HaveCount(1);
        }

        [TestMethod]
        public void RepLogsOnMultibleInvGet()
        {
            ILogFactory logFactory = new CLogFactory();
            IClockFactory clockFactory = new CStaticClockFactory();

            ICustomerRepository2 repository =
                 new CustomerRepository3(clockFactory, logFactory);

            Assert.ThrowsException<ArgumentException>(
                () => repository.Get(Guid.NewGuid()));
            Assert.ThrowsException<ArgumentException>(
                () => repository.Get(Guid.NewGuid()));

            repository.Log.Get().Should().HaveCount(2);
        }

        [TestMethod]
        public void RepLogsTimeOnInvLogGet()
        {
            ILogFactory logFactory = new CLogFactory();
            IClockFactory clockFactory = new CStaticClockFactory();

            ICustomerRepository2 repository =
                new CustomerRepository3(clockFactory, logFactory);
            var referenceTime = repository.Clock.Now;

            Assert.ThrowsException<ArgumentException>(
                () => repository.Get(Guid.NewGuid())
            );

            repository.Log.Get().First().Time
                .Should().Be(referenceTime);
        }
    }
}
