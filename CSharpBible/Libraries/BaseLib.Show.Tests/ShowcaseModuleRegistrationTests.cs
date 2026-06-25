using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;

namespace BaseLib.Show.Tests;

/// <summary>
/// Verifies the registered showcase modules and their learning metadata.
/// </summary>
[TestClass]
public class ShowcaseModuleRegistrationTests
{
    /// <summary>
    /// Ensures that every registered module exposes examples with source code and localized titles.
    /// </summary>
    [TestMethod]
    public void AddBaseLibShowcase_RegistersModulesWithSourceCodeAndLocalizedTitles()
    {
        using ServiceProvider serviceProvider = new ServiceCollection()
            .AddBaseLibShowcase()
            .BuildServiceProvider();

        ShowcaseText text = serviceProvider.GetRequiredService<ShowcaseText>();
        IDemoModule[] modules = serviceProvider.GetServices<IDemoModule>().OrderBy(module => module.SelectionKey).ToArray();

        Assert.AreEqual(9, modules.Length);
        CollectionAssert.AllItemsAreUnique(modules.Select(module => (object)module.SelectionKey).ToArray());

        foreach (IDemoModule module in modules)
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(text.Get(module.TitleKey, CultureInfo.GetCultureInfo("en-US"))), module.TitleKey);
            Assert.IsFalse(string.IsNullOrWhiteSpace(text.Get(module.TitleKey, CultureInfo.GetCultureInfo("de-DE"))), module.TitleKey);
            Assert.IsFalse(string.IsNullOrWhiteSpace(text.Get(module.MenuDescriptionKey, CultureInfo.GetCultureInfo("en-US"))), module.MenuDescriptionKey);
            Assert.IsFalse(string.IsNullOrWhiteSpace(text.Get(module.MenuDescriptionKey, CultureInfo.GetCultureInfo("de-DE"))), module.MenuDescriptionKey);
            Assert.IsTrue(module.Examples.Count > 0, module.TitleKey);

            foreach (DemoExample example in module.Examples)
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(example.SourceCode), example.TitleKey);
                Assert.IsFalse(string.IsNullOrWhiteSpace(text.Get(example.TitleKey, CultureInfo.GetCultureInfo("en-US"))), example.TitleKey);
                Assert.IsFalse(string.IsNullOrWhiteSpace(text.Get(example.TitleKey, CultureInfo.GetCultureInfo("de-DE"))), example.TitleKey);
            }
        }
    }
}
