using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Osb.Core.Tenancy;

namespace Osb.Core.Tests.Tenancy;

[TestClass]
public sealed class TenantPathConfigurationTests
{
    [TestMethod]
    public void FromCommandLine_AcceptsNamedTenantPath()
    {
        var configuration = TenantPathConfiguration.FromCommandLine(
            new[] { "--tenant", @"C:\Data\TenantA" });

        Assert.AreEqual(@"C:\Data\TenantA\", configuration.TenantRootPath);
    }

    [TestMethod]
    public void FromCommandLine_AcceptsSingleAbsoluteTenantPath()
    {
        var configuration = TenantPathConfiguration.FromCommandLine(
            new[] { @"C:\Data\TenantA\" });

        Assert.AreEqual(@"C:\Data\TenantA\", configuration.TenantRootPath);
    }

    [TestMethod]
    public void FromCommandLine_AcceptsInlineNamedTenantPath()
    {
        var configuration = TenantPathConfiguration.FromCommandLine(
            new[] { "--tenant=C:\\Data\\TenantA" });

        Assert.AreEqual(@"C:\Data\TenantA\", configuration.TenantRootPath);
    }

    [TestMethod]
    public void FromCommandLine_RejectsMissingOrUnexpectedArguments()
    {
        Assert.ThrowsExactly<ArgumentException>(() => TenantPathConfiguration.FromCommandLine(Array.Empty<string>()));
        Assert.ThrowsExactly<ArgumentException>(() => TenantPathConfiguration.FromCommandLine(new[] { "--tenant" }));
        Assert.ThrowsExactly<ArgumentException>(() => TenantPathConfiguration.FromCommandLine(new[] { "--other", @"C:\Data\TenantA" }));
    }

    [TestMethod]
    public void FromAbsolutePath_RejectsRelativePath()
    {
        Assert.ThrowsExactly<ArgumentException>(() => TenantPathConfiguration.FromAbsolutePath(@"TenantA"));
    }
}
