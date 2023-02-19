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

    }
}