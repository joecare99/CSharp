using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranspilerLib.DriveBASIC.Data;


/// <author>C. Rosewich</author>
/// <since>18.09.2015</since>
/// <version>0.2</version>
/// <info>Enumeriert die Operation, die bei einem Ausdruck ausgeführt werden soll</info>
public enum EExpressionOperation
{
    /// <author>C.Rosewich</author>
    /// <since>18.09.2015</since>
    /// <info>Plus</info>
    teo_plus = 0,

    /// <author>C.Rosewich</author>
    /// <since>18.09.2015</since>
    /// <info>Minus</info>
    teo_minus = 1,

    /// <author>C.Rosewich</author>
    /// <since>18.09.2015</since>
    /// <info>Multiplikation</info>
    teo_mult = 2,

    /// <author>C.Rosewich</author>
    /// <since>18.09.2015</since>
    /// <info>Division</info>
    teo_div = 3,

    /// <author>C.Rosewich</author>
    /// <since>18.09.2015</since>
    /// <info>Binäre Und-Verknüpfung</info>
    teo_and = 4,

    /// <author>C.Rosewich</author>
    /// <since>18.09.2015</since>
    /// <info>Binäre Oder-Verknüpfung</info>
    teo_or = 5,

    /// <author>C.Rosewich</author>
    /// <since>18.09.2015</since>
    /// <info>Binäre Exklusiv-Oder-Verknüpfung</info>
    teo_xor = 6,

    /// <author>C.Rosewich</author>
    /// <since>18.09.2015</since>
    /// <info>Kleiner als</info>
    teo_lt = 7,

    /// <author>C.Rosewich</author>
    /// <since>18.09.2015</since>
    /// <info>Kleiner oder Gleich</info>
    teo_lte = 8,

    /// <author>C.Rosewich</author>
    /// <since>18.09.2015</since>
    /// <info>Gleich</info>
    teo_eq = 9,

    /// <author>C.Rosewich</author>
    /// <since>18.09.2015</since>
    /// <info>Ungleich</info>
    teo_neq = 10,

    /// <author>C.Rosewich</author>
    /// <since>18.09.2015</since>
    /// <info>Größer als</info>
    teo_gt = 11,

    /// <author>C.Rosewich</author>
    /// <since>18.09.2015</since>
    /// <info>Größer oder gleich</info>
    teo_gte = 12,

    /// <author>C.Rosewich</author>
    /// <since>18.09.2015</since>
    /// <info>Unäre Funktion</info>
    teo_fct = 14,

    /// <author>C.Rosewich</author>
    /// <since>18.09.2015</since>
    /// <info>Indirekter Zugriff (Array)</info>
    teo_indirect = 15
}