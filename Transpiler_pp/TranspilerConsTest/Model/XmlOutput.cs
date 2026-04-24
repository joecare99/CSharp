using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranspilerLib.Interfaces;
using TranspilerLib.Models;

namespace TranspilerConsTest.Model
{
    public class XmlOutput : IOutput
    {
        public void Output(IReader reader, Action<string> write,Action<string> debug)
        {
            var indent = 0;
            while (reader.Read())
            {
                if (reader.IsStartElement())
                {
                    write($"{new string(' ', indent * 2)}<{reader.GetLocalName()}");
                    var xIsEmptyElement = reader.IsEmptyElement;
                    if (!xIsEmptyElement)
                        indent++;
                    var count = reader.GetAttributeCount();
                    for (var i = 0; i < count; i++)
                    {
                        write($" {reader.GetAttributeName(i)}=\"{reader.GetAttributeValue(i)}\"");
                    }
                    if (xIsEmptyElement)
                        write(" /");
                    write(">");
                    if (xIsEmptyElement || !reader.IsEmptyElement || !reader.HasValue)
                        write(Environment.NewLine);
                }
                else if (reader.IsEndElement())
                {
                    indent--;
                    write($"{new string(' ', indent * 2)}</{reader.GetLocalName()}>{Environment.NewLine}");
                }
                else if (reader.HasValue)
                {
                    write($"{reader.getValue()}{Environment.NewLine}");
                }
            }

        }
    }
}
