using System.Text;
using XamlDecompiler.Core.Models;

namespace XamlDecompiler.Core.Services;

internal sealed class CodeBehindWriter
{
    public string Write(GeneratedSourceModel model)
    {
        StringBuilder builder = new();

        if (!string.IsNullOrWhiteSpace(model.UsingBlock))
        {
            builder.AppendLine(model.UsingBlock.TrimEnd());
            builder.AppendLine();
        }

        builder.AppendLine($"namespace {model.Namespace};");
        builder.AppendLine();
        builder.AppendLine($"public partial class {model.ClassName} : {model.RootTypeName}");
        builder.AppendLine("{");

        if (!string.IsNullOrWhiteSpace(model.UserMembersBlock))
        {
            string[] lines = model.UserMembersBlock.Trim().Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n');
            foreach (string line in lines)
            {
                builder.AppendLine($"    {line.TrimEnd()}");
            }
        }

        builder.AppendLine("}");
        return builder.ToString();
    }
}
