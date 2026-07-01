using System.Collections.ObjectModel;
using WinAhnenNew.Collections;

namespace WinAhnenNew.Model.Tests
{
    [TestClass]
    public class ObservableCollectionExtensionsTests
    {
        [TestMethod]
        public void SynchronizeWith_PreservesEqualLeadingItem_AndAppliesDeltaChanges()
        {
            var objUnchangedItem = new CollectionItem("A");
            var objRemovedItem = new CollectionItem("B");
            var objReplacementItem = new CollectionItem("C");
            var objNewItem = new CollectionItem("D");
            var lstTarget = new ObservableCollection<CollectionItem>
            {
                objUnchangedItem,
                objRemovedItem
            };

            lstTarget.SynchronizeWith(new[]
            {
                objUnchangedItem,
                objReplacementItem,
                objNewItem
            });

            Assert.AreSame(objUnchangedItem, lstTarget[0]);
            Assert.AreSame(objReplacementItem, lstTarget[1]);
            Assert.AreSame(objNewItem, lstTarget[2]);
            Assert.AreEqual(3, lstTarget.Count);
        }

        [TestMethod]
        public void SynchronizeWith_EmptySource_ClearsTargetCollection()
        {
            var lstTarget = new ObservableCollection<string>
            {
                "A",
                "B"
            };

            lstTarget.SynchronizeWith(System.Array.Empty<string>());

            Assert.AreEqual(0, lstTarget.Count);
        }

        private sealed class CollectionItem(string sValue)
        {
            public string Value { get; } = sValue;
        }
    }
}
