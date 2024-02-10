using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;
using MVVM.ViewModel;
using MVVM_BaseLibTests.ViewModel;
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
		protected override Dictionary<string, object?> GetDefaultData()
		{
			return new()
			{
				{ nameof(UserControlViewModel.Text1), "Hello World" },
				{ nameof(UserControlViewModel.Text2), "Hello World2" },
				{ nameof(UserControlViewModel.State1), true },
				{ nameof(UserControlViewModel.State2), false },
			};
		}

	}
}
