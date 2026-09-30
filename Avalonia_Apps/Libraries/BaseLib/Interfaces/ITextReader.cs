using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLib.Interfaces;

public interface ITextReader
{
    string ReadLine();
    string ReadToEnd();
}
