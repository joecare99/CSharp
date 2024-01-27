// ***********************************************************************
// Assembly         : MVVM_09_DialogBoxes
// Author           : Mir
// Created          : 07-21-2022
//
// Last Modified By : Mir
// Last Modified On : 08-09-2022
// ***********************************************************************
// <copyright file="MainWindowViewModel.cs" company="JC-Soft">
//     Copyright Â© JC-Soft 2021
// </copyright>
// <summary></summary>
// ***********************************************************************
using MVVM.ViewModel;
using System.Windows;

namespace MVVM_09_DialogBoxes.ViewModel
{
    /// <summary>
    /// Class MainWindowViewModel.
    /// Implements the <see cref="BaseViewModel" />
    /// </summary>
    /// <seealso cref="BaseViewModel" />
    public class MainWindowViewModel : BaseViewModel
    {
        /// <summary>
        /// Delegate OpenDialogHandler
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="email">The email.</param>
        /// <returns>System.ValueTuple&lt;System.String, System.String&gt;.</returns>
        public delegate (string name, string email) OpenDialogHandler(string name, string email);

        /// <summary>
        /// Delegate OpenMessageBoxHandler
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="name">The name.</param>
        /// <returns>MessageBoxResult.</returns>
        public delegate MessageBoxResult OpenMessageBoxHandler(string title, string name);
        /// <summary>
        /// Gets or sets the open dialog.
        /// </summary>
        /// <value>The open dialog.</value>
        public OpenDialogHandler? DoOpenDialog { get; set; }
        /// <summary>
        /// Gets or sets the open message box.
        /// </summary>
        /// <value>The open message box.</value>
        public OpenMessageBoxHandler? DoOpenMessageBox { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name {
			get => _name; set {
				if (SetProperty(ref _name, value)) cnt++;
			}
		}
        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>The email.</value>
        public string Email { get => _email; set => SetProperty(ref _email, value); }

        public string Cnt => $"Count: {cnt}";
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel()
        {
            this.OpenDialogCommand = new DelegateCommand((o) =>
            {
                (Name, Email) = DoOpenDialog?.Invoke(this.Name, this.Email)??("","");
            });

            this.OpenMsgCommand = new DelegateCommand((o) =>
            {
                if (this.DoOpenMessageBox?.Invoke("Frage", "Willst Du Das ?") == MessageBoxResult.Yes)
                {
                    Name = "42 Entwickler";
                }
                else
                {
                    Name = "NÃ¶";
                }
            });

        }

        /// <summary>
        /// Gets or sets the open dialog command.
        /// </summary>
        /// <value>The open dialog command.</value>
        public DelegateCommand OpenDialogCommand { get; set; }
        /// <summary>
        /// Gets or sets the open MSG command.
        /// </summary>
        /// <value>The open MSG command.</value>
        public DelegateCommand OpenMsgCommand { get; set; }

        /// <summary>
        /// The name
        /// </summary>
        string _name = "";
        /// <summary>
        /// The email
        /// </summary>
        string _email = "";
        /// <summary>
        /// The count
        /// </summary>
        private int cnt;
    }
}
