using MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MVVM_05_CommandParCalculator.Model.ICalculatorModel;

namespace MVVM_05_CommandParCalculator.Model
{
    public class CalculatorModel : NotificationObject, ICalculatorModel
    {

        #region Properties
        private static readonly List<EOperations> _unaryOperation = new() {
            EOperations.Nop,
            EOperations.Negate,
            EOperations.Sin,
            EOperations.Cos,
            EOperations.Tan,
            EOperations.Square,
            EOperations.SquareRt,
            EOperations.Inverse,
            EOperations.ExpN,
            EOperations.LogN,
        };

        private static readonly Dictionary<EOperations,int> _OperationLevel = new() {
            { EOperations.CalcResult, 0 },
            { EOperations.Add, 1 },
            { EOperations.Subtract, 1 },
            { EOperations.Multiply, 2 },
            { EOperations.Divide, 2 },
            { EOperations.Power, 3 },
            { EOperations.SquareRtX, 3 },
            };

        private static CalculatorModel _instance;
        private EOperations _op;
        private Func<double, double>? _Operation;
        private double _accumulator = 0d;
        private double? _register = null;
        private double? _memory = null;
        private Stack<(double, EOperations, Func<double, double>)> _stack = new();
        private double _decFak;
        private bool _decMode;
        private bool _editMode;
        private ETrigMode _trigMode;
        private ECalcError _calcError;

        public bool DecMode
        {
            get => _decMode;
            set
            {
                if (!_decMode && value)
                    _decFak = 1d;
                _decMode = value;
            }
        }

        public double Accumulator { get => _accumulator; set => SetProperty(ref _accumulator, value); }
        public double? Register { get => _register; set => SetProperty(ref _register, value); }
        public double? Memory { get => _memory; set => SetProperty(ref _memory, value); }
        public static CalculatorModel Instance => _instance ??= new();

        public IEnumerable<(string, string)> Dependencies => new[]{
            (nameof(canOperator),nameof(Accumulator)),
            (nameof(canOperator),nameof(Register)),
        };


        public ETrigMode TrigMode => _trigMode;

        public ECalcError CalcError => _calcError;

        public int StackSize => _stack.Count;

        #endregion

        public CalculatorModel() { }

        public bool canOperator(EOperations eO)
        {
            if (eO == EOperations.Nop) return true;
            if (eO == EOperations.CalcResult) return _Operation != null && _register != null;
            if (eO > EOperations.CalcResult) return _editMode || _accumulator != 0d;
            return false;
        }

        public bool canCommand(ECommands e)
        {

            if (e == ECommands.DecMode) return !_decMode;
            return true;
        }


        public void NumberCmd(ENumbers oo)
        {
            if (oo is ENumbers e && (int)e is int iN)
            {
                if (!_editMode)
                {
                    _accumulator = 0d;
                    _editMode = true;
                    _decMode = false;
                }
                if (!_decMode)
                    Accumulator = Accumulator * 10d + iN;
                else
                {
                    _decFak *= 0.1d;
                    Accumulator = Accumulator + iN * _decFak;
                }
            }
        }

        public void OperatorCmd(EOperations eO)
        {

            _editMode = false;

            if (eO >= EOperations.CalcResult && _register != null && !_unaryOperation.Contains(eO))
            {
                if (_Operation != null )
                    Accumulator = _Operation.Invoke(_accumulator);

                Register = null;
                DecMode = false;
            }
            if (eO > EOperations.CalcResult && !_unaryOperation.Contains(eO)) 
            { Register = _accumulator; };
            var op = eO switch
            {

                EOperations.CalcResult => null,
                EOperations.Add => (a) => _register!.Value + a,
                EOperations.Subtract => (a) => _register!.Value - a,
                EOperations.Multiply => (a) => _register!.Value * a,
                EOperations.Divide => (a) => _register!.Value / a,
                EOperations.Power => (a) => Math.Pow(_register!.Value, a),
                EOperations.Negate => (a) => -a,
                EOperations.Square => (a) => a * a,
                EOperations.SquareRt => (a) => Math.Sqrt(a),
                EOperations.Inverse => (a) => 1 / a,
                EOperations.Sin => (a) => Math.Sin(a), // Dodo: Factor by Trig-Mode
                EOperations.Cos => (a) => Math.Cos(a),
                EOperations.Tan => (a) => Math.Tan(a),
                EOperations.LogN => (a) => Math.Log(a),
                EOperations.ExpN => (a) => Math.Exp(a),
                _ => (Func<double, double>?)null,
            };
            if (_unaryOperation.Contains(eO))
                Accumulator = op?.Invoke(_accumulator) ?? _accumulator;
            else
            {
                _op = eO;
                _Operation = op;
            }
        }

        public void CalcCmd(ECommands o)
        {
            switch (o)
            {
                case ECommands.Clear: //Clear  
                    Accumulator = 0d;
                    _editMode = false;
                    break;
                case ECommands.ClearAll: //Clear All  
                    Accumulator = 0d;
                    Register = 0d;
                    _Operation = null;
                    _editMode = false;
                    break;
                case ECommands.DecMode: // DecimalSeparator                          
                    DecMode = true;
                    break;
                case ECommands.e:
                    Accumulator = Math.E;
                    break;
                case ECommands.Pi:
                    Accumulator = Math.PI;
                    break;
                case ECommands.MS: Memory = Accumulator; break;
                case ECommands.MR: Accumulator = Memory ?? 0d; break;
                case ECommands.MC: Memory = null; break;
                case ECommands.Mp: Memory = (Memory ?? 0d) + Accumulator; break;
                case ECommands.Mm: Memory = (Memory ?? 0d) - Accumulator; break;
                default:
                    break;
            }
        }
    }
}