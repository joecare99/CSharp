using CustomerRepository.Model;
using CustomerRepository.Model.Factory;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace CustomerRepositoryTests.Model
{
    [TestClass]
    public class UsingFactoryTests
    {
        [TestMethod]
        public void EmptyRepThrowsOnGet()
        {
            ICustomerRepository repository =
                 new CustomerRepository2();

            Assert.ThrowsException<ArgumentException>(
                () => repository.Get(Guid.NewGuid()));
        }

        [TestMethod]
        public void RepLogsOnInvGet()
        {
            CClockFactory.injectClock = new CStaticClock();

            ICustomerRepository2 repository =
                 new CustomerRepository2();

            Assert.ThrowsException<ArgumentException>(
                () => repository.Get(Guid.NewGuid()));

            repository.Log.Get().Should().HaveCount(1);
        }

        [TestMethod]
        public void RepLogsOnMultibleInvGet()
        {
            CClockFactory.injectClock = new CStaticClock();

            ICustomerRepository2 repository =
                 new CustomerRepository2();

            Assert.ThrowsException<ArgumentException>(
                () => repository.Get(Guid.NewGuid()));
            Assert.ThrowsException<ArgumentException>(
                () => repository.Get(Guid.NewGuid()));

            repository.Log.Get().Should().HaveCount(2);
        }

        [TestMethod]
        public void RepLogsTimeOnInvLogGet()
        {
            CClockFactory.injectClock = new CStaticClock();

            ICustomerRepository2 repository =
                new CustomerRepository2();
            var referenceTime = repository.Clock.Now;

            Assert.ThrowsException<ArgumentException>(
                () => repository.Get(Guid.NewGuid())
            );

            repository.Log.Get().First().Time
                .Should().Be(referenceTime);
        }
    }
}
