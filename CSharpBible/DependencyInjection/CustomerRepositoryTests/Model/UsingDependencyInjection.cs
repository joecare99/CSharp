using CustomerRepository.Model;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace CustomerRepositoryTests.Model
{
    [TestClass]
    public class UsingDependencyInjection
    {
        [TestMethod]
        public void RepThrowsExsOnInvGet()
        {
            var clock = new CStaticClock();
            var log = new CLog();

            ICustomerRepository repository = new CustomerRepository5(clock, log);

            Assert.ThrowsException<ArgumentException>(
                () => repository.Get(Guid.NewGuid())
            );
        }

        [TestMethod]
        public void RepLogsOnInvGet()
        {
            var clock = new CStaticClock();
            var log = new CLog();

            ICustomerRepository repository = new CustomerRepository5(clock,log);

            Assert.ThrowsException<ArgumentException>(
                () => repository.Get(Guid.NewGuid())
            );

            log.Get().Should().HaveCount(1);
        }

        [TestMethod]
        public void RepLogsOnMultipleInvGet()
        {
            var clock = new CStaticClock();
            var log = new CLog();

            ICustomerRepository repository = new CustomerRepository5(clock,log);
           // repository.Get(Guid.NewGuid());
            Assert.ThrowsException<ArgumentException>(
                () => repository.Get(Guid.NewGuid())
            );
            Assert.ThrowsException<ArgumentException>(
                () => repository.Get(Guid.NewGuid())
            );

            log.Get().Should().HaveCount(2);
        }

        [TestMethod]
        public void LogsTimeOnInvLogGet()
        {
            var clock = new CStaticClock();
            var log = new CLog();

            ICustomerRepository repository = new CustomerRepository5(clock, log);

            Assert.ThrowsException<ArgumentException>(
                () => repository.Get(Guid.NewGuid())
            );

            log.Get().First().Time
                .Should().Be(clock.Now);
        }

        [TestMethod]
        public void PutTest()
        {
            var clock = new CStaticClock();
            var log = new CLog();

            ICustomerRepository repository = new CustomerRepository5(clock, log);
            CCustomer customer = new();


            Guid g;
            Assert.IsNotNull(g = repository.Put(customer));
            Assert.AreEqual(customer, repository.Get(g));
        }

        [TestMethod]
        public void CountTest()
        {
            var clock = new CStaticClock();
            var log = new CLog();

            ICustomerRepository repository = new CustomerRepository5(clock, log);

            Assert.AreEqual(0, repository.Count);

            CCustomer customer = new();

            repository.Put(customer);
            Assert.AreEqual(1, repository.Count);
        }
    }
}
