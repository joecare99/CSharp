using IDR.App;
using IDR.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IDR.App.Tests;

[TestClass]
public sealed class AppCompositionTests
{
    [TestMethod]
    public void ServicesValidate()
    {
        using var services = App.CreateServices();
        Assert.IsNotNull(services);
        Assert.IsNotNull(services.GetRequiredService<IKnowledgeBaseProvider>());
    }
}
