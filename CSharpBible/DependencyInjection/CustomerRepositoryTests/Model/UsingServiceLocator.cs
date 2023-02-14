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
        public void RepLogsOnInvGet()
        {
            ServiceLocator.Log = new CLog();
            ICustomerRepository repository = new CustomerRepository4();

            Assert.ThrowsException<ArgumentException>(
                () => repository.Get(Guid.NewGuid())
            );

            ServiceLocator.Log.Get().Should().HaveCount(1);
        }

        /// <summary>Repository logs on multible invalid gets.</summary>
        [TestMethod]
        public void RepLogsOnMultibleInvGet()
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
    }
}
