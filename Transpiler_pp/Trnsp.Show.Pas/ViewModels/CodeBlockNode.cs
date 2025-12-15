using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using TranspilerLib.Data;
using TranspilerLib.Interfaces.Code;

namespace Trnsp.Show.Pas.ViewModels;

public partial class CodeBlockNode : ObservableObject
{
    // Restored to last working state
    private readonly ICodeBlock _codeBlock;

    public string Name => _codeBlock.Name;
    public CodeBlockType Type => _codeBlock.Type;
    public string Code => _codeBlock.Code;
    public int SourcePos => _codeBlock.SourcePos;
    public int Length => _codeBlock.Code.Length;

    public string TypeIcon => GetTypeIcon(_codeBlock.Type);
    public string DisplayInfo => !string.IsNullOrEmpty(Name) ? Name : (Code.Length > 20 ? Code.Substring(0, 20) + "..." : Code);

    [ObservableProperty]
    private bool _isSelected;

    public ObservableCollection<CodeBlockNode> Children { get; } = new();

    public CodeBlockNode(ICodeBlock codeBlock)
    {
        _codeBlock = codeBlock;
        foreach (var child in _codeBlock.SubBlocks)
        {
            Children.Add(new CodeBlockNode(child));
        }
    }

    private string GetTypeIcon(CodeBlockType type)
    {
        return type switch
        {
            CodeBlockType.MainBlock => "🚀",
            CodeBlockType.SubBlock => "🧱",
            CodeBlockType.Operation => "⚙",
            CodeBlockType.Variable => "📦",
            CodeBlockType.Function => "ƒ",
            CodeBlockType.Block => "{}",
            CodeBlockType.Label => "🏷",
            CodeBlockType.Comment => "📝",
            CodeBlockType.LComment => "📝",
            CodeBlockType.FLComment => "📝",
            CodeBlockType.String => "abc",
            CodeBlockType.Number => "123",
            CodeBlockType.Declaration => "📄",
            CodeBlockType.Class => "©",
            CodeBlockType.Parameter => "📥",
            CodeBlockType.Namespace => "📛",
            CodeBlockType.Using => "🔗",
            CodeBlockType.Goto => "⤵",
            CodeBlockType.Bracket => "()",
            CodeBlockType.Assignment => "=",
            CodeBlockType.Separator => ";",
            CodeBlockType.Unknown => "❓",
            _ => "?"
        };
    }
}
