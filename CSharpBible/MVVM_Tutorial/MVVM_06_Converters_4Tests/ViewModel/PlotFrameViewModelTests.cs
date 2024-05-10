using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM.ViewModel;
using System.Collections.Generic;

namespace MVVM_06_Converters_4.ViewModel.Tests
{
    [TestClass()]
    public class PlotFrameViewModelTests: BaseTestViewModel<PlotFrameViewModel>
    {
        [TestInitialize]
        public override void Init()
        {
            base.Init();
        }

        [TestMethod()]
        public void PlotFrameViewModelTest()
        {
            Assert.Fail();
        }

        protected override Dictionary<string, object?> GetDefaultData() 
            => base.GetDefaultData();
    }
}