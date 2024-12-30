using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginBase.Interfaces;

public interface IUserInterface
{
    string Title { get; set; }
    bool ShowMessage(string message);
}
