using System.Diagnostics;
using System.Text;

namespace RepoMigrator.Tools.PatchPreparation.Tests;

[TestClass]
[DoNotParallelize]
public sealed class ConvertPatchesScriptTests
{
    private const string ScriptPath = @"C:\Projekte\Cmd\convert_patches.ps1";

    [TestMethod]
    public void ConvertPatches_WhenSmallPatchContainsSvnMetadata_CreatesSingleGitPatchWithCleanedOutput()
    {
        var workingDirectoryPath = CreateTempDirectory();
        try
        {
            var patchPath = Path.Combine(workingDirectoryPath, "rev_12.patch");
            File.WriteAllText(patchPath, CreateSmallPatchFixture(), Encoding.UTF8);

            var result = RunConvertPatches(workingDirectoryPath, splitThresholdBytes: 1024);

            Assert.AreEqual(0, result.ExitCode, result.Output);

            var outputPath = Path.Combine(workingDirectoryPath, "rev_00012.gitpatch");
            Assert.IsTrue(File.Exists(outputPath), "Expected single converted gitpatch output was not created.");

            var output = File.ReadAllText(outputPath);
            StringAssert.Contains(output, "diff --git a/src/newfile.txt b/src/newfile.txt");
            StringAssert.Contains(output, "--- /dev/null");
            StringAssert.Contains(output, "+++ b/src/newfile.txt");
            StringAssert.Contains(output, "new file mode 100644");
            StringAssert.Contains(output, "index 0000000000000000000000000000000000000000..0000000000000000000000000000000000000000");
            StringAssert.Contains(output, "--- a/src/existing.txt");
            StringAssert.Contains(output, "+++ b/src/existing.txt");
            Assert.IsFalse(output.Contains("Index: ", StringComparison.Ordinal));
            Assert.IsFalse(output.Contains("======", StringComparison.Ordinal));
            Assert.IsFalse(output.Contains("(revision 12)", StringComparison.Ordinal));
            Assert.IsFalse(output.Contains("(nonexistent)", StringComparison.Ordinal));
        }
        finally
        {
            DeleteDirectoryIfExists(workingDirectoryPath);
        }
    }

    [TestMethod]
    public void ConvertPatches_WhenPatchContainsSvnIgnoreProperty_EmitsGitIgnoreDiff()
    {
        var workingDirectoryPath = CreateTempDirectory();
        try
        {
            var patchPath = Path.Combine(workingDirectoryPath, "property.patch");
            File.WriteAllText(patchPath, CreateSvnIgnoreFixture(), Encoding.UTF8);

            var result = RunConvertPatches(workingDirectoryPath, splitThresholdBytes: 1024);

            Assert.AreEqual(0, result.ExitCode, result.Output);

            var outputPath = Path.Combine(workingDirectoryPath, "property.gitpatch");
            Assert.IsTrue(File.Exists(outputPath), "Expected converted gitpatch output was not created.");

            var output = File.ReadAllText(outputPath);
            StringAssert.Contains(output, "diff --git a/src/.gitignore b/src/.gitignore");
            StringAssert.Contains(output, "+++ b/src/.gitignore");
            StringAssert.Contains(output, "@@ -0,0 +1,2 @@");
            StringAssert.Contains(output, "+bin/");
            StringAssert.Contains(output, "+obj/");
        }
        finally
        {
            DeleteDirectoryIfExists(workingDirectoryPath);
        }
    }

