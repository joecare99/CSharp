using Document.Odf.Models;
using System.Text;
using System.Xml.Linq;

namespace Document.Odf.Parsing;

public static class OdfTextExtractor
{
    public static string GetPlainText(XDocument contentXml)
    {
        var officeBody = contentXml.Root?.Element(OdfNamespaces.Office + "body"); var officeText = officeBody?.Element(OdfNamespaces.Office + "text"); if (officeText is null) return string.Empty;
        var sb = new StringBuilder(8 * 1024);
        foreach (var node in officeText.DescendantNodes())
        {
            if (node is XElement el)
            {
                if (el.Name == OdfNamespaces.Text + "p" || el.Name == OdfNamespaces.Text + "h")
                {
                    AppendElementText(el, sb);
                    sb.AppendLine();
                }
            }
        }

        return Normalize(sb.ToString());
    }

    private static void AppendElementText(XElement element, StringBuilder sb)
    {
        foreach (var node in element.Nodes())
        {
            switch (node)
            {
                case XText t:
                    sb.Append(t.Value);
                    break;

                case XElement el:
                    if (el.Name == OdfNamespaces.Text + "span")
                    {
                        AppendElementText(el, sb);
                    }
                    else if (el.Name == OdfNamespaces.Text + "s") // non-breaking spaces
                    {
                        var countAttr = (string?)el.Attribute("c");
                        var count = 1;
                        _ = int.TryParse(countAttr, out count);
                        sb.Append('\u00A0', Math.Max(1, count)); // NBSP
                    }
                    else if (el.Name == OdfNamespaces.Text + "tab")
                    {
                        sb.Append('\t');
                    }
                    else if (el.Name == OdfNamespaces.Text + "line-break")
                    {
                        sb.AppendLine();
                    }
                    else
                    {
                        AppendElementText(el, sb);
                    }
                    break;
            }
        }
    }

    private static string Normalize(string text)
    {
        // XML-Parser dekodiert Entities bereits -> nur Zeilenenden vereinheitlichen
        return text.Replace("\r\n", "\n").Replace('\r', '\n');
    }
}