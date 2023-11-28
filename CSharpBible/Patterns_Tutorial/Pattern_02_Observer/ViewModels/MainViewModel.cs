using BaseLib.Interfaces;
using Pattern_02_Observer.Models;
using Pattern_02_Observer.Properties;
using System;

namespace Pattern_02_Observer.ViewModels
{
    /// <summary>This class will act an an observer in the observer-pattern, it reacts on changes in the Values</summary>
    public class MainViewModel : INotifyPropertyChangedAdv, IMainViewModel
    {
        Model model = new Model();
        private DateTime _startTime;

        public event PropertyChangedAdvEventHandler? PropertyChangedAdv;

        public string Greeting => Resources.ResourceManager.GetString(model.Greeting);
        public TimeSpan RunTime => model.Now - _startTime;

        public MainViewModel()
        {
            model.PropertyChangedAdv += OnPropertyChangedAdv;
            _startTime = model.Now;
        }

        private void OnPropertyChangedAdv(object sender, PropertyChangedAdvEventArgs e)
        {
            if (e.PropertyName == nameof(model.Greeting))
                PropertyChangedAdv?.Invoke(this, new PropertyChangedAdvEventArgs(nameof(Greeting), null, Greeting));
        }

        public void SetGreeting(EGreetings eGreetings)
            => model.SetGreeting(eGreetings);
    }
}
