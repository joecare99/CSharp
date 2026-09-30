using IDR.App;
using IDR.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IDR.App.Tests;

[TestClass]
public sealed class CommandLineHostTests
{
    [TestMethod]
    public void TryParseDelphiVersionAcceptsSupportedNames()
    {
        (string Text, DelphiVersion Expected)[] cases =
        [
            ("7", DelphiVersion.Delphi7),
            ("Delphi2007", DelphiVersion.Delphi2007),
            ("2010", DelphiVersion.Delphi2010),
            ("XE2", DelphiVersion.DelphiXE2),
            ("DelphiXE4", DelphiVersion.DelphiXE4)
        ];
        foreach ((string text, DelphiVersion expected) in cases)
        {
            Assert.IsTrue(CommandLineHost.TryParseDelphiVersion(text, out DelphiVersion actual));
            Assert.AreEqual(expected, actual);
        }
    }

    [TestMethod]
    public void TryParseDelphiVersionRejectsUnsupportedNames()
    {
        foreach (string text in new[] { string.Empty, "5.5", "XE5", "unknown" })
        {
            Assert.IsFalse(CommandLineHost.TryParseDelphiVersion(text, out DelphiVersion actual));
            Assert.AreEqual(DelphiVersion.Unknown, actual);
        }
    }

    [TestMethod]
    public void AnalyzeCommandRequiresInputAndVersion()
    {
        using var services = App.CreateServices();
        CommandLineHost host = new(
            services.GetRequiredService<IPeImageLoader>(),
            services.GetRequiredService<IDelphiVersionDetector>(),
            services.GetRequiredService<IKnowledgeBaseProvider>(),
            services.GetRequiredService<IInstructionDecoder>(),
            services.GetRequiredService<IAnalysisService>());

        var parseResult = host.CreateRootCommand().Parse(["analyze"]);

        Assert.IsTrue(parseResult.Errors.Count >= 2);
    }

    [TestMethod]
    public void AnalyzeCommandAcceptsRequiredOptionsAndSummarySwitch()
    {
        using var services = App.CreateServices();
        CommandLineHost host = new(
            services.GetRequiredService<IPeImageLoader>(),
            services.GetRequiredService<IDelphiVersionDetector>(),
            services.GetRequiredService<IKnowledgeBaseProvider>(),
            services.GetRequiredService<IInstructionDecoder>(),
            services.GetRequiredService<IAnalysisService>());

        var parseResult = host.CreateRootCommand().Parse(
            ["analyze", "--input", "sample.exe", "--version", "7", "--summary-only"]);

        Assert.AreEqual(0, parseResult.Errors.Count);
    }
}
