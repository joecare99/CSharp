using Microsoft.VisualStudio.TestTools.UnitTesting;
using Avalonia;
using Avalonia.Headless;
using Avln.ComplexLayout.Views;

namespace Avln_Complex_LayoutTests.Views;

[TestClass]
public class ComplexLayoutViewTests
{
 [AssemblyInitialize]
 public static void Init(TestContext _)
 {
 AppBuilder.Configure<Avln.ComplexLayout.App>()
 .UseHeadless(new AvaloniaHeadlessPlatformOptions())
 .SetupWithoutStarting();
 }

 [TestMethod]
 public void Can_Create_View()
 {
 var view = new ComplexLayoutView();
 Assert.IsNotNull(view);
 }
}
