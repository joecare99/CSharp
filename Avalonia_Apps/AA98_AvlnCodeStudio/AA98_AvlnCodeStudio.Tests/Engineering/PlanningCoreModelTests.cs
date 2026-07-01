using AppKomponentBaseLib.Diagnostics;
using AA98_AvlnCodeStudio.Planning.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

/// <summary>
/// Verifies default values for the extracted planning core models.
/// </summary>
[TestClass]
public class PlanningCoreModelTests
{
    /// <summary>
    /// Verifies that the extracted planning core item starts with neutral defaults.
    /// </summary>
    [TestMethod]
    public void PlanningCoreItem_UsesExpectedDefaults()
    {
        PlanningItem item = new();

        Assert.AreEqual(string.Empty, item.Id);
        Assert.AreEqual(string.Empty, item.Title);
        Assert.AreEqual(PlanningItemKind.Unknown, item.Kind);
        Assert.AreEqual(PlanningItemStatus.Unknown, item.Status);
        Assert.AreEqual(string.Empty, item.SourcePath);
        Assert.IsNull(item.Parent);
        Assert.AreEqual(0, item.RelatedParents.Count);
        Assert.AreEqual(0, item.Children.Count);
        Assert.AreEqual(0, item.Diagnostics.Count);
    }

    /// <summary>
    /// Verifies that planning results use the shared diagnostic model.
    /// </summary>
    [TestMethod]
    public void PlanningCoreResults_UseSharedDiagnostics()
    {
        PlanningReadResult readResult = new();
        PlanningWriteResult writeResult = new();
        Diagnostic diagnostic = new();

        readResult.Diagnostics.Add(diagnostic);
        writeResult.Diagnostics.Add(diagnostic);

        Assert.AreSame(diagnostic, readResult.Diagnostics[0]);
        Assert.AreSame(diagnostic, writeResult.Diagnostics[0]);
    }
}
