/*
--------------------------------------------------------------------------------
 AvaloniaTestMethodAttribute – MSTest-Attribut für Headless-Avalonia-Tests

 Zweck
 - Führt Testmethoden im Avalonia-Dispatcher aus, sodass await-Fortsetzungen im
   Test-Hauptthread erfolgen.

 Ablauf
 1) Ermittele den AppBuilder-Einstiegspunkt über das Assembly-Attribut
    'AvaloniaTestApplicationAttribute' (falls vorhanden).
 2) Fallback auf 'Application', wenn kein Einstiegspunkt angegeben ist.
 3) Starte eine 'HeadlessUnitTestSession' mit dem ermittelten Einstiegspunkt.
 4) Dispatch die Ausführung der Testmethode auf den Avalonia-Dispatcher.
 5) 'ExecuteTestMethod' ruft 'InvokeAsync' auf dem 'ITestMethod' auf und gibt
    das Ergebnis als Array zurück.

 Hinweise
 - Geeignet für Tests, die den Avalonia-Dispatcher benötigen.
 - C# 14, Ziel: .NET 8 / .NET 9

--------------------------------------------------------------------------------
*/
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Avalonia.Headless.MSTest;

/// <summary>
///   <para>
/// This Attribute identifies a TestMethod that starts on Avalonia Dispatcher such that awaited expressions resume in the test's main thread</para>
///   <para>Class AvaloniaTestMethodAttribute.<br />This class cannot be inherited.
/// <br /> Implements the <see cref="TestMethodAttribute" /></para>
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
public sealed class AvaloniaTestMethodAttribute([CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1) 
    : TestMethodAttribute(callerFilePath, callerLineNumber)
{
    /// <summary>
    /// Executes the provided MSTest method on the Avalonia dispatcher within a headless UI session,
    /// ensuring that awaited continuations inside the test run on the test's main (dispatcher) thread.
    /// </summary>
    /// <param name="testMethod">
    /// The MSTest <see cref="ITestMethod"/> descriptor representing the test method to execute.
    /// </param>
    /// <returns>
    /// A single-element array containing the <see cref="Microsoft.VisualStudio.TestTools.UnitTesting.TestResult"/>
    /// produced by the invoked test method. This shape matches MSTest's contract for custom attributes.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Purpose: This override bridges MSTest's asynchronous execution with Avalonia's dispatcher model so that
    /// any async/await operations inside the test continue on the Avalonia UI dispatcher thread. This is essential
    /// for tests that interact with Avalonia controls, application state, or rely on dispatcher-affine APIs.
    /// </para>
    /// <para>
    /// Execution flow:
    /// </para>
    /// <list type="number">
    ///   <item>
    ///     <description>
    ///       Determine the Avalonia AppBuilder entry point by reading the optional assembly-level
    ///       <see cref="AvaloniaTestApplicationAttribute"/>. If the attribute is not present, fall back to
    ///       <see cref="Application"/> which provides a reasonable default for headless tests.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       Create a new <see cref="HeadlessUnitTestSession"/> for the resolved entry point. This sets up the
    ///       Avalonia headless environment and initializes the dispatcher loop needed by UI-bound code.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       Dispatch the actual test invocation to the Avalonia dispatcher via <c>_session.Dispatch</c>, calling
    ///       the local helper that invokes <see cref="ITestMethod.InvokeAsync(object)"/>. This guarantees that the
    ///       test (and its asynchronous continuations) run on the dispatcher thread.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       Synchronously unwrap the dispatched Task using <c>GetAwaiter().GetResult()</c> while still returning
    ///       a <see cref="System.Threading.Tasks.Task{TResult}"/> per MSTest's async contract. Using the synchronous
    ///       unwrap here ensures the dispatcher context remains valid for the duration of the test execution,
    ///       and any exceptions thrown by the test are propagated without being wrapped in an <see cref="AggregateException"/>.
    ///     </description>
    ///   </item>
    /// </list>
    /// <para>
    /// Threading and synchronization:
    /// </para>
    /// <list type="bullet">
    ///   <item><description>All awaited continuations resume on the Avalonia dispatcher thread.</description></item>
    ///   <item><description>
    ///     The session lifetime is scoped to the method via <c>using</c>, ensuring proper teardown of the headless
    ///     environment even if the test fails.
    ///   </description></item>
    /// </list>
    /// <para>
    /// Exceptions:
    /// Any exception thrown by the test method propagates through the dispatcher invocation and is surfaced
    /// to MSTest as a normal test failure. The synchronous unwrap avoids <see cref="AggregateException"/> nesting,
    /// preserving original stack traces.
    /// </para>
    /// <para>
    /// Cancellation:
    /// No external <see cref="System.Threading.CancellationToken"/> is exposed; <c>default</c> is passed to the
    /// dispatcher. If cancellation is needed in the future, this can be extended to accept a token.
    /// </para>
    /// <para>
    /// See also:
    /// <see cref="AvaloniaTestApplicationAttribute"/>,
    /// <see cref="HeadlessUnitTestSession"/>,
    /// <see cref="Application"/>,
    /// <see cref="ITestMethod"/>.
    /// </para>
    public override async Task<TestResult[]> ExecuteAsync(ITestMethod testMethod)
    {
        // Resolve the assembly that contains the test method.
        var assembly = testMethod.MethodInfo.DeclaringType!.Assembly;

        // Try to obtain the AppBuilder entry point from the optional assembly-level attribute.
        var appBuilderEntryPointType = assembly.GetCustomAttribute<AvaloniaTestApplicationAttribute>()
            ?.AppBuilderEntryPointType;

        // Fallback to the default Application if no explicit entry point was provided.
        appBuilderEntryPointType ??= typeof(Application);

        // Start a headless Avalonia session and ensure it is disposed after the test finishes.
        using var _session = HeadlessUnitTestSession.StartNew(appBuilderEntryPointType);
        {
            // Dispatch the test execution to the Avalonia dispatcher to guarantee dispatcher-affine awaits.
            // Synchronously unwrap to preserve exception semantics and ensure execution remains on the dispatcher.
            return _session.Dispatch(() => ExecuteTestMethod(testMethod!), default).GetAwaiter().GetResult();
        }
    }

    /// <summary>Executes the test method.</summary>
    /// <param name="testMethod">The test method.</param>
    /// <returns>TestResult[].</returns>
    private static async Task<TestResult[]> ExecuteTestMethod(ITestMethod testMethod)
    {
        return [await testMethod.InvokeAsync(null).ConfigureAwait(false)];
    }
}