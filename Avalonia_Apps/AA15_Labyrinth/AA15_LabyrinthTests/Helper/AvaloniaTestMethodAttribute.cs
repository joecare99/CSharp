using System.Reflection;

namespace Avalonia.Headless.MSTest;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
public sealed class AvaloniaTestMethodAttribute : TestMethodAttribute
{
 public override async Task<TestResult[]> ExecuteAsync(ITestMethod testMethod)
 {
 var assembly = testMethod.MethodInfo.DeclaringType!.Assembly;
 var appBuilderEntryPointType = assembly.GetCustomAttribute<AvaloniaTestApplicationAttribute>()
 ?.AppBuilderEntryPointType;

 appBuilderEntryPointType ??= typeof(Application);

 using var _session = HeadlessUnitTestSession.StartNew(appBuilderEntryPointType);
 {
 return _session.Dispatch(() => ExecuteTestMethod(testMethod!), default).GetAwaiter().GetResult();
 }
 }

 private static async Task<TestResult[]> ExecuteTestMethod(ITestMethod testMethod)
 {
 return [await testMethod.InvokeAsync(null).ConfigureAwait(false)];
 }
}
