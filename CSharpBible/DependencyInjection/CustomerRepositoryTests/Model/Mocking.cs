using CustomerRepository.Model;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace CustomerRepositoryTests.Model
{
    [TestClass]
    public class Mocking
    {
        [TestMethod]
        public void RepThrowsExOnInvGet()
        {
            var clock = new Mock<IClock>();
            var log = new Mock<ILog>();
            log.Setup(I => I.Error(It.IsAny<DateTimeOffset>(), 
                It.IsAny<string>()));

            var repository = 
                new CustomerRepository5(clock.Object, log.Object);

            Assert.ThrowsException<ArgumentException>(
                () => repository.Get(Guid.NewGuid())
            );

            log.Verify(I => I.Error(It.IsAny<DateTimeOffset>(),
                It.IsAny<string>()), Times.Exactly(1));
        }

        [TestMethod]
        public void RepLogsTimeOnInvGet()
        {
            var clock = new Mock<IClock>();
            var log = new Mock<ILog>();
            log.Setup(I => I.Error(It.IsAny<DateTimeOffset>(),
                It.IsAny<string>()));

            var referencetime = clock.Object.Now;

            var repository =
                new CustomerRepository5(clock.Object, log.Object);
            Assert.ThrowsException<ArgumentException>(
                () => repository.Get(Guid.NewGuid())
            );

            log.Verify(I => I.Error(It.IsAny<DateTimeOffset>(),
                It.IsAny<string>()), Times.Exactly(1));

        }

        [TestMethod]
        public void RepLogsOnMultibleInvGet()
        {
            var clock = new Mock<IClock>();
            var log = new Mock<ILog>();
            log.Setup(I => I.Error(It.IsAny<DateTimeOffset>(),
                It.IsAny<string>()));

            ICustomerRepository repository = new CustomerRepository5(clock.Object, log.Object);
            // repository.Get(Guid.NewGuid());
            Assert.ThrowsException<ArgumentException>(
                () => repository.Get(Guid.NewGuid())
            );
            Assert.ThrowsException<ArgumentException>(
                () => repository.Get(Guid.NewGuid())
            );

            log.Verify(I => I.Error(It.IsAny<DateTimeOffset>(),
                It.IsAny<string>()), Times.Exactly(2));
        }
    }
}
