using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM.ViewModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MVVM_28_1_CTDataGridExt.ViewModels.Tests
{
    [TestClass]
    public class DataGridViewModelTests:BaseTestViewModel<DataGridViewModel>
    {
        [TestInitialize]
        public override void Init()
        {

            base.Init();
        }
        [TestMethod]
        public void SetupTest() {
            Assert.IsNotNull(testModel);
            Assert.IsInstanceOfType(testModel, typeof(DataGridViewModel));
        }

        protected override Dictionary<string, object?> GetDefaultData() 
            => new() { 
                { nameof(DataGridViewModel.IsItemSelected),false},
            };
    }
}
