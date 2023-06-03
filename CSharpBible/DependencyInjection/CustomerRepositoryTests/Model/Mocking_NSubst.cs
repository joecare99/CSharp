using CustomerRepository.Model;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;

namespace CustomerRepositoryTests.Model
{
    [TestClass]
    public class Mocking_NSubst
    {
        [TestMethod]
        public void RepThrowsExOnInvGet()
        {
            var clock = Substitute.For<IClock>();
            var log = Substitute.For<ILog>();
            clock.Now.Returns(new DateTime(2023,05,01));
            
            var repository = 
                new CustomerRepository5(clock, log);

            Assert.ThrowsException<ArgumentException>(
                () => repository.Get(Guid.NewGuid())
            );

            log.Received().Error(new DateTime(2023, 05, 01), "no such customer");
        }

        [TestMethod]
        public void RepLogsTimeOnInvGet()
        {
            var clock = Substitute.For<IClock>();
            var log = Substitute.For<ILog>();
            clock.Now.Returns(new DateTime(2023, 05, 02));

            var referencetime = clock.Now;

            var repository =
                new CustomerRepository5(clock, log);
            Assert.ThrowsException<ArgumentException>(
                () => repository.Get(Guid.NewGuid())
            );

            log.Received().Error(new DateTime(2023, 05, 02), "no such customer");

        }

        [TestMethod]
        public void RepLogsOnMultibleInvGet()
        {
            var clock = Substitute.For<IClock>();
            var log = Substitute.For<ILog>();
            clock.Now.Returns(new DateTime(2023, 05, 03));

            ICustomerRepository repository = new CustomerRepository5(clock, log);
            // repository.Get(Guid.NewGuid());
            Assert.ThrowsException<ArgumentException>(
                () => repository.Get(Guid.NewGuid())
            );
            Assert.ThrowsException<ArgumentException>(
                () => repository.Get(Guid.NewGuid())
            );

            log.Received().Error(new DateTime(2023, 05, 03), "no such customer");

        }
    }
}
