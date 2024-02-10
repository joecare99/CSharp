using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;
using MVVM.ViewModel;
using BaseLib.Helper;
using System.Collections.Generic;

namespace MVVM_24b_UserControl.ViewModels.Tests
{
    [TestClass()]
    public class UserControlViewModelTests: BaseTestViewModel<UserControlViewModel>
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
		[DataRow(nameof(UserControlViewModel.Text1), "Hello World2_", new[] { @"PropChg(MVVM_24b_UserControl.ViewModels.UserControlViewModel,Text1)=Hello World2_
" })]
		[DataRow(nameof(UserControlViewModel.Text2), "Hello World2", new[] { @"" })]
		[DataRow(nameof(UserControlViewModel.Text2), "Hello World2_", new[] { @"PropChg(MVVM_24b_UserControl.ViewModels.UserControlViewModel,Text2)=Hello World2_
" })]
		[DataRow(nameof(UserControlViewModel.State1), true, new[] { @"" })]
		[DataRow(nameof(UserControlViewModel.State1), false, new[] { @"PropChg(MVVM_24b_UserControl.ViewModels.UserControlViewModel,State1)=False
" })]
		[DataRow(nameof(UserControlViewModel.State2), true, new[] { @"PropChg(MVVM_24b_UserControl.ViewModels.UserControlViewModel,State2)=True
" })]
		[DataRow(nameof(UserControlViewModel.State2), false, new[] { @"" })]
		public void PropTest(string sPropame, object? sValue, string[] asExp)
		{
			var data = GetDefaultData();
			var expected = data[sPropame];
			var actual = testModel.GetProp(sPropame);
			Assert.AreEqual(expected, actual);
			if (sValue is bool bValue)
			{
				testModel.SetProp(sPropame, (bool?)bValue);
			}
			else
			{
				testModel.SetProp(sPropame, sValue);
			}
			testModel.SetProp(sPropame, sValue);
			Assert.AreEqual(sValue, testModel.GetProp(sPropame));
			Assert.AreEqual(asExp[0], DebugLog);
		}

		protected override Dictionary<string, object?> GetDefaultData() => new()
			{
				{ nameof(UserControlViewModel.Text1), "Hello World" },
				{ nameof(UserControlViewModel.Text2), "Hello World2" },
				{ nameof(UserControlViewModel.State1), true },
				{ nameof(UserControlViewModel.State2), false },
			};

	}
}
