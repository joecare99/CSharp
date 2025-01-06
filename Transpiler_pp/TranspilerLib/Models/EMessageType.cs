using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranspilerLib.Models;

public enum EMessageType
{
    mtFatal,
    mtError,
    mtWarning,
    mtNote,
    mtHint,
    mtInfo,
    mtDebug
}
