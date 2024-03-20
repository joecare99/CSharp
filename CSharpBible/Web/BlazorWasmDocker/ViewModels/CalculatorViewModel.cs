using BlazorWasmDocker.Models.Interfaces;
using BlazorWasmDocker.ViewModels.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;

namespace BlazorWasmDocker.ViewModels
{

    public partial class CalculatorViewModel : ObservableObject, ICalculatorViewModel
    {
        private Dictionary<string, ECalcCommand> _calculatorCommands=new() { 
            {"+",ECalcCommand.Add },
            {"-",ECalcCommand.Subtract },
            {"*",ECalcCommand.Multiply },
            {"/",ECalcCommand.Divide }
        };

        public string Display => _calculatorModel.Display.ToString();

		public bool xEnterMode { get ; set ; }

		public string OpMode => _calculatorModel.OpMode.ToString();

		private ICalculatorModel _calculatorModel;

        public CalculatorViewModel(ICalculatorModel calculatorModel)
        {
            _calculatorModel = calculatorModel;
            _calculatorModel.PropertyChanged += (s, e) => OnPropertyChanged(e.PropertyName);
        }

        [RelayCommand]
        private void AddNumber(string parameter)
        {
            if (xEnterMode) return;
            if (int.TryParse(parameter,out var number))
			_calculatorModel.AddNumberCommand.Execute(number);
        }

        [RelayCommand]
        private void Operation(string operation)
        {
			if (xEnterMode) return;
			ECalcCommand operationEnum;
            if (!_calculatorCommands.TryGetValue(operation, out operationEnum))
                operationEnum = ECalcCommand.None;
            _calculatorModel.OperationCommand.Execute(operationEnum);
        }

        [RelayCommand]
        private void Calculate()
        {
			if (xEnterMode) return;
			_calculatorModel.CalculateCommand.Execute(null);
        }

        [RelayCommand]
        private void Clear(string parameter)
        {
			if (xEnterMode) return;
			_calculatorModel.ClearCommand.Execute(parameter);
        }

    }
}
