using BlazorWasmDocker.Models.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace BlazorWasmDocker.Models;

	public partial class CalculatorModel:ObservableObject, ICalculatorModel
	{
		[ObservableProperty]
		private double _display;

		[ObservableProperty]
		private bool _xNeg;

		[ObservableProperty]
		private ECalcCommand _opMode;
		private double SecondNumber;
		private double FirstNumber;
		private bool xEditMode;

		[RelayCommand]
		private void AddNumber(int number)
		{
			Display = Display * 10d + number;
			xEditMode = true;
		}

		[RelayCommand]
		private void Clear(string parameter)
		{
			if (parameter == "All")
			{
				Display = 0d;
				OpMode = ECalcCommand.None;
			}
			else if (parameter == "<-")
			{
				BackSpace();
			}
		}

		[RelayCommand]
		private void Calculate()
		{
			SecondNumber = Display;
			switch (OpMode)
			{
				case ECalcCommand.Add:
					Display = (FirstNumber + SecondNumber);
					break;
				case ECalcCommand.Subtract:
					Display = (FirstNumber - SecondNumber);
					break;
				case ECalcCommand.Multiply:
					Display = (FirstNumber * SecondNumber);
					break;
				case ECalcCommand.Divide:
					Display = (FirstNumber / SecondNumber);
					break;
			}
		}

		[RelayCommand]
		private void Operation(ECalcCommand operation)
		{
			if (OpMode != ECalcCommand.None)
			{
				Calculate();
			}
			FirstNumber = Display;
			OpMode = operation;
			Display = 0d;
		}

		void BackSpace()
		{
			if (!xEditMode)
			{
				Display = 0d;
				xEditMode = false;
				return;
			}
			Display = Math.Floor(Display /10d);
		}

	}
