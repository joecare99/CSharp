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
        public void RepLogsOnMultipleInvGet()
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

        [TestMethod]
        public void PutTest()
        {
            ICustomerRepository2 repository =
                new CustomerRepository2();
            CCustomer customer = new();


            Guid g;
            Assert.IsNotNull(g = repository.Put(customer));
            Assert.AreEqual(customer, repository.Get(g));
        }

        [TestMethod]
        public void CountTest()
        {
            ICustomerRepository2 repository =
                new CustomerRepository2();
            Assert.AreEqual(0, repository.Count);

            CCustomer customer = new();

            repository.Put(customer);
            Assert.AreEqual(1, repository.Count);
        }
    }
}
