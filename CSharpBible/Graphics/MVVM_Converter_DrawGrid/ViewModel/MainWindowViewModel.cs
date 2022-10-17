using MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM_Converter_DrawGrid.ViewModel
{
    /// <summary>
    /// Class MainWindowViewModel.
    /// Implements the <see cref="BaseViewModel" />
    /// </summary>
    /// <seealso cref="BaseViewModel" />
    public class MainWindowViewModel:BaseViewModel
    {
        /// <summary>
        /// Gets or sets the show client.
        /// </summary>
        /// <value>The show client.</value>
        public Func<string, BaseViewModel> ShowClient { get; set; }

        /// <summary>
        /// Gets or sets the load level command.
        /// </summary>
        /// <value>The load level command.</value>
        public DelegateCommand LoadLevelCommand { get; set; } = new DelegateCommand((o) => Model.Model.LoadLevel());
        /// <summary>
        /// Gets or sets the next level command.
        /// </summary>
        /// <value>The next level command.</value>
        public DelegateCommand NextLevelCommand { get; set; } = new DelegateCommand((o) => Model.Model.NextLevel());
        /// <summary>
        /// Gets or sets the previous level command.
        /// </summary>
        /// <value>The previous level command.</value>
        public DelegateCommand PrevLevelCommand { get; set; } = new DelegateCommand((o) => Model.Model.PrevLevel());

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel()
        {

        }



    }
}
