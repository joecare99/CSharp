using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ollama.Tools;

namespace Ollama.CodingAgent.Tests;

[TestClass]
public sealed class FileToolCoverageTests
{
    [TestMethod]
    public async Task WorkspaceTools_ValidateAndExecuteBoundaryCases()
    {
        using TestWorkspace workspace = new();
        Directory.CreateDirectory(workspace.GetPath("nested"));
        await File.WriteAllLinesAsync(workspace.GetPath("nested", "sample.txt"), ["first", "second", "third"]);
        WorkspacePathPolicy policy = new(workspace.RootPath);

        ListWorkspaceFilesTool listTool = new(policy);
        Assert.IsTrue(listTool.Validate(string.Empty).IsValid);
        Assert.IsFalse(listTool.Validate("""{"maxFiles":0}""").IsValid);
        Assert.IsFalse(listTool.Validate("""{"relativePath":"..\outside"}""").IsValid);
        OllamaToolResult listed = await listTool.ExecuteAsync("""{"relativePath":"nested","maxFiles":1}""");
        Assert.IsTrue(listed.Success);
        StringAssert.Contains(listed.Output, "sample.txt");
        Assert.IsFalse((await listTool.ExecuteAsync("""{"relativePath":"missing"}""")).Success);
        Assert.AreEqual("list_workspace_files", listTool.Name);
        Assert.IsTrue(listTool.Schema.Parameters.Count > 0);

        ReadWorkspaceFileTool readTool = new(policy);
        Assert.IsFalse(readTool.Validate(string.Empty).IsValid);
        Assert.IsFalse(readTool.Validate("""{"relativePath":"sample.txt","startLine":0}""").IsValid);
        Assert.IsFalse(readTool.Validate("""{"relativePath":"sample.txt","lineCount":401}""").IsValid);
        Assert.IsTrue(readTool.Validate("""{"relativePath":"nested/sample.txt","startLine":1,"lineCount":2}""").IsValid);
        OllamaToolResult selection = await readTool.ExecuteAsync("""{"relativePath":"nested/sample.txt","startLine":2,"lineCount":1}""");
        Assert.AreEqual("2: second", selection.Output);
        Assert.AreEqual(string.Empty, (await readTool.ExecuteAsync("""{"relativePath":"nested/sample.txt","startLine":8}""")).Output);
        Assert.IsFalse((await readTool.ExecuteAsync("""{"relativePath":"missing.txt"}""")).Success);
        Assert.AreEqual("read_workspace_file", readTool.Name);
        Assert.IsTrue(readTool.Schema.Parameters.Count > 0);

        WriteWorkspaceFileTool writeTool = new(policy);
        Assert.IsFalse(writeTool.Validate(string.Empty).IsValid);
        Assert.IsFalse(writeTool.Validate("""{"relativePath":"bad.exe","content":"x"}""").IsValid);
        Assert.IsFalse(writeTool.Validate($$"""{"relativePath":"large.txt","content":"{{new string('x', 200_001)}}"}""").IsValid);
        OllamaToolResult write = await writeTool.ExecuteAsync("""{"relativePath":"nested/sample.txt","content":"replacement","overwrite":true}""");
        Assert.IsTrue(write.Success);
        Assert.IsFalse((await writeTool.ExecuteAsync("""{"relativePath":"nested/sample.txt","content":"another","overwrite":false}""")).Success);
        Assert.IsTrue(writeTool.Validate("""{"relativePath":"new.md","content":"text"}""").IsValid);
        Assert.AreEqual("write_workspace_file", writeTool.Name);
        Assert.IsTrue(writeTool.Schema.Parameters.Count > 0);
    }

    [TestMethod]
    public async Task DotnetTools_ValidateTargetsAndRunBoundedProject()
    {
        using TestWorkspace workspace = new();
        string projectFile = workspace.GetPath("TestProject.csproj");
        await File.WriteAllTextAsync(projectFile, "<Project Sdk=\"Microsoft.NET.Sdk\" />");
        WorkspacePathPolicy policy = new(workspace.RootPath);
        RunDotnetBuildTool buildTool = new(policy);
        RunDotnetTestTool testTool = new(policy);

        Assert.IsFalse(buildTool.Validate(string.Empty).IsValid);
        Assert.IsFalse(buildTool.Validate("""{"relativePath":"missing.csproj"}""").IsValid);
        Assert.IsTrue(buildTool.Validate("""{"relativePath":"TestProject.csproj"}""").IsValid);
        Assert.IsFalse(testTool.Validate(string.Empty).IsValid);
        Assert.IsFalse(testTool.Validate("""{"relativePath":"test.txt"}""").IsValid);
        Assert.IsTrue(testTool.Validate("""{"relativePath":"TestProject.csproj"}""").IsValid);

        OllamaToolResult buildMissing = await buildTool.ExecuteAsync("""{"relativePath":"missing.csproj"}""");
        OllamaToolResult testMissing = await testTool.ExecuteAsync("""{"relativePath":"missing.csproj"}""");
        Assert.IsFalse(buildMissing.Success);
        Assert.IsFalse(testMissing.Success);
        Assert.AreEqual("run_dotnet_build", buildTool.Name);
        Assert.AreEqual("run_dotnet_test", testTool.Name);
        Assert.IsTrue(buildTool.Schema.Parameters.Count > 0);
        Assert.IsTrue(testTool.Schema.Parameters.Count > 0);

        MethodInfo buildCombine = typeof(RunDotnetBuildTool).GetMethod("CombineOutput", BindingFlags.NonPublic | BindingFlags.Static)!;
        MethodInfo testCombine = typeof(RunDotnetTestTool).GetMethod("CombineOutput", BindingFlags.NonPublic | BindingFlags.Static)!;
        Assert.AreEqual("output", (string)buildCombine.Invoke(null, [" output ", string.Empty])!);
        Assert.AreEqual("error", (string)testCombine.Invoke(null, [string.Empty, " error "])!);
    }
}
