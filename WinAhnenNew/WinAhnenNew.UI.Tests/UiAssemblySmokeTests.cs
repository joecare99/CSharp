using System.Windows.Controls;
using WinAhnenNew.Controls;
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

        [TestMethod]
        public void DetailPageView_IsAvailableAsWpfPage()
        {
            Assert.IsTrue(typeof(Page).IsAssignableFrom(typeof(DetailPageView)));
        }

        [TestMethod]
        public void RelationshipsPageView_IsAvailableAsWpfPage()
        {
            Assert.IsTrue(typeof(Page).IsAssignableFrom(typeof(RelationshipsPageView)));
        }

        [TestMethod]
        public void PersonHeaderView_IsAvailableAsWpfControl()
        {
            Assert.IsTrue(typeof(UserControl).IsAssignableFrom(typeof(PersonHeaderView)));
        }
    }
}
