using AutoFixture;
using AutoFixture.AutoMoq;
using CustomerRepository.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CustomerRepositoryTests.Model
{
    [TestClass]
    public class Mocking_AutoFix_AutoMoq
    {
        [TestMethod]
        public void RepThrowsExOnInvGet()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            ICustomerRepository repository =
                fixture.Create<CustomerRepository5>();

            Assert.ThrowsException<ArgumentException>(
                () => repository.Get(Guid.NewGuid())
            );
        }
    }
}
