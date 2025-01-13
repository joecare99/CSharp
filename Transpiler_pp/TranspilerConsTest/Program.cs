using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TranspilerConsTest.Model;
using TranspilerLib.Interfaces.Code;
using TranspilerLib.Models;
using TranspilerLib.Models.Scanner;

internal class Program
{
    private static void Main(string[] args)
    {
        var xr = new System.Xml.XmlTextReader(args[0]);
        var reader = new IECReader(xr);
        IOutput output = new ExtOutput();
        var extcode = new List<string>();
        output.Output(reader, (s)=>extcode.Add(s), (s) => Debug.Write(s));
        ICodeBase code = new IECCode();
        code.OriginalCode = string.Join("",extcode);
        Console.WriteLine(code.OriginalCode);
        var cb= code.Parse();
        Console.WriteLine(code.ToCode(cb));

    }

}