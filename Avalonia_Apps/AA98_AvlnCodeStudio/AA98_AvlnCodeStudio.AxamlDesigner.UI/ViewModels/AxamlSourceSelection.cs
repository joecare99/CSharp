using AA98_AvlnCodeStudio.Model.Workspaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace AA98_AvlnCodeStudio.AxamlDesigner.UI.ViewModels;

internal sealed class AxamlSourceSelection
{
    private readonly List<(int Start, int End, AxamlPreviewNode? Node)> _elements;

    private AxamlSourceSelection(List<(int Start, int End, AxamlPreviewNode? Node)> elements) =>
        _elements = elements;

    public static AxamlSourceSelection? Create(string markup, AxamlPreviewNode? root, string? diagnostic)
    {
        if (root is null || root.IsRecoveryRoot || !string.IsNullOrEmpty(diagnostic))
        {
            return null;
        }

        Dictionary<int, AxamlPreviewNode> nodes = [];
        if (!CollectNodes(root, nodes))
        {
            return null;
        }

        List<int> lines = [0];
        for (int i = 0; i < markup.Length; i++)
        {
            if (markup[i] == '\n')
            {
                lines.Add(i + 1);
            }
        }

        List<(int Start, int End, AxamlPreviewNode? Node)> elements = [];
        Stack<(int Start, AxamlPreviewNode? Node)> stack = [];
        try
        {
            using StringReader input = new(markup);
            using XmlReader reader = XmlReader.Create(input, new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit,
                XmlResolver = null,
            });
            while (reader.Read())
            {
                if (reader.NodeType is not (XmlNodeType.Element or XmlNodeType.EndElement))
                {
                    continue;
                }

                IXmlLineInfo line = (IXmlLineInfo)reader;
                int reported = lines[line.LineNumber - 1] + line.LinePosition - 1;
                int start = markup.LastIndexOf('<', Math.Min(reported, markup.Length - 1));
                if (start < lines[line.LineNumber - 1])
                {
                    return null;
                }

                int end = FindTagEnd(markup, start);
                if (end < 0)
                {
                    return null;
                }

                if (reader.NodeType == XmlNodeType.Element)
                {
                    nodes.TryGetValue(start, out AxamlPreviewNode? node);
                    if (reader.IsEmptyElement)
                    {
                        elements.Add((start, end + 1, node));
                    }
                    else
                    {
                        stack.Push((start, node));
                    }
                }
                else
                {
                    var opening = stack.Pop();
                    elements.Add((opening.Start, end + 1, opening.Node));
                }
            }

            return stack.Count == 0 ? new AxamlSourceSelection(elements) : null;
        }
        catch (XmlException)
        {
            return null;
        }
    }

    public AxamlPreviewNode? Find(int offset) =>
        _elements.Where(element => element.Start <= offset && offset < element.End)
            .OrderBy(element => element.End - element.Start)
            .Select(element => element.Node)
            .FirstOrDefault();

    private static bool CollectNodes(AxamlPreviewNode node, Dictionary<int, AxamlPreviewNode> nodes)
    {
        if (node.SourceStartOffset >= 0 && !nodes.TryAdd(node.SourceStartOffset, node))
        {
            return false;
        }

        return node.Children.All(child => CollectNodes(child, nodes));
    }

    private static int FindTagEnd(string markup, int start)
    {
        char quote = '\0';
        for (int i = start + 1; i < markup.Length; i++)
        {
            char current = markup[i];
            if (quote != '\0')
            {
                if (current == quote)
                {
                    quote = '\0';
                }
            }
            else if (current is '\'' or '"')
            {
                quote = current;
            }
            else if (current == '>')
            {
                return i;
            }
        }

        return -1;
    }
}
