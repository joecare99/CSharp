using System.Windows.Controls;
using WinAhnenNew.Views;

namespace WinAhnenNew.UI.Tests
{
    [TestClass]
    public class UiAssemblySmokeTests
    {
        [TestMethod]
        public void EditPageView_IsAvailableAsWpfPage()
        {
            Assert.IsTrue(typeof(Page).IsAssignableFrom(typeof(EditPageView)));
        }
    }
}
