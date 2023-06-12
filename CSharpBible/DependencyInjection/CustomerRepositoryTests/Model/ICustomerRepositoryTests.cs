using CustomerRepository.Model;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace CustomerRepositoryTests.Model
{
    [TestClass]
    public class ICustomerRepositoryTests
    {
        [TestMethod]
        public void EmptyRepThrowsOnGet()
        {
            ICustomerRepository repository = new CustomerRepository1();

            Assert.ThrowsException<ArgumentException>(
                () => repository.Get(Guid.NewGuid())
            );
        }

        [TestMethod]
        public void RepLogsOnInvGet()
        {
            ICustomerRepository2 repository = new CustomerRepository1();

            Assert.ThrowsException<ArgumentException>(
                () => repository.Get(Guid.NewGuid())
            );

            repository.Log.Get().Should().HaveCount(1);
        }

        [TestMethod]
        public void RepLogsTimeOnInvLogGet() {
            ICustomerRepository2 repository = new CustomerRepository1();

            Assert.ThrowsException<ArgumentException>(
                () => repository.Get(Guid.NewGuid())
            );

            var entry = repository.Log.Get().First();
            Math.Abs((entry.Time - DateTime.Now)
                .TotalSeconds).Should().BeLessThan(1);
        }

        [TestMethod]
        public void PutTest()
        {
            ICustomerRepository2 repository = new CustomerRepository1();
            CCustomer customer = new();


            Guid g;
            Assert.IsNotNull(g = repository.Put(customer));
            Assert.AreEqual(customer,repository.Get(g));
        }

        [TestMethod]
        public void CountTest()
        {
            ICustomerRepository2 repository = new CustomerRepository1();
            Assert.AreEqual(0, repository.Count);

            CCustomer customer = new();

            repository.Put(customer);
            Assert.AreEqual(1, repository.Count);
        }
    }
}