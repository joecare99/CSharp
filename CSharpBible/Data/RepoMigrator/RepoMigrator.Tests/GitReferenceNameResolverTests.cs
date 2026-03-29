using RepoMigrator.Core;

namespace RepoMigrator.Tests;

[TestClass]
public sealed class GitReferenceNameResolverTests
{
    [DataTestMethod]
    [DataRow("work", "", "work")]
    [DataRow("work", "work", "work-import")]
    [DataRow("work", "work|work-import", "work-import-20250326")]
    [DataRow("work", "work|work-import|work-import-20250326", "work-import-20250326-2")]
    public void ResolveAvailableName_ReturnsExpectedValue(string sBaseName, string sExistingNames, string sExpectedName)
    {
        var lstExistingNames = string.IsNullOrWhiteSpace(sExistingNames)
            ? Array.Empty<string>()
            : sExistingNames.Split('|', StringSplitOptions.RemoveEmptyEntries);

        var sResolvedName = GitReferenceNameResolver.ResolveAvailableName(sBaseName, lstExistingNames, new DateOnly(2025, 3, 26));

        Assert.AreEqual(sExpectedName, sResolvedName);
    }
}
