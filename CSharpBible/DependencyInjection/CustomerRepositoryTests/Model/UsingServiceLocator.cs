using CustomerRepository.Model;
using CustomerRepository.ServiceLocator;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace CustomerRepositoryTests.Model
{
    /// <summary>Test the repository using a ServiceLocator <strong>(!)</strong></summary>
    [TestClass]
    public class UsingServiceLocator
    {

        /// <summary>Repository logs on invalid get.</summary>
        [TestMethod]
        public void RepThrowsExOnInvGet()
        {
            ServiceLocator.Log = new CLog();

            ICustomerRepository repository = new CustomerRepository4();

            Assert.ThrowsException<ArgumentException>(
                () => repository.Get(Guid.NewGuid())
            );
        }

        /// <summary>Repository logs on invalid get.</summary>
        [TestMethod]
        public void RepLogsOnInvGet()
        {
            ServiceLocator.Log = new CLog();

            ICustomerRepository repository = new CustomerRepository4();

            Assert.ThrowsException<ArgumentException>(
                () => repository.Get(Guid.NewGuid())
            );

            ServiceLocator.Log.Get().Should().HaveCount(1);
        }

        /// <summary>Repository logs on multiple invalid gets.</summary>
        [TestMethod]
        public void RepLogsOnMultipleInvGet()
        {
            ServiceLocator.Log = new CLog();

            ICustomerRepository repository = new CustomerRepository4();

            #region these call is expected to fail ...
            try{ repository.Get(Guid.NewGuid()); }
            catch { }
            #endregion

            Assert.ThrowsException<ArgumentException>(
                () => repository.Get(Guid.NewGuid())
            );

            ServiceLocator.Log.Get().Should().HaveCount(2);
        }

        /// <summary>Logs the time on invalid get.</summary>
        [TestMethod]
        public void LogsTimeOnInvGet()
        {
            ServiceLocator.Clock = new CStaticClock();
            ServiceLocator.Log = new CLog();

            ICustomerRepository repository = new CustomerRepository4();

            Assert.ThrowsException<ArgumentException>(
                () => repository.Get(Guid.NewGuid())
            );

            ServiceLocator.Log.Get().First().Time
                .Should().Be(ServiceLocator.Clock.Now);
        }

        [TestMethod]
        public void PutTest()
        {
            ICustomerRepository repository = new CustomerRepository4();
            CCustomer customer = new();


            Guid g;
            Assert.IsNotNull(g = repository.Put(customer));
            Assert.AreEqual(customer, repository.Get(g));
        }

        [TestMethod]
        public void CountTest()
        {
            ICustomerRepository repository = new CustomerRepository4();

            Assert.AreEqual(0, repository.Count);

            CCustomer customer = new();

            repository.Put(customer);
            Assert.AreEqual(1, repository.Count);
        }
    }
}
