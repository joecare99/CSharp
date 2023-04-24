using CustomerRepository.Model;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace CustomerRepositoryTests.Model
{
    [TestClass]
    public class MSDepInj
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        private ServiceProvider container { get; set; }
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

        [TestInitialize]
        public void Init()
        {
            var services = new ServiceCollection();
            services.AddTransient<IClock, CStaticClock>();
            services.AddSingleton<ILog, CLog>();
            services.AddTransient<ICustomerRepository, CustomerRepository5>();
            container = services.BuildServiceProvider();
        }

        [TestMethod]
        public void RepThrowsExOnInvGet()
        {
            var services = new ServiceCollection();
            services.AddTransient<IClock, CStaticClock>();
            services.AddSingleton<ILog , CLog>();
            services.AddTransient<ICustomerRepository, CustomerRepository5>();
            var container = services.BuildServiceProvider();

            var log = container.GetService<ILog>();
                
            ICustomerRepository? repository =
                container?.GetService<ICustomerRepository>();

            Assert.ThrowsException<ArgumentException>(
                () => repository?.Get(Guid.NewGuid())
            );

            log?.Get().Should().HaveCount( 1 );
        }

        [TestMethod]
        public void RepThrowsExOnInvGet2()
        {
            var log = container.GetService<ILog>();

            var repository =
                container.GetService<ICustomerRepository>();

            Assert.ThrowsException<ArgumentException>(
                () => repository?.Get(Guid.NewGuid())
            );

            log?.Get().Should().HaveCount(1);
        }

        [TestMethod]
        public void RepThrows2ExOn2InvGet2()
        {
            var log = container.GetService<ILog>();

            var repository =
                container.GetService<ICustomerRepository>();

            Assert.ThrowsException<ArgumentException>(
                () => repository?.Get(Guid.NewGuid())
            );
            Assert.ThrowsException<ArgumentException>(
                () => repository?.Get(Guid.NewGuid())
            );

            log?.Get().Should().HaveCount(2);
        }

        [TestMethod]
        public void LogsTimeOnInvLogGet()
        {
            var log = container.GetService<ILog>();
            var clock = container.GetService<IClock>();

            var repository =
                container.GetService<ICustomerRepository>();

            Assert.ThrowsException<ArgumentException>(
                () => repository?.Get(Guid.NewGuid())
            );

            log?.Get().First().Time
                 .Should().Be(clock?.Now);
        }
    }
}
