using System;

namespace Basic_Del03_General.Models
{
    public class Model
    {
        public string Greeting { get; set; } = "Grtn_HelloWorld";

        public Func<int, int, double> myCalc = _Dummy;

        private static double _Divide(int arg1, int arg2) => (double)arg1 / arg2;
        private static double _Dummy(int arg1, int arg2) => 0.0d;
    }
}
