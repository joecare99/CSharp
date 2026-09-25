using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Osb.ModernHost.Services;

namespace Osb.ModernHost.Tests;

[TestClass]
public sealed class ModernHostStartupTests
{
    [TestMethod]
    public void ExistingAbsoluteTenantPathMakesHostReady()
    {
        var tenantPath = CreateTenantDirectory();

        try
        {
            var state = new ModernHostStartupService(new[] { "--tenant", tenantPath }).CreateState();

            Assert.IsTrue(state.IsReady);
            Assert.IsNotNull(state.Tenant);
            Assert.IsNull(state.ErrorMessage);
            Assert.AreEqual(
                Path.GetFullPath(tenantPath).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar,
                state.Tenant!.TenantRootPath);
        }
        finally
        {
            Directory.Delete(tenantPath);
        }
    }

    [TestMethod]
    public void MissingTenantDirectoryProducesActionableDiagnostic()
    {
        var tenantPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        var state = new ModernHostStartupService(new[] { "--tenant", tenantPath }).CreateState();

        Assert.IsFalse(state.IsReady);
        Assert.IsNotNull(state.Tenant);
        StringAssert.Contains(state.ErrorMessage, "does not exist");
    }

    [TestMethod]
    public void InvalidArgumentsProduceUsageDiagnostic()
    {
        var state = new ModernHostStartupService(Array.Empty<string>()).CreateState();

        Assert.IsFalse(state.IsReady);
        Assert.IsNull(state.Tenant);
        StringAssert.Contains(state.ErrorMessage, "--tenant");
    }

    private static string CreateTenantDirectory()
    {
        var tenantPath = Path.Combine(Path.GetTempPath(), "OsbModernHostTests-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tenantPath);
        return tenantPath;
    }
}
