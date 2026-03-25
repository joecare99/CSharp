using System.Xml.Linq;
namespace Document.Odf.Models;

public static class OdfNamespaces
{
    public static readonly XNamespace Office = "urn:oasis:names:tc:opendocument:xmlns:office:1.0";
    public static readonly XNamespace Text = "urn:oasis:names:tc:opendocument:xmlns:text:1.0"; 
    public static readonly XNamespace Draw = "urn:oasis:names:tc:opendocument:xmlns:drawing:1.0"; 
    public static readonly XNamespace Style = "urn:oasis:names:tc:opendocument:xmlns:style:1.0"; 
    public static readonly XNamespace Table = "urn:oasis:names:tc:opendocument:xmlns:table:1.0"; 
    public static readonly XNamespace XLink = "http://www.w3.org/1999/xlink";
    public static readonly XNamespace Fo = "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0";
    public static readonly XNamespace Svg = "urn:oasis:names:tc:opendocument:xmlns:svg-compatible:1.0";
    public static readonly XNamespace Dc = "http://purl.org/dc/elements/1.1/";
    public static readonly XNamespace Meta = "urn:oasis:names:tc:opendocument:xmlns:meta:1.0";
    public static readonly XNamespace Manifest = "urn:oasis:names:tc:opendocument:xmlns:manifest:1.0";
}