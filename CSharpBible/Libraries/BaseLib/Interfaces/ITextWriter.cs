using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLib.Interfaces;

public interface ITextWriter
{
    void Write(string text);
    void WriteLine(string text);
}
