using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranspilerLib.DriveBASIC.Data;

/// <author>C. Rosewich</author>
/// <since>22.06.2009</since>
/// <version>0.1</version>
public enum EVarType
{
    /// <author>C. Rosewich</author>
    /// <since>18.05.2011</since>
    /// <info>Universeller Typ</info>
    vt_universal = -1,
    /// <author>C. Rosewich</author>
    /// <since>18.05.2011</since>
    /// <info>Typ-ID für Bool-Variablen</info>
    vt_bool = 0,
    /// <author>C. Rosewich</author>
    /// <since>18.05.2011</since>
    /// <info>Typ-ID für Real/float-Variablen</info>
    vt_real = 1,
    /// <author>C. Rosewich</author>
    /// <since>18.05.2011</since>
    /// <info>Typ-ID für Punkt-Variablen</info>
    vt_point = 2,
    /// <author>C. Rosewich</author>
    /// <since>18.05.2011</since>
    /// <info>Typ-ID für Punkt-Dimension-Zugriff</info>
    vt_dimension = 3
}