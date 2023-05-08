using CommunityToolkit.Mvvm.Input;
using MVVM.ViewModel;
using MVVM_05_CommandParCalculator.Model;
using System;
using System.ComponentModel;

namespace MVVM_05_CommandParCalculator.ViewModels
{
    public class CommandParCalculatorViewModel : BaseViewModel
    {
        #region Properties
        public static Func<ICalculatorModel> GetModel { get; set; } = () => CalculatorModel.Instance;
        private ICalculatorModel _model;
        public RelayCommand<ENumbers> NumberCommand { get; set; }
        public RelayCommand<EOperations> OperatorCommand { get; set; }
        public RelayCommand<ECommands> CalculatorCommand { get; set; }

        public double Accumulator => _model.Accumulator;
        public double Memory => _model.Memory ?? double.NaN;
        public double Register => _model.Register ?? double.NaN;
        public string Status => $"{_model.CalcError} {_model.TrigMode} ";
        #endregion

        #region Methods
        public bool canOperator(EOperations eO) => FuncProxy(eO, _model.canOperator);
        public bool canCommand(ECommands eC) => FuncProxy(eC, _model.canCommand);

        public CommandParCalculatorViewModel()
        {
            _model = GetModel();
            _model.PropertyChanged += OnPropertyChanged;
            NumberCommand = new(_model.NumberCmd);
            OperatorCommand = new(_model.OperatorCmd,canOperator);
            CalculatorCommand = new(_model.CalcCmd, canCommand);
            foreach (var d in _model.Dependencies)
                AddPropertyDependency(d.Dest, d.Src);
            AddPropertyDependency(nameof(OperatorCommand), nameof(canOperator));
            AddPropertyDependency(nameof(CalculatorCommand), nameof(canCommand));
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Accumulator))
                RaisePropertyChanged(e.PropertyName);
            if (e.PropertyName == nameof(Memory))
                RaisePropertyChanged(e.PropertyName);
            if (e.PropertyName == nameof(Register))
                RaisePropertyChanged(e.PropertyName);
        }
        #endregion

    }
}
