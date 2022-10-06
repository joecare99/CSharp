using MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM_TiledDisplay.ViewModel
{
    /// <summary>
    /// Class MainViewViewModel.
    /// Implements the <see cref="BaseViewModel" />
    /// </summary>
    /// <seealso cref="BaseViewModel" />
    public class MainViewViewModel : BaseViewModel
    {
        #region Properties
        public DelegateCommand<object> cmdButton { get; set; }
        public event EventHandler<string> SetFrame = default;
        #endregion

        #region Methods
        public MainViewViewModel()
        {
            cmdButton = new DelegateCommand<object>(DoCmdButton, CanDoCmdButton);
        }

        private bool CanDoCmdButton(object obj)
        {
            return true;
        }

        private void DoCmdButton(object obj)
        {
            if (obj is string s)
                SetFrame?.Invoke(this, s);
        }
        #endregion  
    }
}
