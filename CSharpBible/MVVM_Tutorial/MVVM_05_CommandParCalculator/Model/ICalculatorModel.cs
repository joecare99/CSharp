using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

namespace MVVM_05_CommandParCalculator.Model
{
    public interface ICalculatorModel :INotifyPropertyChanged
    {

        double Accumulator { get; }
        double? Register { get; }
        double? Memory { get; }
        IEnumerable<(string Dest,string Src)> Dependencies { get; }
        ETrigMode TrigMode { get; }
        ECalcError CalcError { get; }

        int StackSize { get; }
        bool canCommand(ECommands eC);
        void CalcCmd(ECommands o);
        bool canOperator(EOperations eO);
        void OperatorCmd(EOperations eO);
        void NumberCmd(ENumbers o);
    }
}