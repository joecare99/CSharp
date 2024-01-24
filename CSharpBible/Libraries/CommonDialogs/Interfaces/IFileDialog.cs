using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonDialogs.Interfaces
{
    public interface IFileDialog
    {
        string FileName { get; set; }

        bool? ShowDialog();
        bool? ShowDialog(object owner);
    }
}
