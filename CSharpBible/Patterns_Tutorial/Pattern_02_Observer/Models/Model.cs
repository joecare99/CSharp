using BaseLib.Helper;
using BaseLib.Interfaces;
using System;

namespace Pattern_02_Observer.Models
{

    /// <summary>
    /// This model will be the Subject in the Observer-Pattern
    /// </summary>
    /// <seealso cref="BaseLib.Interfaces.INotifyPropertyChangedAdv" />
    public class Model : INotifyPropertyChangedAdv
    {
        public Func<DateTime> GetNow { get; set; }=()=>DateTime.Now;

        private string _greeting ="";

        public string Greeting { get => _greeting; set => value.SetProperty(ref _greeting,GreetingChanged); } 
        private void GreetingChanged(string arg1, string arg2, string arg3)
        {
            PropertyChangedAdv?.Invoke(this, new PropertyChangedAdvEventArgs(arg1, arg2, arg3));
        }

        public DateTime Now => GetNow();

        public event PropertyChangedAdvEventHandler? PropertyChangedAdv;

        public void SetGreeting(EGreetings eGreetings) 
            => Greeting = eGreetings.ToString();
    }
}
