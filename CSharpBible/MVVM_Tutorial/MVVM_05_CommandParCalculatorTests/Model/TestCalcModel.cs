using MVVM.ViewModel;
using System;
using System.Collections.Generic;

namespace MVVM_05_CommandParCalculator.Model
{
    internal class TestCalcModel : NotificationObject, ICalculatorModel
    {
        private double _accumulator = 0d;
        private double? _register;
        private double? _memory;
        private ETrigMode _trigMode;
        private ECalcError _calcError;
        private int _stackSize;
        private Action<string> _doLog;

        public double Accumulator { get => _accumulator; set => SetProperty(ref _accumulator, value); }
        public double? Register { get => _register; set => SetProperty(ref _register, value); }
        public double? Memory { get => _memory; set => SetProperty(ref _memory, value); }
                public IEnumerable<(string Dest, string Src)> Dependencies => new[]{
            (nameof(canOperator),nameof(Accumulator)),
            (nameof(canOperator),nameof(Register)),
        };
        public ETrigMode TrigMode { get => _trigMode; set => SetProperty(ref _trigMode, value); }
                public ECalcError CalcError { get => _calcError; set => SetProperty(ref _calcError, value); }
                public int StackSize { get => _stackSize; set => SetProperty(ref _stackSize, value); }
        public bool xResult { get; set; } = false;

        public TestCalcModel(Action<string> DoLog)
        {
            _doLog = DoLog;
        }

        public void CalcCmd(ECommands o)
        {
            _doLog($"CalcCmd({o})");
        }

        public bool canCommand(ECommands eC)
        {
            _doLog($"canCommand({eC})={xResult}");
            return xResult;
        }

        public bool canOperator(EOperations eO)
        {
            _doLog($"canOperator({eO}={xResult})");
            return xResult;
        }

        public void NumberCmd(ENumbers o)
        {
            _doLog($"NumberCmd({o})");
        }

        public void OperatorCmd(EOperations eO)
        {
            _doLog($"OperatorCmd({eO})");
        }
    }
}