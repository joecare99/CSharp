using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using TranspilerLib.Data;
using TranspilerLib.Interfaces.Code;

namespace Trnsp.Show.Pas.ViewModels
{
    public partial class CodeBlockNode : ObservableObject
    {
        private readonly ICodeBlock _codeBlock;

        public string Name => _codeBlock.Name;
        public CodeBlockType Type => _codeBlock.Type;
        public string Code => _codeBlock.Code;
        public ObservableCollection<CodeBlockNode> Children { get; } = new();

        public CodeBlockNode(ICodeBlock codeBlock)
        {
            _codeBlock = codeBlock;
            foreach (var child in _codeBlock.SubBlocks)
            {
                Children.Add(new CodeBlockNode(child));
            }
        }
    }
}
