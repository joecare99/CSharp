using Castle.MicroKernel.Registration;
using Castle.Windsor;
using CustomerRepository.Model;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CustomerRepositoryTests.Model
{
    [TestClass]
    public class IoC_CastleWindsor
    {
        [TestMethod]
        public void RepThrowsExOnInvGet()
        {
            var container = new WindsorContainer();
            container.Register(Component.For<IClock>()
                .ImplementedBy<CStaticClock>());
            container.Register(Component.For<ILog>()
                .ImplementedBy<CLog>().LifestyleSingleton());
            container.Register(Component.For<ICustomerRepository>()
                .ImplementedBy<CustomerRepository5>().LifestyleTransient());

            var log = container.Resolve<ILog>();
                
            ICustomerRepository repository =
                container.Resolve<ICustomerRepository>();

            Assert.ThrowsException<ArgumentException>(
                () => repository.Get(Guid.NewGuid())
            );

            log.Get().Should().HaveCount( 1 );
        }
    }
}
