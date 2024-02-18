using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestStatements.DependencyInjection
{
    public interface IMessageWriter
    {
        void Write(string message);
    }
}
