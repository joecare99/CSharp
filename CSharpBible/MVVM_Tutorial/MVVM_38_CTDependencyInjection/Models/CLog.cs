using MVVM_38_CTDependencyInjection.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM_38_CTDependencyInjection.Models
{
    public class CLog : ILog
    {
        public void Log(string message) => Debug.WriteLine(message);
    }
}
