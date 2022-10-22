// ***********************************************************************
// Assembly         : DialogBoxes
// Author           : Mir
// Created          : 12-29-2021
//
// Last Modified By : Mir
// Last Modified On : 07-20-2022
// ***********************************************************************
// <copyright file="DialogWindowViewModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2021
// </copyright>
// <summary></summary>
// ***********************************************************************
using MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogBoxes.ViewModel
{
    /// <summary>
    /// Class DialogWindowViewModel.
    /// Implements the <see cref="BaseViewModel" />
    /// </summary>
    /// <seealso cref="BaseViewModel" />
    public class DialogWindowViewModel : BaseViewModel
    {
        /// <summary>
        /// The name
        /// </summary>
        private string _name = "";
        /// <summary>
        /// The email
        /// </summary>
        private string _email = "";

        /// <summary>
        /// Occurs when [ok].
        /// </summary>
        public event EventHandler OK;
        /// <summary>
        /// Occurs when [cancel].
        /// </summary>
        public event EventHandler Cancel;
        /// <summary>
        /// Gets or sets the ok command.
        /// </summary>
        /// <value>The ok command.</value>
        public DelegateCommand OKCommand { get; set; }
        /// <summary>
        /// Gets or sets the cancel command.
        /// </summary>
        /// <value>The cancel command.</value>
        public DelegateCommand CancelCommand { get; set;}

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogWindowViewModel"/> class.
        /// </summary>
        public DialogWindowViewModel()
        {
            this.OKCommand = new DelegateCommand((o) =>
            {
                this.OK?.Invoke(this, EventArgs.Empty);
            });
            this.CancelCommand = new DelegateCommand((o) =>
            {
                Name = String.Empty;
                Email = String.Empty;
                this.Cancel?.Invoke(this, EventArgs.Empty);
            });
        }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get => _name; set => SetProperty(ref _name, value); }
        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>The email.</value>
        public string Email { get => _email; set => SetProperty(ref _email, value); }
    }
}