    [TestMethod]
    public void ConvertPatches_WhenSplitThresholdIsLow_CreatesPartFilesWithPaddedNames()
    {
        var workingDirectoryPath = CreateTempDirectory();
        try
        {
            var patchPath = Path.Combine(workingDirectoryPath, "rev_7.patch");
            File.WriteAllText(patchPath, CreateSplitFixture(), Encoding.UTF8);

            var result = RunConvertPatches(workingDirectoryPath, splitThresholdBytes: 200);

            Assert.AreEqual(0, result.ExitCode, result.Output);

            var partOnePath = Path.Combine(workingDirectoryPath, "rev_00007.part_01.gitpatch");
            var partTwoPath = Path.Combine(workingDirectoryPath, "rev_00007.part_02.gitpatch");
            Assert.IsTrue(File.Exists(partOnePath), "Expected first split gitpatch output was not created.");
            Assert.IsTrue(File.Exists(partTwoPath), "Expected second split gitpatch output was not created.");

            var partOne = File.ReadAllText(partOnePath);
            var partTwo = File.ReadAllText(partTwoPath);
            StringAssert.Contains(partOne, "Subject: [SVN r7]");
            StringAssert.Contains(partTwo, "Subject: [SVN r7]");
            StringAssert.Contains(partOne, "diff --git a/src/one.txt b/src/one.txt");
            StringAssert.Contains(partTwo, "diff --git a/src/two.txt b/src/two.txt");
        }
        finally
        {
            DeleteDirectoryIfExists(workingDirectoryPath);
        }
    }

    private static (int ExitCode, string Output) RunConvertPatches(string inputDirectoryPath, int splitThresholdBytes)
    {
        Assert.IsTrue(File.Exists(ScriptPath), $"Expected script '{ScriptPath}' to exist.");

        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-NoProfile -ExecutionPolicy Bypass -File \"{ScriptPath}\" -InputPath \"{inputDirectoryPath}\" -SplitThresholdBytes {splitThresholdBytes}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
                WorkingDirectory = inputDirectoryPath
            }
        };

        process.Start();
        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();
        process.WaitForExit();
        return (process.ExitCode, output + error);
    }

    private static string CreateTempDirectory()
    {
        var directoryPath = Path.Combine(Path.GetTempPath(), $"RepoMigrator-ConvertPatches-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directoryPath);
        return directoryPath;
    }

    private static void DeleteDirectoryIfExists(string directoryPath)
    {
        if (Directory.Exists(directoryPath))
            Directory.Delete(directoryPath, recursive: true);
    }

    private static string CreateSmallPatchFixture()
        => """
From: Alice Example <alice@example.invalid>
Date: 2024-01-01 12:00:00 +0000
Subject: [SVN r12]
Small patch
---
Index: src/newfile.txt
===================================================================
diff --git a/src/newfile.txt b/src/newfile.txt
new file mode 100644
--- a/src/newfile.txt (nonexistent)
+++ b/src/newfile.txt (revision 12)
@@ -0,0 +1,1 @@
+Hello
Index: src/existing.txt
===================================================================
diff --git a/src/existing.txt b/src/existing.txt
--- a/src/existing.txt (revision 11)
+++ b/src/existing.txt (revision 12)
@@ -1 +1 @@
-old
+new
""";

    private static string CreateSvnIgnoreFixture()
        => """
From: Alice Example <alice@example.invalid>
Date: 2024-01-01 12:00:00 +0000
Subject: [SVN r13]
Ignore patch
---
diff --git a/src/readme.txt b/src/readme.txt
--- a/src/readme.txt (revision 12)
+++ b/src/readme.txt (revision 13)
@@ -1 +1 @@
-old
+new
Property changes on: src
___________________________________________________________________
Added: svn:ignore
## -0,0 +1,2 ##
+bin/
+obj/
diff --git a/src/after-ignore.txt b/src/after-ignore.txt
--- a/src/after-ignore.txt (revision 12)
+++ b/src/after-ignore.txt (revision 13)
@@ -1 +1 @@
-keep
+keep-updated
""";

    private static string CreateSplitFixture()
        => """
From: Alice Example <alice@example.invalid>
Date: 2024-01-01 12:00:00 +0000
Subject: [SVN r7]
Split patch
---
diff --git a/src/one.txt b/src/one.txt
--- a/src/one.txt (revision 6)
+++ b/src/one.txt (revision 7)
@@ -1 +1,3 @@
-a
+aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa
+bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb
+cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc
diff --git a/src/two.txt b/src/two.txt
--- a/src/two.txt (revision 6)
+++ b/src/two.txt (revision 7)
@@ -1 +1,3 @@
-d
+dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd
+eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
+ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff
""";
}
