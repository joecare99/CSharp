using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranspilerLib.Interfaces;

namespace TranspilerConsTest.Model;

public class JsonOutput : IOutput
{
    public void Output(IReader reader, Action<string> write, Action<string> debug)
    {
        var indent = 0;
        write("{");
        var xElExists = false;
        while (reader.Read())
        {
            if (reader.IsStartElement())
            {
                write($"{(xElExists ? ", " : "")} \"{reader.GetLocalName()}\" : {{");
                xElExists = false;
                var xIsEmptyElement = reader.IsEmptyElement;
                var count = reader.GetAttributeCount();
                for (var i = 0; i < count; i++)
                {
                    write($"{(xElExists ? ", " : "")} \"{reader.GetAttributeName(i)}\" : \"{reader.GetAttributeValue(i)}\"");
                    xElExists = true;
                }
                if (xIsEmptyElement)
                {
                    write("}");
                    xElExists = true;
                }
            }
            else if (reader.IsEndElement())
            {
                write("}");
                xElExists = true;

            }
            else if (reader.HasValue)
            {
                write($"{(xElExists ? ", " : "")} \"@\" : \"{Quoted(reader.getValue().ToString())}\"");
            }
        }
        write("}");

        string Quoted(string value)
        {
            return value.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", "\\r").Replace("\n", "\\n").Replace("\t", "\\t");
        }
    }
}
