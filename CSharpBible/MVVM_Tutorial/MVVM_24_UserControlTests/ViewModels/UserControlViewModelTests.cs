using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;
using MVVM.ViewModel;
using BaseLib.Helper;
using System.Collections.Generic;

namespace MVVM_24_UserControl.ViewModels.Tests
{
    [TestClass()]
    public class UserControlViewModelTests : BaseTestViewModel<UserControlViewModel>
    {
        [TestMethod()]
        public void SetupTest()
        {
            Assert.IsNotNull(testModel);
            Assert.IsNotNull(testModel2);
            Assert.IsInstanceOfType(testModel, typeof(UserControlViewModel));
            Assert.IsInstanceOfType(testModel, typeof(BaseViewModel));
            Assert.IsInstanceOfType(testModel, typeof(INotifyPropertyChanged));
        }

        [DataTestMethod()]
        [DataRow(nameof(UserControlViewModel.Text1), "Hello World", new[] { @"" })]
        [DataRow(nameof(UserControlViewModel.Text1), "Hello World2_", new[] { @"PropChg(MVVM_24_UserControl.ViewModels.UserControlViewModel,Text1)=Hello World2_
" })]
		[DataRow(nameof(UserControlViewModel.Text2), "Hello World 2", new[] { @"" })]
		[DataRow(nameof(UserControlViewModel.Text2), "Hello World2_", new[] { @"" })]
		public void PropTest(string sPropame, object? sValue, string[] asExp)
        {
			var data = GetDefaultData();
			var expected = data[sPropame];
			var actual = testModel.GetProp(sPropame);
			Assert.AreEqual(expected, actual);
            testModel.SetProp(sPropame, sValue);
			Assert.AreEqual(sValue, testModel.GetProp(sPropame));
            Assert.AreEqual(asExp[0], DebugLog);
		}

		protected override Dictionary<string, object?> GetDefaultData() => new()
			{
				{ nameof(UserControlViewModel.Text1), "Hello World" },
				{ nameof(UserControlViewModel.Text2), "Hello World 2" },
				//{ nameof(UserControlViewModel.State1), true },
				//{ nameof(UserControlViewModel.State2), false },
			};
	}
}
