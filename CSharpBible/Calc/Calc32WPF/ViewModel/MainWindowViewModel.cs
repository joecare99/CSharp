using MVVM.ViewModel;
using Calc32.NonVisual;
using System;

/// <summary>
/// The ViewModel namespace.
/// </summary>
namespace Calc32WPF.ViewModel
{
    /// <summary>Class MainWindowViewModel.
    /// Implements the <see cref="T:MVVM.ViewModel.BaseViewModel" /></summary>
    public class MainWindowViewModel: BaseViewModel
    {
        /// <summary>The calculator class</summary>
        private CalculatorClass calculatorClass;

        /// <summary>Gets or sets the akkumulator.
        /// <see cref="CalculatorClass.Akkumulator" /></summary>
        /// <value>The akkumulator.</value>
        public int Akkumulator
        {
            get => calculatorClass.Akkumulator; set
            {
                calculatorClass.Akkumulator = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>Gets or sets the memory.
        /// <see cref="CalculatorClass.Memory" /></summary>
        /// <value>The memory.</value>
        public int Memory
        {
            get => calculatorClass.Memory; set
            {
                calculatorClass.Memory = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>Initializes a new instance of the <see cref="T:Calc32WPF.ViewModel.MainWindowViewModel" /> class.</summary>
        public MainWindowViewModel()
        {
            calculatorClass = new CalculatorClass();
            calculatorClass.OnChange += CalculatorClass_OnChange;
            this.btnNumber =new DelegateCommand( (o) => { 
                calculatorClass.NumberButton(int.Parse((string)o)); 
            });
            this.btnOperation = new DelegateCommand( (o) => {
                calculatorClass.Operation(-int.Parse((string)o)); 
            });
            this.btnBackspace = new DelegateCommand(
                (o) => { calculatorClass.Operation((int)(o ?? 0)); },
                (o)=>calculatorClass.Akkumulator!=0);
        }

        private void CalculatorClass_OnChange(object sender, EventArgs e)
        {
            RaisePropertyChanged(nameof(Akkumulator));
            RaisePropertyChanged(nameof(Memory));
            RaisePropertyChanged(nameof(OperationText));
        }

        /// <summary>Gets or sets the number-button - delegate.</summary>
        /// <value>The BTN number.</value>
        public DelegateCommand btnNumber { get; set; }
        /// <summary>Gets or sets the Delegate for the operation command.</summary>
        /// <value>The BTN operation.</value>
        public DelegateCommand btnOperation { get; set; }
        public DelegateCommand btnBackspace { get; set; }

        public string OperationText { get => calculatorClass.OperationText; }


    }
}
