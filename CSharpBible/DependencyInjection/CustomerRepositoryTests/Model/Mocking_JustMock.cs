using CustomerRepository.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using System;

namespace CustomerRepositoryTests.Model
{
    [TestClass]
    public class Mocking_JustMock
    {
        [TestMethod]
        public void RepThrowsExOnInvGet()
        {
            var clock = Mock.Create<IClock>(Behavior.Strict);
            var log = Mock.Create<ILog>(Behavior.Strict);
            Mock.Arrange(()=>log.Error(new DateTime(2023, 06, 01), "no such customer")).MustBeCalled();
            Mock.Arrange(() => clock.Now).Returns(new DateTime(2023, 06, 01)).MustBeCalled();
            var repository = 
                new CustomerRepository5(clock, log);

            Assert.ThrowsException<ArgumentException>(
                () => repository.Get(Guid.NewGuid())
            );

            Mock.Assert(clock);
            Mock.Assert(() => clock.Now, Occurs.Exactly(1));

            Mock.Assert(log);
            Mock.Assert(()=>log.Error(new DateTime(2023, 06, 01), "no such customer"), Occurs.Exactly(1));
        }

        [TestMethod]
        public void RepLogsTimeOnInvGet()
        {
            var clock = Mock.Create<IClock>(Behavior.Strict);
            var log = Mock.Create<ILog>(Behavior.Strict);
            Mock.Arrange(() => log.Error(new DateTime(2023, 06, 01), "no such customer")).MustBeCalled();
            Mock.Arrange(() => clock.Now).Returns(new DateTime(2023, 06, 01)).MustBeCalled();

            var referencetime = clock.Now;

            var repository =
                new CustomerRepository5(clock, log);
            Assert.ThrowsException<ArgumentException>(
                () => repository.Get(Guid.NewGuid())
            );

            Mock.Assert(clock);
            Mock.Assert(() => clock.Now, Occurs.Exactly(2));

            Mock.Assert(log);
            Mock.Assert(() => log.Error(referencetime, "no such customer"), Occurs.Exactly(1));

        }

        [TestMethod]
        public void RepLogsOnMultibleInvGet()
        {
            var clock = Mock.Create<IClock>(Behavior.Strict);
            var log = Mock.Create<ILog>(Behavior.Strict);
            Mock.Arrange(() => clock.Now).Returns(new DateTime(2023, 06, 01)).MustBeCalled();
            Mock.Arrange(() => log.Error(new DateTime(2023, 06, 01), "no such customer")).MustBeCalled();
            Mock.Arrange(() => log.Error(new DateTime(2023, 06, 02), "no such customer")).MustBeCalled();

            ICustomerRepository repository = new CustomerRepository5(clock, log);
            // repository.Get(Guid.NewGuid());
            Assert.ThrowsException<ArgumentException>(
                () => repository.Get(Guid.NewGuid())
            );
            Mock.Arrange(() => clock.Now).Returns(new DateTime(2023, 06, 02)).MustBeCalled();
            Assert.ThrowsException<ArgumentException>(
                () => repository.Get(Guid.NewGuid())
            );

            Mock.Assert(clock);
            Mock.Assert(() => clock.Now, Occurs.Exactly(2));

            Mock.Assert(log);
            Mock.Assert(() => log.Error(new DateTime(2023, 06, 01), "no such customer"), Occurs.Exactly(1));
            Mock.Assert(() => log.Error(new DateTime(2023, 06, 02), "no such customer"), Occurs.Exactly(1));
        }
    }
}
