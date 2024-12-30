using PluginBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AppWithPlugin;

public class Program
{
    public static void Main(string[] args)
    {
        var app = new Model.AppWithPlugin();
        app.Initialize(args);
        app.Main(args);
    }
}