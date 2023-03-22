// ***********************************************************************
// Assembly         : Calc32WPF_net
// Author           : Mir
// Created          : 12-22-2021
//
// Last Modified By : Mir
// Last Modified On : 10-22-2022
// ***********************************************************************
// <copyright file="MainWindowViewModel.cs" company="Calc32WPF_net">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using MVVM.ViewModel;
using Calc32.NonVisual;
using System;

/// <summary>
/// The ViewModel namespace.
/// </summary>
namespace Calc32WPF.ViewModel
{
    /// <summary>
    /// Class MainWindowViewModel.
    /// Implements the <see cref="T:MVVM.ViewModel.BaseViewModel" />
    /// </summary>
    public class MainWindowViewModel: BaseViewModel
    {
        #region Properties
        #region private properties
        /// <summary>
        /// The calculator class
        /// </summary>
        private CalculatorClass calculatorClass;
        #endregion

        /// <summary>
        /// Gets or sets the akkumulator.
        /// <see cref="CalculatorClass.Akkumulator" />
        /// </summary>
        /// <value>The akkumulator.</value>
        public int Akkumulator
        {
            get => calculatorClass.Akkumulator; set
            {
                calculatorClass.Akkumulator = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the memory.
        /// <see cref="CalculatorClass.Memory" />
        /// </summary>
        /// <value>The memory.</value>
        public int Memory
        {
            get => calculatorClass.Memory; set
            {
                calculatorClass.Memory = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets the operation text.
        /// </summary>
        /// <value>The operation text.</value>
        public string OperationText { get => calculatorClass.OperationText; }

        /// <summary>
        /// Gets or sets the number-button - delegate.
        /// </summary>
        /// <value>The BTN number.</value>
        public DelegateCommand btnNumber { get; set; }
        /// <summary>
        /// Gets or sets the Delegate for the operation command.
        /// </summary>
        /// <value>The BTN operation.</value>
        public DelegateCommand btnOperation { get; set; }
        /// <summary>
        /// Gets or sets the BTN backspace.
        /// </summary>
        /// <value>The BTN backspace.</value>
        public DelegateCommand btnBackspace { get; set; }
        #endregion
        #region Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Calc32WPF.ViewModel.MainWindowViewModel" /> class.
        /// </summary>
        public MainWindowViewModel()
        {
            calculatorClass = new CalculatorClass();
            calculatorClass.OnChange += CalculatorClass_OnChange;
            btnNumber =new DelegateCommand( (o) => { 
                calculatorClass.NumberButton(int.Parse((string)o)); 
            });
            btnOperation = new DelegateCommand( (o) => {
                calculatorClass.Operation(-int.Parse((string)o)); 
            });
            btnBackspace = new DelegateCommand(
                (o) => { calculatorClass.Operation((int)(o ?? 0)); },
                (o)=>calculatorClass.Akkumulator!=0);
            CommandCanExecuteBinding.Add((nameof(calculatorClass.Akkumulator), nameof(btnBackspace)));
        }

        /// <summary>
        /// Handles the OnChange event of the CalculatorClass control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void CalculatorClass_OnChange(object sender, (string, int, int) e)
        {
            RaisePropertyChanged(nameof(Akkumulator));
            RaisePropertyChanged(nameof(Memory));
            RaisePropertyChanged(nameof(OperationText));
        }
        #endregion
    }
}
