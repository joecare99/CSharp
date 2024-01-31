using System;
using MVVM_38_CTDependencyInjection.Models.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MVVM_38_CTDependencyInjection.Models
{
    public class CTimer : ITimer
    {
        private Timer _timer;

        public event ElapsedEventHandler Elapsed;
        public double Interval { get => _timer.Interval; set => _timer.Interval=value; }

        public void Start() => _timer.Start();
        public void Stop() => _timer.Stop();

        public CTimer()
        {
            _timer = new Timer();
            _timer.Elapsed += (s, e) => Elapsed?.Invoke(this, e);
        }
    }
}
