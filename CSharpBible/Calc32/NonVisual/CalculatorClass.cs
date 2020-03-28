using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace CSharpBible.Calc32.NonVisual
{

    public class CalculatorClass : Component
    {
        enum eOpMode
        {
            CalcResult = 1, Plus = 2, Minus = 3, Multiply = 4, Divide = 5,
            BinaryAnd = 6, BinaryOr = 7, BinaryXor = 8, BinaryNot = 9
        };

        readonly string[] sMode = { "", "", "+", "-", "*", "/", "/\\", "\\/", "><", "!" };
        // Fields
        private int nAkkumulator; // Editorfeld
        private eOpMode nMode;
        private bool bEditMode;
        private bool bNegMode;
        private int nMemory; // Gemerkte Zahl für Operationen

        public event EventHandler OnChange;

        // Properties
        public int Akkumulator
        {
            get => nAkkumulator;
            set
            {
                if (value == nAkkumulator) return;
                nAkkumulator = value;
                OnChange?.Invoke(this, null);
            }
        }

        public int Memory
        {
            get => nMemory;
            set
            {
                if (value == nMemory) return;
                nMemory = value;
                OnChange?.Invoke(this, null);
            }
        }
        public string OperationText
        {
            get => sMode[(int)nMode];
        }

        public CalculatorClass()
        {
            nAkkumulator = 0;
            nMode = 0;
            OnChange = null;
        }

        public void Button(int aNumber)
        {
            if (bEditMode)
            {
                Akkumulator = nAkkumulator * 10 + aNumber;
            }
            else
            {
                bEditMode = true;
                Akkumulator = aNumber;
            }
        }

        internal void Operation(int v)
        {
            if ((v > 0) && (v <= (int)eOpMode.BinaryNot))
            {
                bEditMode = false;
                switch (nMode)
                {
                    case eOpMode.Plus:
                        nAkkumulator = nMemory + nAkkumulator;
                        break;
                    case eOpMode.Minus:
                        nAkkumulator = nMemory - nAkkumulator;
                        break;
                    case eOpMode.Multiply:
                        nAkkumulator = nMemory * nAkkumulator;
                        break;
                    case eOpMode.Divide:
                        nAkkumulator = nMemory / nAkkumulator;
                        break;
                    case eOpMode.BinaryAnd:
                        nAkkumulator = nMemory & nAkkumulator;
                        break;
                    case eOpMode.BinaryOr:
                        nAkkumulator = nMemory | nAkkumulator;
                        break;
                    case eOpMode.BinaryXor:
                        nAkkumulator = nMemory ^ nAkkumulator;
                        break;
                    default:
                        break;
                }

                if ((eOpMode)v == eOpMode.CalcResult)
                {
                    nMode = (eOpMode)v;
                    Memory = 0;
                }
                else if ((eOpMode) v == eOpMode.BinaryNot)
                {
                    Akkumulator = ~nAkkumulator;
                }
                else
                {
                    nMode = (eOpMode)v;
                    Memory = nAkkumulator;
                }

            }
        }

        public void BackSpace()
        {
            if (bEditMode)
            {
                Akkumulator = nAkkumulator / 10;
            }
            else
            {
                Akkumulator = 0;
            }
        }
    }
}
