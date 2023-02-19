﻿using CustomerRepository.Model;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using System;
using System.Linq;

namespace CustomerRepositoryTests.Model
{
    [TestClass]
    public class Ninject
    {
        private StandardKernel kernel { get; set; }

        [TestInitialize]
        public void Init()
        {
            kernel = new StandardKernel();
            kernel.Bind<IClock>().To<CStaticClock>();
            kernel.Bind<ILog>().To<CLog>().InSingletonScope();
            kernel.Bind<ICustomerRepository>().To<CustomerRepository5>();
        }

        [TestMethod]
        public void RepThrowsExOnInvGet()
        {
            var kernel = new StandardKernel();
            kernel.Bind<IClock>().To<CStaticClock>();
            kernel.Bind<ILog>().To<CLog>().InSingletonScope();
            kernel.Bind<ICustomerRepository>().To<CustomerRepository5>();

            var log = kernel.Get<ILog>();
                
            ICustomerRepository repository =
                kernel.Get<ICustomerRepository>();

            Assert.ThrowsException<ArgumentException>(
                () => repository.Get(Guid.NewGuid())
            );

            log.Get().Should().HaveCount( 1 );
        }

        [TestMethod]
        public void RepThrowsExOnInvGet2()
        {
            var log = kernel.Get<ILog>();

            var repository =
                kernel.Get<ICustomerRepository>();

            Assert.ThrowsException<ArgumentException>(
                () => repository?.Get(Guid.NewGuid())
            );

            log?.Get().Should().HaveCount(1);
        }

        [TestMethod]
        public void RepThrows2ExOn2InvGet2()
        {
            var log = kernel.Get<ILog>();
            var repository =
                kernel.Get<ICustomerRepository>();

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
            var log = kernel.Get<ILog>();
            var clock = kernel.Get<IClock>();

            var repository =
                kernel.Get<ICustomerRepository>();

            Assert.ThrowsException<ArgumentException>(
                () => repository?.Get(Guid.NewGuid())
            );

            log.Get().First().Time
                 .Should().Be(clock.Now);
        }
    }
}
